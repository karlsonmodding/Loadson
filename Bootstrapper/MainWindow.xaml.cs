using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Bootstrapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            logText.Text = "Loadson\n";
            logText.Text += "  made by devilExE\n";
            logText.Text += "  licensed under MIT license\n";
            logText.Text += "\n";

            logText.Text += "Starting Loadson Bootstrapper v1\n";
            HttpClient hc = new HttpClient();
            string API_ENDPOINT = "https://raw.githubusercontent.com/karlsonmodding/Loadson/deployment"; // no trailing [slash]

            logText.Text += "Searching for Loadson directory\n";
            string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            if (!Directory.Exists(root))
            {
                logText.Text += "Downloading all files\n";
                string[] filetree = hc.GetStringAsync(API_ENDPOINT + "/filetree").GetAwaiter().GetResult().Split('\n');
                foreach(string file in filetree)
                {
                    if (file.Length == 0) continue;
                    if(file.EndsWith("/"))
                    {
                        List<string> path = new List<string> { root };
                        path.AddRange(file.Substring(0, file.Length - 1).Split('/'));
                        Directory.CreateDirectory(Path.Combine(path.ToArray()));
                    }
                    else
                    {
                        List<string> path = new List<string> { root };
                        path.AddRange(file.Split('/'));
                        File.WriteAllBytes(Path.Combine(path.ToArray()), hc.GetByteArrayAsync(API_ENDPOINT + "/files/" + file.Replace(" ", "%20")).GetAwaiter().GetResult());
                    }
                }
            }
            else
            {
                List<string> update = new List<string>();
                logText.Text += "Downloading filetree and hashmap\n";
                string[] filetree = hc.GetStringAsync(API_ENDPOINT + "/filetree").GetAwaiter().GetResult().Split('\n');
                string hashmap_raw = hc.GetStringAsync(API_ENDPOINT + "/hashmap").GetAwaiter().GetResult();
                Dictionary<string, string> hashmap = new Dictionary<string, string>();
                foreach(string hashinfo in hashmap_raw.Split('\n'))
                {
                    if(hashinfo.Length == 0) continue;
                    hashmap.Add(hashinfo.Split(':')[0], hashinfo.Split(':')[1]);
                }
                foreach (string file in filetree)
                {
                    if (file.Length == 0) continue;
                    if (file.EndsWith("/"))
                    {
                        List<string> path = new List<string> { root };
                        path.AddRange(file.Substring(0, file.Length - 1).Split('/'));
                        if (!Directory.Exists(Path.Combine(path.ToArray())))
                            Directory.CreateDirectory(Path.Combine(path.ToArray()));
                    }
                    else
                    {
                        List<string> path = new List<string> { root };
                        path.AddRange(file.Split('/'));
                        if (!File.Exists(Path.Combine(path.ToArray())))
                            update.Add(file);
                        else if (hashmap.ContainsKey(file) && hashmap[file] != CheckHash(Path.Combine(path.ToArray()))) {
                            File.Delete(Path.Combine(path.ToArray()));
                            update.Add(file);
                        }
                    }
                }
                logText.Text += "Downloading " + update.Count + " files..\n";
                foreach(string file in update)
                {
                    List<string> path = new List<string> { root };
                    path.AddRange(file.Split('/'));
                    File.WriteAllBytes(Path.Combine(path.ToArray()), hc.GetByteArrayAsync(API_ENDPOINT + "/files/" + file.Replace(" ", "%20")).GetAwaiter().GetResult());
                }
            }
            logText.Text += "Starting Loadson..";
            Process p = new Process
            {
                StartInfo = {
                    FileName = Path.Combine(root, "Launcher", "Launcher.exe"),
                    WorkingDirectory = Path.Combine(root, "Launcher"),
                    UseShellExecute = false
                }
            };
            p.StartInfo.EnvironmentVariables.Add("Loadson", "true");
            p.Start();
            Close();
        }

        static string CheckHash(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
