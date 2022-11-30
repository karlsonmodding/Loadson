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
        public static void Log(string str)
        {
            LoadsonInternal.Console.Log("[" + Assembly.GetCallingAssembly().GetName().Name + "] " + str);
        }
    }
}
