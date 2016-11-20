using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Model
{
    public class UpdateTagsJob : Job
    {
        public string TagsToUpdate { get; set; }
        private string folderPath = string.Empty;
        public string FolderPath
        {
            get
            {
                return folderPath;
            }

            set
            {
                folderPath = value;
            }
        }

        private IList<MonitoredFile> files = new List<MonitoredFile>();
        public IList<MonitoredFile> Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
            }
        }

        public UpdateTagsJob()
            :base()
        {
            this.Type = JobType.UpdateTags;
        }

        public UpdateTagsJob(JobType type)
           : base(type)
        {
        }

        public UpdateTagsJob(string folderPath, IList<MonitoredFile> files)
            : this()
        {
            this.FolderPath = folderPath;
            this.Files = files;
        }

        public static UpdateTagsJob Create(string folderPath, IList<MonitoredFile> files)
        {
            return new UpdateTagsJob(folderPath, files);
        }
    }
}
