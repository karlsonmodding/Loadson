using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

// static void Doorstop.Entrypoint.Start()
namespace Doorstop
{
    public class Entrypoint
    {
        static string LOADSON_ROOT;

        public static void Start()
        {
            if (Environment.GetCommandLineArgs().Contains("-vanilla")) return;
            
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            Directory.CreateDirectory(Path.Combine(LOADSON_ROOT, "Internal"));
            var stream_name = typeof(Entrypoint).Assembly.GetManifestResourceNames()[0];
            byte[] bytes;
            using (var base_preloader = typeof(Entrypoint).Assembly.GetManifestResourceStream(stream_name))
            {
                bytes = new byte[base_preloader.Length];
                base_preloader.Read(bytes, 0, bytes.Length);
            }
            new Thread(() =>
            {
                while (AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name == "Assembly-CSharp") == 0) { }
                while (AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name == "UnityEngine") == 0) { }
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;

                var preloader = AppDomain.CurrentDomain.Load(bytes);
                preloader.GetType("Updater.Entrypoint").GetMethod("Start").Invoke(null, Array.Empty<object>());
            }).Start();
        }

        static Dictionary<string, Assembly> loadedAssemblies = new Dictionary<string, Assembly>();

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            loadedAssemblies[args.LoadedAssembly.FullName] = args.LoadedAssembly;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (loadedAssemblies.ContainsKey(args.Name))
                return loadedAssemblies[args.Name];
            var resolved = Path.Combine(LOADSON_ROOT, "Internal", "Loadson deps", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFrom(resolved);
            return null;
        }
    }
}
