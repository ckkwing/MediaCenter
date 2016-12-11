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
        private IList<TagInfo> tagInfos = new List<TagInfo>();
        public IList<TagInfo> TagInfos
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

        private IList<MonitoredFolderInfo> monitoredFolders = new List<MonitoredFolderInfo>();
        public IList<MonitoredFolderInfo> MonitoredFolders
        {
            get
            {
                return monitoredFolders;
            }

            private set
            {
                monitoredFolders = value;
            }
        }

        private IList<MonitoredFile> monitoredFiles = new List<MonitoredFile>();
        public IList<MonitoredFile> MonitoredFiles
        {
            get
            {
                return monitoredFiles;
            }

            set
            {
                monitoredFiles = value;
            }
        }

        private IList<string> monitoredFolderStrings = new List<string>();
        public IList<string> MonitoredFolderStrings
        {
            get
            {
                return MonitoredFolders.Select(item => item.Path).ToList(); ;
            }
        }

        public void Init()
        {
            RefreshTagInfos();
            RefreshMonitoredFolders();
            RefreshMonitoredFiles();
        }

        public void Uninit()
        {

        }

        public void RefreshMonitoredFolders()
        {
            MonitoredFolders = DBHelper.GetExistMonitoredFolderList();
        }

        public void RefreshMonitoredFiles()
        {
            MonitoredFiles = DBHelper.GetFilesUnderFolder(string.Empty);
        }

        public void RefreshTagInfos()
        {
            TagInfos = DBHelper.GetTags();
        }

        public IList<TagInfo> GetContainTags(MonitoredFile monitoredFile)
        {
            IList<TagInfo> tagInfos = new List<TagInfo>();
            if (monitoredFile.IsNull())
                return tagInfos;

            string[] array = monitoredFile.Tag.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                int tagId = -1;
                if (!Int32.TryParse(array[i], out tagId))
                    continue;
                TagInfo tagInfo = TagInfos.FirstOrDefault(tag => tag.ID == tagId);
                if (tagInfo.IsNull())
                    continue;
                tagInfos.Add(tagInfo);
            }
            return tagInfos;
        }

    }
}
