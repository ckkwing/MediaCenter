using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.FileScan
{
    public class ScannedFileInfo
    {
        public StandardFileExtensions.FileCategory Category { get; set; }
        public FileInfo File { get; set; }
    }
}
