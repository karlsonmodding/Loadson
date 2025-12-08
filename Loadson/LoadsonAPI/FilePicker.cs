using LoadsonExtensions;
using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace LoadsonAPI
{
    public static class FilePicker
    {
        public delegate void OnCancel();
        public delegate void OnSelect(string fileName);
        /// <summary>
        /// Show file picker (displayed with Unity ImGUI for cross-platform)
        /// </summary>
        /// <param name="title">ImGUI window title</param>
        /// <param name="path">Starting folder</param>
        /// <param name="filter">Filter for files as (name,filter) where filter is [name].[extension] (eg. '*.*' '*.png') or multiple filters separated with '|'</param>
        /// <param name="select">Function to be called when a file is picked</param>
        /// <param name="cancel">Function to be called when file picker is closed</param>
        public static void PickFile(string title, string path, List<(string, string)> filter, OnSelect select, OnCancel cancel)
        {
#if !LoadsonAPI
            Launcher.FilePicker.PickFile(title, path, filter, (fileName) => ModLoader.SafeCall(() => select(fileName)), () => ModLoader.SafeCall(() => cancel()));
#endif
        }
    }
}
