using FileExplorer.Model;
using IDAL;
using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;

namespace MediaCenter.Infrastructure
{
    public class DBHelper
    {
        private static readonly IDBEntity iDBEntity = DataAccess.CreateDBEntity();
        private static readonly IMonitorFolder iMonitorFolder = DataAccess.CreateMonitorFolder();
        private static readonly IScannedFile iScannedFile = DataAccess.CreateScannedFile();
        private static readonly ITag iTag = DataAccess.CreateTag();

        public static bool InitDBProfile()
        {
            return iDBEntity.InitDBProfile();
        }

        public static int InsertFolders(IList<IFolder> folders)
        {
            IList<MonitoredFolderInfo> convertedFolders = new List<MonitoredFolderInfo>();
            foreach (IFolder iFolder in folders)
            {
                MonitoredFolderInfo newFolder = MonitoredFolderInfo.Convert(iFolder);
                if (newFolder.IsNull())
                    continue;
                convertedFolders.Add(newFolder);
            }
            return iMonitorFolder.InsertPatchFolders(convertedFolders);
        }

        public static int InsertFolders(IList<MonitoredFolderInfo> folders)
        {
            return iMonitorFolder.InsertPatchFolders(folders);
        }

        public static int UpdateFolders(IList<MonitoredFolderInfo> folders)
        {
            return iMonitorFolder.UpdateFolders(folders);
        }

        public static void DeleteFolders(IList<MonitoredFolderInfo> folders)
        {
            iMonitorFolder.DeletePatchFolders(folders);
        }

        public static IList<MonitoredFolderInfo> GetExistMonitoredFolderList()
        {
            return iMonitorFolder.GetMonitoredFolderList();
        }

        public static IList<string> GetExistMonitoredFolderStringList()
        {
            return iMonitorFolder.GetMonitoredFolderList().Select(item => item.Path).ToList();
        }

        public static int InsertFiles(IList<MonitoredFile> files)
        {
            return iScannedFile.InsertPatchFiles(files);
        }

        public static IList<MonitoredFile> GetFilesUnderFolder(string folderPath)
        {
            return iScannedFile.GetFilesUnderFolder(folderPath);
        }

        public static void GetFilesUnderFolderAsync(string folderPath, Action<IList<MonitoredFile>> callback,ref bool isCancel)
        {
            iScannedFile.GetFilesUnderFolderAsync(folderPath, callback,ref isCancel);
        }

        public static IList<MonitoredFile> GetFilesByTags(string tags)
        {
            return iScannedFile.GetFilesByTags(tags);
        }

        public static bool UpdateFile(MonitoredFile file)
        {
            return iScannedFile.UpdateFile(file);
        }

        public static int UpdateFiles(IList<MonitoredFile> files)
        {
            return iScannedFile.UpdateFiles(files);
        }

        public static void DeleteFilesUnderFolders(IList<MonitoredFolderInfo> folders)
        {
            iScannedFile.DeleteFilesUnderFolders(folders);
        }

        public static IList<TagInfo> GetTags()
        {
            return iTag.GetTags();
        }

        public static bool InsertTag(TagInfo tagInfo)
        {
            return iTag.InsertTag(tagInfo);
        }

        public static int InsertTags(IList<TagInfo> tagInfos)
        {
            return iTag.InsertPatchTags(tagInfos);
        }
    }
}
