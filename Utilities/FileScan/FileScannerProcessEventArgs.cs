using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileScan
{
    public enum ProcessType
    {
        Start,
        InProcess,
        Cancelling,
        End,
    }

    public class FileScannerProcessEventArgs : EventArgs
    {
        private ProcessType processType = ProcessType.Start;
        public ProcessType ProcessType
        {
            get
            {
                return processType;
            }

            private set
            {
                processType = value;
            }
        }

        public DirectoryInfo CurrentDir { get; set; }
        public IList<ScannedFileInfo> Files = new List<ScannedFileInfo>();
        public ScannedFileInfo CurrentFile { get; set; }

        public FileScannerProcessEventArgs(ProcessType processType)
        {
            this.ProcessType = processType;
        }
    }
}
