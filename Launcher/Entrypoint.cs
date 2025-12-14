using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Launcher
{
    public class Entrypoint
    {
        public static bool HasDiscordAPI;
        public static void Start(bool discord)
        {
            HasDiscordAPI = discord;
            new GameObject("Launcher").AddComponent<LauncherBehaviour>();
        }
    }

    class ModInfo
    {
        public string GUID;
        public string Name;
        public string Author;
        public string Description;
        public Texture2D Image;
        public string[] Deps;
        public bool HasBundle;
        public int WorkshopID;
        public string FilePath;
        public bool Enabled;
        public bool isLegacy;
        public ModInfo(string filePath)
        {
            FilePath = filePath;
            Enabled = !Path.GetDirectoryName(filePath).EndsWith("Disabled");
            using (BinaryReader br = new BinaryReader(File.OpenRead(FilePath)))
            {
                Name = br.ReadString();
                Author = br.ReadString();
                Description = br.ReadString();
                int x = br.ReadInt32();
                if(x >= 0)
                {
                    isLegacy = true;
                    List<string> ModDeps = new List<string>();
                    while (x-- > 0)
                        ModDeps.Add(br.ReadString());
                    Deps = ModDeps.ToArray();
                    WorkshopID = br.ReadInt32();
                    x = br.ReadInt32();
                    GUID = ExtractGUID(br.ReadBytes(x));
                    x = br.ReadInt32();
                    Image = new Texture2D(1, 1);
                    Image.LoadImage(br.ReadBytes(x));
                    x = br.ReadInt32();
                    HasBundle = x > 0;
                }
                else
                {
                    isLegacy = false;
                    x = br.ReadInt32();
                    List<string> ExternalDeps = new List<string>();
                    while (x-- > 0)
                    {
                        ExternalDeps.Add(br.ReadString() + $"({br.ReadString()})");
                        br.ReadBytes(br.ReadInt32());
                    }
                    Deps = ExternalDeps.ToArray();
                    x = br.ReadInt32();
                    GUID = ExtractGUID(br.ReadBytes(x));
                    x = br.ReadInt32();
                    Image = new Texture2D(1, 1);
                    Image.LoadImage(br.ReadBytes(x));
                    x = br.ReadInt32();
                    HasBundle = x > 0;
                }
            }
        }

        static string ExtractGUID(byte[] asmData)
        {
            return Assembly.Load(asmData).GetName().Name;
        }
    }

    class ModComparer : IComparer<ModInfo>
    {
        public int Compare(ModInfo x, ModInfo y) => Path.GetFileName(x.FilePath).CompareTo(Path.GetFileName(y.FilePath));
    }

    public class LauncherBehaviour : MonoBehaviour
    {
        List<ModInfo> mods = new List<ModInfo>();
        string LOADSON_ROOT;
        Texture2D grayTx;
        bool has_kmp;
        public void Start()
        {
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            grayTx = new Texture2D(1, 1);
            grayTx.SetPixel(0, 0, new Color(35f / 255f, 31f / 255f, 32f / 255f));
            grayTx.Apply();
            ReloadMods();
            modManagerScroll = new Vector2(0, 0);
            has_kmp = File.Exists(Path.Combine(LOADSON_ROOT, "KarlsonMP", "Preloader.dll"));
        }
        void ReloadMods()
        {
            mods.Clear();
            foreach (var file in Directory.GetFiles(Path.Combine(LOADSON_ROOT, "Mods")))
                mods.Add(new ModInfo(file));
            foreach (var file in Directory.GetFiles(Path.Combine(LOADSON_ROOT, "Mods", "Disabled")))
                mods.Add(new ModInfo(file));
            mods.Sort(new ModComparer());
        }

        bool init = false;
        GUIStyle backdrop;
        string backdropText;
        Vector2 modManagerScroll;
        ModInfo showInfoMod = null, deletePromptMod = null;
        int buttonGap;

        public void OnGUI()
        {
            if (!init)
            {
                init = true;
                backdrop = new GUIStyle(GUI.skin.label);
                backdrop.alignment = TextAnchor.MiddleCenter;
                var baseSize = backdrop.CalcSize(new GUIContent("<b>LOADSON</b>"));
                // scale baseSize to screen
                baseSize *= Screen.height / baseSize.y;
                if (baseSize.x > Screen.width)
                    baseSize *= Screen.width / baseSize.x;
                backdropText = $"<size={Mathf.FloorToInt(baseSize.y / 2)}><color=#413d3eff><b>LOADSON</b></color></size>";
                var widthLeft = Screen.width - 300;
                buttonGap = (widthLeft - 300) / 4;
                FilePicker.init();
            }
            FilePicker._ongui();
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), grayTx, new Rect(0, 0, 1, 1));
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), backdropText, backdrop);
            if (GUI.Button(new Rect(300 + buttonGap, Screen.height - 30, 100, 30), "<size=14>Start Loadson</size>")) Load();
            if (GUI.Button(new Rect(400 + 2 * buttonGap, Screen.height - 30, 100, 30), "<size=14>Start Vanilla</size>")) SceneManager.LoadScene(0);
            if (GUI.Button(new Rect(500 + 3 * buttonGap, Screen.height - 30, 100, 30), "<size=14>Exit Game</size>")) Application.Quit();
            if (!has_kmp && GUI.Button(new Rect(Screen.width - 150, 0, 150, 30), "<size=14>Download KarlsonMP</size>"))
            {
                Directory.CreateDirectory(Path.Combine(LOADSON_ROOT, "KarlsonMP"));
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(delegate { return true; });
                using (var wc = new WebClient())
                {
                    File.WriteAllBytes(Path.Combine(LOADSON_ROOT, "KarlsonMP", "Preloader.dll"), wc.DownloadData("https://raw.githubusercontent.com/karlsonmodding/KarlsonMP/refs/heads/deploy/files/Preloader.dll"));
                    has_kmp = true;
                }
            }
            if (has_kmp && GUI.Button(new Rect(Screen.width - 150, 0, 150, 30), "<size=14>Open KarlsonMP</size>")) LoadKMP();

            if (GUI.Button(new Rect(300, 0, 100, 30), "<size=14>Install Mod</size>"))
                FilePicker.PickFile("Select mod to install", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), new List<(string, string)> { ("KLM file", "*.klm"), ("KLMI file", "*.klmi"), }, (file) =>
                {
                    if (!File.Exists(file)) return;
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    if (Path.GetExtension(file) == ".klm")
                    {
                        if (File.Exists(Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm")))
                            File.Delete(Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm"));
                        if (File.Exists(Path.Combine(LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm")))
                            File.Delete(Path.Combine(LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm"));
                        File.Copy(file, Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm"));
                    }
                    else
                    {
                        if (File.Exists(Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm")))
                            File.Delete(Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm"));
                        if (File.Exists(Path.Combine(LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm")))
                            File.Delete(Path.Combine(LOADSON_ROOT, "Mods", "Disabled", fileName + ".klm"));
                        using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                        {
                            // klmi files only exist for legacy mods
                            int _extDeps = br.ReadInt32();
                            List<(string, byte[])> extDeps = new List<(string, byte[])>();
                            while (_extDeps-- > 0)
                            {
                                var name = br.ReadString();
                                var _len = br.ReadInt32();
                                var data = br.ReadBytes(_len);
                                extDeps.Add((name, data));
                            }
                            int _modSize = br.ReadInt32();
                            byte[] modData = br.ReadBytes(_modSize);
                            foreach (var dep in extDeps)
                                File.WriteAllBytes(Path.Combine(LOADSON_ROOT, "Internal", "Common deps", dep.Item1), dep.Item2);
                            File.WriteAllBytes(Path.Combine(LOADSON_ROOT, "Mods", fileName + ".klm"), modData);
                        }
                    }
                    ReloadMods();
                }, () => { });
            GUI.Window(0, new Rect(0, 0, 300, Screen.height), _ =>
            {
                if (GUI.Button(new Rect(200, 0, 100, 20), "Open Folder"))
                    Process.Start(Path.Combine(LOADSON_ROOT, "Mods"));
                if (GUI.Button(new Rect(0, 0, 100, 20), "Refresh"))
                    ReloadMods();
                modManagerScroll = GUI.BeginScrollView(new Rect(5, 20, 295, Screen.height - 20), modManagerScroll, new Rect(0, 0, 275, mods.Count * 75));
                for (int i = 0; i < mods.Count; i++)
                {
                    GUI.BeginGroup(new Rect(0, 75 * i, 275, 70));
                    GUI.Box(new Rect(0, 0, 275, 70), "");
                    if (mods[i].Enabled != GUI.Toggle(new Rect(0, 0, 20, 20), mods[i].Enabled, ""))
                    {
                        mods[i].Enabled = !mods[i].Enabled;
                        var file = Path.GetFileName(mods[i].FilePath);
                        string newFile;
                        if (mods[i].Enabled)
                            newFile = Path.Combine(LOADSON_ROOT, "Mods", file);
                        else
                            newFile = Path.Combine(LOADSON_ROOT, "Mods", "Disabled", file);
                        File.Move(mods[i].FilePath, newFile);
                        mods[i].FilePath = newFile;
                    }
                    GUI.DrawTexture(new Rect(25, 5, 60, 60), mods[i].Image);
                    if (mods[i].isLegacy)
                        GUI.Label(new Rect(90, 0, 165, 25), "<color=yellow>[L]</color> " + mods[i].Name);
                    else
                        GUI.Label(new Rect(90, 0, 165, 25), mods[i].Name);
                    GUI.Label(new Rect(90, 25, 165, 25), "by " + mods[i].Author);
                    if(GUI.Button(new Rect(90, 50, 50, 20), "Info"))
                        showInfoMod = mods[i];
                    if (GUI.Button(new Rect(145, 50, 50, 20), "Delete"))
                        deletePromptMod = mods[i];
                    GUI.EndGroup();
                }
                GUI.EndScrollView();
            }, "Mod Manager");

            if (showInfoMod != null)
                GUI.ModalWindow(1, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 225, 400, 450), _ =>
                {
                    if (GUI.Button(new Rect(350, 0, 50, 20), "Close"))
                        showInfoMod = null;
                    GUI.DrawTexture(new Rect(5, 25, 100, 100), showInfoMod.Image);
                    GUI.Label(new Rect(110, 25, 280, 100),
                        $"[File] {Path.GetFileName(showInfoMod.FilePath)}\n" +
                        $"[GUID] {showInfoMod.GUID}\n" +
                        $"[Name] {showInfoMod.Name}\n" +
                        $"[Author] {showInfoMod.Author}\n" +
                        $"[Deps] ({showInfoMod.Deps.Length}) {string.Join(", ", showInfoMod.Deps)} {(showInfoMod.isLegacy ? " <color=yellow>This mod uses a legacy format</color>." : "")}\n" +
                        (showInfoMod.HasBundle ? "This mod has an AssetBundle" : "This mod does not have an AssetBundle"));
                    GUI.Box(new Rect(5, 130, 390, 315), "");
                    GUI.Label(new Rect(10, 135, 380, 305), showInfoMod.Description);
                }, "Mod Details");

            if (deletePromptMod != null)
                GUI.ModalWindow(2, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 43, 400, 87), _ =>
                {
                    GUI.Label(new Rect(5, 20, 400, 22), "Are you sure you want to delete " + deletePromptMod.Name + "?");
                    GUI.Label(new Rect(5, 40, 400, 22), "You won't be able to get it back unless you re-install it.");
                    if(GUI.Button(new Rect(5, 62, 192, 20), "Confirm delete"))
                    {
                        File.Delete(deletePromptMod.FilePath);
                        ReloadMods();
                        deletePromptMod = null;
                    }
                    if(GUI.Button(new Rect(203, 62, 192, 20), "Cancel"))
                    {
                        deletePromptMod = null;
                    }
                }, "Delete " + deletePromptMod.Name + "?");
        }

        static bool loaded = false;
        void Load()
        {
            if (loaded)
                return;
            loaded = true;
            var asm = Assembly.LoadFrom(Path.Combine(LOADSON_ROOT, "Internal", "Loadson.dll"));
            asm.GetType("LoadsonInternal.Loader").GetMethod("Start").Invoke(null, new object[] { Entrypoint.HasDiscordAPI });
            Destroy(gameObject);
        }

        static bool kmp_loaded = false;
        void LoadKMP()
        {
            if (kmp_loaded) return;
            kmp_loaded = true;
            // add kmp assembly resolver
            AppDomain.CurrentDomain.AssemblyResolve += KarlsonMP_AssemblyResolve;
            var asm = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(LOADSON_ROOT, "KarlsonMP", "Preloader.dll")));
            asm.GetType("Preloader.Entrypoint").GetMethod("Start").Invoke(null, Array.Empty<object>());
            SceneManager.LoadScene(0);
        }

        private Assembly KarlsonMP_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var resolved = Path.Combine(LOADSON_ROOT, "KarlsonMP", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFrom(resolved);
            return null;
        }
    }
}
