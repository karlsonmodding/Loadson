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
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        [DllImport("kernel32.dll")]
        static extern bool SetConsoleTitle(string lpConsoleTitle);
        private static FileStream CreateFileStream(string name, uint win32DesiredAccess, uint win32ShareMode,
                                FileAccess dotNetFileAccess)
        {
            var file = new SafeFileHandle(CreateFileW(name, win32DesiredAccess, win32ShareMode, IntPtr.Zero, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, IntPtr.Zero), true);
            if (!file.IsInvalid)
            {
                var fs = new FileStream(file, dotNetFileAccess);
                return fs;
            }
            return null;
        }
        [DllImport("kernel32.dll",
            EntryPoint = "CreateFileW",
            SetLastError = true,
            CharSet = CharSet.Auto,
            CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr CreateFileW(
             string lpFileName,
             UInt32 dwDesiredAccess,
             UInt32 dwShareMode,
             IntPtr lpSecurityAttributes,
             UInt32 dwCreationDisposition,
             UInt32 dwFlagsAndAttributes,
             IntPtr hTemplateFile
           );

        private const UInt32 GENERIC_WRITE = 0x40000000;
        private const UInt32 GENERIC_READ = 0x80000000;
        private const UInt32 FILE_SHARE_READ = 0x00000001;
        private const UInt32 FILE_SHARE_WRITE = 0x00000002;
        private const UInt32 OPEN_EXISTING = 0x00000003;
        private const UInt32 FILE_ATTRIBUTE_NORMAL = 0x80;
        private const UInt32 ERROR_ACCESS_DENIED = 5;

        private static FileStream conout;
        private static void writecon(string s)
        {
            var data = Encoding.ASCII.GetBytes(s + "\n");
            conout.Write(data, 0, data.Length);
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

        private const UInt32 WM_CLOSE = 0x0010;

        public static string LOADSON_ROOT;
        private static Assembly loadson;

        public static void Inject()
        {
            AllocConsole();
            SetConsoleTitle("Loadson Kernel");
            Console.Title = "Loadson Kernel";
            conout = CreateFileStream("CONOUT$", GENERIC_WRITE, FILE_SHARE_WRITE, FileAccess.Write);
            writecon("Loadson");
            writecon("  made by devilExE");
            writecon("  licensed under MIT license");
            writecon("");
            writecon("Loadson Kernel v1");

            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            writecon("Loadosn root: " + LOADSON_ROOT);
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            writecon("Created AssemblyResolve");
            writecon("Loading Loadson..");
            loadson = Assembly.LoadFrom(Path.Combine(LOADSON_ROOT, "Internal", "loadson.dll"));
            writecon("Starting Loadson..");
            loadson.GetType("LoadsonInternal.Loader").GetMethod("Start").Invoke(null, Array.Empty<object>());
            writecon("\n\nKernel Finished. You may close this window.\n\n");
            conout.Close();
            FreeConsole();
            IntPtr conHnd = FindWindow(null, "Loadson Kernel");
            SendMessage(conHnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
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
