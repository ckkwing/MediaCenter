using FileExplorer.Factory;
using FileExplorer.Shell;
using NBEngineLib;
using System;
using System.Collections.Generic;

namespace FileExplorer.Model
{
    public class CDRomRootFolder : CDRomFolder
    {
        /// <summary>
        /// Keep Job,else subitem will be release by the GC
        /// </summary>
        private JobData jobData { get; set; }
        private string JsonPath { get; set; }

        public CDRomRootFolder(string jsonPath)
            : base()
        {
            if (jsonPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            using (DataSourceShell shellItem = LocalExplorerFactory.GetPCRootShellItem())
            {
                this.Name = shellItem.DisplayName;
                this.Icon = shellItem.Icon;
            }

            this.JsonPath = jsonPath;
            this.IsExpanded = true;
            this.IsSelected = true;
            this.IsChecked = true;
            this.Parent = this;
        }

        protected override IEnumerable<IFolder> GetFolders()
        {
            READ_DATA_TYPES types = READ_DATA_TYPES.RD_OPEN_BACKUP_FOR_RESTORE |
                                    READ_DATA_TYPES.RD_NOT_CHANGE_VERSION |
                                    READ_DATA_TYPES.RD_READ_BACKUP_TREE;

            jobData = new JobData();
            NB_ERROR_CODE errorCode = jobData.Load(types, JsonPath, false, true);
            if (errorCode == NB_ERROR_CODE.EC_SUCCESS && !isDisposed)
            {
                this.CurrentFolder = jobData.GetCollection(NB_JOBDATA_COLLECTION_TYPE.NB_JOBDATA_COLLECTION_FILEBACKUPITEM) as IFileBackupItem;
                if (this.CurrentFolder != null && !isDisposed)
                {
                    return base.GetFolders();
                }
            }
            return this.Folders;
        }

        protected override void OnDisposing(bool isDisposing)
        {
            jobData = null;
            base.OnDisposing(isDisposing);
        }
    }

}
