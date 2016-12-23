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
    public interface IScannedFile
    {
        int InsertPatchFiles(IList<MonitoredFile> files);
        IList<MonitoredFile> GetFilesUnderFolder(string folderPath);
        IList<MonitoredFile> GetFilesByTags(string tags);
        void GetFilesUnderFolderAsync(string folderPath, Action<IList<MonitoredFile>> callback,ref bool isCancel);
        bool UpdateFile(MonitoredFile file);
        int UpdateFiles(IList<MonitoredFile> files);
        void DeleteFilesUnderFolders(IList<MonitoredFolderInfo> monitoredFolderInfos);
    }
}
