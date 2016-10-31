using FileExplorer.Model;
using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IMonitorFolder
    {
        bool InsertFolder(string folderPath);
        //int InsertPatchFolders(IList<string> folders);
        int InsertPatchFolders(IList<MonitoredFolderInfo> folders);
        IList<MonitoredFolderInfo> GetMonitoredFolderList();
    }
}
