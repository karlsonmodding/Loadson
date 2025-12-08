using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LoadsonExtensions
{
    public static class Texture2D_Extensions
    {
        /// <summary>
        /// Load an image from Embedded Resources into this texture.
        /// </summary>
        /// <param name="texture2D">Texture (syntactic sugar)</param>
        /// <param name="resource">Resource name (best way to find it is with dnSpy)</param>
        public static void LoadFromResources(this Texture2D texture2D, string resource)
        {
#if !LoadsonAPI
            Launcher.FilePicker.LoadFromResources(texture2D, resource);
#endif
        }
    }
}
