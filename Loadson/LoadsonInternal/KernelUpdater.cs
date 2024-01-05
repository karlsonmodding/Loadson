using LoadsonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Security.Cryptography;
using System.IO;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace LoadsonInternal
{
    public static class KernelUpdater
    {
        private static int wid;
        private static Rect wir;
        private static WebClient wc;
        private static bool needToUpdate = false;

        public static void CheckForUpdates()
        {
            wid = ImGUI_WID.GetWindowId();
            wir = new Rect((Screen.width - 400) / 2, (Screen.height - 100) / 2, 400, 100);

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            wc = new WebClient();
            string md5 = wc.DownloadString("https://github.com/karlsonmodding/Loadson/raw/deployment/Karlson/_Loadson.md5");
            string check = CheckHash(Path.Combine(File.ReadAllText(Path.Combine(Loader.LOADSON_ROOT, "Internal", "karlsonpath")), "_Loadson.dll"));
            if(md5 != check)
            {
                needToUpdate = true;
            }
        }

        public static void _ongui()
        {
            if(needToUpdate)
            {
                GUI.Box(wir, "");
                GUI.Box(wir, "");
                wir = GUI.Window(wid, wir, (_) =>
                {
                    GUI.Label(new Rect(5, 15, 400, 100), "There is an update to the Loadson kernel (_Loadson.dll)\nDo you want to update?\n(This requieres restarting the game)");
                    if (GUI.Button(new Rect(25, 75, 150, 20), "Yes"))
                    {
                        // download the new kernel
                        File.WriteAllBytes(Path.Combine(File.ReadAllText(Path.Combine(Loader.LOADSON_ROOT, "Internal", "karlsonpath")), "_Loadson.dll"), wc.DownloadData("https://github.com/karlsonmodding/Loadson/raw/deployment/Karlson/_Loadson.dll"));
                        Environment.SetEnvironmentVariable("DOORSTOP_INITIALIZED", null);
                        Environment.SetEnvironmentVariable("DOORSTOP_DISABLE", null);
                        // restart
                        Process.Start(new ProcessStartInfo
                        {
                            FileName = Path.Combine(File.ReadAllText(Path.Combine(Loader.LOADSON_ROOT, "Internal", "karlsonpath")), "Karlson.exe"),
                            WorkingDirectory = File.ReadAllText(Path.Combine(Loader.LOADSON_ROOT, "Internal", "karlsonpath")),
                            Arguments = "-silent",
                        });
                        // quit app
                        Application.Quit();
                    }
                    if (GUI.Button(new Rect(225, 75, 150, 20), "No"))
                    {
                        needToUpdate = false;
                    }
                    GUI.DragWindow(new Rect(0, 0, 400, 10));
                }, "Loadson Kernel Update");
            }
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
