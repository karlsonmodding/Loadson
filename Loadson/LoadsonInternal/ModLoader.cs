using Loadson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonInternal
{
    public class ModLoader
    {
        public static void SafeCall(Action call)
        {
            try
            {
                call();
            }
            catch (Exception e)
            {
                Console.Log(e.ToString());
            }
        }

        public static void BuildList()
        {
            Console.Log("Loadson root: " + Loader.LOADSON_ROOT);
            foreach (string file in from x in Directory.GetFiles(Path.Combine(Loader.LOADSON_ROOT, "Mods")) where x.EndsWith(".klm") select x)
            {
                using (BinaryReader br = new BinaryReader(File.OpenRead(file)))
                {
                    string ModName = br.ReadString();
                    string ModAuthor = br.ReadString();
                    string ModDescription = br.ReadString();
                    Console.Log("Preloading [" + Path.GetFileName(file) + "] " + ModName + " by " + ModAuthor);
                    int _modDepCount = br.ReadInt32();
                    List<string> ModDeps = new List<string>();
                    for (int i = 0; i < _modDepCount; i++)
                        ModDeps.Add(br.ReadString());
                    int ModWorkshopID = br.ReadInt32();
                    int _modSize = br.ReadInt32();
                    byte[] ModBinary = br.ReadBytes(_modSize);
                    int _iconSize = br.ReadInt32();
                    byte[] ModIcon = br.ReadBytes(_iconSize);
                    int _assetBundleSize = br.ReadInt32();
                    byte[] ModAssetBundle = br.ReadBytes(_assetBundleSize);
                    ModEntry.List.Add(new ModEntry(ModName, ModAuthor, ModDescription, ModDeps, ModWorkshopID, ModBinary, ModIcon, ModAssetBundle, file));
                }
            }
        }

        public static int LoadedMods { get; private set; } = 0;
        public static void LoadList()
        {
            Console.Log("Injecting assembly resolve");
            AppDomain.CurrentDomain.AssemblyResolve += GenericAssemblyResolve;
            Console.Log("Loading mods..");
            int loadedMods = 1, countall = 0;
            while (loadedMods > 0)
            {
                loadedMods = 0;
                foreach (ModEntry mod in from x in ModEntry.List where x.Deps.Count == 0 && x.instance == null select x)
                {
                    Console.Log("Loading " + mod.DisplayName);
                    mod.assembly = AppDomain.CurrentDomain.Load(mod.AsmData);
                    Type t = (from x in mod.assembly.GetTypes()
                              where x.BaseType == typeof(Mod)
                              select x).FirstOrDefault();
                    if (t == null)
                    {
                        Console.Log("<color=red> Couldn't find base type</color>");
                        Console.OpenConsole();
                        continue;
                    }
                    mod.instance = (Mod)Activator.CreateInstance(t, null);
                    if (mod.instance == null)
                    {
                        Console.Log("<color=red> Loading failed..</color>");
                        Console.OpenConsole();
                        ModEntry.List.Remove(mod);
                        continue;
                    }
                    SafeCall(mod.instance.OnEnable);
                    mod.enabled = true;
                    Console.Log("<i> </i>Loaded succesfully");

                    foreach (ModEntry clear in from x in ModEntry.List where x.Deps.Contains(mod.ModGUID) select x) clear.Deps.Remove(mod.ModGUID);
                    loadedMods++;
                    countall++;
                }
            }
            if (countall < ModEntry.List.Count)
            {
                Console.Log("<color=red>Not all mods were loaded</color>");
                foreach (ModEntry mod in from x in ModEntry.List where x.Deps.Count > 0 select x)
                {
                    string s = "";
                    foreach (string d in mod.Deps) s += d + ',';
                    Console.Log("[" + mod.ModGUID + "] Missing: " + s);
                    ModEntry.List.Remove(mod);
                }
                Console.OpenConsole();
            }
            Console.Log("Loaded " + countall + " mods");
            LoadedMods = countall;
            Loader.activity.Details = "Playing with " + countall + " mods";
            Loader.discord.GetActivityManager().UpdateActivity(Loader.activity, (_) => { });
        }

        private static Assembly GenericAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.Log("Resolving " + new AssemblyName(args.Name).Name + " (" + args.Name + ")");
            string resolved = Path.Combine(Loader.LOADSON_ROOT, "Internal", "Loadson deps", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFile(resolved);
            resolved = Path.Combine(Loader.LOADSON_ROOT, "Internal", "Common deps", new AssemblyName(args.Name).Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFile(resolved);
            ModEntry resolveMod = (from x in ModEntry.List where x.assembly.FullName == args.Name select x).FirstOrDefault(null);
            if (resolveMod != null) return resolveMod.assembly;
            return null; // jump to next
        }
    }
}
