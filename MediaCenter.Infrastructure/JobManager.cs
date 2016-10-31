using MediaCenter.Infrastructure.Core.Error;
using MediaCenter.Infrastructure.Core.Model;
using System;
using System.Collections.Generic;
using MediaCenter.Infrastructure.Core;

namespace MediaCenter.Infrastructure
{
    public sealed class JobManager
    {
        public static JobManager Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly JobManager instance = new JobManager();
        }

        public event JobRunningStateChangedEventHandler JobRunningStateChanged;
        private IList<Job> _jobList = new List<Job>();
        private Job _defaultJob;
        public Job DefaultJob
        {
            get { return _defaultJob; }
            set
            {
                _defaultJob = value;
            }
        }

        JobManager()
        {

        }

        public bool Init()
        {
            Core.Core.Instance.JobRunningStateChanged += OnJobRunningStateChanged;
            //Job job = Job.CreateNewJob(JobType.LoadDBMedias);
            //AddJob(job);
            //ForceStart(job);
            return true;
        }

        public void Uninit()
        {
            Core.Core.Instance.JobRunningStateChanged -= OnJobRunningStateChanged;
        }

        public ErrorCode ForceStart(Job job)
        {
            Core.Core.Instance.ForceStart(job);
            return ErrorCode.ErrorOK;
        }

        public ErrorCode ForceStop(Job job)
        {
            Core.Core.Instance.ForceStop(job);
            return ErrorCode.ErrorOK;
        }

        public ErrorCode AddJob(Job job)
        {
            _jobList.Add(job);
            if (_defaultJob == null && _jobList.Count > 0)
            {
                DefaultJob = _jobList[0];
            }
            Core.Core.Instance.AddJob(job);

            return ErrorCode.ErrorOK;
        }

        public ErrorCode DeleteJob(Job job)
        {
            Job findJob = null;
            foreach (Job existedJob in _jobList)
            {
                if (string.Compare(existedJob.JobID, job.JobID, true) == 0)
                {
                    findJob = job;
                    break;
                }
            }

            if (findJob != null)
            {
                _jobList.Remove(findJob);
                Core.Core.Instance.RemoveJob(findJob);
                if (DefaultJob == job)
                {
                    if (_jobList.Count > 0)
                    {
                        DefaultJob = _jobList[0];
                    }
                    else
                    {
                        DefaultJob = null;
                    }
                }
            }
            return ErrorCode.ErrorOK;
        }


        private void OnJobRunningStateChanged(object sender, Job job)
        {
            if (JobRunningStateChanged != null)
            {
                JobRunningStateChanged(sender, job);
            }
        }
    }
}
