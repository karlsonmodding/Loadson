using Discord;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);
        #endregion

        public static string LOADSON_ROOT;
        public static Harmony Harmony;
        public static MonoHooks MonoHooks;
        public static Discord.Discord discord;
        public static Activity activity;
        public static string discord_token = "";
        public static User discord_user = new User { Id = 0 };
        public static bool discord_exists;
        public static long DISCORD_CLIENTID = 1101662131868409947;

        public static void Start()
        {
            Preferences.Load();

            Console.Init();
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
            Loadson.Preferences._load();

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

        public static void InitDiscord()
        {
            try
            {
                discord = new Discord.Discord(DISCORD_CLIENTID, (uint)CreateFlags.NoRequireDiscord);
                discord.SetLogHook(LogLevel.Debug, (LogLevel level, string message) =>
                {
                    Console.Log($"[Discord/{level}] {message}");
                });
                discord.RunCallbacks();
            }
            catch (ResultException result)
            {
                discord_exists = false;
                Console.Log($"<color=red>Failed to initialize Discord. {result}</color>");
                Console.OpenConsole();
                return;
            }
            discord_exists = true;
            discord.GetApplicationManager().GetOAuth2Token((Result result, ref Discord.OAuth2Token token) =>
            {
                if (result != Result.Ok) return;
                discord_token = token.AccessToken;
            });
            discord.GetUserManager().OnCurrentUserUpdate += () =>
            {
                discord_user = discord.GetUserManager().GetCurrentUser();
            };

            activity = new Activity
            {
                ApplicationId = DISCORD_CLIENTID,
                Assets = new ActivityAssets
                {
                    LargeImage = "loadson",
                    LargeText = "Loadson v" + Version.ver,
                    SmallImage = "karlson",
                    SmallText = "Karlson (itch.io) made by Dani"
                },
                Details = "Loading mods..",
                State = "github.com/karlsonmodding",
                Timestamps = new ActivityTimestamps
                {
                    Start = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
                }
            };
            discord.GetActivityManager().UpdateActivity(activity, (_) => { });
        }
    }
}
