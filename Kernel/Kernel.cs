using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Kernel
{
    public class Kernel
    {
        public static string LOADSON_ROOT;
        private static Assembly loadson;

        public static void Inject()
        {
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            loadson = Assembly.LoadFrom(Path.Combine(LOADSON_ROOT, "Internal", "loadson.dll"));
            loadson.GetType("LoadsonInternal.Loader").GetMethod("Start").Invoke(null, Array.Empty<object>());
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (new AssemblyName(args.Name).Name == "Loadson")
                return loadson;
            string resolved = Path.Combine(LOADSON_ROOT, "internal", "Loadson deps", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved))
                return Assembly.LoadFile(resolved);
            return null;
        }
    }
}
