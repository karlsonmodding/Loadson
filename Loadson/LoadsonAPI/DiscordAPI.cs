using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonAPI
{
    public static class DiscordAPI
    {
        /// <summary>
        /// Get the Discord instance created by Loadson
        /// </summary>
        public static Discord.Discord Discord
        {
            get => LoadsonInternal.Loader.discord;
        }
        /// <summary>
        /// Get the local Discord user
        /// </summary>
        public static Discord.User User
        {
            get => LoadsonInternal.Loader.discord_user;
        }
        /// <summary>
        /// Get the Bearer token for username and profile picture.
        /// You can use this in your server-side API to validate user.
        /// </summary>
        public static string Bearer
        {
            get => LoadsonInternal.Loader.discord_token;
        }
        /// <summary>
        /// True if discord initialized without errors.
        /// </summary>
        public static bool HasDiscord
        {
            get => LoadsonInternal.Loader.discord_exists;
        }
    }
}
