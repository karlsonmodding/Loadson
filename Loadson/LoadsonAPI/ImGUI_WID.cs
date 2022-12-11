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
        /// <summary>
        /// Get next available Unity ImGUI window id
        /// </summary>
        /// <returns>The window id</returns>
        public static int GetWindowId() => current++;
    }
}
