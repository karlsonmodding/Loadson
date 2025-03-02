#if !LoadsonAPI
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonInternal
{
    public static class _GUIDFetcher
    {
        public static string ExtractGUID(byte[] asmData)
        {
            return Assembly.Load(asmData).GetName().Name;
        }
    }
}
#endif