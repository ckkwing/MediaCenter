using FileExplorer.Model;
using IDAL.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IDBFile
    {
        int InsertPatchFiles(IList<Model.DBFileInfo> files);
        IList<Model.DBFileInfo> GetFilesUnderFolder(string folderPath);
        IList<Model.DBFileInfo> GetFilesByTags(string tags);
        void GetFilesUnderFolderAsync(string folderPath, Action<IList<Model.DBFileInfo>> callback,ref bool isCancel);
        bool UpdateFile(Model.DBFileInfo file);
        int UpdateFiles(IList<Model.DBFileInfo> files);
        void DeleteFilesUnderFolders(IList<DBFolderInfo> monitoredFolderInfos);
        void DeleteFiles(IList<DBFileInfo> files);
    }
}
