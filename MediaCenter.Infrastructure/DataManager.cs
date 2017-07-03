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

        private IList<DBFolderInfo> newMonitoredFolderInfos = new List<DBFolderInfo>();
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
                        if (e.IsNull() || e.CurrentDir.IsNull())
                            return;

                        switch(e.InnerType)
                        {
                            case InnerType.OneFileScanned:
                                break;
                            case InnerType.OneDirectoryScanned:
                                ExecuteWhenDirectoryScanned(e);
                                break;
                            default:
                                return;
                        }

                        //DBFolderInfo monitoredFolderInfo = DBFolderInfo.Convert(e.CurrentDir);
                        //monitoredFolderInfo.IsScanned = true;
                        //newMonitoredFolderInfos.Add(monitoredFolderInfo);
                        //IList<DBFolderInfo> monitoredFolderInfos = new List<DBFolderInfo>() {
                        //        monitoredFolderInfo
                        //    };
                        //DBFolderInfo existFolder = DBCache.Folders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true));
                        //if (null == existFolder)
                        //{
                        //    DBHelper.InsertFolders(monitoredFolderInfos);
                        //}
                        //else
                        //{
                        //    existFolder.IsScanned = true;
                        //    monitoredFolderInfos = new List<DBFolderInfo>() {
                        //        existFolder
                        //    };
                        //    DBHelper.UpdateFolders(monitoredFolderInfos);
                        //}
                        //IList<IDAL.Model.DBFileInfo> filesToAdd = new List<IDAL.Model.DBFileInfo>();
                        //foreach (ScannedFileInfo fileInfo in e.Files)
                        //{
                        //    if (null == fileInfo || null == fileInfo.File)
                        //        continue;
                        //    if (null == DBCache.Files.FirstOrDefault(file => 0 == string.Compare(fileInfo.File.FullName, file.Path)))
                        //    {
                        //        IDAL.Model.DBFileInfo file = IDAL.Model.DBFileInfo.Convert(fileInfo);
                        //        file.ParentID = monitoredFolderInfo.ID;
                        //        filesToAdd.Add(file);
                        //    }
                        //}
                        //DBHelper.InsertFiles(filesToAdd);
                    }
                    break;
                case ProcessType.End:
                    {
                        //RemoveUnMonitoredFoldersAndFiles();
                        DBCache.RefreshFiles();
                        DBCache.RefreshFolders();
                    }
                    break;
            }
        }

        private void ExecuteWhenDirectoryScanned(FileScannerProcessEventArgs e)
        {
            //DBFolderInfo monitoredFolderInfo = null; 
            //IList<DBFolderInfo> monitoredFolderInfos;
            //DBFolderInfo existFolder = DBCache.Folders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true));
            //if (null == existFolder)
            //{
            //    monitoredFolderInfo = DBFolderInfo.Convert(e.CurrentDir);
            //    monitoredFolderInfos = new List<DBFolderInfo>() {
            //        monitoredFolderInfo
            //    };
            //    DBHelper.InsertFolders(monitoredFolderInfos);
            //}
            //else
            //{
            //    monitoredFolderInfo = existFolder;
            //    monitoredFolderInfos = new List<DBFolderInfo>() {
            //        monitoredFolderInfo
            //    };
            //    DBHelper.UpdateFolders(monitoredFolderInfos);
            //}

            //monitoredFolderInfo.IsScanned = true;
            //newMonitoredFolderInfos.Add(monitoredFolderInfo);

            DBFolderInfo existFolder = DBCache.Folders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true));
            if (!existFolder.IsNull())
            {
                existFolder.IsScanned = true;
                DBHelper.UpdateFolders(new List<DBFolderInfo>() { existFolder });
            }

            IList<DBFileInfo> filesToAdd = new List<DBFileInfo>();
            IList<DBFileInfo> filesToUpgrade = new List<DBFileInfo>();
            foreach (ScannedFileInfo fileInfo in e.Files)
            {
                if (null == fileInfo || null == fileInfo.File)
                    continue;
                DBFileInfo newfile = DBFileInfo.Convert(fileInfo);
                //newfile.ParentID = monitoredFolderInfo.ID;
                DBFileInfo findFile = DBCache.Files.FirstOrDefault(file => 0 == string.Compare(fileInfo.File.FullName, file.Path));
                if (findFile.IsNull())
                {

                    filesToAdd.Add(newfile);
                }
                else
                {
                    filesToUpgrade.Add(findFile);
                }
            }
            //if (monitoredFolderInfo.ID == -1)
            //{
            //    int i = 0;
            //    ;
            //}
            DBHelper.InsertFiles(filesToAdd);
            DBHelper.UpdateFiles(filesToUpgrade);
        }

        private void ExecuteWhenFileScanned(FileScannerProcessEventArgs e)
        {

        }

        //private void RemoveUnMonitoredFoldersAndFiles()
        //{
        //    IList<DBFolderInfo> foldersToDeleteFromDB = new List<DBFolderInfo>();
        //    foreach (DBFolderInfo folder in DBCache.Folders)
        //    {
        //        bool isNeedToDelete = true;
        //        foreach (DBFolderInfo newFolder in newMonitoredFolderInfos)
        //        {
        //            if (0 == string.Compare(folder.Path, newFolder.Path, true))
        //            {
        //                isNeedToDelete = false;
        //                break;
        //            }
        //        }
        //        if (isNeedToDelete)
        //            foldersToDeleteFromDB.Add(folder);
        //    }
        //    DBHelper.DeleteFolders(foldersToDeleteFromDB);
        //    DBHelper.DeleteFilesUnderFolders(foldersToDeleteFromDB);
        //}

    }
}
