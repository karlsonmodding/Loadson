using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoadsonAPI
{
    public class ImGUI_WID
    {
        private static int current = 0;
        public static int GetWindowId() => current++;
    }
}
