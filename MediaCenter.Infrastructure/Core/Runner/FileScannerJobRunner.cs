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
using Utilities.Extension;
using Utilities.FileScan;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public class FileScannerJobRunner : JobRunner
    {
        private FileScannerJob fileScannerJob = null;
        private FileScanner fileScanner = new FileScanner();
        private IList<DBFolderInfo> newMonitoredFolderInfos = new List<DBFolderInfo>();
        private IList<DBFolderInfo> needToRemovedPaths = new List<DBFolderInfo>();
        private IList<DBFolderInfo> needToAddPaths = new List<DBFolderInfo>();
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
            
            IList<DBFolderInfo> existedFileList = DataManager.Instance.DBCache.Folders;
            needToRemovedPaths = existedFileList.Where(folder => !fileScannerJob.FoldersPath.Contains(folder.Path)).ToList();
            foreach (string path in fileScannerJob.FoldersPath)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                bool isExist = false;
                foreach (DBFolderInfo folder in existedFileList)
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
                        DBFolderInfo folder = DBFolderInfo.Convert(dir);
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
            DataManager.Instance.DBCache.RefreshFolders();

            IList<DBFileInfo> filesNeedToBeRemoved = new List<DBFileInfo>();

            foreach (DBFileInfo existFile in DataManager.Instance.DBCache.Files)
            {
                if (existFile.IsNull())
                    continue;
                var list = DataManager.Instance.DBCache.FolderStrings.Where(folder => existFile.Path.IsStartsWith(folder)).ToList();
                if (0 == list.Count)
                    filesNeedToBeRemoved.Add(existFile);
            }
            DBHelper.DeleteFiles(filesNeedToBeRemoved);

            IList<string> pathsToScan = new List<string>();
            foreach (DBFolderInfo folder in DataManager.Instance.DBCache.Folders)
            {
                if (folder.IsScanned)
                    continue;
                pathsToScan.Add(folder.Path);
            }
            DataManager.Instance.FileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = pathsToScan };
            DataManager.Instance.FileScanner.StartAsync();

            return true;
        }

        protected override void JobRunning_End(DateTime dtLastRunTime)
        {
            base.JobRunning_End(dtLastRunTime); 
        }
    }
}
