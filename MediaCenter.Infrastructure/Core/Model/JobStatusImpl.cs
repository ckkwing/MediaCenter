using MediaCenter.Infrastructure.Core.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MediaCenter.Infrastructure.Core.Model
{
    public partial class JobStatusImpl : IJobStatus
    {
        private JobState runningState = JobState.Pending;
        public JobState RunningState
        {
            get
            {
                return runningState;
            }

            set
            {
                runningState = value;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
