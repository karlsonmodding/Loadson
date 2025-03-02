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
#if !LoadsonAPI
            get => LoadsonInternal.Loader.discord;
#else
            get => null;
#endif
        }
        /// <summary>
        /// Get the local Discord user
        /// </summary>
        public static Discord.User User
        {
#if !LoadsonAPI
            get => LoadsonInternal.Loader.discord_user;
#else
            get => new Discord.User();
#endif
        }
        /// <summary>
        /// Get the Bearer token for username and profile picture.
        /// You can use this in your server-side API to validate user.
        /// </summary>
        public static string Bearer
        {
#if !LoadsonAPI
            get => LoadsonInternal.Loader.discord_token;
#else
            get => "";
#endif
        }
        /// <summary>
        /// True if discord initialized without errors.
        /// </summary>
        public static bool HasDiscord
        {
#if !LoadsonAPI
            get => LoadsonInternal.Loader.discord_exists;
#else
            get => false;
#endif
        }
    }
}
