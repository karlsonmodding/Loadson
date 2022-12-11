using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LoadsonInternal
{
    public class ModEntry
    {
        public static List<ModEntry> List = new List<ModEntry>();

        public string ModGUID;
        public string DisplayName;
        public string Author;
        public string Description;
        public List<string> Deps = new List<string>();
        public string[] DepsRef;
        public int WorkshopId;

        public byte[] AsmData;
        public Texture2D Icon;
        public AssetBundle AssetBundle;

        public Assembly assembly;
        public Loadson.Mod instance = null;
        public bool enabled = false;

        public ModEntry(string _ModGUID, string _ModName, string _ModAuthor, string _ModDescription, List<string> _ModDeps, int _ModWorkshopID, byte[] _AsmData, byte[] icon, byte[] assetbundle)
        {
            ModGUID = _ModGUID;
            DisplayName = _ModName;
            Author = _ModAuthor;
            Description = _ModDescription;
            Deps = _ModDeps;
            DepsRef = Deps.ToArray();
            WorkshopId = _ModWorkshopID;
            AsmData = _AsmData;
            Icon = new Texture2D(64, 64);
            Icon.LoadImage(icon);
            AssetBundle = AssetBundle.LoadFromMemory(assetbundle);
        }
    }
}
