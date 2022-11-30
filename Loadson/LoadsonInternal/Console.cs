using System;
using System.Linq;
using UnityEngine;

namespace LoadsonInternal
{
    public class Console
    {
        private static string content = "Loadson\n  made by devilExE\n  licensed under MIT license\n\n";
        public static void Log(string s)
        {
            content += s + '\n';
            startLog += s + "\n";
            if (content.Split('\n').Count() > 40)
                content = content.Substring(content.IndexOf('\n') + 1);
        }

        private static bool consoleOpen = false;
        public static void OpenConsole() { consoleOpen = true; }

        private static Texture2D blackTx = new Texture2D(1, 1);

        public static string startLog = "Loadson\n  made by devilExE\n  licensed under MIT license\n\n";
        public static void _ongui()
        {
            if (consoleOpen)
            {
                GUI.Box(new Rect(5f, 5f, 1500f, 825f), "Loadson console");
                GUI.Label(new Rect(10f, 25f, 1490f, 800f), content);
            }
            if (!Hook_Managers_Start.done)
            {
                blackTx.SetPixel(0, 0, new Color(35f / 255f, 31f / 255f, 32f / 255f));
                blackTx.Apply();
                GUI.DrawTextureWithTexCoords(new Rect(0f, 0f, Screen.width, Screen.height), blackTx, new Rect(0, 0, 1, 1));
                if (startLog.Split('\n').Length >= 50)
                    startLog = startLog.Substring(startLog.IndexOf('\n') + 1);
                GUI.Label(new Rect(100f, 100f, 1000f, 1000f), "[Loadson]\n" + startLog);
            }
            GUI.Label(new Rect(0f, Screen.height - 20f, 1000f, 100f), "<b>Loadson v" + Version.ver + "</b> Loaded " + ModLoader.LoadedMods + " mods.");
        }

        public static void _update()
        {
            if (Input.GetKeyDown(KeyCode.Backslash)) consoleOpen = !consoleOpen;
        }
    }
}
