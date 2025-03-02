using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonExtensions
{
    public static class UnityObject_Extensions
    {
        public static void Destroy(this UnityEngine.Object @object) => UnityEngine.Object.Destroy(@object);
        public static void DestroyImmediate(this UnityEngine.Object @object) => UnityEngine.Object.DestroyImmediate(@object);
    }
}
