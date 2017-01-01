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

    public enum InnerType
    {
        NA,
        OneDirectoryScanned,
        OneFileScanned
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

        private InnerType innerType = InnerType.NA;
        public InnerType InnerType
        {
            get
            {
                return innerType;
            }

            set
            {
                innerType = value;
            }
        }

        //public bool IsOneDirScanned { get; set; }

        public DirectoryInfo CurrentDir { get; set; }
        public IList<ScannedFileInfo> Files = new List<ScannedFileInfo>();
        public ScannedFileInfo CurrentFile { get; set; }

        public FileScannerProcessEventArgs(ProcessType processType, InnerType innerType)
        {
            this.ProcessType = processType;
            this.InnerType = innerType;
            //IsOneDirScanned = false;
        }
    }
}
