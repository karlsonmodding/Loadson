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
            Console.WriteLine("DevKit installed succesfully the lib folder. Press any key to exit..");
            Console.ReadKey();
        }
    }
}
