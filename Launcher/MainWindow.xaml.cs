using Microsoft.Win32;
using MInject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region >>> PInvoke
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool SetWindowText(IntPtr hwnd, String lpString);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct WINDOWPLACEMENT
        {
            public int Length;
            public int Flags;
            public ShowState ShowCmd;
            public POINT MinPosition;
            public POINT MaxPosition;
            public RECT NormalPosition;
            public static WINDOWPLACEMENT Default
            {
                get
                {
                    WINDOWPLACEMENT result = new WINDOWPLACEMENT();
                    result.Length = Marshal.SizeOf(result);
                    return result;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left, Top, Right, Bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                Left = left;
                Top = top;
                Right = right;
                Bottom = bottom;
            }

            public int X
            {
                get { return Left; }
                set { Right -= (Left - value); Left = value; }
            }

            public int Y
            {
                get { return Top; }
                set { Bottom -= (Top - value); Top = value; }
            }

            public int Height
            {
                get { return Bottom - Top; }
                set { Bottom = value + Top; }
            }

            public int Width
            {
                get { return Right - Left; }
                set { Right = value + Left; }
            }


            public static bool operator ==(RECT r1, RECT r2)
            {
                return r1.Equals(r2);
            }

            public static bool operator !=(RECT r1, RECT r2)
            {
                return !r1.Equals(r2);
            }

            public bool Equals(RECT r)
            {
                return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
            }

            public override string ToString()
            {
                return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
            }

            public override bool Equals(object obj)
            {
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public POINT(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
        }

        public enum ShowState : int
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDlgItem(IntPtr hDlg, int nIDDlgItem);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(HandleRef hWnd, uint Msg, IntPtr wParam, string lParam);
        public const uint WM_SETTEXT = 0x000C;

        private void InteropSetText(IntPtr iptrHWndDialog, int iControlID, string strTextToSet)
        {
            IntPtr iptrHWndControl = GetDlgItem(iptrHWndDialog, iControlID);
            HandleRef hrefHWndTarget = new HandleRef(null, iptrHWndControl);
            SendMessage(hrefHWndTarget, WM_SETTEXT, IntPtr.Zero, strTextToSet);
        }

        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        const uint WM_KEYDOWN = 0x0100;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);

        #endregion

        public MainWindow()
        {
            InitializeComponent();
            Start();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private string karlsonFolder;
        private void Start()
        {
            if (!File.Exists(Path.Combine(App.ROOT, "Internal", "karlsonpath")))
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Select your karlson install",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "Karlson Executable|karlson.exe"
                };
                if ((bool)ofd.ShowDialog())
                    File.WriteAllText(Path.Combine(App.ROOT, "Internal", "karlsonpath"), Path.GetDirectoryName(ofd.FileName));
                else
                    Environment.Exit(0);
            }
            karlsonFolder = File.ReadAllText(Path.Combine(App.ROOT, "Internal", "karlsonpath"));
            if (!File.Exists(Path.Combine(karlsonFolder, "Karlson.exe")))
            {
                OpenFileDialog ofd = new OpenFileDialog
                {
                    Title = "Update your karlson install",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    Filter = "Karlson Executable|karlson.exe"
                };
                if ((bool)ofd.ShowDialog())
                    File.WriteAllText(Path.Combine(App.ROOT, "Internal", "karlsonpath"), Path.GetDirectoryName(ofd.FileName));
                else
                    Environment.Exit(0);
            }

            // get version of loadson internal
            Assembly asm = Assembly.LoadFile(Path.Combine(App.ROOT, "Internal", "loadson.dll"));
            string loadson_version = (string)asm.GetType("Version").GetField("ver").GetValue(null);
            // set version info
            Status.Text = "Launcher Version: " + App.VERSION + "\n" +
                "Loadson Version: " + loadson_version;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Process karlson = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(karlsonFolder, "Karlson.exe"))
            };
            karlson.StartInfo.UseShellExecute = false;
            karlson.StartInfo.EnvironmentVariables.Add("KarlsonLoaderDir", App.ROOT);
            if (!karlson.Start())
            {
                MessageBox(IntPtr.Zero, "Couldn't start Karlson, please try again.", "[Loadson Injector] Error", 0x00040010);
                return;
            }
            while (karlson.MainWindowHandle == IntPtr.Zero) Thread.Sleep(0);
            PostMessage(karlson.MainWindowHandle, WM_KEYDOWN, 0xD, 0); // enter key
            Thread.Sleep(100);

            // run MInject
            // Method: Kernel.Kernel.Start()
            if (MonoProcess.Attach(karlson, out MonoProcess m_karlson))
            {
                byte[] assemblyBytes = File.ReadAllBytes(Path.Combine(App.ROOT, "Internal", "Kernel.dll"));
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
            Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Process p = new Process
            {
                StartInfo = new ProcessStartInfo(Path.Combine(karlsonFolder, "Karlson"))
                {
                    UseShellExecute = false
                }
            };
            p.Start();
            while (p.MainWindowHandle == (IntPtr)0)
            {
                Thread.Sleep(1);
            }
            InteropSetText(p.MainWindowHandle, 1, "Apply");
            InteropSetText(p.MainWindowHandle, 2, "Cancel");
            SetWindowText(p.MainWindowHandle, "Edit Karlson Configuration");
            while (true)
            {
                if (p.MainWindowHandle != (IntPtr)0)
                {
                    WINDOWPLACEMENT wp = new WINDOWPLACEMENT();
                    GetWindowPlacement(p.MainWindowHandle, ref wp);
                    if (wp.ShowCmd == ShowState.SW_HIDE)
                        break;
                }
                Thread.Sleep(0);
            }
            Thread.Sleep(5);
            p.Kill();
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(Path.Combine(App.ROOT, "Mods")))
                Directory.CreateDirectory(Path.Combine(App.ROOT, "Mods"));
            Process.Start(Path.Combine(App.ROOT, "Mods"));
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Update your karlson install",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                Filter = "Karlson Executable|karlson.exe"
            };
            if ((bool)ofd.ShowDialog())
            {
                File.WriteAllText(Path.Combine(App.ROOT, "Internal", "karlsonpath"), Path.GetDirectoryName(ofd.FileName));
                karlsonFolder = Path.GetDirectoryName(ofd.FileName);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Width = 800;
            Height = 500;
            ModManager.Visibility = Visibility.Visible;
            // reset center screen
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);

            //render mods
            ModContainer.Children.Clear();
            List<string> modList = new List<string>();
            foreach (var file in Directory.GetFiles(Path.Combine(App.ROOT, "Mods")))
                if (file.EndsWith(".klm")) modList.Add(file);
            foreach (var file in Directory.GetFiles(Path.Combine(App.ROOT, "Mods", "Disabled")))
                if (file.EndsWith(".klm")) modList.Add(file);
            modList.Sort(new FileComparer());
            foreach(var m in modList)
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(m)))
                {
                    string ModGUID = Path.GetFileNameWithoutExtension(m);
                    string ModName = br.ReadString();
                    string ModAuthor = br.ReadString();
                    string ModDescription = br.ReadString();
                    int _modDepCount = br.ReadInt32();
                    List<string> ModDeps = new List<string>();
                    for (int i = 0; i < _modDepCount; i++)
                        ModDeps.Add(br.ReadString());
                    int ModWorkshopID = br.ReadInt32();
                    int _modSize = br.ReadInt32();
                    br.ReadBytes(_modSize);
                    int _iconSize = br.ReadInt32();
                    byte[] ModIcon = br.ReadBytes(_iconSize);
                    int _assetBundleSize = br.ReadInt32();
                    br.ReadBytes(_assetBundleSize);
                    Grid g = new Grid();
                    ModContainer.Children.Add(g);
                    g.Width = 741;
                    g.Height = 100;
                    g.VerticalAlignment = VerticalAlignment.Top;
                    g.HorizontalAlignment = HorizontalAlignment.Center;
                    g.Margin = new Thickness(0, 10, 0, 0);
                    g.Background = new SolidColorBrush(Color.FromRgb(51, 51, 51));
                    CheckBox cb = new CheckBox();
                    g.Children.Add(cb);
                    cb.Margin = new Thickness(2, 2, 0, 0);
                    cb.VerticalAlignment = VerticalAlignment.Top;
                    cb.HorizontalAlignment = HorizontalAlignment.Left;
                    cb.IsChecked = !Path.GetDirectoryName(m).EndsWith("Disabled");
                    cb.Checked += (_sender, _e) => File.Move(Path.Combine(App.ROOT, "Mods", "Disabled", ModGUID + ".klm"), Path.Combine(App.ROOT, "Mods", ModGUID + ".klm"));
                    cb.Unchecked += (_sender, _e) =>
                    {
                        if (!Directory.Exists(Path.Combine(App.ROOT, "Mods", "Disabled")))
                            Directory.CreateDirectory(Path.Combine(App.ROOT, "Mods", "Disabled"));
                        File.Move(Path.Combine(App.ROOT, "Mods", ModGUID + ".klm"), Path.Combine(App.ROOT, "Mods", "Disabled", ModGUID + ".klm"));
                    };
                    Image im = new Image();
                    g.Children.Add(im);
                    im.Height = 100;
                    im.Width = 100;
                    im.Margin = new Thickness(20, 0, 0, 0);
                    im.VerticalAlignment = VerticalAlignment.Top;
                    im.HorizontalAlignment = HorizontalAlignment.Left;
                    im.Source = GetThumbnail(ModIcon);
                    TextBlock tb = new TextBlock();
                    g.Children.Add(tb);
                    tb.Margin = new Thickness(130, 5, 0, 0);
                    tb.HorizontalAlignment = HorizontalAlignment.Left;
                    tb.VerticalAlignment = VerticalAlignment.Top;
                    tb.Width = 610;
                    tb.Height = 67;
                    tb.Text = ModName + "\n" + "by " + ModAuthor;
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                    tb.FontSize = 20;
                    Button btn1 = new Button();
                    g.Children.Add(btn1);
                    btn1.HorizontalAlignment = HorizontalAlignment.Left;
                    btn1.VerticalAlignment = VerticalAlignment.Top;
                    btn1.Margin = new Thickness(130, 77, 0, 0);
                    btn1.Width = 63;
                    btn1.Content = "Info";
                    btn1.Click += (_sender, _e) =>
                    {
                        new ModInfo(ModIcon, ModGUID, ModName, ModAuthor, File.GetCreationTime(m), ModDescription, ModDeps, _assetBundleSize > 0).ShowDialog();
                    };
                    Button btn2 = new Button();
                    g.Children.Add(btn2);
                    btn2.HorizontalAlignment = HorizontalAlignment.Left;
                    btn2.VerticalAlignment = VerticalAlignment.Top;
                    btn2.Margin = new Thickness(198, 77, 0, 0);
                    btn2.Width = 63;
                    btn2.Content = "Delete";
                    btn2.Click += (_sender, _e) =>
                    {
                        if (MessageBox(IntPtr.Zero, "Are you sure you want to delete the mod " + ModName + " (" + ModGUID + ") by " + ModAuthor + "?\nYou won't be able to get it back unless you reinstall it!", "Loadson", (uint)(0x00000004L | 0x00000020L)) == 6)
                        { // yes
                            File.Delete(m);
                            ModContainer.Children.Remove(g);
                        }
                    };
                }
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Width = 190;
            Height = 173;
            ModManager.Visibility = Visibility.Collapsed;
            // reset center screen
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;
            double windowWidth = Width;
            double windowHeight = Height;
            Left = (screenWidth / 2) - (windowWidth / 2);
            Top = (screenHeight / 2) - (windowHeight / 2);
        }

        private class FileComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return Path.GetFileName(x).CompareTo(Path.GetFileName(y));
            }
        }
        private ImageSource GetThumbnail(byte[] icon)
        {
            MemoryStream memoryStream = new MemoryStream(icon);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.DecodePixelWidth = 64;
            bitmap.DecodePixelHeight = 64;
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            bitmap.Freeze();

            return bitmap;
        }
    }
}
