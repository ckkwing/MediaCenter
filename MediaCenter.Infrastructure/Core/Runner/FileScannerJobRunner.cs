using IDAL.Model;
using MediaCenter.Infrastructure.Core.Model;
using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileScan;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public class FileScannerJobRunner : JobRunner
    {
        private FileScannerJob fileScannerJob = null;
        private FileScanner fileScanner = new FileScanner();
        private IList<MonitoredFolderInfo> newMonitoredFolderInfos = new List<MonitoredFolderInfo>();
        private IList<MonitoredFolderInfo> needToRemovedPaths = new List<MonitoredFolderInfo>();
        private IList<MonitoredFolderInfo> needToAddPaths = new List<MonitoredFolderInfo>();
        private IEventAggregator eventAggregator;
        public FileScannerJobRunner(Job job) : base(job)
        {
        }

        public override bool IsReadyToStart()
        {
            return true;
        }

        protected override bool JobRunning_Init()
        {
            this.eventAggregator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>();
            if (null == eventAggregator)
                return false;
            fileScannerJob = Job as FileScannerJob;
            if (null == fileScannerJob)
                return false;
            DataManager.Instance.FileScanner.Stop();

            //fileScanner.ProcessEvent += FileScanner_ProcessEvent;
            IList<MonitoredFolderInfo> existedFileList = DBHelper.GetExistMonitoredFolderList();
            needToRemovedPaths = existedFileList.Where(folder => !fileScannerJob.FoldersPath.Contains(folder.Path)).ToList();
            foreach (string path in fileScannerJob.FoldersPath)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                bool isExist = false;
                foreach (MonitoredFolderInfo folder in existedFileList)
                {
                    if (0 == string.Compare(path, folder.Path, true))
                    {
                        isExist = true;
                        break;
                    }
                }
                if (!isExist)
                {
                    DirectoryInfo dir = new DirectoryInfo(path);
                    if (null != dir)
                    {
                        MonitoredFolderInfo folder = MonitoredFolderInfo.Convert(dir);
                        needToAddPaths.Add(folder);
                    }
                }
            }
            return base.JobRunning_Init();
        }

        protected override bool JobRunning_DoWork()
        {
            
            if (needToRemovedPaths.Count > 0)
            {
                DBHelper.DeleteFolders(needToRemovedPaths);
                eventAggregator.GetEvent<DBFolderChangedEvent>().Publish(new DBFolderChangedArgs() { Type = DBFolderChangedArgs.ChangedType.Removed, MonitoredFolderList = needToRemovedPaths });
            }
            DBHelper.InsertFolders(needToAddPaths);
            DataManager.Instance.DBCache.RefreshMonitoredFolders();
            return true;
        }

        protected override void JobRunning_End(DateTime dtLastRunTime)
        {
            base.JobRunning_End(dtLastRunTime);

            IList<string> pathsToScan = new List<string>();
            foreach(MonitoredFolderInfo folder in DataManager.Instance.DBCache.MonitoredFolders)
            {
                if (folder.IsScanned)
                    continue;
                pathsToScan.Add(folder.Path);
            }
            DataManager.Instance.FileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = pathsToScan };
            DataManager.Instance.FileScanner.StartAsync();
        }

        private void RemoveUnMonitoredFoldersAndFiles()
        {
            IList<MonitoredFolderInfo> foldersToDeleteFromDB = new List<MonitoredFolderInfo>();
            foreach (MonitoredFolderInfo folder in DataManager.Instance.DBCache.MonitoredFolders)
            {
                bool isNeedToDelete = true;
                foreach(MonitoredFolderInfo newFolder in newMonitoredFolderInfos)
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

        private void FileScanner_ProcessEvent(object sender, FileScannerProcessEventArgs e)
        {
            switch (e.ProcessType)
            {
                case ProcessType.Start:
                    break;
                case ProcessType.InProcess:
                    {
                        //MonitoredFolderInfo monitoredFolderInfo = MonitoredFolderInfo.Convert(e.CurrentDir);
                        //newMonitoredFolderInfos.Add(monitoredFolderInfo);
                        //if (null == DataManager.Instance.DBCache.MonitoredFolders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true)))
                        //{
                        //    IList<MonitoredFolderInfo> monitoredFolderInfos = new List<MonitoredFolderInfo>() {
                        //        monitoredFolderInfo
                        //    };
                        //    DBHelper.InsertFolders(monitoredFolderInfos);
                        //}
                        //IList<MonitoredFile> filesToAdd = new List<MonitoredFile>();
                        //foreach(ScannedFileInfo fileInfo in e.Files)
                        //{
                        //    if (null == fileInfo || null == fileInfo.File)
                        //        continue;
                        //    if (null == DataManager.Instance.DBCache.MonitoredFiles.FirstOrDefault(file => 0 == string.Compare(fileInfo.File.FullName, file.Path)))
                        //    {
                        //        MonitoredFile file = MonitoredFile.Convert(fileInfo);
                        //        file.ParentID = monitoredFolderInfo.ID;
                        //        filesToAdd.Add(file);
                        //    }
                        //}
                        //DBHelper.InsertFiles(filesToAdd);
                    }
                    break;
                case ProcessType.End:
                    break;
            }
        }
    }
}
