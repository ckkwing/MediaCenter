using MediaCenter.Infrastructure.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public class JobRunnerBuilder
    {
        public static JobRunner Build(Job job)
        {
            JobRunner jbr = null;
            switch (job.Type)
            {
                case JobType.LoadDBMedias:
                    jbr = new LoadDBMediasJobRunner(job);
                    break;
                case JobType.UpdateTags:
                    jbr = new UpdateTagsJobRunner(job);
                    break;
                case JobType.FileScanner:
                    jbr = new FileScannerJobRunner(job);
                    break;
            }
            return jbr;
        }
    }
}
