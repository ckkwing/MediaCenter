using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaCenter.Infrastructure.Core.Model;
using IDAL.Model;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public class UpdateTagsJobRunner : JobRunner
    {
        private UpdateTagsJob updateTagsJob = null;
        private IList<MonitoredFile> filesToUpdate = new List<MonitoredFile>();
        public UpdateTagsJobRunner(Job job) : base(job)
        {
        }

        public override bool IsReadyToStart()
        {
            return true;
        }

        protected override bool JobRunning_Init()
        {
            updateTagsJob = this.Job as UpdateTagsJob;
            if (null == updateTagsJob)
                return false;
            if (!string.IsNullOrEmpty(updateTagsJob.FolderPath))
                filesToUpdate = DBHelper.GetFilesUnderFolder(updateTagsJob.FolderPath);
            foreach(MonitoredFile monitoredFile in updateTagsJob.Files)
            {
                if (null == monitoredFile)
                    continue;
                filesToUpdate.Add(monitoredFile);
            }
            return true;
        }

        protected override bool JobRunning_DoWork()
        {
            filesToUpdate.ToList().ForEach(file => file.Tag = updateTagsJob.TagsToUpdate);
            int iSuccessfulRow = DBHelper.UpdateFiles(filesToUpdate);
            return true;
        }
    }
}
