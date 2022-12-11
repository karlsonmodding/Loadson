﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        #endregion

        public static string LOADSON_ROOT;
        public static Harmony Harmony;
        public static MonoHooks MonoHooks;

        public static void Start()
        {
            Preferences.Load();

            if (Preferences.instance.unityLog) Console.Init();
            Application.logMessageReceived += (condition, stackTrace, _) =>
            {
                if(Preferences.instance.unityLog) Console.Log(condition + " " + stackTrace);
            };

            IntPtr hWnd = FindWindow(null, Application.productName);
            SetWindowText(hWnd, "Loadson");

            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            
            GameObject go = new GameObject("Loadson_MonoHooks");
            MonoHooks = go.AddComponent<MonoHooks>();
            UnityEngine.Object.DontDestroyOnLoad(go);

            ModMenu._init();

            Harmony = new Harmony("loadson");
            try
            {
                Harmony.PatchAll();
            } catch (Exception e)
            {
                Console.Log(e.ToString());
                Console.OpenConsole();
            }
            
        }
    }
}
