using FileExplorer.Model;
using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IDBFolder
    {
        bool InsertFolder(string folderPath);
        //int InsertPatchFolders(IList<string> folders);
        int InsertPatchFolders(IList<DBFolderInfo> folders);
        int UpdateFolders(IList<DBFolderInfo> folders);
        IList<DBFolderInfo> GetMonitoredFolderList();
        void DeletePatchFolders(IList<DBFolderInfo> folders);
    }
}
