using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for ModInfo.xaml
    /// </summary>
    public partial class ModInfo : Window
    {
        public ModInfo(byte[] _icon, string _guid, string _name, string _author, DateTime _modify, string _description, List<string> _deps, bool _hasBundle)
        {
            InitializeComponent();
            icon.Source = GetThumbnail(_icon);
            name.Text = "[File] " + _guid + ".klm\n";
            name.Text += "[Name] " + _name + "\n";
            name.Text += "[Author] " + _author + "\n";
            name.Text += "[Dependencies] (" + _deps.Count + ") " + string.Join(", ", _deps.ToArray()) + "\n";
            name.Text += (_hasBundle ? "This mod has an AssetBundle" : "This mod does not have an AssetBundle") + "\n";
            name.Text += "You installed this mod on " + _modify.ToString("dd.MM.yyyy HH:mm");
            description.Text = _description;
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
