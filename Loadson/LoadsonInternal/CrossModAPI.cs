using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonInternal
{
    public static class CrossModAPI
    {
        public delegate object cmm(object[] args);
        public static void AddMethod(string name, cmm action)
        {
            if(MethodMap.ContainsKey(name))
            {
                Console.Log("<color=red>[" + Assembly.GetCallingAssembly().GetName().Name + "] Tried adding cross-mod api method " + name + " but already exists</color>");
                Console.OpenConsole();
                return;
            }
            MethodMap.Add(name, action);
        }

        public static object CallMethod(string name, object[] args)
        {
            if (!MethodMap.ContainsKey(name))
            {
                Console.Log("<color=red>[" + Assembly.GetCallingAssembly().GetName().Name + "] Tried calling cross-mod api method " + name + " but it doesn't exist</color>");
                Console.OpenConsole();
                return null;
            }
            return MethodMap[name](args);
        }

        private static Dictionary<string, cmm> MethodMap = new Dictionary<string, cmm>();
    }
}
