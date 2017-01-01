using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Model
{
    public class FileScannerJob : Job
    {
        IList<string> foldersPath = new List<string>();

        public IList<string> FoldersPath
        {
            get
            {
                return foldersPath;
            }

            set
            {
                foldersPath = value;
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
            this.FoldersPath = filesPath;
        }

        public static FileScannerJob Create(IList<string> filesPath)
        {
            return new FileScannerJob(filesPath);
        }
    }
}
