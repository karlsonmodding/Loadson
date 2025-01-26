using HarmonyLib;
using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace LoadsonAPI
{
    public class TimerText
    {
        public delegate string resolver();

        /// <summary>
        /// Add text below the in-game timer.
        /// </summary>
        /// <param name="text">Lambda function that returns a string. If result is empty string "", nothing is printed.</param>
        public static void AddText(resolver text)
        {
            // get mod instance
            ModEntry e = null;
            // selector doesn't work, don't ask me why, i'm going insane
            foreach (var a in ModEntry.List)
            {

                if (a.assembly.GetName().Name == Assembly.GetCallingAssembly().GetName().Name)
                {
                    e = a;
                    break;
                }
            }
            if (e == null)
            {
                LoadsonInternal.Console.Log("<color=red>Couldn't find matching mod for " + Assembly.GetCallingAssembly().GetName() + "</color>");
                LoadsonInternal.Console.OpenConsole();
                return;
            }
            if (e.enabled)
            {
                LoadsonInternal.Console.Log("<color=red>[" + e.ModGUID + "] Tried adding timer text outside of OnEnable</color>");
                LoadsonInternal.Console.OpenConsole();
                return;
            }
            strings.Add(text);
        }

        public static List<resolver> strings = new List<resolver>();
    }

    [HarmonyPatch(typeof(Timer), "Update")]
    public class Hook_Timer_Update
    {
        public static void Postfix(Timer __instance, TextMeshProUGUI ___text, float ___timer)
        {
            ___text.text = __instance.GetFormattedTime(___timer);
            foreach(var r in TimerText.strings)
            {
                string res = ModLoader.SafeCall(() => r());
                if (res == null || res.Length == 0) continue;
                ___text.text += "\n" + res;
            }
        }
    }
}
