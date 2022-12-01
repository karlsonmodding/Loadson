using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Net.Http;

namespace DevKit_Bootstrapper
{
    class Program
    {
        private const string API_ENDPOINT = "https://raw.githubusercontent.com/karlsonmodding/Loadson/deployment";

        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = "Loadson - DevKit";
            Console.WriteLine("Loadson");
            Console.WriteLine("  made by devilExE");
            Console.WriteLine("  licensed under MIT License");
            Console.WriteLine("Loadson DevKit");
            Console.WriteLine("\nSearching for solution in CWD");
            string solutionRoot = "";
            if (File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Loadson.sln")))
            {
                solutionRoot = AppDomain.CurrentDomain.BaseDirectory;
            }
            else
            {
                Console.WriteLine("Select Loadson.sln file to get the solution's root directory");
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Title = "Select your project root";
                    ofd.Filter = "Loadson solution file|Loadson.sln";
                    ofd.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    if (ofd.ShowDialog() == DialogResult.OK)
                        solutionRoot = Path.GetDirectoryName(ofd.FileName);
                    else
                        Environment.Exit(0);
                }
            }

            if (Directory.Exists(Path.Combine(solutionRoot, "lib")))
            {
                Console.WriteLine("It appears that the lib folder already exists. If you want to reinstall the devkit, remove the lib folder and try again");
                Console.ReadKey();
                Environment.Exit(0);
            }
            
            Console.WriteLine("Solution root: " + solutionRoot);
            Directory.CreateDirectory(Path.Combine(solutionRoot, "lib"));

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
            foreach(string f in Directory.GetFiles(Path.Combine(gameRoot, "Karlson_Data", "Managed")))
            {
                if (!f.EndsWith(".dll")) continue;
                if (Path.GetFileName(f) == "Assembly-CSharp.dll" || Path.GetFileName(f) == "Unity.TextMeshPro.dll" || Path.GetFileName(f).StartsWith("UnityEngine"))
                {
                    File.Copy(f, Path.Combine(solutionRoot, "lib", Path.GetFileName(f)));
                    Console.WriteLine("Copied " + Path.GetFileName(f));
                }
            }
            Console.WriteLine("Downloading other assemblies..");
            HttpClient hc = new HttpClient();
            File.WriteAllBytes(Path.Combine(solutionRoot, "lib", "0Harmony.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/files/Internal/Loadson%20deps/0Harmony.dll").GetAwaiter().GetResult());
            File.WriteAllBytes(Path.Combine(solutionRoot, "lib", "Mono.Cecil.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/files/Internal/Loadson%20deps/Mono.Cecil.dll").GetAwaiter().GetResult());
            File.WriteAllBytes(Path.Combine(solutionRoot, "lib", "MonoMod.RuntimeDetour.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/files/Internal/Loadson%20deps/MonoMod.RuntimeDetour.dll").GetAwaiter().GetResult());
            File.WriteAllBytes(Path.Combine(solutionRoot, "lib", "MonoMod.Utils.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/files/Internal/Loadson%20deps/MonoMod.Utils.dll").GetAwaiter().GetResult());
            File.WriteAllBytes(Path.Combine(solutionRoot, "lib", "MInject.dll"), hc.GetByteArrayAsync(API_ENDPOINT + "/files/Launcher/MInject.dll").GetAwaiter().GetResult());
            Console.WriteLine("DevKit installed succesfully the lib folder. Press any key to exit..");
            Console.ReadKey();
        }

        public static void RawCopy(Stream input, string outputFilePath)
        {
            int bufferSize = 1024 * 1024;

            using (FileStream fileStream = new FileStream(outputFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite))
            {
                fileStream.SetLength(input.Length);
                int bytesRead = -1;
                byte[] bytes = new byte[bufferSize];

                while ((bytesRead = input.Read(bytes, 0, bufferSize)) > 0)
                {
                    fileStream.Write(bytes, 0, bytesRead);
                }
            }
        }
    }
}
