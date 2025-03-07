using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Updater
{
    public class Entrypoint
    {
        public static void Start() => UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;

        public static void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadmode)
        {
            if (scene.buildIndex != 0)
            {
                // retry loading first scene
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                return;
            }
            UnityEngine.Object.DestroyImmediate(UnityEngine.Object.FindObjectOfType<Managers>().gameObject);
            var go = new GameObject("LoadsonUpdater");
            go.AddComponent<UpdaterBehaviour>();
        }
    }

    class UpdaterBehaviour : MonoBehaviour
    {
        public const string API_ENDPOINT = "https://raw.githubusercontent.com/karlsonmodding/Loadson/refs/heads/deploy";
        public static WebClient wc;
        bool guiInit = false;
        Texture2D grayTx;
        GUIStyle centerText;
        bool checking = true;
        bool failed = false;
        List<string> update = new List<string>();
        bool[] checkmarks;
        string LOADSON_ROOT;
        bool downloading = false;
        int downloadProgress = 0, downloadTotal = 0;
        public void Start()
        {
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            grayTx = new Texture2D(1, 1);
            grayTx.SetPixel(0, 0, new Color(35f / 255f, 31f / 255f, 32f / 255f));
            grayTx.Apply();

            // check if discord exists
            DiscordAPI.Check();

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
            wc = new WebClient();
            new Thread(() =>
            {
                try
                {
                    var filetree = wc.DownloadString(API_ENDPOINT + "/filetree").Split('\n');
                    var hashmap_raw = wc.DownloadString(API_ENDPOINT + "/hashmap");
                    var hashmap = new Dictionary<string, string>();
                    foreach(var hashinfo in hashmap_raw.Split('\n'))
                    {
                        if (hashinfo.Length == 0) continue;
                        hashmap.Add(hashinfo.Split(':')[0], hashinfo.Split(':')[1]);
                    }
                    foreach(var _file in filetree)
                    {
                        if (_file.Length == 0) continue;
                        string root = LOADSON_ROOT;
                        string file = _file;
                        if(file.StartsWith("/"))
                        {
                            file = file.Substring(1);
                            root = Directory.GetCurrentDirectory();
                        }    
                        if(file.EndsWith("/"))
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
                            else if (hashmap.ContainsKey(_file) && hashmap[_file] != CheckHash(Path.Combine(path.ToArray())))
                            {
                                update.Add(_file);
                            }
                        }
                    }
                    checkmarks = new bool[update.Count];
                    for(int i = 0; i < checkmarks.Length; i++)
                        checkmarks[i] = true;
                    checking = false;
                } catch(Exception ex)
                {
                    UnityEngine.Debug.Log(ex.ToString());
                    failed = true;
                }
            }).Start();
        }
        public void OnGUI()
        {
            if (!guiInit)
            {
                guiInit = true;

                // load arial font (for cross-platform)
                var resource_name = typeof(Entrypoint).Assembly.GetManifestResourceNames()[0];
                AssetBundle bundle = AssetBundle.LoadFromStream(typeof(Entrypoint).Assembly.GetManifestResourceStream(resource_name));
                Font font = bundle.LoadAsset<Font>("arial");
                GUI.skin.font = font;
                bundle.Unload(false);

                centerText = new GUIStyle(GUI.skin.label);
                centerText.alignment = TextAnchor.UpperCenter;
                centerText.fontSize = 30;
                centerText.fontStyle = FontStyle.Bold;
            }
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), grayTx, new Rect(0, 0, 1, 1));
            GUI.Label(new Rect(0, 0, Screen.width, 50), "Loadson Updater", centerText);
            if(failed)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 65), _ =>
                {
                    GUI.Label(new Rect(5, 20, 300, 20), "Updater failed (check your internet connection)");
                    if(GUI.Button(new Rect(5, 40, 140, 20), "Skip Updater")) { Load(); }
                    if(GUI.Button(new Rect(155, 40, 140, 20), "Exit Game")) { Application.Quit(); }
                }, "Updater error");
                return;
            }
            if(checking)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 65), _ =>
                {
                    GUI.Label(new Rect(5, 20, 300, 20), "Please wait..");
                    GUI.Label(new Rect(5, 40, 300, 20), "Checking for updates");
                }, "Updater");
                return;
            }
            if (DiscordAPI.needToDownload)
            {
                if (!DiscordAPI.downloading)
                    GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 65), _ =>
                    {
                        GUI.Label(new Rect(5, 20, 300, 20), "Discord Game SDK is not installed.");
                        if (GUI.Button(new Rect(5, 40, 140, 20), "Install Now")) DiscordAPI.Download();
                        if (GUI.Button(new Rect(155, 40, 140, 20), "Don't Install")) DiscordAPI.needToDownload = false;
                    }, "Discord API");
                else
                    GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 65), _ =>
                    {
                        GUI.Label(new Rect(5, 20, 300, 20), "Please wait..");
                        GUI.Label(new Rect(5, 40, 300, 20), "Downloading Discord Game SDK");
                    }, "Discord API");
                return;
            }
            if (update.Count == 0)
            {
                Load();
                return;
            }
            if(!downloading)
            {
                GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 150, 300, 305), _ =>
                {
                    // TODO: calculate height
                    scrollPos = GUI.BeginScrollView(new Rect(5, 20, 290, 255), scrollPos, new Rect(0, 0, 275, update.Count * 20 + 25));
                    GUI.Label(new Rect(0, 0, 280, 20), "The following files need to be updated:");
                    for (int i = 0; i < update.Count; i++)
                        checkmarks[i] = GUI.Toggle(new Rect(0, 20 + 20 * i, 280, 20), checkmarks[i], update[i]);
                    GUI.EndScrollView();
                    if (GUI.Button(new Rect(5, 280, 140, 20), checkmarks.All(x => x) ? "Update all files" : checkmarks.Any(x => x) ? "Update selected files" : "Don't update")) { UpdateFiles(); }
                    if (GUI.Button(new Rect(155, 280, 140, 20), "Exit Game")) { Application.Quit(); }
                }, "Updater list");
                return;
            }
            GUI.Window(0, new Rect(Screen.width / 2 - 150, Screen.height / 2 - 30, 300, 65), _ =>
            {
                GUI.Label(new Rect(5, 20, 300, 20), "Please wait, downloading files..");
                GUI.Label(new Rect(5, 40, 300, 20), $"Progress: {downloadProgress} / {downloadTotal}");
            }, "Updating files");
        }
        Vector2 scrollPos = new Vector2(0, 0);

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

        static bool loaded = false;
        void Load()
        {
            if(loaded) return;
            loaded = true;
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= Entrypoint.SceneManager_sceneLoaded;
            var assembly = Assembly.LoadFrom(Path.Combine(LOADSON_ROOT, "Internal", "Launcher.dll"));
            assembly.GetType("Launcher.Entrypoint").GetMethod("Start").Invoke(null, new object[] { DiscordAPI.hasDiscord });
            Destroy(gameObject);
        }
        void UpdateFiles()
        {
            if(!checkmarks.Any(x => x))
            { // no files selected, go straight to loading
                Load();
                return;
            }
            downloading = true;
            downloadProgress = 0;
            downloadTotal = checkmarks.Count(x => x);
            new Thread(() =>
            {
                for(int i = 0; i < update.Count; i++)
                {
                    if (!checkmarks[i])
                        continue;
                    var file = update[i];
                    List<string> path;
                    if (file.StartsWith("/"))
                    {
                        path = new List<string> { Directory.GetCurrentDirectory() };
                        file = file.Substring(1);
                    }
                    else
                    {
                        path = new List<string> { LOADSON_ROOT };
                    }
                    path.AddRange(file.Split('/'));
                    File.WriteAllBytes(Path.Combine(path.ToArray()), wc.DownloadData(API_ENDPOINT + "/files/" + file.Replace(" ", "%20")));
                    ++downloadProgress;
                }
                downloading = false;
                Load();
            }).Start();
        }
    }
}
