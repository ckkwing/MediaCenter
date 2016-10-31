using FileExplorer.Helper;
using NBEngineLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileExplorer.Model
{
    public class CDRomFolder : LocalFolder
    {
        protected CDRomFolder()
        {
        }

        protected IFileBackupItem CurrentFolder { get; set; }

        public CDRomFolder(IFileBackupItem folder, IFolder parent)
            : base(string.Empty, parent)
        {
            if (folder.IsNull() || parent.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.CurrentFolder = folder;
            this.Parent = parent;
            this.FullPath = GetProperty<string>(CurrentFolder, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_PATH);
            ///Driver name end with '\'
            if (this.FullPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                this.Name = this.FullPath;
            }
            else
            {
                this.Name = GetProperty<string>(CurrentFolder, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_WINNAME);
            }
            this.LastModifyTime = GetProperty<DateTime>(CurrentFolder, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_MODIFIEDDATE);
            this.AddPlaceHolder();
        }

        #region IFolderAsync

        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Clear();
                });
                LoadAllData();
            }
            return this.Folders;
        }

        protected override IEnumerable<IFile> GetFiles()
        {
            if (!IsFileLoaded)
            {
                RunOnUIThread(() =>
                {
                    this.Files.Clear();
                });
                LoadAllData();
            }
            return this.Files;
        }

        private void LoadAllData()
        {
            IsFolderLoaded = false;
            IsFileLoaded = false;

            RunOnUIThread(() =>
            {
                this.Folders.Clear();
                this.Files.Clear();
            });

            var item = GetFristItem(this.CurrentFolder);
            IList<IFolder> folderList = new List<IFolder>();
            IList<IFile> fileList = new List<IFile>();
            while (!item.IsNull())
            {
                if (GetIsFolder(item))
                {
                    CDRomFolder folder = new CDRomFolder(item, this);
                    folderList.Add(folder);
                }
                else
                {
                    CDRomFile file = new CDRomFile(item, this);
                    fileList.Add(file);
                }
                item = GetNextItem(item);
            }

            IsFolderLoaded = AddItemsByChunk(SetFolderOrder(folderList), this.Folders, this.Items);
            IsFileLoaded = AddItemsByChunk(SetFileOrder(fileList), this.Files, this.Items);
        }

        private bool GetIsFolder(IFileBackupItem item)
        {
            uint propValue = GetProperty<uint>(item, NB_FILEBACKUPITEM_PROPERTY_TYPE.NB_FILEBACKUPITEM_PROPERTY_ATTRIBUTES);
            bool result = (propValue & ((uint)FileAttributes.Directory)) > 0;
            return result;
        }

        private T GetProperty<T>(IFileBackupItem item, NB_FILEBACKUPITEM_PROPERTY_TYPE propName)
        {
            T result = default(T);
            if (item.IsNull())
                return result;
            try
            {
                result = item.GetProperty(propName);
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            return result;
        }

        private IFileBackupItem GetFristItem(IFileBackupItem backupItem)
        {
            return GetItem(backupItem, NB_FILEBACKUPITEM_COLLECTION_TYPE.NB_FILEBACKUPITEM_COLLECTION_CHILD);
        }

        private IFileBackupItem GetNextItem(IFileBackupItem backupItem)
        {
            return GetItem(backupItem, NB_FILEBACKUPITEM_COLLECTION_TYPE.NB_FILEBACKUPITEM_COLLECTION_NEXT);
        }

        private IFileBackupItem GetItem(IFileBackupItem backupItem, NB_FILEBACKUPITEM_COLLECTION_TYPE itemType)
        {
            IFileBackupItem result = null;
            if (backupItem.IsNull())
                return result;
            try
            {
                result = backupItem.GetCollection(itemType) as IFileBackupItem;
            }
            catch (Exception ex)
            {
                LogHelper.Debug("Failed", ex);
            }
            return result;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            this.CurrentFolder = null;
            base.OnDisposing(isDisposing);
        }

        #endregion


        public override object Clone()
        {
            CDRomFolder file = new CDRomFolder();
            CloneMembers(file);
            return file;
        }

    }
}
