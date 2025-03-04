using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
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
        public void Start()
        {
            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            grayTx = new Texture2D(1, 1);
            grayTx.SetPixel(0, 0, new Color(35f / 255f, 31f / 255f, 32f / 255f));
            grayTx.Apply();
            ReloadMods();
            modManagerScroll = new Vector2(0, 0);
        }
        void ReloadMods()
        {
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
            }
            GUI.DrawTextureWithTexCoords(new Rect(0, 0, Screen.width, Screen.height), grayTx, new Rect(0, 0, 1, 1));
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), backdropText, backdrop);
            if (GUI.Button(new Rect(300 + buttonGap, Screen.height - 30, 100, 30), "<size=14>Start Loadson</size>")) Load();
            if (GUI.Button(new Rect(400 + 2 * buttonGap, Screen.height - 30, 100, 30), "<size=14>Start Vanilla</size>")) SceneManager.LoadScene(0);
            if (GUI.Button(new Rect(500 + 3 * buttonGap, Screen.height - 30, 100, 30), "<size=14>Exit Game</size>")) Application.Quit();

            GUI.Window(0, new Rect(0, 0, 300, Screen.height), _ =>
            {
                if (GUI.Button(new Rect(200, 0, 100, 20), "Open Folder"))
                    Process.Start(Path.Combine(LOADSON_ROOT, "Mods"));
                modManagerScroll = GUI.BeginScrollView(new Rect(5, 20, 295, Screen.height - 20), modManagerScroll, new Rect(0, 0, 275, mods.Count * 65 - 5));
                for (int i = 0; i < mods.Count; i++)
                {
                    GUI.BeginGroup(new Rect(0, 65 * i, 275, 60));
                    GUI.Box(new Rect(0, 0, 275, 60), "");
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
                    GUI.DrawTexture(new Rect(25, 0, 60, 60), mods[i].Image);
                    GUI.Label(new Rect(90, 0, 165, 20), mods[i].Name);
                    GUI.Label(new Rect(90, 20, 165, 20), "by " + mods[i].Author);
                    if(GUI.Button(new Rect(90, 40, 50, 20), "Info"))
                        showInfoMod = mods[i];
                    if (GUI.Button(new Rect(145, 40, 50, 20), "Delete"))
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
                        $"[Deps] ({showInfoMod.Deps.Length}) {string.Join(", ", showInfoMod.Deps)}\n" +
                        (showInfoMod.HasBundle ? "This mod has an AssetBundle" : "This mod does not have an AssetBundle"));
                    GUI.Box(new Rect(5, 130, 390, 315), "");
                    GUI.Label(new Rect(10, 135, 380, 305), showInfoMod.Description);
                }, "Mod Details");

            if (deletePromptMod != null)
                GUI.ModalWindow(2, new Rect(Screen.width / 2 - 200, Screen.height / 2 - 40, 400, 85), _ =>
                {
                    GUI.Label(new Rect(5, 20, 400, 20), "Are you sure you want to delete " + deletePromptMod.Name + "?");
                    GUI.Label(new Rect(5, 40, 400, 20), "You won't be able to get it back unless you re-install it.");
                    if(GUI.Button(new Rect(5, 60, 195, 20), "Confirm delete"))
                    {
                        File.Delete(deletePromptMod.Name);
                        deletePromptMod = null;
                        ReloadMods();
                    }
                    if(GUI.Button(new Rect(200, 60, 195, 20), "Cancel"))
                    {
                        deletePromptMod = null;
                    }
                }, "Delete " + deletePromptMod.Name + "?");
        }

        void Load()
        {
            var asm = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(LOADSON_ROOT, "Internal", "Loadson.dll")));
            asm.GetType("LoadsonInternal.Loader").GetMethod("Start").Invoke(null, new object[] { Entrypoint.HasDiscordAPI });
        }
    }
}
