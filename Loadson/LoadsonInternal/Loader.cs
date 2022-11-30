using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LoadsonInternal
{
    public class Loader
    {
        #region >>> PInvoke
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(IntPtr hwnd, string lpString);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string className, string windowName);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr hWnd, string text, string caption, uint type);
        #endregion

        public static string LOADSON_ROOT;
        public static AppDomain rootDomain;
        public static Harmony Harmony;
        public static MonoHooks MonoHooks;

        public static void Start()
        {
            Application.logMessageReceived += (condition, stackTrace, _) => Console.Log(condition + " " + stackTrace);

            IntPtr hWnd = FindWindow(null, Application.productName);
            SetWindowText(hWnd, "Loadson");

            if (Environment.GetEnvironmentVariable("KarlsonLoaderDir") == null)
            {
                MessageBox(IntPtr.Zero, "There was an error launching Loadson (no environment variable set). Please re-launch with Loadson.exe", "[Loadson] Error", 0x00040010);
                Process.GetCurrentProcess().Kill();
                return;
            }

            LOADSON_ROOT = Environment.GetEnvironmentVariable("KarlsonLoaderDir");

            Harmony = new Harmony("loadson");
            Harmony.PatchAll();

            GameObject go = new GameObject("Loadson_MonoHooks");
            MonoHooks = go.AddComponent<MonoHooks>();
            UnityEngine.Object.DontDestroyOnLoad(go);
        }
    }
}
