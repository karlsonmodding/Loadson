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
            try
            {
                foreach(ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                    mod.instance.OnGUI();
            } catch { }
            Console._ongui();
        }

        public void Update()
        {
            try
            {
                foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                    mod.instance.Update(Time.deltaTime);
            }
            catch { }
            Console._update();
        }

        public void FixedUpdate()
        {
            try
            {
                foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                    mod.instance.FixedUpdate(Time.fixedDeltaTime);
            }
            catch { }
        }

        public void OnApplicationQuit()
        {
            try
            {
                foreach (ModEntry mod in from x in ModEntry.List where x.instance != null select x)
                    mod.instance.OnDisable();
            }
            catch { }
            Process.GetCurrentProcess().Kill();
        }
    }
}
