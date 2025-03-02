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
            foreach(ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                ModLoader.SafeCall(mod.instance.OnGUI);
            Console._ongui();
            ModMenu._ongui();
            LoadsonAPI.FilePicker._ongui();
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