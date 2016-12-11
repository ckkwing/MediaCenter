using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Model
{
    public class FileScannerJob : Job
    {
        IList<string> filesPath = new List<string>();

        public IList<string> FilesPath
        {
            get
            {
                return filesPath;
            }

            set
            {
                filesPath = value;
            }
        }

        public FileScannerJob()
            :base()
        {
            this.Type = JobType.FileScanner;
        }

        public FileScannerJob(JobType type)
           : base(type)
        {
        }

        public FileScannerJob(IList<string> filesPath)
            :this()
        {
            this.FilesPath = filesPath;
        }

        public static FileScannerJob Create(IList<string> filesPath)
        {
            return new FileScannerJob(filesPath);
        }
    }
}
