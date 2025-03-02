using Discord;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LoadsonInternal
{
    public class DiscordRPC
    {
#if !LoadsonAPI
        static Activity lastActivity;
        static long timestampStart;

        static List<Dictionary<string, object>> pendingUpdates = new List<Dictionary<string, object>>();
        public static bool AnyPendingUpdates => pendingUpdates.Count > 0;

        public static void Init()
        {
            // determine platform
            string platform = "";
            var managed_path = Application.dataPath;
            if (managed_path.EndsWith("Karlson_Data"))
            {
                if (Environment.Is64BitProcess)
                    platform = "[Win64]";
                else
                    platform = "[Win32]";
            }
            else if (managed_path.EndsWith("Karlson_linux_Data"))
                platform = "[Linux]";
            else
                platform = "[MacOS]";
            timestampStart = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            lastActivity = new Activity
            {
                ApplicationId = Loader.DISCORD_CLIENTID,
                Assets = new ActivityAssets
                {
                    LargeImage = "loadson",
                    LargeText = "Loadson v" + Version.ver,
                    SmallImage = "karlson",
                    SmallText = platform + " Karlson (itch.io)"
                },
                Details = "Loading mods..",
                State = "github.com/karlsonmodding",
                Timestamps = new ActivityTimestamps
                {
                    Start = timestampStart
                }
            };
            Loader.discord.GetActivityManager().UpdateActivity(lastActivity, (_) => ProcessUpdate());
        }
#endif
        public static void UpdateRPC(string largeImage = "", string largeText = "", string smallImage = "", string smallText = "", string details = "", string state = "", ActivityParty? party = null, ActivitySecrets? secrets = null)
        {
#if !LoadsonAPI
            // push to pendingUpdates
            var changes = new Dictionary<string, object>();
            if (largeImage != "")
                changes.Add("LargeImage", largeImage);
            if (largeText != "")
                changes.Add("LargeText", largeText);
            if (smallImage != "")
                changes.Add("SmallImage", smallImage);
            if (smallText != "")
                changes.Add("SmallText", smallText);
            if (details != "")
                changes.Add("Details", details);
            if (state != "")
                changes.Add("State", state);
            if (party != null)
                changes.Add("Party", party.Value);
            if (secrets != null)
                changes.Add("Secrets", secrets.Value);
            pendingUpdates.Add(changes);
            if (pendingUpdates.Count == 1)
                ProcessUpdate();
        }

        static void ProcessUpdate()
        {
            if (pendingUpdates.Count == 0)
                return;
            var changes = pendingUpdates.First();
            pendingUpdates.RemoveAt(0);
            foreach(var change in changes)
            {
                switch (change.Key)
                {
                    case "LargeImage":
                        lastActivity.Assets.LargeImage = (string)change.Value;
                        break;
                    case "LargeText":
                        lastActivity.Assets.LargeText = (string)change.Value;
                        break;
                    case "SmallImage":
                        lastActivity.Assets.SmallImage = (string)change.Value;
                        break;
                    case "SmallText":
                        lastActivity.Assets.SmallText = (string)change.Value;
                        break;
                    case "Details":
                        lastActivity.Details = (string)change.Value;
                        break;
                    case "State":
                        lastActivity.State = (string)change.Value;
                        break;
                    case "Party":
                        lastActivity.Party = (ActivityParty)change.Value;
                        break;
                    case "Secrets":
                        lastActivity.Secrets = (ActivitySecrets)change.Value;
                        break;
                }
            }
            Loader.discord.GetActivityManager().UpdateActivity(lastActivity, (_) => ProcessUpdate());
        }
#else
        }
#endif
    }
}
