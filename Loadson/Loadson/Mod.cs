using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Loadson
{
    public abstract class Mod
    {
        // Loader activity
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }

        // MonoBehaviour
        public virtual void OnGUI() { }
        public virtual void Update(float deltaTime) { }
        public virtual void FixedUpdate(float fixedDeltaTime) { }

        // Mod-specific API
        /// <summary>
        /// Load an asset by its name from your asset bundle. The same as <see cref="UnityEngine.AssetBundle.LoadAsset{T}(string)"/>
        /// </summary>
        /// <typeparam name="T">Asset type</typeparam>
        /// <param name="name">Asset name</param>
        /// <returns>The loaded asset</returns>
        protected T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            // get mod instance
            ModEntry e = null;
            // selector doesn't work, don't ask me why, i'm going insane
            foreach(var a in ModEntry.List)
            {
                if(a.assembly.GetName().Name == Assembly.GetCallingAssembly().GetName().Name)
                {
                    e = a;
                    break;
                }
            }
            if(e == null)
            {
                LoadsonInternal.Console.Log("<color=red>Couldn't find matching mod for " + Assembly.GetCallingAssembly().GetName() + "</color>");
                LoadsonInternal.Console.OpenConsole();
                return default;
            }
            if(e.enabled)
            {
                LoadsonInternal.Console.Log("<color=red>[" + e.ModGUID + "] Tried loading asset outside of OnEnable</color>");
                LoadsonInternal.Console.OpenConsole();
                return default;
            }
            return e.AssetBundle.LoadAsset<T>(name);
        }

        /// <summary>
        /// "Write" and API function that can be later called by <see cref="CallAPIFunction(string, object[])"/>
        /// </summary>
        /// <param name="name">Name of the function. Be carefull to add a prefix to it that is unique to your mod.</param>
        /// <param name="execute">The API function itself, takes one parameter as list of objects, returns an object (can be null for void functions)</param>
        protected void AddAPIFunction(string name, CrossModAPI.cmm execute)
        {
            CrossModAPI.AddMethod(name, execute);
        }

        /// <summary>
        /// "Call" an API function. Be sure to add the mod that you are calling to the dependencies list.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        /// <param name="args">Function parameters as an object list</param>
        /// <returns>API function return value</returns>
        protected object CallAPIFunction(string name, object[] args)
        {
            return CrossModAPI.CallMethod(name, args);
        }

        /// <summary>
        /// Get User Files directory where you can store any files.
        /// </summary>
        /// <returns>The directory. Please don't escape with '/..'</returns>
        protected string GetUserFilesFolder()
        {
            // get mod instance
            ModEntry e = null;
            // selector doesn't work, don't ask me why, i'm going insane
            foreach (var a in ModEntry.List)
            {
                if (a.assembly.GetName().Name == Assembly.GetCallingAssembly().GetName().Name)
                {
                    e = a;
                    break;
                }
            }
            if (e == null)
            {
                LoadsonInternal.Console.Log("<color=red>Couldn't find matching mod for " + Assembly.GetCallingAssembly().GetName() + "</color>");
                LoadsonInternal.Console.OpenConsole();
                return default;
            }
            if (!Directory.Exists(Path.Combine(Loader.LOADSON_ROOT, "UserFiles")))
                Directory.CreateDirectory(Path.Combine(Loader.LOADSON_ROOT, "UserFiles"));
            string dir = Path.Combine(Loader.LOADSON_ROOT, "UserFiles", e.ModGUID);
            if(!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
