using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace ModBuilder
{
    class Program
    {
        public const string API_ENDPOINT = "https://raw.githubusercontent.com/karlsonmodding/Loadson/deploy";
        public const string VERSION = "v4";

        [STAThread]
        static void Main(string[] args)
        {
            DateTime start = DateTime.Now;
            Console.Title = "Loadson ModBuilder";
            Console.WriteLine("Loadson");
            Console.WriteLine("  made by devilExE");
            Console.WriteLine("  licensed under MIT License\n");
            Console.WriteLine("Loadson ModBuilder " + VERSION);
            if (!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoadsonMod.sln")))
            {
                Console.WriteLine("Couldn't find LoadsonMod.sln");
                Console.WriteLine("Make sure you only run this in your mod source folder directory");
                Console.ReadKey();
                return;
            }
            if (!Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib")))
            {
                if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "run ModBuilder.exe to initialize project")))
                    File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "run ModBuilder.exe to initialize project"));
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib"));
                string gameRoot = "";
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    Console.WriteLine("Select Karlson.exe file from your game to get the game's root directory for assembly files");
                    ofd.Title = "Select your Karlson instalation";
                    ofd.Filter = "Karlson executable file|Karlson.exe";
                    ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    DialogResult dr = ofd.ShowDialog();
                    if (dr == DialogResult.OK)
                        gameRoot = Path.GetDirectoryName(ofd.FileName);
                    else
                        return;
                }
                Console.WriteLine("Game root: " + gameRoot);
                Console.WriteLine("Copying game files..");
                foreach (string f in Directory.GetFiles(Path.Combine(gameRoot, "Karlson_Data", "Managed")))
                {
                    if (!f.EndsWith(".dll")) continue;
                    if (Path.GetFileName(f) == "Assembly-CSharp.dll" || Path.GetFileName(f) == "Unity.TextMeshPro.dll" || Path.GetFileName(f).StartsWith("UnityEngine"))
                    {
                        File.Copy(f, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", Path.GetFileName(f)));
                        Console.WriteLine("Copied " + Path.GetFileName(f));
                    }
                }
                Console.WriteLine("Downloading LoadsonAPI.dll");
                HttpClient hc = new HttpClient();
                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "LoadsonAPI.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/loadsonapi/LoadsonAPI.dll").GetAwaiter().GetResult());
                File.WriteAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lib", "LoadsonAPI.xml"), hc.GetByteArrayAsync(API_ENDPOINT + "/loadsonapi/LoadsonAPI.xml").GetAwaiter().GetResult());
                Console.WriteLine("ModBuilder installed succesfully the lib folder.");
                if(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoadsonMod.csproj")))
                {
                    Console.WriteLine("Enter the name of the mod here (GUID, has to be unique among other mods). To set the Display Name edit the 'metadata.txt' file.");
                    Console.Write("Mod name: ");
                    string name = Console.ReadLine().Replace(" ", "_");
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoadsonMod.csproj"), new StreamReader(Assembly.GetEntryAssembly().GetManifestResourceStream("ModBuilder.modproj")).ReadToEnd().Replace("%name%", name));
                    Console.WriteLine("Mod initialized. Press any key to exit..");
                }
                else
                {
                    Console.WriteLine("Press any key to exit..");
                }
                
                Console.ReadKey();
                return;
            }
            if(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadata.txt")))
            {
                Console.WriteLine("Couldn't find metadata.txt file.");
                Console.WriteLine("Either copy it from the MDK zip or start with a fresh MDK copy.");
                Console.ReadKey();
                return;
            }
            if(!File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.png")))
            {
                Console.WriteLine("Couldn't find icon.png file.");
                Console.WriteLine("Either copy it from the MDK zip or start with a fresh MDK copy.");
                Console.ReadKey();
                return;
            }
            string mod_guid = args.Length == 1 ? args[0] : Path.GetFileNameWithoutExtension(Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release")).FirstOrDefault());
            Console.WriteLine("Detected mod GUID: " + mod_guid);
            Console.WriteLine("Loading mod binary..");
            byte[] mod_asm = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release", mod_guid + ".dll"));
            Console.WriteLine("Loading metadata..");
            string metadata = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "metadata.txt"));
            string m_name = "null", m_author = "null", m_description = "null";
            List<string> deps = new List<string>();
            foreach(string a in metadata.Split('\n'))
            {
                string line = a.Trim();
                if (line.Length == 0) continue;
                string key = line.Substring(0, line.IndexOf('='));
                string val = line.Substring(line.IndexOf('=') + 1); // split at first '=', so val can also have '=' inside
                switch(key)
                {
                    default:
                        Console.WriteLine("WARNING: Found unknown key " + key + " in metadata.");
                        break;
                    case "displayname":
                        m_name = val;
                        break;
                    case "author":
                        m_author = val;
                        break;
                    case "description":
                        m_description = val.Replace("\\n", "\n"); // add LF
                        break;
                    case "deps":
                        if (val.Length == 0 || val == "") break;
                        foreach (string entry in val.Split(',')) deps.Add(entry);
                        break;
                }
            }
            Console.WriteLine("Loading icon data..");
            byte[] img_data = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "icon.png"));
            Console.WriteLine("Loading assetbundle..");
            byte[] assetbundle = null;
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assetbundle")))
                assetbundle = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assetbundle"));
            Console.WriteLine("Building mod..");
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klm")))
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klm"));
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klmi")))
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klmi"));
            using (FileStream fs = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klm")))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(m_name);
                bw.Write(m_author);
                bw.Write(m_description);
                bw.Write(deps.Count);
                if (deps.Count > 0)
                    foreach (string dep in deps) bw.Write(dep);
                bw.Write(-1); // WorkshopID
                bw.Write(mod_asm.Length);
                bw.Write(mod_asm);
                bw.Write(img_data.Length);
                bw.Write(img_data);
                if (assetbundle == null) bw.Write(0);
                else { bw.Write(assetbundle.Length); bw.Write(assetbundle); }
                bw.Close();
            }
            using (FileStream fs = File.OpenWrite(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klmi")))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                List<string> extDeps = new List<string>();
                foreach (var file in Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin", "Release")))
                {
                    if (!file.EndsWith(".dll"))
                        continue;
                    if (Path.GetFileNameWithoutExtension(file) == mod_guid)
                        continue;
                    extDeps.Add(file);
                }
                Console.WriteLine("External deps: " + extDeps.Count);
                bw.Write(extDeps.Count); // external deps
                foreach (var dep in extDeps)
                {
                    Console.WriteLine("Adding " + Path.GetFileName(dep));
                    bw.Write(Path.GetFileName(dep));
                    var bytes = File.ReadAllBytes(dep);
                    bw.Write(bytes.Length);
                    bw.Write(bytes);
                }
                byte[] mod = File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klm"));
                bw.Write(mod.Length);
                bw.Write(mod);
                bw.Close();
            }
            Console.WriteLine("\nBuild succesfully in (" + Math.Ceiling((DateTime.Now - start).TotalMilliseconds) + "ms).");
            Console.WriteLine("Mod size: " + File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klm")).Length + " B");
            Console.WriteLine("Install size: " + File.ReadAllBytes(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mod_guid + ".klmi")).Length + " B");
        }
    }
}
