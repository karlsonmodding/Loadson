using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonAPI
{
    public static class MenuEntry
    {
        /// <summary>
        /// Create a custom menu category for your mod
        /// </summary>
        /// <param name="list">List containing of sub-menu entries: (name - to be displayed, action - on click)</param>
        public static void AddMenuEntry(List<(string, Action)> list)
        {
            MenuCustom.AddCategory(Assembly.GetCallingAssembly().GetName().Name, list);
        }

        /// <summary>
        /// Change sub-menu entries
        /// </summary>
        /// <param name="list">List containing of sub-menu entries: (name - to be displayed, action - on click)</param>
        public static void UpdateMenuEntry(List<(string, Action)> list)
        {
            MenuCustom.UpdateCategory(Assembly.GetCallingAssembly().GetName().Name, list);
        }

        /// <summary>
        /// Removes your mod category from the custom menu
        /// </summary>
        public static void RemoveMenuEntry()
        {
            MenuCustom.RemoveCategory(Assembly.GetCallingAssembly().GetName().Name);
        }
    }
}
