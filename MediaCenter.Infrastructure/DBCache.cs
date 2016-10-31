using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void Init()
        {
            RefreshMonitoredFolders();
        }

        public void Uninit()
        {

        }

        public void RefreshMonitoredFolders()
        {
            MonitoredFolders = DBHelper.GetExistMonitoredFolderList();
        }

        public void RefreshTagInfos()
        {
            TagInfos = DBHelper.GetTags();
        }
    }
}
