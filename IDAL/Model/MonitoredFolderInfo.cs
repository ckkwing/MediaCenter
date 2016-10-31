using FileExplorer.Model;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL.Model
{
    public class MonitoredFolderInfo : MonitoredFile
    {
        public new static MonitoredFolderInfo Create()
        {
            return new MonitoredFolderInfo();
        }

        public static MonitoredFolderInfo Convert(IFolder iFolder)
        {
            if (null == iFolder)
                return Create();

            return new MonitoredFolderInfo() { Name = iFolder.Name, Path = iFolder.FullPath };
        }

    }
}
