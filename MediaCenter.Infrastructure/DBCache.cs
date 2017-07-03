using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extension;

namespace MediaCenter.Infrastructure
{
    public class DBCache
    {
        private IList<DBTagInfo> tagInfos = new List<DBTagInfo>();
        public IList<DBTagInfo> TagInfos
        {
            get
            {
                return tagInfos;
            }

            private set
            {
                tagInfos = value;
            }
        }

        private IList<DBFolderInfo> folders = new List<DBFolderInfo>();
        public IList<DBFolderInfo> Folders
        {
            get
            {
                return folders;
            }

            private set
            {
                folders = value;
            }
        }

        private IList<DBFileInfo> files = new List<DBFileInfo>();
        public IList<DBFileInfo> Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
            }
        }

        private IList<string> folderStrings = new List<string>();
        public IList<string> FolderStrings
        {
            get
            {
                return Folders.Select(item => item.Path).ToList(); ;
            }
        }

        public void Init()
        {
            RefreshTags();
            RefreshFolders();
            RefreshFiles();
        }

        public void Uninit()
        {

        }

        public void RefreshFolders()
        {
            Folders = DBHelper.GetExistFolderList();
        }

        public void RefreshFiles()
        {
            Files = DBHelper.GetFilesUnderFolder(string.Empty);
        }

        public void RefreshTags()
        {
            TagInfos = DBHelper.GetTags();
        }

        public IList<DBTagInfo> GetContainTags(DBFileInfo monitoredFile)
        {
            IList<DBTagInfo> tagInfos = new List<DBTagInfo>();
            if (monitoredFile.IsNull())
                return tagInfos;

            string[] array = monitoredFile.Tag.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                int tagId = -1;
                if (!Int32.TryParse(array[i], out tagId))
                    continue;
                DBTagInfo tagInfo = TagInfos.FirstOrDefault(tag => tag.ID == tagId);
                if (tagInfo.IsNull())
                    continue;
                tagInfos.Add(tagInfo);
            }
            return tagInfos;
        }

    }
}
