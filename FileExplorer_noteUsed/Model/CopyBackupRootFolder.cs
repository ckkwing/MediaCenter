using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileExplorer.Model
{
    public class CopyBackupRootFolder : CopyBackupFolder
    {
        private static string[] drive_split_string = new string[] { "_@#@" };
        public string RootFolder { get; private set; }

        public CopyBackupRootFolder(string rootFolder, string displayName)
            : base(string.Empty, null, null)
        {
            if (rootFolder.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }

            RootFolder = rootFolder;

            using (DataSourceShell shellItem = LocalExplorerFactory.GetPCRootShellItem())
            {
                this.Name = displayName;
                this.Icon = shellItem.Icon;
            }

            this.IsCheckVisible = false;
            this.IsExpanded = true;
            this.IsSelected = true;
            this.Parent = this;
        }

        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                IList<IFolder> tempItems = new List<IFolder>();
                DirectoryInfo dirInfo = new DirectoryInfo(this.RootFolder);
                DirectoryInfo[] dirs = dirInfo.GetDirectories();
                if (dirs != null)
                {
                    foreach (DirectoryInfo dir in dirs)
                    {
                        IFolder folder = null;
                        string name = string.Empty;
                        string[] drive = dir.Name.Split(drive_split_string, StringSplitOptions.RemoveEmptyEntries);
                        if (drive != null && drive.Length > 0)
                        {
                            name = drive[0];
                        }
                        name += ":\\";
                        folder = new CopyBackupFolder(dir, name, this);
                        tempItems.Add(folder);
                    }
                }

                IsFolderLoaded = AddItemsByChunk(tempItems, this.Folders, this.Items);
            }
            return this.Folders;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
        }
    }
}
