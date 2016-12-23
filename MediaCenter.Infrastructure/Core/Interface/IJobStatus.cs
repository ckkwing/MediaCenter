using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Interface
{
    public enum JobState
    {
        Pending,//pending for some conditions, such as network, Update to Date
        Running
    }
    public interface IJobStatus : INotifyPropertyChanged
    {
        JobState RunningState { get; set; }
    }
}
