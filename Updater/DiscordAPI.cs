using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Updater
{
    public class DiscordAPI
    {
        public static bool needToDownload = false;
        public static bool downloading = false;
        public static bool hasDiscord = false;
        static string install_path, download_path;
        public static void Check()
        {
            // determine install path
            var managed_path = Application.dataPath;
            if(managed_path.EndsWith("Karlson_Data"))
            {
                if(Environment.Is64BitProcess)
                {
                    install_path = Path.Combine(managed_path, "Plugins", "x86_64", "discord_game_sdk.dll");
                    download_path = "/discordsdk/x86_64/discord_game_sdk.dll";
                }
                else
                {
                    install_path = Path.Combine(managed_path, "Plugins", "x86", "discord_game_sdk.dll");
                    download_path = "/discordsdk/x86/discord_game_sdk.dll";
                }
            }
            else if(managed_path.EndsWith("Karlson_linux_Data"))
            {
                install_path = Path.Combine(managed_path, "Plugins", "x86_64", "discord_game_sdk.so");
                download_path = "/discordsdk/x86_64/discord_game_sdk.so";
            }
            // TODO: error for unknown platform
            // TODO: check what to do on macos
            if (install_path == "" || download_path == "")
                return;
            if (!File.Exists(install_path))
                needToDownload = true;
            else
                hasDiscord = true;
        }

        public static void Download()
        {
            if (install_path == "" || download_path == "")
                return;
            downloading = true;
            new Thread(() =>
            {
                Directory.CreateDirectory(Path.GetDirectoryName(install_path));
                File.WriteAllBytes(install_path, UpdaterBehaviour.wc.DownloadData(UpdaterBehaviour.API_ENDPOINT + download_path));
                downloading = false;
                needToDownload = false;
                hasDiscord = true;
            }).Start();
        }
    }
}
