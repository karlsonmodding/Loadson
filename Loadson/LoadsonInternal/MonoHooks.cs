#if !LoadsonAPI
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
    public class MonoHooks : MonoBehaviour
    {
        public void OnGUI()
        {
            Console._ongui();
            ModMenu._ongui();
            Launcher.FilePicker._ongui(); // take over the launcher mono behaviour
            foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                ModLoader.SafeCall(mod.instance.OnGUI);

            GUI.Label(new Rect(1, Screen.height - 17, 1000, 100), string.Format("<b>Loadson v{0}</b> Loaded {1}{2} mod{3}.", Version.ver, Hook_Managers_Start.unity_exporer ? "UE and " : "", ModLoader.LoadedMods, ModLoader.LoadedMods == 1 ? "" : "s"));
        }

        public void Update()
        {
            foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                ModLoader.SafeCall(() => mod.instance.Update(Time.deltaTime));
            Console._update();
            if(Loader.discord_exists)
            {
                try
                {
                    Loader.discord.RunCallbacks();
                }
                catch
                {
                    // disable discord if error
                    Loader.discord_exists = false;
                }
            }
        }

        public void FixedUpdate()
        {
            foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                ModLoader.SafeCall(() => mod.instance.FixedUpdate(Time.fixedDeltaTime));
        }

        public void OnApplicationQuit()
        {
            foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                ModLoader.SafeCall(mod.instance.OnDisable);
            Loadson.Preferences._save();
            Preferences.Save();
            Process.GetCurrentProcess().Kill();
        }
    }
}
#endif