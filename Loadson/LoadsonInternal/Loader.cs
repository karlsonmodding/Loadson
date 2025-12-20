#if !LoadsonAPI
using Discord;
using HarmonyLib;
using LoadsonAPI;
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
        public static string LOADSON_ROOT;
        public static Harmony Harmony;
        public static MonoHooks MonoHooks;
        public static Discord.Discord discord;
        public static Activity activity;
        public static string discord_token = "";
        public static User discord_user = new User { Id = 0 };
        public static bool discord_exists;
        public static long DISCORD_CLIENTID = 1101662131868409947;
        public static bool discord_lib_installed;

        public enum Platform
        {
            Unknown,
            Win64,
            Win32,
            Linux,
            MacOS
        }
        public static Platform platform;

        public static void Start(bool discord)
        {
            discord_lib_installed = discord;

            // determine platform
            var managed_path = Application.dataPath;
            if (managed_path.EndsWith("Karlson_Data"))
            {
                if (Environment.Is64BitProcess)
                    platform = Platform.Win64;
                else
                    platform = Platform.Win32;
            }
            else if (managed_path.EndsWith("Karlson_linux_Data"))
                platform = Platform.Linux;
            else
                platform = Platform.MacOS;

            UnityEngine.Debug.Log("Hello from LoadsonInternal");
            Preferences.Load();

            Application.logMessageReceived += (condition, stackTrace, _) =>
            {
                if(Preferences.instance.unityLog) Console.Log(condition + " " + stackTrace);
            };

            LOADSON_ROOT = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Loadson");
            
            GameObject go = new GameObject("Loadson_MonoHooks");
            MonoHooks = go.AddComponent<MonoHooks>();
            UnityEngine.Object.DontDestroyOnLoad(go);

            Harmony = new Harmony("loadson");
            try
            {
                Harmony.PatchAll();
                if(platform == Platform.Linux)
                {
                    Harmony.Patch(typeof(PlayerMovement).GetMethod("Pause", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance), postfix: new HarmonyMethod(typeof(Hook_PlayerMovement_Pause).GetMethod("PausePostfix")));
                }
            } catch (Exception e)
            {
                Console.Log(e.ToString());
                Console.OpenConsole();
            }

            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }

        public static void InitDiscord()
        {
            if(!discord_lib_installed)
            {
                discord_exists = false;
                Console.Log($"<color=red>Failed to initialize Discord. Discord Game SDK is not installed</color>");
                return;
            }
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

            DiscordRPC.Init();
        }
    }
}
#endif