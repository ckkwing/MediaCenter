using FileExplorer.Helper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Utilities.Extension;
using Utilities.WindowsAPI;

namespace FileExplorer.Shell
{
    /// <summary>
    /// provider global information
    /// </summary>
    public static class CommonProvider
    {
        private static System.Windows.Media.ImageSource defaultFolderImage;

        public static System.Windows.Media.ImageSource DefaultFolderImage
        {
            get
            {
                if (defaultFolderImage == null)
                {
                    string path = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                    ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
                    ShellAPI.SHGFI vFlags =
                        ShellAPI.SHGFI.SHGFI_SMALLICON |
                        ShellAPI.SHGFI.SHGFI_ICON |
                        ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES;

                    ShellAPI.SHGetFileInfo(path, (uint)FileAttributes.Directory, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
                    defaultFolderImage = GetImageSourceFromIcon(shellInfo.hIcon);
                    if (shellInfo.hIcon != IntPtr.Zero)
                    {
                        User32API.DestroyIcon(shellInfo.hIcon);
                        shellInfo.hIcon = IntPtr.Zero;
                    }
                }
                return CommonProvider.defaultFolderImage;
            }
        }

        #region Icons
        /// <summary>
        /// Gets image source from icon intptr
        /// </summary>
        internal static System.Windows.Media.ImageSource GetImageSourceFromIcon(IntPtr hIcon)
        {
            try
            {
                if (hIcon == IntPtr.Zero)
                    return null;

                ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                        hIcon,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                img.Freeze();
                return img;
            }
            catch (ArgumentException exp)
            {
                Debug.Assert(false, exp.Message);
            }
            catch (Exception exp)
            {
                Debug.Assert(false, exp.Message);
            }
            return null;
        }


        /// <summary>
        /// Load file icon
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="attributes">The attr.</param>
        public static ImageSource LoadFileInfo(string path, FileAttributes attributes)
        {
            if (path == null) return null;
            if (attributes == FileAttributes.Normal)
            {
                if (!CommonProvider.File.Exists(path))
                {
                    path = Path.GetFileName(path);
                }
            }

            //TT91027 - The folder's icon doesn't display in the content list while restore the backup job which target is FTP.
            //change ftp path '/' to '\' for XP.
            path = path.Replace('/', '\\');
            ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI vFlags =
                ShellAPI.SHGFI.SHGFI_SMALLICON |
                ShellAPI.SHGFI.SHGFI_ICON |
                ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES;

            ShellAPI.SHGetFileInfo(path, (uint)attributes, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
            var ico = GetImageSourceFromIcon(shellInfo.hIcon);

            if (shellInfo.hIcon != IntPtr.Zero)
            {
                User32API.DestroyIcon(shellInfo.hIcon);
                shellInfo.hIcon = IntPtr.Zero;
            }

            if (ico == null && attributes == FileAttributes.Directory)
            {
                ico = DefaultFolderImage;
            }
            return ico;
        }


        /// <summary>
        /// Load file icon
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="attributes">The attr.</param>
        public static ImageSource LoadFileInfo(string path, FileAttributes attributes, out string typeName)
        {
            typeName = string.Empty;
            if (path == null) return null;
            if (attributes == FileAttributes.Normal)
            {
                if (!CommonProvider.File.Exists(path))
                {
                    path = Path.GetFileName(path);
                }
            }
            //TT91027 - The folder's icon doesn't display in the content list while restore the backup job which target is FTP.
            //change ftp path '/' to '\' for XP.
            path = path.Replace('/', '\\');
            ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI vFlags =
                ShellAPI.SHGFI.SHGFI_SMALLICON |
                ShellAPI.SHGFI.SHGFI_ICON |
                ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES |
                ShellAPI.SHGFI.SHGFI_TYPENAME;

            ShellAPI.SHGetFileInfo(path, (uint)attributes, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
            var ico = GetImageSourceFromIcon(shellInfo.hIcon);
            if (shellInfo.hIcon != IntPtr.Zero)
            {
                User32API.DestroyIcon(shellInfo.hIcon);
                shellInfo.hIcon = IntPtr.Zero;
            }
            typeName = shellInfo.szTypeName;
            if (ico == null && attributes == FileAttributes.Directory)
            {
                ico = DefaultFolderImage;
            }
            return ico;
        }


        public static ImageSource ToImageSource(this Icon icon)
        {
            try
            {
                if (icon == null) return null;
                ImageSource img = Imaging.CreateBitmapSourceFromHIcon(
                                      icon.Handle,
                                        new Int32Rect(0, 0, icon.Width, icon.Height),
                                        BitmapSizeOptions.FromEmptyOptions());
                img.Freeze();
                return img;
            }
            catch { }
            return null;
        }

        public static ImageSource LoadDriveInfo(string path, out string displayName)
        {
            displayName = string.Empty;
            if (path == null) return null;
            //TT91027 - The folder's icon doesn't display in the content list while restore the backup job which target is FTP.
            //change ftp path '/' to '\' for XP.
            path = path.Replace('/', '\\');
            ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI vFlags =
                ShellAPI.SHGFI.SHGFI_SMALLICON |
                ShellAPI.SHGFI.SHGFI_ICON |
                ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES |
                ShellAPI.SHGFI.SHGFI_DISPLAYNAME;

            ShellAPI.SHGetFileInfo(path, (uint)FileAttributes.Directory, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
            var ico = GetImageSourceFromIcon(shellInfo.hIcon);
            if (shellInfo.hIcon != IntPtr.Zero)
            {
                User32API.DestroyIcon(shellInfo.hIcon);
                shellInfo.hIcon = IntPtr.Zero;
            }
            displayName = shellInfo.szDisplayName;
            return ico;
        }

        public static ImageSource LoadDriveIcon(string path)
        {
            if (path == null) return null;
            //TT91027 - The folder's icon doesn't display in the content list while restore the backup job which target is FTP.
            //change ftp path '/' to '\' for XP.
            path = path.Replace('/', '\\');
            ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI vFlags =
                ShellAPI.SHGFI.SHGFI_LARGEICON |
                ShellAPI.SHGFI.SHGFI_ICON |
                ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES |
                ShellAPI.SHGFI.SHGFI_DISPLAYNAME;

            ShellAPI.SHGetFileInfo(path, (uint)FileAttributes.Directory, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
            var ico = GetImageSourceFromIcon(shellInfo.hIcon);
            if (shellInfo.hIcon != IntPtr.Zero)
            {
                User32API.DestroyIcon(shellInfo.hIcon);
                shellInfo.hIcon = IntPtr.Zero;
            }
            return ico;
        }

        /// <summary>
        /// Load file  icon, file type from shell
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="attributes">The attr.</param>
        public static System.Windows.Media.ImageSource LoadFileInfo(string path, out string typeName)
        {
            FileAttributes attributes;
            if (CommonProvider.Directory.Exists(path))
                attributes = FileAttributes.Directory;
            else if (CommonProvider.File.Exists(path))
                attributes = FileAttributes.Normal;
            else
            {
                typeName = string.Empty;
                return null;
            }

            typeName = string.Empty;
            if (path == null) return null;
            //TT91027 - The folder's icon doesn't display in the content list while restore the backup job which target is FTP.
            //change ftp path '/' to '\' for XP.
            path = path.Replace('/', '\\');
            ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
            ShellAPI.SHGFI vFlags =
                ShellAPI.SHGFI.SHGFI_SMALLICON |
                ShellAPI.SHGFI.SHGFI_ICON |
                ShellAPI.SHGFI.SHGFI_USEFILEATTRIBUTES |
                ShellAPI.SHGFI.SHGFI_TYPENAME;

            ShellAPI.SHGetFileInfo(path, (uint)attributes, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
            var ico = GetImageSourceFromIcon(shellInfo.hIcon);
            //displayName = shellInfo.szDisplayName;
            if (shellInfo.hIcon != IntPtr.Zero)
            {
                User32API.DestroyIcon(shellInfo.hIcon);
                shellInfo.hIcon = IntPtr.Zero;
            }
            typeName = shellInfo.szTypeName;
            if (ico == null && attributes == FileAttributes.Directory)
            {
                ico = DefaultFolderImage;
            }
            return ico;
        }

        /// <summary>
        /// load computer display name and icon
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="computerIcon"></param>
        public static void LoadComputer(out string displayName, out ImageSource computerIcon)
        {
            try
            {
                IntPtr ptrIDL = IntPtr.Zero;
                var result = ShellAPI.SHGetSpecialFolderLocation(IntPtr.Zero, ShellAPI.CSIDL.CSIDL_DRIVES, ref ptrIDL);
                if (result != 0)
                    Marshal.ThrowExceptionForHR(result);

                ShellAPI.SHFILEINFO shellInfo = new ShellAPI.SHFILEINFO();
                ShellAPI.SHGFI vFlags =
                    ShellAPI.SHGFI.SHGFI_SMALLICON |
                    ShellAPI.SHGFI.SHGFI_ICON |
                    ShellAPI.SHGFI.SHGFI_PIDL |
                    ShellAPI.SHGFI.SHGFI_DISPLAYNAME |
                    ShellAPI.SHGFI.SHGFI_TYPENAME |
                    ShellAPI.SHGFI.SHGFI_ADDOVERLAYS;
                ShellAPI.SHGetFileInfo(ptrIDL, 0, ref shellInfo, (uint)Marshal.SizeOf(shellInfo), vFlags);
                displayName = shellInfo.szDisplayName;
                computerIcon = GetImageSourceFromIcon(shellInfo.hIcon);
                if (shellInfo.hIcon != IntPtr.Zero)
                {
                    User32API.DestroyIcon(shellInfo.hIcon);
                    shellInfo.hIcon = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                displayName = string.Empty;
                computerIcon = null;
                LogHelper.Debug("Application Exception:", ex);
            }
        }
        #endregion

        #region static method

        #region For Path
        /// <summary>
        ///  check father path or grand-father path.
        ///  if same ,return true
        /// </summary>
        /// <param name="topPath">check parent path</param>
        /// <param name="subPath">check sub path</param>
        /// <returns></returns>
        public static bool CheckIsTopLevel(string topPath, string subPath)
        {
            if (topPath == null || subPath == null) return false;
            if (string.Compare(topPath, subPath, StringComparison.OrdinalIgnoreCase) == 0) return true;

            var parentArray = topPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            var subArray = subPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (parentArray.Length > subArray.Length) return false;
            for (int i = 0; i < parentArray.Length; i++)
            {
                if (!string.Equals(parentArray[i], subArray[i], StringComparison.CurrentCultureIgnoreCase)) return false;
            }
            return true;
        }

        /// <summary>
        ///  check father path or grand-father path for personal path.
        ///  if same ,return true
        ///  It is different from CheckIsTopLevel.
        ///  For example, "My Documents" is sub path of "UserProfile". But sometimes full path for "UserProfile"
        ///  is C:\UserName while "My Documents" is D:\UserName\Documents.
        /// </summary>
        /// <param name="topPath">check parent path</param>
        /// <param name="subPath">check sub path</param>
        /// <returns></returns>
        public static bool CheckIsPersonalTopLevel(string topPath, string subPath)
        {
            if (!topPath.IsEqual(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)))
            {
                return false;
            }

            if (subPath.IsEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
                || subPath.IsEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
                || subPath.IsEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic))
                || subPath.IsEqual(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///  check is parent or not.
        ///  if same ,return false
        /// </summary>
        /// <param name="parentPath">check parent path</param>
        /// <param name="subPath">check sub path</param>
        /// <returns></returns>
        public static bool CheckIsParent(string parentPath, string sonPath)
        {
            if (string.IsNullOrEmpty(parentPath) || string.IsNullOrEmpty(sonPath)) return false;
            if (parentPath.IsShellPath() || sonPath.IsShellPath()) return false;
            if (string.Compare(parentPath, sonPath, StringComparison.OrdinalIgnoreCase) == 0) return false;

            if (sonPath.EndsWith(@"\"))
                sonPath = sonPath.Substring(0, sonPath.Length - 1);

            try
            {
                if (string.Equals(parentPath, Path.GetPathRoot(parentPath), StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!parentPath.EndsWith(@"\"))
                        parentPath = parentPath + @"\";
                    return string.Equals(parentPath, Path.GetDirectoryName(sonPath), StringComparison.CurrentCultureIgnoreCase);
                }
                else
                {
                    if (parentPath.EndsWith(@"\"))
                        parentPath = parentPath.Substring(0, parentPath.Length - 1);
                    return string.Equals(parentPath, Path.GetDirectoryName(sonPath), StringComparison.CurrentCultureIgnoreCase);
                }
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Application Exception:", ex);
                return false;
            }
        }


        /// <summary>
        /// get path , source path must be directory path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rangeNumber"></param>
        /// <returns></returns>
        public static string GetPathRange(string path, int rangeNumber)
        {
            if (string.IsNullOrEmpty(path)) return string.Empty;

            var pathArray = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            if (rangeNumber > pathArray.Length) return string.Empty;
            StringBuilder newPath = new StringBuilder();
            for (int i = 0; i < rangeNumber; i++)
            {
                if (i == rangeNumber - 1)
                    newPath.Append(pathArray[i]);
                else
                    newPath.Append(pathArray[i] + @"\");
            }
            return newPath.ToString();
        }

        /// <summary>
        /// get path , source path must be directory path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rangeNumber"></param>
        /// <returns></returns>
        public static int GetRangeNumber(string path)
        {
            if (string.IsNullOrEmpty(path)) return -1;

            var pathArray = path.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            return pathArray.Length;
        }

        /// <summary>
        /// if one drive broken, use [Directory.Exists] may cause 10 seconds,
        /// so design this function with timeout.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static bool? CheckPathExistWithTimeout(string path, double timeout)
        {
            return CommonProvider.Directory.Exists(path, (int)(timeout * 1000));
        }
        #endregion

        #region For Drive
        /// <summary>
        /// get drive type from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static DriveType? GetDriveType(string path)
        {
            if (!CommonProvider.Directory.Exists(path) && !CommonProvider.File.Exists(path)) return null;

            var drivePath = path.Substring(0, 2);
            try
            {
                DriveInfo drive = new DriveInfo(drivePath);
                return drive.DriveType;
            }
            catch (ArgumentNullException ex)
            {
                Debug.Write(ex);
                return null;
            }
            catch (ArgumentException ex)
            {
                Debug.Write(ex);
                return null;
            }
        }

        /// <summary>
        /// get drive type from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetDriveFormat(string path)
        {
            try
            {
                if (!CommonProvider.Directory.Exists(path) && !CommonProvider.File.Exists(path)) return null;
                var drivePath = Path.GetPathRoot(path);
                DriveInfo drive = new DriveInfo(drivePath);
                return drive.DriveFormat;
            }
            catch (ArgumentNullException ex)
            {
                Debug.Write(ex);
                return null;
            }
            catch (ArgumentException ex)
            {
                Debug.Write(ex);
                return null;
            }
        }
        #endregion

        #region FileAttribute
        /// <summary>
        /// check file arrtibute,if  system ,return false
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool CheckFileAttributes(string filePath)
        {
            if (filePath.IsNullOrEmpty()) return false;
            if (filePath.Length > 248) return false;
            //for shell object, or not exist path
            if (!CommonProvider.Directory.Exists(filePath) && !CommonProvider.File.Exists(filePath)) return true;
            //for partition path
            if (Path.GetPathRoot(filePath).IsPathEqual(filePath)) return true;
            try
            {
                FileAttributes fileAttributes = System.IO.File.GetAttributes(filePath);
                if ((fileAttributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    return false;
                if ((fileAttributes & FileAttributes.System) == FileAttributes.System)
                    return false;
            }
            catch { }
            return true;
        }

        static List<string> defaultExcludedList = new List<string>();
        public static List<string> GetDefaultExcludedPathList()
        {
            try
            {
                if (defaultExcludedList.Count == 0)
                {
                    string strProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                    string strProgameFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                    string strWindows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                    string strProgramFilesW6432 = Environment.GetEnvironmentVariable("ProgramW6432");

                    if (!strWindows.IsNullOrEmpty())
                    {
                        defaultExcludedList.Add(strWindows);
                    }

                    if (!strProgramFiles.IsNullOrEmpty())
                    {
                        defaultExcludedList.Add(strProgramFiles);
                    }

                    if (!strProgramFilesW6432.IsNullOrEmpty() && strProgramFilesW6432 != strProgramFiles)
                    {
                        defaultExcludedList.Add(strProgramFilesW6432);
                    }

                    if (!strProgameFilesX86.IsNullOrEmpty() && strProgameFilesX86 != strProgramFiles)
                    {
                        defaultExcludedList.Add(strProgameFilesX86);
                    }

                    // It is enough to add above four methods. So comment out following code for safe.
                    //string strCommonProgram = Environment.GetFolderPath(Environment.SpecialFolder.CommonPrograms);
                    //string strCommonProgramFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFilesX86);
                    //string strCommonProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles);
                    //string strSystemFolder = Environment.GetFolderPath(Environment.SpecialFolder.System);
                    //string strSystemX86 = Environment.GetFolderPath(Environment.SpecialFolder.SystemX86);
                    //defaultExcludedList.Add(strCommonProgram);
                    //defaultExcludedList.Add(strCommonProgramFilesX86);
                    //defaultExcludedList.Add(strCommonProgramFiles);
                    //defaultExcludedList.Add(strSystemFolder);
                    //defaultExcludedList.Add(strSystemX86);
                }
            }
            catch (System.Exception ex)
            {
                LogHelper.Debug("Exception:", ex);
            }

            return defaultExcludedList;
        }

        /// <summary>
        /// check whether file path is exclude by BIU.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool CheckFilePathFilters(string filePath, bool bExcludeWithDefaultFilters)
        {
            if (!bExcludeWithDefaultFilters)
                return true;

            if (filePath.IsNullOrEmpty()) return false;
            if (filePath.Length > 248) return false;
            //for shell object, or not exist path
            if (!CommonProvider.Directory.Exists(filePath) && !CommonProvider.File.Exists(filePath)) return true;
            //for partition path
            if (Path.GetPathRoot(filePath).IsPathEqual(filePath)) return true;
            try
            {
                List<string> pathList = GetDefaultExcludedPathList();
                foreach (string filterPath in pathList)
                {
                    string filterSubPath = filterPath;
                    filterSubPath.TrimEnd('*');

                    if (IsPathIncluded(filterPath, filePath))
                    {
                        return false;
                    }
                }
            }
            catch { }

            return true;
        }

        /// <summary>
        /// Detest parentPath includes childPathFile or not.
        /// </summary>
        /// <param name="parentPath"></param>
        /// <param name="childPathFile"></param>
        /// <returns></returns>    
        public static bool IsPathIncluded(string parentPath, string childPathFile)
        {
            bool isIncluded = false;
            if (!string.IsNullOrEmpty(parentPath) && !string.IsNullOrEmpty(childPathFile)
                && parentPath.Length <= childPathFile.Length)
            {
                if (parentPath.Length < childPathFile.Length)
                {
                    char lastChar = parentPath[parentPath.Length - 1];
                    if (lastChar != '\\' && lastChar != '/')
                    {
                        parentPath += '\\';
                    }
                }

                isIncluded = string.Compare(parentPath, 0, childPathFile, 0, parentPath.Length, true) == 0;
            }

            return isIncluded;
        }

        #endregion

        #region get shell sub paths, convert shell path to FileSystem paths
        /// <summary>
        /// collect paths
        /// </summary>
        static List<string> shellSubPaths = new List<string>();

        /// <summary>
        /// get  paths , from shell folder
        /// </summary>
        /// <param name="folder">shell folder</param>
        /// <returns></returns>
        public static List<string> GetShellSubPaths(DataSourceShell shell)
        {
            shellSubPaths.Clear();
            if (shell == null) return null;
            getSubPathByRecursion(shell);
            return shellSubPaths;
        }

        /// <summary>
        /// get path from shell
        /// </summary>
        /// <param name="shell"></param>
        static void getSubPathByRecursion(DataSourceShell shell)
        {
            var items = shell.GetSubItems();

            foreach (var item in items)
            {
                if (item.Path.IsNullOrEmpty())
                {
                    getSubPathByRecursion(item);
                }
                else
                {
                    shellSubPaths.Add(item.Path);
                }
            }
        }
        #endregion
        #endregion

        #region shell object identifer

        /// <summary>
        /// identifer for every shellobject
        /// </summary>
        public const string ComputerParseName = "::{20D04FE0-3AEA-1069-A2D8-08002B30309D}";
        public const string RecycleBinParseName = "::{645FF040-5081-101B-9F08-00AA002F954E}";
        public const string ControlParseName = "::{5399E694-6CE5-4D6C-8FCE-1D8870FDCBA0}";
        public const string LibrariesParseName = "::{031E4825-7B94-4DC3-B131-E946B44C8DD5}";
        public const string NetworkParseName = "::{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}";
        public const string NetworkForXPParseName = "::{208D2C60-3AEA-1069-A2D7-08002B30309D}";

        #endregion

        #region Class Directory
        public static class Directory
        {
            /// <summary>
            /// if one drive broken, use [Directory.Exists] may cause 10 seconds,
            /// so design this function with timeout.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public static bool Exists(string path, int timeout = 0)
            {
                if (timeout <= 0)
                {
                    return System.IO.Directory.Exists(path);
                }

                bool isWait = true;
                bool isReady = false;

                CancellationTokenSource cancelSource = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    isReady = System.IO.Directory.Exists(path);
                    isWait = false;
                }, cancelSource.Token);

                int ticks = 0;
                while (isWait)
                {
                    if (ticks >= timeout)
                    {
                        break;
                    }
                    ticks++;
                    Thread.Sleep(1);
                }
                cancelSource.Cancel();
                return isReady;
            }
        }
        #endregion

        #region Class File
        public static class File
        {
            /// <summary>
            /// if one drive broken, use [Directory.Exists] may cause 10 seconds,
            /// so design this function with timeout.
            /// </summary>
            /// <param name="path"></param>
            /// <param name="timeout"></param>
            /// <returns></returns>
            public static bool Exists(string path, int timeout = 0)
            {
                if (timeout <= 0)
                {
                    return System.IO.File.Exists(path);
                }

                bool isWait = true;
                bool isReady = false;

                CancellationTokenSource cancelSource = new CancellationTokenSource();
                Task.Factory.StartNew(() =>
                {
                    isReady = System.IO.File.Exists(path);
                    isWait = false;
                }, cancelSource.Token);

                int ticks = 0;
                while (isWait)
                {
                    if (ticks >= timeout)
                    {
                        break;
                    }
                    ticks++;
                    Thread.Sleep(1);
                }
                cancelSource.Cancel();
                return isReady;
            }
        }
        #endregion
    }
}
