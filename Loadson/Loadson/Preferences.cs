using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Loadson
{
    public class Preferences
    {
        /// <summary>
        /// Get your set preferences. All modifications to the dictionary are automatically saved.
        /// </summary>
        /// <returns>Your dictionary. Empty dictionary if it doesn't exist (first load)</returns>
        public static Dictionary<string, string> GetPreferences()
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
                return default;
            }
            if (!all_data.ContainsKey(e.ModGUID))
                all_data.Add(e.ModGUID, new Dictionary<string, string>());
            return all_data[e.ModGUID];
        }

        private static Dictionary<string, Dictionary<string, string>> all_data = new Dictionary<string, Dictionary<string, string>>();

        public static void _load()
        {
            Console.Log("Loading user preferences");
            if(!PlayerPrefs.HasKey("LoadsonUserPrefs"))
            {
                Console.Log("creating save");
                _save();
            }
            all_data = Decode(PlayerPrefs.GetString("LoadsonUserPrefs"));
        }

        public static void _save()
        {
            Console.Log("saving player preferences");
            PlayerPrefs.SetString("LoadsonUserPrefs", Encode(all_data));
        }

        private static Dictionary<string, Dictionary<string, string>> Decode(string data)
        {
            Dictionary<string, Dictionary<string, string>> dict = new Dictionary<string, Dictionary<string, string>>();
            using (BinaryReader br = new BinaryReader(new MemoryStream(Convert.FromBase64String(data))))
            {
                int count = br.ReadInt32();
                while(count-- > 0)
                {
                    string name = br.ReadString();
                    dict.Add(name, new Dictionary<string, string>());
                    int _cnt = br.ReadInt32();
                    while(_cnt-- > 0 ) {
                        dict[name].Add(br.ReadString(), br.ReadString());
                    }
                }
            }
            return dict;
        }

        private static string Encode(Dictionary<string, Dictionary<string, string>> data)
        {
            using(MemoryStream ms = new MemoryStream())
            using(BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write(data.Count);
                Console.Log(""+data.Count);
                foreach(var kv in data.ToList())
                {
                    bw.Write(kv.Key);
                    Console.Log("" + kv.Key);
                    bw.Write(kv.Value.Count);
                    Console.Log("" + kv.Value.Count);
                    foreach (var kv2 in kv.Value.ToList())
                    {
                        bw.Write(kv2.Key);
                        Console.Log("" + kv2.Key);
                        bw.Write(kv2.Value);
                        Console.Log("" + kv2.Value);
                    }
                }
                bw.Flush();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }
}
