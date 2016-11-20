using MediaCenter.Infrastructure.Core.Model;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Event
{
    public class LoadDataCompletedEvent : PubSubEvent<LoadMediasJob>
    {
    }
}
