using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Builder
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Creating folders");
            Directory.CreateDirectory(Path.Combine("files", "Internal", "Loadson deps"));
            Directory.CreateDirectory(Path.Combine("loadsonapi"));
            Console.WriteLine("Building LoadsonAPI");
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = "/C dotnet build Loadson -c LoadsonAPI",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe"
                }
            };
            proc.Start();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            File.Copy(Path.Combine("Loadson", "bin", "LoadsonAPI", "Loadson.dll"), Path.Combine("loadsonapi", "LoadsonAPI.dll"));
            File.Copy(Path.Combine("Loadson", "bin", "LoadsonAPI", "Loadson.xml"), Path.Combine("loadsonapi", "LoadsonAPI.xml"));
            Console.WriteLine("Building Loadson"); proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = "/C dotnet build -c Release",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "cmd.exe"
                }
            };
            proc.Start();
            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            proc.WaitForExit();
            File.Copy(Path.Combine("Kernel", "bin", "Release", "_Loadson.dll"), Path.Combine("files", "_Loadson.dll"));
            File.Copy(Path.Combine("Launcher", "bin", "Release", "Launcher.dll"), Path.Combine("files", "Internal", "Launcher.dll"));
            File.Copy(Path.Combine("Loadson", "bin", "Release", "Loadson.dll"), Path.Combine("files", "Internal", "Loadson.dll"));
            File.Copy(Path.Combine("Loadson", "bin", "Release", "0Harmony.dll"), Path.Combine("files", "Internal", "Loadson deps", "0Harmony.dll"));
            Console.WriteLine("Constructing hashmap");
            File.WriteAllText("hashmap", "");
            File.AppendAllText("hashmap", "Internal/Launcher.dll:" + HashFile(Path.Combine("files", "Internal", "Launcher.dll")) + "\n");
            File.AppendAllText("hashmap", "Internal/Loadson.dll:" + HashFile(Path.Combine("files", "Internal", "Loadson.dll")) + "\n");
            File.AppendAllText("hashmap", "Internal/Loadson deps/0Harmony.dll:" + HashFile(Path.Combine("files", "Internal", "Loadson deps", "0Harmony.dll")) + "\n");
            File.AppendAllText("hashmap", "/_Loadson.dll:" + HashFile(Path.Combine("files", "_Loadson.dll")) + "\n");
        }

        static string HashFile(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
