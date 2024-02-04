using HarmonyLib;
using System;
using System.Collections;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LoadsonInternal
{
    [HarmonyPatch(typeof(Managers), "Start")]
    public class Hook_Managers_Start
    {
        public static bool done { get; private set; } = false;
        public static bool Prefix(Managers __instance)
        {
            Console.Init();
            Console.Log("Detoured Managers.Start");

            ForcedCultureInfo.Install();

            ModMenu._init();
            Loadson.Preferences._load();

            KernelUpdater.CheckForUpdates();

            // check for unity explorer
            if(File.Exists(Path.Combine(Loader.LOADSON_ROOT, "UnityExplorer.STANDALONE.Mono.dll")))
            {
                Assembly obe = AppDomain.CurrentDomain.Load(File.ReadAllBytes(Path.Combine(Loader.LOADSON_ROOT, "UnityExplorer.STANDALONE.Mono.dll")));
                obe.GetType("UnityExplorer.ExplorerStandalone").GetMethod("CreateInstance", Array.Empty<Type>()).Invoke(null, Array.Empty<object>());
            }

            Loader.InitDiscord();
            UnityEngine.Object.DontDestroyOnLoad(__instance.gameObject);
            Time.timeScale = 1f;
            Application.targetFrameRate = 240;
            QualitySettings.vSyncCount = 0;

            AudioListener.volume = 0;
            AudioListener.pause = true;

            // load tutorial 0
            SceneManager.sceneLoaded += _scene;
            UnityEngine.Object.Destroy(Audio.AudioManager.Instance);
            SceneManager.LoadScene("0Tutorial", LoadSceneMode.Single);
            return false;
        }

        private static void _scene(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "MainMenu")
            {
                if (!done)
                {
                    Loader.MonoHooks.StartCoroutine(AudioPatch());

                    done = true;
                }
                (from x in UnityEngine.Object.FindObjectsOfType<TMPro.TextMeshProUGUI>() where x.text == "KARLSON" select x).First()
                    .text = "LOADSON\n<size=15><cspace=-0.1em>Version <cspace=-0.2em>" + Version.ver + "</cspace></cspace></size>";
                ModMenu._scene();
                MenuCustom._scene();
                PreferencesCustom._scene();
            }
            if (done) return;
            if (scene.name == "0Tutorial")
            {
                Console.Log("Initializing prefabs.. Tutorial (1/2)");
                LoadsonAPI.PrefabManager.Init();
                SceneManager.LoadScene("4Escape0", LoadSceneMode.Single);
            }
            if (scene.name == "4Escape0")
            {
                Console.Log("Initializing prefabs.. Escape 0 (2/2)");
                LoadsonAPI.PrefabManager.Init2();
                SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
                Console.Log("Building mod list..");
                ModLoader.BuildList();
                Console.Log("Mod count: " + ModEntry.List.Count + "\nLoading mods..");
                ModLoader.LoadList();
            }
        }

        // Credit: https://github.com/karlsonmodding/KarlsonTAS/blob/main/Main.cs#L109 - Mang432
        private static IEnumerator AudioPatch()
        {
            yield return new WaitForSeconds(0.1f);
            Console.Log("running audio patch");
            AudioListener.volume = 1;
            AudioListener.pause = false;
            foreach (GameObject ga in Resources.FindObjectsOfTypeAll<GameObject>())
            {
                Options oa = ga.GetComponent<Options>();
                if (oa != null)
                {
                    ga.SetActive(true);
                    oa.enabled = true;
                    yield return null;
                    ga.SetActive(false);
                    break;
                }
            }
        }
    }

    [HarmonyPatch(typeof(Debug), "Start")]
    public class Hook_Debug_Start
    {
        public static void Postfix(ref bool ___fpsOn, ref bool ___speedOn)
        {
            if(Preferences.instance.forceFpsAndSpeed)
            {
                ___fpsOn = true;
                ___speedOn = true;
            }
        }
    }
}