using Microsoft.Win32;
using MInject;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Remoting.Contexts;
using System.Security.Principal;
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

            if(Environment.GetCommandLineArgs().Length > 2 && Environment.GetCommandLineArgs()[1] == "-install")
            { // install a klmi mod
                if(!File.Exists(Environment.GetCommandLineArgs()[2]))
                {
                    MessageBox(IntPtr.Zero, "The file you are trying to install does not exist.", "[Loadson] Error", 0x00040010);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                if (!Environment.GetCommandLineArgs()[2].EndsWith(".klmi"))
                {
                    MessageBox(IntPtr.Zero, "You are trying to install a non-klmi file.\nOnly Karlson Loader Mod Install files can be installed", "[Loadson] Error", 0x00040010);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                if(File.Exists(Path.Combine(ROOT, "Mods", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[2]) + ".klm")))
                {
                    MessageBox(IntPtr.Zero, "You already installed this mod.", "[Loadson] Error", 0x00040010);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
                using(BinaryReader br = new BinaryReader(File.OpenRead(Environment.GetCommandLineArgs()[2])))
                {
                    int _extDeps = br.ReadInt32();
                    List<(string, byte[])> extDeps = new List<(string, byte[])>();
                    for(int i = 0; i < _extDeps; i++)
                    {
                        string name = br.ReadString();
                        int _len = br.ReadInt32();
                        byte[] data = br.ReadBytes(_len);
                        extDeps.Add((name, data));
                    }
                    int _modSize = br.ReadInt32();
                    byte[] modData = br.ReadBytes(_modSize);
                    foreach(var dep in extDeps)
                        File.WriteAllBytes(Path.Combine(ROOT, "Internal", "Common deps", dep.Item1), dep.Item2);
                    File.WriteAllBytes(Path.Combine(ROOT, "Mods", Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[2]) + ".klm"), modData);
                    MessageBox(IntPtr.Zero, "Mod " + Path.GetFileNameWithoutExtension(Environment.GetCommandLineArgs()[2]) + " installed succesfully", "[Loadson] Info", 0x00000040);
                    Process.GetCurrentProcess().Kill();
                    return;
                }
            }

            if (Environment.GetCommandLineArgs().Length > 2 && Environment.GetCommandLineArgs()[1] == "-disable")
            {
                // wait for karlson exit
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
#if false // set to false to be able to launch without bootstrapper
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

            // add .klmi handler
            if(!Registry.ClassesRoot.GetSubKeyNames().Contains(".klmi"))
            {
                if(!IsAdministrator())
                {
                    if (MessageBox(IntPtr.Zero, "Loadson Mod Install files (.klmi) are not associated.\nDo you want to associate them now?\n(Requieres restart with Administrator)", "Loadson", (uint)(0x00000004L | 0x00000040L)) == 6)
                    { // yes
                        Process proc = new Process();
                        proc.StartInfo.FileName = Path.Combine(ROOT, "Launcher", "Launcher.exe");
                        proc.StartInfo.UseShellExecute = true;
                        proc.StartInfo.Verb = "runas";
                        proc.Start();
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                }
                else
                {
                    RegistryKey filetype = Registry.ClassesRoot.CreateSubKey(".klmi");
                    filetype.SetValue("", "LoadsonModInstaller");
                    filetype.Close();
                    RegistryKey app = Registry.ClassesRoot.CreateSubKey("LoadsonModInstaller");
                    app.SetValue("", "Loadson Mod Install");
                    app.CreateSubKey("DefaultIcon").SetValue("", $"\"{Path.Combine(ROOT, "Launcher", "Launcher.exe")}\",0");
                    RegistryKey shell = app.CreateSubKey("shell");
                    RegistryKey open = shell.CreateSubKey("open");
                    open.SetValue("", "Install mod");
                    open.CreateSubKey("command").SetValue("", $"\"{Path.Combine(ROOT, "Launcher", "Launcher.exe")}\" -install \"%1\"");
                    app.Close();
                }
            }
            

            new MainWindow().Show();
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
