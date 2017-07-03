using IDAL.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Event
{
    public class DBFolderChangedArgs : EventArgs
    {
        public enum ChangedType
        {
            Removed,
            Added,
            Modified
        }

        public ChangedType Type { get; set; }

        public IList<DBFolderInfo> MonitoredFolderList { get; set; }
    }

    public class DBFolderChangedEvent : PubSubEvent<DBFolderChangedArgs>
    {
    }
}
