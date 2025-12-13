#if !LoadsonAPI
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
        public static T SafeCall<T>(Func<T> call)
        {
            try
            {
                return call();
            }
            catch(Exception e)
            {
                Console.Log(e.ToString());
                return default;
            }
        }

        static readonly Dictionary<string, (System.Version, byte[])> external_deps = new Dictionary<string, (System.Version, byte[])>();

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
                    int _modDepCount = br.ReadInt32();
                    if(_modDepCount >= 0)
                    {
                        // legacy loading
                        Console.Log("Preloading LEGACY [" + Path.GetFileName(file) + "] " + ModName + " by " + ModAuthor);
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
                    else if(_modDepCount == -1)
                    {
                        Console.Log("Preloading [" + Path.GetFileName(file) + "] " + ModName + " by " + ModAuthor);
                        int count = br.ReadInt32();
                        while(count-- > 0)
                        {
                            string name = br.ReadString();
                            System.Version ver = System.Version.Parse(br.ReadString());
                            Console.Log($"  External dependency {name} ({ver})");
                            byte[] bytes = br.ReadBytes(br.ReadInt32());
                            if (!external_deps.ContainsKey(name))
                                external_deps.Add(name, (ver, bytes));
                            else
                            {
                                // check if versions are compatible (ie major is the same)
                                // pick the newer one if that's the case
                                // else don't load this mod and show error.
                                System.Version old_version = external_deps[name].Item1;
                                if(old_version.Major == ver.Major)
                                {
                                    if (ver.CompareTo(old_version) > 0)
                                    {
                                        external_deps[name] = (ver, bytes);
                                        Console.Log($"    Using this version instead of the previously loaded one.");
                                    }
                                    else
                                    {
                                        Console.Log($"    Ignored because a newer or the same version is already loaded.");
                                    }
                                }
                                else
                                {
                                    Console.Log($"    <color=red>This version is incompatible with the already loaded one ({old_version}).\n    Not loading this mod due to this conflict.</color>");
                                    continue;
                                }
                            }
                        }
                        int _modSize = br.ReadInt32();
                        byte[] ModBinary = br.ReadBytes(_modSize);
                        int _iconSize = br.ReadInt32();
                        byte[] ModIcon = br.ReadBytes(_iconSize);
                        int _assetBundleSize = br.ReadInt32();
                        byte[] ModAssetBundle = br.ReadBytes(_assetBundleSize);
                        ModEntry.List.Add(new ModEntry(ModName, ModAuthor, ModDescription, ModBinary, ModIcon, ModAssetBundle, file));
                    }
                    else
                    {
                        Console.Log("<color=red>[" + Path.GetFileName(file) + "] " + ModName + " by " + ModAuthor + " reported unknown mod version " + (-_modDepCount) + ".</color>");
                        continue;
                    }
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
            if(Loader.discord_exists)
            {
                DiscordRPC.UpdateRPC(details: "Playing with " + countall + " mods");
            }
        }

        private static Assembly GenericAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var Name = new AssemblyName(args.Name);
            Console.Log("Resolving " + Name.Name + " (" + args.Name + ")");

            string resolved = Path.Combine(Loader.LOADSON_ROOT, "Internal", "Loadson deps", Name.Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFile(resolved);

            if (external_deps.ContainsKey(Name.Name))
                // don't really need to check for compatiblity because preloader takes care of that
                return Assembly.Load(external_deps[Name.Name].Item2);

            // common deps will also get depracated in favor of embedded ones
            resolved = Path.Combine(Loader.LOADSON_ROOT, "Internal", "Common deps", Name.Name + ".dll");
            if (File.Exists(resolved)) return Assembly.LoadFile(resolved);

            ModEntry resolveMod = (from x in ModEntry.List where x.assembly.FullName == args.Name select x).FirstOrDefault(null);
            if (resolveMod != null) return resolveMod.assembly;

            return null; // jump to next
        }
    }
}
#endif