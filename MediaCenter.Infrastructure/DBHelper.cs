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
        private static readonly IDBFolder iMonitorFolder = DataAccess.CreateMonitorFolder();
        private static readonly IDBFile iScannedFile = DataAccess.CreateScannedFile();
        private static readonly IDBTag iTag = DataAccess.CreateTag();

        public static bool InitDBProfile()
        {
            return iDBEntity.InitDBProfile();
        }

        public static int InsertFolders(IList<IFolder> folders)
        {
            IList<DBFolderInfo> convertedFolders = new List<DBFolderInfo>();
            foreach (IFolder iFolder in folders)
            {
                DBFolderInfo newFolder = DBFolderInfo.Convert(iFolder);
                if (newFolder.IsNull())
                    continue;
                convertedFolders.Add(newFolder);
            }
            return iMonitorFolder.InsertPatchFolders(convertedFolders);
        }

        public static int InsertFolders(IList<DBFolderInfo> folders)
        {
            return iMonitorFolder.InsertPatchFolders(folders);
        }

        public static int UpdateFolders(IList<DBFolderInfo> folders)
        {
            return iMonitorFolder.UpdateFolders(folders);
        }

        public static void DeleteFolders(IList<DBFolderInfo> folders)
        {
            iMonitorFolder.DeletePatchFolders(folders);
        }

        public static IList<DBFolderInfo> GetExistFolderList()
        {
            return iMonitorFolder.GetMonitoredFolderList();
        }

        public static IList<string> GetExistFolderStringList()
        {
            return iMonitorFolder.GetMonitoredFolderList().Select(item => item.Path).ToList();
        }

        public static int InsertFiles(IList<IDAL.Model.DBFileInfo> files)
        {
            return iScannedFile.InsertPatchFiles(files);
        }

        public static IList<IDAL.Model.DBFileInfo> GetFilesUnderFolder(string folderPath)
        {
            return iScannedFile.GetFilesUnderFolder(folderPath);
        }

        public static void GetFilesUnderFolderAsync(string folderPath, Action<IList<IDAL.Model.DBFileInfo>> callback,ref bool isCancel)
        {
            iScannedFile.GetFilesUnderFolderAsync(folderPath, callback,ref isCancel);
        }

        public static IList<IDAL.Model.DBFileInfo> GetFilesByTags(string tags)
        {
            return iScannedFile.GetFilesByTags(tags);
        }

        public static bool UpdateFile(IDAL.Model.DBFileInfo file)
        {
            return iScannedFile.UpdateFile(file);
        }

        public static int UpdateFiles(IList<IDAL.Model.DBFileInfo> files)
        {
            return iScannedFile.UpdateFiles(files);
        }

        public static void DeleteFiles(IList<DBFileInfo> files)
        {
            iScannedFile.DeleteFiles(files);
        }

        public static void DeleteFilesUnderFolders(IList<DBFolderInfo> folders)
        {
            iScannedFile.DeleteFilesUnderFolders(folders);
        }

        public static IList<DBTagInfo> GetTags()
        {
            return iTag.GetTags();
        }

        public static bool InsertTag(DBTagInfo tagInfo)
        {
            return iTag.InsertTag(tagInfo);
        }

        public static int InsertTags(IList<DBTagInfo> tagInfos)
        {
            return iTag.InsertPatchTags(tagInfos);
        }
    }
}
