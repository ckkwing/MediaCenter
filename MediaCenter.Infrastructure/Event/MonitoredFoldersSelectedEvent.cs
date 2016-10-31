using FileExplorer.Model;
using IDAL.Model;
using Microsoft.Practices.Prism.PubSubEvents;

namespace MediaCenter.Infrastructure.Event
{
    public class MonitoredFoldersSelectedEvent : PubSubEvent<IFolder>
    {
    }
}
