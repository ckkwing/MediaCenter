using IDAL.Model;
using MediaCenter.Infrastructure.Core.Model;
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
        public FileScannerJobRunner(Job job) : base(job)
        {
        }

        public override bool IsReadyToStart()
        {
            return true;
        }

        protected override bool JobRunning_Init()
        {
            fileScanner.ProcessEvent += FileScanner_ProcessEvent;
            return base.JobRunning_Init();
        }

        protected override bool JobRunning_DoWork()
        {
            fileScannerJob = this.Job as FileScannerJob;

            //IList<string> existedFileList = DBHelper.GetExistMonitoredFolderStringList();
            //IEnumerable<string> newFolders = fileScannerJob.FilesPath.Where(path => !existedFileList.Contains(path));
            //fileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = newFolders.ToList() };

            fileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = fileScannerJob.FilesPath };
            fileScanner.StartSync();
            return true;
        }

        protected override void JobRunning_End(DateTime dtLastRunTime)
        {
            base.JobRunning_End(dtLastRunTime);
            fileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            //DBHelper.InsertFilesToDB(FileScanner.FilesInDirectory);
            RemoveUnMonitoredFoldersAndFiles();
            DataManager.Instance.DBCache.RefreshMonitoredFiles();
            DataManager.Instance.DBCache.RefreshMonitoredFolders();
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
                        MonitoredFolderInfo monitoredFolderInfo = MonitoredFolderInfo.Convert(e.CurrentDir);
                        newMonitoredFolderInfos.Add(monitoredFolderInfo);
                        if (null == DataManager.Instance.DBCache.MonitoredFolders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true)))
                        {
                            IList<MonitoredFolderInfo> monitoredFolderInfos = new List<MonitoredFolderInfo>() {
                                monitoredFolderInfo
                            };
                            DBHelper.InsertFolders(monitoredFolderInfos);
                        }
                        IList<MonitoredFile> filesToAdd = new List<MonitoredFile>();
                        foreach(ScannedFileInfo fileInfo in e.Files)
                        {
                            if (null == fileInfo || null == fileInfo.File)
                                continue;
                            if (null == DataManager.Instance.DBCache.MonitoredFiles.FirstOrDefault(file => 0 == string.Compare(fileInfo.File.FullName, file.Path)))
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
                    break;
            }
        }
    }
}
