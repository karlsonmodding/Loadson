using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows;

namespace UpdateChecker
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            HttpClient hc = new HttpClient();
            string API_ENDPOINT = "https://raw.githubusercontent.com/karlsonmodding/Loadson/deployment"; // no trailing [slash]

            string root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            if (!Directory.Exists(root))
            {
                string[] filetree = hc.GetStringAsync(API_ENDPOINT + "/filetree").GetAwaiter().GetResult().Split('\n');
                foreach (string file in filetree)
                {
                    if (file.Length == 0) continue;
                    if (file.EndsWith("/"))
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
                string[] filetree = hc.GetStringAsync(API_ENDPOINT + "/filetree").GetAwaiter().GetResult().Split('\n');
                string hashmap_raw = hc.GetStringAsync(API_ENDPOINT + "/hashmap").GetAwaiter().GetResult();
                Dictionary<string, string> hashmap = new Dictionary<string, string>();
                foreach (string hashinfo in hashmap_raw.Split('\n'))
                {
                    if (hashinfo.Length == 0) continue;
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
                        else if (hashmap.ContainsKey(file) && hashmap[file] != CheckHash(Path.Combine(path.ToArray())))
                        {
                            File.Delete(Path.Combine(path.ToArray()));
                            update.Add(file);
                        }
                    }
                }
                foreach (string file in update)
                {
                    List<string> path = new List<string> { root };
                    path.AddRange(file.Split('/'));
                    File.WriteAllBytes(Path.Combine(path.ToArray()), hc.GetByteArrayAsync(API_ENDPOINT + "/files/" + file.Replace(" ", "%20")).GetAwaiter().GetResult());
                }
            }

            Environment.Exit(0);
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
