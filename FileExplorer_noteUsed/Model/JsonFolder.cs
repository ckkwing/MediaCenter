using FileExplorer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Data = BackItUp.Infrastructure.FileExplorerDatas;

namespace FileExplorer.Model
{
    public class JsonFolder : LocalFolder
    {
        protected JsonFolder()
        {
        }

        public JsonFolder(Data.JsonFolder folder, IFolder parent)
            : base(string.Empty, parent)
        {
            if (folder.IsNull() || parent.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.JasonFolder = folder;
            this.Parent = parent;
            if (folder.Name.EndsWith(":"))
            {
                ///Check is driver
                string folderName = folder.Name + @"\";
                this.Name = folderName;
                this.FullPath = folderName;
            }
            else
            {
                this.Name = Path.GetFileName(folder.Name);
                this.FullPath = folder.Name;
            }
            this.AddPlaceHolder();
        }

        Data.JsonFolder JasonFolder { get; set; }

        #region IFolderAsync

        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Clear();
                });

                var jsonFolders = this.JasonFolder.Items.Where(item => item.IsFolder);
                IList<IFolder> tempItems = new List<IFolder>();
                foreach (var item in jsonFolders)
                {
                    string folderPath = Path.Combine(this.FullPath, item.Name);
                    Data.JsonFolder jsonFolder = JsonRootFolder.GetFolderFromCache(folderPath);
                    if (!jsonFolder.IsNull())
                    {
                        JsonFolder folder = new JsonFolder(jsonFolder, this);
                        tempItems.Add(folder);
                    }
                }

                IsFolderLoaded = AddItemsByChunk(SetFolderOrder(tempItems), this.Folders, this.Items);
            }
            return this.Folders;
        }

        protected override IEnumerable<IFile> GetFiles()
        {
            if (!IsFileLoaded)
            {
                var jsonFiles = this.JasonFolder.Items.Where(item => !item.IsFolder);
                IList<IFile> tempItems = new List<IFile>();
                foreach (var item in jsonFiles)
                {
                    JsonFile file = new JsonFile(item, this);
                    tempItems.Add(file);
                }

                IsFileLoaded = AddItemsByChunk(SetFileOrder(tempItems), this.Files, this.Items);
            }
            return this.Files;
        }

        public override object Clone()
        {
            JsonFolder file = new JsonFolder();
            CloneMembers(file);
            return file;
        }
        #endregion
    }
}
