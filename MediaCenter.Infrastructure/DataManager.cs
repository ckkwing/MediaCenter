using IDAL.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Utilities.Extension;
using Utilities.FileScan;

namespace MediaCenter.Infrastructure
{
    public sealed class DataManager
    {
        public static DataManager Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly DataManager instance = new DataManager();
        }

        private IList<MonitoredFolderInfo> newMonitoredFolderInfos = new List<MonitoredFolderInfo>();
        private FileScanner fileScanner = new FileScanner();
        public FileScanner FileScanner
        {
            get
            {
                return fileScanner;
            }
        }

        private DBCache dbCache = new DBCache();
        public DBCache DBCache
        {
            get
            {
                return dbCache;
            }

            set
            {
                dbCache = value;
            }
        }

        DataManager()
        {

        }

        public void Init()
        {
            DBCache.Init();
            FileScanner.ProcessEvent += FileScanner_ProcessEvent;
        }

        public void Uninit()
        {
            FileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            DBCache.Uninit();
        }

        private void FileScanner_ProcessEvent(object sender, FileScannerProcessEventArgs e)
        {
            switch (e.ProcessType)
            {
                case ProcessType.Start:
                    newMonitoredFolderInfos.Clear();
                    break;
                case ProcessType.InProcess:
                    {
                        if (e.InnerType != InnerType.OneDirectoryScanned)
                            return;
                        if (e.IsNull() || e.CurrentDir.IsNull())
                            return;
                        MonitoredFolderInfo monitoredFolderInfo = MonitoredFolderInfo.Convert(e.CurrentDir);
                        monitoredFolderInfo.IsScanned = true;
                        newMonitoredFolderInfos.Add(monitoredFolderInfo);
                        IList<MonitoredFolderInfo> monitoredFolderInfos = new List<MonitoredFolderInfo>() {
                                monitoredFolderInfo
                            };
                        MonitoredFolderInfo existFolder = DBCache.MonitoredFolders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true));
                        if (null == existFolder)
                        {
                            DBHelper.InsertFolders(monitoredFolderInfos);
                        }
                        else
                        {
                            existFolder.IsScanned = true;
                            monitoredFolderInfos = new List<MonitoredFolderInfo>() {
                                existFolder
                            };
                            DBHelper.UpdateFolders(monitoredFolderInfos);
                        }
                        IList<MonitoredFile> filesToAdd = new List<MonitoredFile>();
                        foreach (ScannedFileInfo fileInfo in e.Files)
                        {
                            if (null == fileInfo || null == fileInfo.File)
                                continue;
                            if (null == DBCache.MonitoredFiles.FirstOrDefault(file => 0 == string.Compare(fileInfo.File.FullName, file.Path)))
                            {
                                MonitoredFile file = MonitoredFile.Convert(fileInfo);
                                file.ParentID = monitoredFolderInfo.ID;
                                filesToAdd.Add(file);
                            }
                        }
                        DBHelper.InsertFiles(filesToAdd);
                    }
                    break;
                case ProcessType.End:
                    {
                        RemoveUnMonitoredFoldersAndFiles();
                        DBCache.RefreshMonitoredFiles();
                        DBCache.RefreshMonitoredFolders();
                    }
                    break;
            }
        }

        private void RemoveUnMonitoredFoldersAndFiles()
        {
            IList<MonitoredFolderInfo> foldersToDeleteFromDB = new List<MonitoredFolderInfo>();
            foreach (MonitoredFolderInfo folder in DBCache.MonitoredFolders)
            {
                bool isNeedToDelete = true;
                foreach (MonitoredFolderInfo newFolder in newMonitoredFolderInfos)
                {
                    if (0 == string.Compare(folder.Path, newFolder.Path, true))
                    {
                        isNeedToDelete = false;
                        break;
                    }
                }
                if (isNeedToDelete)
                    foldersToDeleteFromDB.Add(folder);
            }
            DBHelper.DeleteFolders(foldersToDeleteFromDB);
            DBHelper.DeleteFilesUnderFolders(foldersToDeleteFromDB);
        }

    }
}
