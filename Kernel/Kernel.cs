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
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        public static string LOADSON_ROOT;
        private static Assembly loadson;
        private static MethodInfo unityLog;

        public static void Start()
        {
            if (Environment.GetCommandLineArgs().Contains("-vanilla")) return;
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");

            // run update checker
#if true // change this to false to run off-grid
            Process.Start(Path.Combine(LOADSON_ROOT, "Internal", "UpdateChecker.exe")).WaitForExit();
#endif

            // start loadson window
            Process p = new Process
            {
                StartInfo = {
                    FileName = Path.Combine(LOADSON_ROOT, "Launcher", "Launcher.exe"),
                    WorkingDirectory = Path.Combine(LOADSON_ROOT, "Launcher"),
                    UseShellExecute = false
                }
            };
            p.StartInfo.EnvironmentVariables.Add("Loadson", "v2");
            p.Start();
            p.WaitForExit();
            if(p.ExitCode == 0)
            { // exit
                Process.GetCurrentProcess().Kill();
                return;
            }
            if (p.ExitCode == 1)
                // vanilla
                return;
            new Thread(new ThreadStart(() =>
            {
                try
                {
                    // wait for configuration screen
                    while (FindWindow(null, "Karlson Configuration") == IntPtr.Zero) Thread.Sleep(100);
                    SetWindowText(FindWindow(null, "Karlson Configuration"), "[Loadson] Karlson Configuration");

                    // wait for user to exit configuration screen
                    while (FindWindow(null, "[Loadson] Karlson Configuration") != IntPtr.Zero) Thread.Sleep(100);

                    // wait for assembly-csharp
                    while (AppDomain.CurrentDomain.GetAssemblies().Count(x => x.GetName().Name == "Assembly-CSharp") == 0) Thread.Sleep(100);

                    // load Loadson
                    unityLog = AppDomain.CurrentDomain.GetAssemblies().First(x => x.GetName().Name == "UnityEngine.CoreModule").GetType("UnityEngine.Debug").GetMethod("Log", new Type[] { typeof(object) });
                    AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                    loadson = Assembly.LoadFrom(Path.Combine(LOADSON_ROOT, "Internal", "loadson.dll"));
                    loadson.GetType("LoadsonInternal.Loader").GetMethod("Start").Invoke(null, Array.Empty<object>());
                }
                catch(Exception ex)
                {
                    File.AppendAllText("log", ex.ToString() + "\n");
                }
            })).Start();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            unityLog.Invoke(null, new object[] { args.Name });
            if (new AssemblyName(args.Name).Name == "Loadson")
                return loadson;
            string resolved = Path.Combine(LOADSON_ROOT, "internal", "Loadson deps", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved))
                return Assembly.LoadFile(resolved);
            unityLog.Invoke(null, new object[] { "Couldn't resolve last" });
            return null;
        }
    }
}
