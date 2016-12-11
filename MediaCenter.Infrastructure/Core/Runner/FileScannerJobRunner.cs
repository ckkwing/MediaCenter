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

            IList<string> existedFileList = DBHelper.GetExistMonitoredFolderStringList();
            IEnumerable<string> newFolders = fileScannerJob.FilesPath.Where(path => !existedFileList.Contains(path));
            //IEnumerable<IFolder> foldersToAdd = selectedFiles.Where(item => newFolders.Contains(item.FullPath));
            //DBHelper.InsertFoldersToMonitor(newFolders);

            fileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = newFolders.ToList() };
            fileScanner.StartSync();
            return true;
        }

        protected override void JobRunning_End(DateTime dtLastRunTime)
        {
            base.JobRunning_End(dtLastRunTime);
            fileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            //DBHelper.InsertFilesToDB(FileScanner.FilesInDirectory);
            DataManager.Instance.DBCache.RefreshMonitoredFiles();
            DataManager.Instance.DBCache.RefreshMonitoredFolders();
        }

        private void FileScanner_ProcessEvent(object sender, FileScannerProcessEventArgs e)
        {
            switch (e.ProcessType)
            {
                case ProcessType.Start:
                    break;
                case ProcessType.InProcess:
                    {
                        if (null == DataManager.Instance.DBCache.MonitoredFolders.FirstOrDefault(folder => 0 == string.Compare(folder.Path, e.CurrentDir.FullName, true)))
                        {
                            IList<MonitoredFolderInfo> monitoredFolderInfo = new List<MonitoredFolderInfo>() {
                                MonitoredFolderInfo.Convert(e.CurrentDir)
                            };
                            DBHelper.InsertFolders(monitoredFolderInfo);
                        }
                        IList<FileInfo> filesToAdd = new List<FileInfo>();
                        foreach(FileInfo fileInfo in e.Files)
                        {
                            if (null == DataManager.Instance.DBCache.MonitoredFiles.FirstOrDefault(file => 0 == string.Compare(fileInfo.FullName, file.Path)))
                                filesToAdd.Add(fileInfo);
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
