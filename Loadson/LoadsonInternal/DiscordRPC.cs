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
            string platform_name = "";
            switch(Loader.platform)
            {
                case Loader.Platform.Win64: platform_name = "[Win64] "; break;
                case Loader.Platform.Win32: platform_name = "[Win32] "; break;
                case Loader.Platform.Linux: platform_name = "[Linux] "; break;
                case Loader.Platform.MacOS: platform_name = "[MacOS] "; break;
            }
            timestampStart = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            lastActivity = new Activity
            {
                ApplicationId = Loader.DISCORD_CLIENTID,
                Assets = new ActivityAssets
                {
                    LargeImage = "loadson",
                    LargeText = "Loadson v" + Version.ver,
                    SmallImage = "karlson",
                    SmallText = platform_name + "Karlson (itch.io)"
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
        /// <summary>
        /// Change Discord RPC. Only non-default values will be changed.
        /// </summary>
        /// <param name="largeImage"></param>
        /// <param name="largeText"></param>
        /// <param name="smallImage"></param>
        /// <param name="smallText"></param>
        /// <param name="details"></param>
        /// <param name="state"></param>
        /// <param name="party"></param>
        /// <param name="secrets"></param>
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
