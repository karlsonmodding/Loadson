using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Loadson
{
    public static class Console
    {
        /// <summary>
        /// Log a string to the console (and file)
        /// </summary>
        /// <param name="str">The string to be logged</param>
        public static void Log(string str)
        {
            LoadsonInternal.Console.Log("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + str);
        }
    }
}
