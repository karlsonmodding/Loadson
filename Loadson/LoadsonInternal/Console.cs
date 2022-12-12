using LoadsonAPI;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace LoadsonInternal
{
    public class Console
    {
        private static string content = "Loadson\n  made by devilExE\n  licensed under MIT license\n\n";
        public static void Log(string s)
        {
            if(Preferences.instance.fileLog) File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log"), s + "\n");
            content += s + '\n';
            if(content.Split('\n').Length > 500) content = content.Substring(content.IndexOf("\n") + 1);
        }

        public static void Init()
        {
            windowId = ImGUI_WID.GetWindowId();
            if (Preferences.instance.fileLog) File.AppendAllText(Path.Combine(Directory.GetCurrentDirectory(), "log"), $"\n\n[{DateTime.Now}]\n");
        }
        private static int windowId = -1;

        private static bool consoleOpen = false;
        public static void OpenConsole() { consoleOpen = true; }

        private static Texture2D blackTx = new Texture2D(1, 1);

        private static Rect consoleWindow = new Rect(50, 50, 1000, 800);
        private static Vector2 consoleScroll = new Vector2(0, 0);

        //public static string startLog = "Loadson\n  made by devilExE\n  licensed under MIT license\n\n";
        public static void _ongui()
        {
            if (consoleOpen)
            {
                consoleWindow = GUI.Window(windowId, consoleWindow, (_) =>
                {
                    GUI.DragWindow(new Rect(0, 0, 1000, 20));
                    Vector2 size = GUI.skin.label.CalcSize(new GUIContent(content));
                    consoleScroll = GUI.BeginScrollView(new Rect(5, 20, 995, 780), consoleScroll, new Rect(0, 0, size.x + 50, size.y + 50));
                    GUI.Label(new Rect(0, 0, size.x + 50, size.y + 50), content);
                    GUI.EndScrollView();
                }, "Loadson console");
            }
            if (!Hook_Managers_Start.done)
            {
                blackTx.SetPixel(0, 0, new Color(35f / 255f, 31f / 255f, 32f / 255f));
                blackTx.Apply();
                GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, Screen.width, Screen.height), blackTx, new Rect(0, 0, 1, 1));
                GUI.Label(new Rect(0f, 0f, Screen.width, Screen.height), "[Loadson]\n" + content);
            }
            GUI.Label(new Rect(0f, Screen.height - 20f, 1000f, 100f), "<b>Loadson v" + Version.ver + "</b> Loaded " + ModLoader.LoadedMods + " mods.");
        }

        public static void _update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash)) consoleOpen = !consoleOpen;
        }
    }
}
