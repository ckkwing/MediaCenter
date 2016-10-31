using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data = BackItUp.Infrastructure.FileExplorerDatas;

namespace FileExplorer.Model
{
    public class JsonRootFolder : JsonFolder
    {
        public static string JsonPath { get; private set; }

        #region Static members

        /// <summary>
        /// Cache for quick search folder by path
        /// Driver path  contain path seperate: c:\
        /// Folder Path : c:\Windows
        /// </summary>
        private static IDictionary<string, Data.JsonFolder> JsonFolderCache = new Dictionary<string, Data.JsonFolder>();

        public static void ClearCache()
        {
            if (!JsonFolderCache.IsNullOrEmpty())
            {
                JsonFolderCache.Clear();
            }
        }

        public static Data.JsonFolder GetFolderFromCache(string path)
        {
            Data.JsonFolder folder = null;
            if (path.IsNullOrEmpty() || !JsonFolderCache.ContainsKey(path))
            {
                return folder;
            }

            folder = JsonFolderCache[path];
            return folder;
        }

        #endregion


        public JsonRootFolder(string jsonPath)
            : base()
        {
            if (jsonPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            JsonPath = jsonPath;

            using (DataSourceShell shellItem = LocalExplorerFactory.GetPCRootShellItem())
            {
                this.Name = shellItem.DisplayName;
                this.Icon = shellItem.Icon;
            }

            this.IsExpanded = true;
            this.IsSelected = true;
            this.IsChecked = true;
            this.Parent = this;
        }

        protected override IEnumerable<IFolder> GetFolders()
        {
            JsonParser factory = new JsonParser();
            IList<Data.JsonFolder> items = factory.GetJsonItems(JsonPath);
            JsonFolderCache.Clear();

            foreach (var item in items)
            {
                JsonFolderCache[item.Name] = item;
            }

            var allPaths = JsonFolderCache.Keys.OrderBy(item => item.Length);

            IList<string> rootList = new List<string>();
            foreach (var folderPath in allPaths)
            {
                if (!rootList.Any(o => CheckIsTopLevel(o, folderPath)))
                {
                    //find a top level path
                    rootList.Add(folderPath);
                }
            }

            IList<IFolder> tempItems = new List<IFolder>();
            foreach (var item in rootList)
            {
                if (JsonFolderCache.ContainsKey(item))
                {
                    Data.JsonFolder jsonFolder = JsonFolderCache[item];
                    JsonFolder folder = new JsonFolder(jsonFolder, this);
                    tempItems.Add(folder);
                }
            }

            IsFolderLoaded = AddItemsByChunk(tempItems, this.Folders, this.Items);
            return this.Folders;
        }

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
            if (topPath.Equals(subPath)) return true;
            char[] splitChars = new char[] { Path.DirectorySeparatorChar };

            var parentArray = topPath.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            var subArray = subPath.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
            if (parentArray.Length > subArray.Length) return false;
            for (int i = 0; i < parentArray.Length; i++)
            {
                if (!parentArray[i].Equals(subArray[i]))
                {
                    return false;
                }
            }
            return true;
        }

        protected override IEnumerable<IFile> GetFiles()
        {
            IsFileLoaded = true;
            return this.Files;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            ClearCache();
            base.OnDisposing(isDisposing);
        }
    }
}
