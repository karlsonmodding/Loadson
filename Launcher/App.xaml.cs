using MInject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region >>> PInvoke

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        const uint WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        #endregion

        public const string VERSION = "1.1";
        public const int TIMEOUT = 50;
        public static string ROOT;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");

            if (Environment.GetCommandLineArgs().Length > 2 && Environment.GetCommandLineArgs()[1] == "-disable")
            {
                Thread.Sleep(200); // wait for karlson exit
                while (Process.GetProcessesByName("Karlson.exe").Length > 0) Thread.Sleep(0);
                Thread.Sleep(50); // wait for karlson exit
                if (!Directory.Exists(Path.Combine(ROOT, "Mods", "Disabled")))
                    Directory.CreateDirectory(Path.Combine(ROOT, "Mods", "Disabled"));
                string modName = Environment.GetCommandLineArgs()[2];
                for (int i = 3; i < Environment.GetCommandLineArgs().Length; i++)
                    modName += " " + Environment.GetCommandLineArgs()[i];
                File.Move(Path.Combine(ROOT, "Mods", modName), Path.Combine(ROOT, "Mods", "Disabled", modName));
                Process.Start(Path.Combine(ROOT, "Launcher", "Launcher.exe"), "-silent");
                Process.GetCurrentProcess().Kill();
                return;
            }
            if(Environment.GetCommandLineArgs().Length > 1 && Environment.GetCommandLineArgs()[1] == "-silent")
            {
                string krlPath = File.ReadAllText(Path.Combine(ROOT, "Internal", "karlsonpath")).Trim();
                Process karlson = new Process
                {
                    StartInfo = new ProcessStartInfo(Path.Combine(krlPath, "Karlson.exe"))
                };
                karlson.Start();
                while (karlson.MainWindowHandle == IntPtr.Zero) Thread.Sleep(0);
                PostMessage(karlson.MainWindowHandle, WM_KEYDOWN, 0xD, 0); // enter key
                Thread.Sleep(TIMEOUT);

                // run MInject
                // Method: Kernel.Kernel.Start()
                if (MonoProcess.Attach(karlson, out MonoProcess m_karlson))
                {
                    byte[] assemblyBytes = File.ReadAllBytes(Path.Combine(ROOT, "Internal", "Kernel.dll"));
                    IntPtr monoDomain = m_karlson.GetRootDomain();
                    m_karlson.ThreadAttach(monoDomain);
                    m_karlson.SecuritySetMode(0);
                    m_karlson.DisableAssemblyLoadCallback();

                    IntPtr rawAssemblyImage = m_karlson.ImageOpenFromDataFull(assemblyBytes);
                    IntPtr assemblyPointer = m_karlson.AssemblyLoadFromFull(rawAssemblyImage);
                    IntPtr assemblyImage = m_karlson.AssemblyGetImage(assemblyPointer);
                    IntPtr classPointer = m_karlson.ClassFromName(assemblyImage, "Kernel", "Kernel");
                    IntPtr methodPointer = m_karlson.ClassGetMethodFromName(classPointer, "Inject");

                    m_karlson.RuntimeInvoke(methodPointer);
                    m_karlson.EnableAssemblyLoadCallback();
                    m_karlson.Dispose();
                }
                else
                {
                    karlson.Kill();
                    MessageBox(IntPtr.Zero, "Couldn't execute MInject.\nPlease retry", "[Loadson Injector] Error", 0x00040010);
                    return;
                }
                Process.GetCurrentProcess().Kill();
                return;
            }
#if true // set to false to be able to launch without bootstrapper
            if(Environment.GetEnvironmentVariable("Loadson") == null)
            {
                MessageBox(IntPtr.Zero, "Please launch Loadson with Loadson.exe (Bootstrapper).", "[Loadson Launcher] Error", 0x00040010);
                Process.GetCurrentProcess().Kill();
                return;
            }
#endif
            if (!Directory.Exists(ROOT))
            {
                MessageBox(IntPtr.Zero, "Couldn't find Loadson directory. Try running Loadson.exe", "[Loadson Launcher] Error", 0x00040010);
                Process.GetCurrentProcess().Kill();
                return;
            }
            new MainWindow().Show();
        }
    }
}
