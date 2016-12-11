using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaCenter.Infrastructure.Core.Model;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public class LoadDBMediasJobRunner : JobRunner
    {
        public LoadDBMediasJobRunner(Job job) : base(job)
        {
        }

        public override bool IsReadyToStart()
        {
            return true;
        }

        protected override bool JobRunning_DoWork()
        {
            LoadMediasJob loadMediaJob = this.Job as LoadMediasJob;
            //DataManager.Instance.DBCache.MonitoredFiles = loadMediaJob.Files = DBHelper.GetFilesUnderFolder(string.Empty);
            switch(loadMediaJob.Pattern.Category)
            {
                case LoadMediasJob.Category.Folder:
                    loadMediaJob.Files = DBHelper.GetFilesUnderFolder(loadMediaJob.Pattern.keyword);
                    break;
                case LoadMediasJob.Category.Tag:
                    loadMediaJob.Files = DBHelper.GetFilesByTags(loadMediaJob.Pattern.keyword);
                    break;
            }
           
            return true;
        }

        protected override void JobRunning_End(DateTime dtLastRunTime)
        {
            base.JobRunning_End(dtLastRunTime);
            DataManager.Instance.DBCache.RefreshMonitoredFolders();
        }
    }
}
