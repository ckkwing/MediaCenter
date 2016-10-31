using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Model
{
    public enum JobPriority
    {
        RealTime,
        Normal
    }

    public enum JobType
    {
        LoadDBMedias
    }

    public abstract class Job : ICloneable, INotifyPropertyChanged
    {
        public string JobID { get; set; }
        
        public string JobName { get; set; }
        public JobType Type { get; set; }

        //Active State
        private bool isActived = false;
        public bool IsActived
        {
            get { return isActived; }
            set
            {
                isActived = value;
                OnPropertyChanged("IsActived");
                //if (null != JobStatus)
                //{
                //    JobStatus.IsActived = isActived;
                //}
            }
        }

        private JobPriority _priority = JobPriority.Normal;
        public JobPriority JobPriority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        public Job()
        {
            this.JobID = Guid.NewGuid().ToString();
            this.JobPriority = JobPriority.Normal;
            this.IsActived = true;
        }

        public Job(JobType type)
            :this()
        {
            this.Type = type;
        }

        //public static Job CreateNewJob(JobType jobType)
        //{
        //    Job job = null;
        //    switch (jobType)
        //    {
        //        case JobType.LoadDBMedias:
        //            job = new LoadMediasJob();
        //            break;
        //    }

        //    if (job != null)
        //    {
        //        job.JobID = Guid.NewGuid().ToString();
        //        //job.CreatedTime = DateTime.Now;
        //        job.Type = jobType;
        //        job.JobPriority = JobPriority.Normal;
        //        job.IsActived = true;
        //        //job.JobStatus = new JobStatusImpl(job);
        //        //job.Error = new JobError() { ErrorCode = ErrorCode.ErrorOK };
        //    }
        //    return job;
        //}

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler Handler = PropertyChanged;
            if (Handler != null) Handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public object Clone()
        {
            throw new NotImplementedException();
        }
    }
}
