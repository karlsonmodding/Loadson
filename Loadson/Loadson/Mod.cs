using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
        protected T LoadAsset<T>(string name)
        {
            // TODO: Add resolver
            return default;
        }

        /// <summary>
        /// "Write" and API function that can be later called by <see cref="CallAPIFunction(string, object[])"/>
        /// </summary>
        /// <param name="name">Name of the function. Be carefull to add a prefix to it that is unique to your mod.</param>
        /// <param name="execute">The API function itself, takes one parameter as list of objects, returns an object (can be null for void functions)</param>
        protected void AddAPIFunction(string name, Action<object[], object> execute)
        {
            // TODO: Implement functionality and callable to modlist
        }

        /// <summary>
        /// "Call" an API function. Be sure to add the mod that you are calling to the dependencies list.
        /// </summary>
        /// <param name="name">Name of the function.</param>
        /// <param name="param">Function parameters as an object list</param>
        /// <returns>API function return value</returns>
        protected object CallAPIFunction(string name, object[] param)
        {
            // TODO: Call api function from callable modlist
            return null;
        }
    }
}
