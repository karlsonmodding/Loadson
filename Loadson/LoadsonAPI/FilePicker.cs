using LoadsonExtensions;
using LoadsonInternal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
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
            if (inFilePick)
                ModLoader.SafeCall(() => Cancel()); // cancel previous file pick
            inFilePick = true;
            Title = title;
            CurrentFolder = path;
            Filter = filter;
            Select = select;
            Cancel = cancel;
            windowRect = new Rect(Screen.width / 2 - 300, Screen.height / 2 - 200, 600, 400);
            innerScroll = new Vector2(0, 0);
            driveScroll = new Vector2(0, 0);
            CurrentFile = "";
            Dropup = new Dropup(Filter.Select(x => x.Item1).ToArray(), 0);
            showHiddenFiles = false;
            refreshFiles();
        }
        static bool inFilePick = false;
        static string Title;
        static string CurrentFolder;
        static string CurrentFile;
        static List<(string, string)> Filter;
        static Dropup Dropup;
        static OnSelect Select;
        static OnCancel Cancel;

        static int windowId = -1;
        static Rect windowRect;
        static Vector2 innerScroll, driveScroll;
        static Texture2D imageUpArrow, imageDrive, imageDirectory, imageGeneric, imagePicture, imageArchive, imageAudio, imageExecutable, imagePdf, imageText, imageVideo;
        static GUIStyle fileButton;
        static string[] drives;
        static DirectoryInfo[] directories;
        static (FileInfo, Texture2D)[] files;
        static bool showHiddenFiles;
        public static void init()
        {
            windowId = ImGUI_WID.GetWindowId();
            imageUpArrow = new Texture2D(1, 1);
            imageUpArrow.LoadFromResources("Assets.UpArrow.png");
            imageDrive = new Texture2D(1, 1);
            imageDrive.LoadFromResources("Assets.DriveIcon.png");
            imageDirectory = new Texture2D(1, 1);
            imageDirectory.LoadFromResources("Assets.FolderIcon.png");
            imageGeneric = new Texture2D(1, 1);
            imageGeneric.LoadFromResources("Assets.DefaultFileIcon.png");
            imagePicture = new Texture2D(1, 1);
            imagePicture.LoadFromResources("Assets.ImageFileIcon.png");
            imageArchive = new Texture2D(1, 1);
            imageArchive.LoadFromResources("Assets.ArchiveIcon.png");
            imageAudio = new Texture2D(1, 1);
            imageAudio.LoadFromResources("Assets.AudioFileIcon.png");
            imageExecutable = new Texture2D(1, 1);
            imageExecutable.LoadFromResources("Assets.ExecutableIcon.png");
            imagePdf = new Texture2D(1, 1);
            imagePdf.LoadFromResources("Assets.PdfFileIcon.png");
            imageText = new Texture2D(1, 1);
            imageText.LoadFromResources("Assets.TextFileIcon.png");
            imageVideo = new Texture2D(1, 1);
            imageVideo.LoadFromResources("Assets.VideoFileIcon.png");
            fileButton = new GUIStyle();
            fileButton.normal.background = Texture2D.blackTexture;
            fileButton.normal.textColor = Color.white;
            drives = DriveInfo.GetDrives().Select(x => x.Name).ToArray();
        }
        public static void _ongui()
        {
            if (!inFilePick)
                return;
            GUI.Box(windowRect, "");
            windowRect = GUI.ModalWindow(windowId, windowRect, _ =>
            {
                if (GUI.Button(new Rect(5, 20, 20, 20), imageUpArrow, fileButton))
                {
                    CurrentFolder = Directory.GetParent(CurrentFolder)?.FullName ?? CurrentFolder;
                    refreshFiles();
                }
                var oldFolder = CurrentFolder;
                CurrentFolder = GUI.TextField(new Rect(30, 20, 565, 20), CurrentFolder);
                if (oldFolder != CurrentFolder)
                    refreshFiles();
                GUI.Box(new Rect(5, 45, 100, 300), "");
                driveScroll = GUI.BeginScrollView(new Rect(5, 45, 100, 300), driveScroll, new Rect(0, 0, 100, 20 * drives.Length));
                for (int i = 0; i < drives.Length; i++)
                    if (GUI.Button(new Rect(0, i * 20, 100, 20), new GUIContent($" <color={(CurrentFolder.StartsWith(drives[i]) ? "orange" : "white")}>" + drives[i] + "</color>", imageDrive), fileButton))
                    {
                        CurrentFolder = drives[i];
                        refreshFiles();
                    }
                GUI.EndScrollView();
                GUI.Box(new Rect(110, 45, 485, 300), "");
                // TODO: calc
                if (Dropup.Draw(new Rect(495, 350, 100, 20)))
                    refreshFiles();
                innerScroll = GUI.BeginScrollView(new Rect(110, 45, 485, 300), innerScroll, new Rect(0, 0, 100, 20 * (files.Length + directories.Length)));
                for (int i = 0; i < directories.Length; i++)
                    if (GUI.Button(new Rect(0, i * 20, 485, 20), new GUIContent(" " + directories[i].Name, imageDirectory), fileButton))
                    {
                        CurrentFolder = directories[i].FullName;
                        refreshFiles();
                    }
                int startY = 20 * directories.Length;
                for (int i = 0; i < files.Length; i++)
                {
                    var ext = Path.GetExtension(files[i].Item1.FullName);
                    if (GUI.Button(new Rect(0, startY + i * 20, 485, 20), new GUIContent($" <color={(CurrentFile == files[i].Item1.Name ? "orange" : "white")}>" + files[i].Item1.Name + "</color>", files[i].Item2), fileButton))
                        CurrentFile = files[i].Item1.Name;
                }
                GUI.EndScrollView();
                if (Dropup.Draw(new Rect(495, 350, 100, 20)))
                    refreshFiles();
                CurrentFile = GUI.TextField(new Rect(5, 350, 485, 20), CurrentFile);
                bool oldShow = showHiddenFiles;
                showHiddenFiles = GUI.Toggle(new Rect(5, 375, 200, 20), showHiddenFiles, "Show hidden files");
                if (oldShow != showHiddenFiles)
                    refreshFiles();
                if (GUI.Button(new Rect(440, 375, 75, 20), "Select"))
                {
                    inFilePick = false;
                    ModLoader.SafeCall(() => Select(Path.Combine(CurrentFolder, CurrentFile)));
                }
                if (GUI.Button(new Rect(520, 375, 75, 20), "Cancel"))
                {
                    inFilePick = false;
                    ModLoader.SafeCall(() => Cancel());
                }

                GUI.DragWindow();
            }, Title);
        }
        static void refreshFiles()
        {
            if (!Directory.Exists(CurrentFolder))
                return;
            List<(FileInfo, Texture2D)> Files = new List<(FileInfo, Texture2D)>();
            List<DirectoryInfo> Directories = new List<DirectoryInfo>();
            foreach (var entry in Directory.GetFileSystemEntries(CurrentFolder))
            {
                if (Directory.Exists(entry) && (showHiddenFiles || (new DirectoryInfo(entry).Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0))
                    Directories.Add(new DirectoryInfo(entry));
                else if (FilterMatch(entry) && (showHiddenFiles || (new FileInfo(entry).Attributes & (FileAttributes.Hidden | FileAttributes.System)) == 0))
                {
                    Texture2D image = imageGeneric;
                    var ext = Path.GetExtension(entry);
                    if (ext == ".txt" || ext == ".doc" || ext == ".docx" || ext == ".xml" || ext == ".json" || ext == ".rtf")
                        image = imageText;
                    else if (ext == ".pdf")
                        image = imagePdf;
                    else if (ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".gif" || ext == ".bmp" || ext == ".psd" || ext == ".tif" || ext == ".tiff")
                        image = imagePicture;
                    else if (ext == ".mp4" || ext == ".mkv" || ext == ".mov" || ext == ".avi" || ext == ".flv" || ext == ".wemb" || ext == ".wmv")
                        image = imageVideo;
                    else if (ext == ".mp3" || ext == ".wav" || ext == ".aac" || ext == ".m4a" || ext == ".ogg" || ext == ".wma" || ext == ".flac")
                        image = imageAudio;
                    else if (ext == ".zip" || ext == ".rar" || ext == ".7z" || ext == ".gz" || ext == ".tar" || ext == ".bz2")
                        image = imageArchive;
                    else if (ext == ".exe" || ext == ".dll" || ext == ".so" || ext == ".dylib")
                        image = imageExecutable;
                    Files.Add((new FileInfo(entry), image));
                }
            }
            files = Files.ToArray();
            directories = Directories.ToArray();
        }
        static bool FilterMatch(string file)
        {
            foreach (var filter in Filter[Dropup.Index].Item2.Split('|'))
            {
                var file_name = filter.Substring(0, filter.LastIndexOf('.'));
                var extension = "." + filter.Split('.').Last().ToLowerInvariant();
                if ((file_name == "*" || Path.GetFileNameWithoutExtension(file) == file_name) &&
                    (extension == ".*" || Path.GetExtension(file).ToLowerInvariant() == extension))
                    return true;
            }
            return false;
        }
#else
        }
#endif
    }
#if !LoadsonAPI
    public class Dropup
    {
        public Dropup(string[] options, int defaultIdx)
        {
            Options = options;
            Index = defaultIdx;
            buttonText = new GUIStyle();
            buttonText.normal.background = Texture2D.blackTexture;
            buttonText.alignment = TextAnchor.MiddleLeft;
        }
        public string[] Options;
        public int Index;
        private bool dropped = false;
        GUIStyle buttonText;
        public bool Draw(Rect pos)
        {
            if (!dropped)
            {
                if (GUI.Button(pos, Options[Index])) dropped = true;
                GUI.Label(new Rect(pos.x + pos.width - pos.height, pos.y, pos.height, pos.height), "▲");
            }
            else
            {
                if (GUI.Button(pos, Options[Index])) dropped = false;
                GUI.Label(new Rect(pos.x + pos.width - pos.height, pos.y, pos.height, pos.height), "▼");
                GUI.Box(new Rect(pos.x, pos.y - pos.height * Options.Length, pos.width, pos.height * Options.Length), "");
                for (int i = 0; i < Options.Length; i++)
                {
                    string color = "<color=white>";
                    if (i == Index) color = "<color=green>";
                    if (GUI.Button(new Rect(pos.x, pos.y - pos.height * (Options.Length - i), pos.width, pos.height), color + Options[i] + "</color>", buttonText))
                    {
                        Index = i;
                        dropped = false;
                        return true;
                    }
                }
            }
            return false;
        }
    }
#endif
}
