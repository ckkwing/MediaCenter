using MediaCenter.Infrastructure.Core.Model;
using MediaCenter.Infrastructure.Core.Runner;
using NLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core
{
    public delegate void JobRunningStateChangedEventHandler(object sender, Job job);
    public class JobScheduler : IJobRunnerStatusCallback
    {
        private const string LOG_TAG = "JobScheduler";
        private IList<Job> _jobList = new List<Job>();
        private IList<JobRunner> _jobRunnerList = new List<JobRunner>();
        protected bool _isExit = false;
        protected ManualResetEvent _rescheduleEvent = new ManualResetEvent(false);
        protected Thread _schedulerThread = null;
        protected ILog _logger;
        protected JobRunner _curRunningJobRunner;
        //protected EventDispatcher _eventDispatcher;
        public event JobRunningStateChangedEventHandler JobRunningStateChanged;

        private bool _isAppExit = false;
        public bool IsAppExit
        {
            get { return _isAppExit; }
            set
            {
                _isAppExit = value;
                lock (_jobRunnerList)
                {
                    if (_curRunningJobRunner != null)
                    {
                        _curRunningJobRunner.IsAppExit = true;
                    }
                }
            }
        }

        public IList<JobRunner> JobRunnerList
        {
            get { return _jobRunnerList; }
        }

        public JobScheduler()
        {
            _logger = LogManager.GetLogger(LOG_TAG);
        }

        public bool Init()
        {
            _logger.Debug("Init");
            lock (_jobRunnerList)
            {
                _isExit = false;
                _rescheduleEvent.Reset();
                //_eventDispatcher = new EventDispatcher();
                //_eventDispatcher.DispatchFinished += OnFinishDispatchEvent;
                //_eventDispatcher.ShowDialog += OnShowDialog;
                _schedulerThread = new Thread(DispatchProc);
                _schedulerThread.IsBackground = true;
                _schedulerThread.Name = "schedulerThread Dispatch Task Thread";
                _schedulerThread.SetApartmentState(ApartmentState.STA);
                _schedulerThread.Start();
            }
            return true;
        }

        private void DispatchProc()
        {
            _logger.Debug("Schedule Dispatch Proc Start");

            while (!_isExit && !IsAppExit)
            {
                lock (_jobRunnerList)
                {
                    ScheduleJob();
                    _rescheduleEvent.Reset();
                }
                _rescheduleEvent.WaitOne();
            }
            _logger.Debug("Scheduler DispatchTask Thread Leave");
        }

        private void ScheduleJob()
        {
            _logger.Debug("---------Schedule One Job----------");
            if (_curRunningJobRunner != null)
            {
                _logger.Debug("---------_curRunningJobRunner is running, exit----------");
                return;
            }

            JobRunner readyToStartJob = null;
            foreach (JobRunner jbr in _jobRunnerList)
            {
                if (jbr.Job.JobPriority == JobPriority.RealTime)
                {
                    readyToStartJob = jbr;
                    _logger.Debug("---------Find JobPriority.RealTime Job----------");
                    break;
                }
            }

            if (readyToStartJob == null)
            {
                IList<JobRunner> readyJobRunnerList = new List<JobRunner>();
                foreach (JobRunner jbr in _jobRunnerList)
                {
                    if (jbr.IsReadyToStart())
                    {
                        _logger.DebugFormat("JobRunner IsReadyToStart {0}", jbr.Job);
                        readyJobRunnerList.Add(jbr);
                    }
                }

                foreach (JobRunner jbr in readyJobRunnerList)
                {
                    if (readyToStartJob == null)
                    {
                        readyToStartJob = jbr;
                        break;
                    }
                }
            }

            if (readyToStartJob != null)
            {
                _curRunningJobRunner = readyToStartJob;
                if (!readyToStartJob.Start())
                {
                    _curRunningJobRunner = null;
                }
                foreach (JobRunner jbr in _jobRunnerList)
                {
                    jbr.Job.JobPriority = JobPriority.Normal;
                }
                _logger.DebugFormat("---------Start Run Job {0}:----------", readyToStartJob);
            }
            else
            {
                _logger.Debug("---------No ready to Run job, Exit----------");
            }
        }

        private void UnInitialize()
        {
            _logger.Debug("UnInitialize Enter");
            //if (_eventDispatcher != null)
            //{
            //    _eventDispatcher.DispatchFinished -= OnFinishDispatchEvent;
            //}

            try
            {
                if (_schedulerThread != null)
                {
                    _isExit = true;
                    _rescheduleEvent.Set();
                    _schedulerThread.Join(5000);
                    if (_schedulerThread.IsAlive)
                    {
                        try
                        {
                            _schedulerThread.Abort();
                        }
                        catch (Exception ex2)
                        {
                            _logger.Error("taskThread.Abort();", ex2);
                        }
                    }
                    _schedulerThread = null;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("UnInitialize", ex);
            }
            _logger.Debug("UnInitialize Leave");
        }

        public void Destory()
        {
            _logger.Debug("Destory Enter");
            if (_schedulerThread != null)
            {
                UnInitialize();
            }
            _logger.Debug("Destory Leave");
        }

        private void OnFinishDispatchEvent(object obj, EventArgs args)
        {
            _logger.DebugFormat("OnFinishDispatchEvent, send={0}", obj);
            _rescheduleEvent.Set();
        }

        public bool StartJob(Job job)
        {
            _logger.DebugFormat("StartJob, {0}", job);
            lock (_jobRunnerList)
            {
                bool result = true;
                JobRunner jbr = GetJobRunner(job);
                if (jbr != null)
                {
                    if (_curRunningJobRunner != null && _curRunningJobRunner != jbr)
                    {
                        _curRunningJobRunner.Stop(true);
                    }

                    if (_curRunningJobRunner != null && _curRunningJobRunner == jbr)
                    {
                        _logger.DebugFormat("Current job is running, ignorent this real time flag, {0}", job);
                    }
                    else
                    {
                        jbr.Job.JobPriority = JobPriority.RealTime;
                    }
                }
                else
                {
                    result = false;
                }
                _rescheduleEvent.Set();
                return result;
            }
        }

        public bool StopJob(Job job)
        {
            _logger.DebugFormat("StopJob, {0}", job);
            lock (_jobRunnerList)
            {
                bool result = true;
                JobRunner jbr = GetJobRunner(job);
                if (jbr != null)
                {
                    if (_curRunningJobRunner != null && _curRunningJobRunner == jbr)
                    {
                        jbr.Stop(false);
                    }
                }
                else
                {
                    result = false;
                }
                _rescheduleEvent.Set();
                return result;
            }
        }

        public bool AddJob(Job job)
        {
            _logger.DebugFormat("AddJob, {0}", job);
            lock (_jobRunnerList)
            {
                bool result = false;
                JobRunner jbr = JobRunnerBuilder.Build(job);
                if (jbr != null)
                {
                    bool retInit = true;
                    if (job.IsActived)
                    {
                        retInit = jbr.Init(this);
                    }

                    if (retInit)
                    {
                        _jobList.Add(job);
                        _jobRunnerList.Add(jbr);
                        _rescheduleEvent.Set();
                        result = true;
                    }
                    else
                    {
                        _logger.Debug("Job Init Failed");
                        result = false;
                    }
                }
                else
                {
                    _logger.DebugFormat("AddJob Failed, Job Runnder == null, {0}", job);
                }

                return result;
            }
        }

        public bool UpdateJob(Job job)
        {
            _logger.DebugFormat("UpdateJob, {0}", job);
            lock (_jobRunnerList)
            {
                bool result = true;
                JobRunner jbr = GetJobRunner(job);
                if (jbr != null)
                {
                    if (job.IsActived)
                    {
                        jbr.Uninit();
                        result = jbr.Init(this);
                    }
                    else
                    {
                        result = true;
                    }
                }
                else
                {
                    result = false;
                }
                _rescheduleEvent.Set();
                return result;
            }
        }

        public bool RemoveJob(Job job)
        {
            _logger.DebugFormat("RemoveJob, {0}", job);
            lock (_jobRunnerList)
            {
                bool result = true;
                JobRunner jbr = GetJobRunner(job);
                if (jbr != null)
                {
                    _jobRunnerList.Remove(jbr);
                    if (_jobList.Contains(job))
                    {
                        _jobList.Remove(job);
                    }

                    if (_curRunningJobRunner != null && _curRunningJobRunner == jbr)
                    {
                        jbr.Stop(false);
                    }

                    jbr.Uninit();

                    if (_curRunningJobRunner != null && _curRunningJobRunner == jbr)
                    {
                        _curRunningJobRunner = null;
                    }
                }
                else
                {
                    result = false;
                }
                _rescheduleEvent.Set();
                return result;
            }
        }

        private JobRunner GetJobRunner(Job job)
        {
            JobRunner findJobrunner = null;
            foreach (JobRunner jbr in _jobRunnerList)
            {
                if (jbr.Job == job)
                {
                    findJobrunner = jbr;
                    break;
                }
            }

            if (findJobrunner == null)
            {
                _logger.ErrorFormat("GetJobRunner Failed:{0}", job);
            }

            return findJobrunner;
        }

        public void OnJobStart(JobRunner jbr)
        {
            if (jbr == null)
            {
                return;
            }
            _logger.DebugFormat("OnJobStart callback, JobRunner={0}", jbr);

            if (JobRunningStateChanged != null)
            {
                JobRunningStateChanged(jbr, jbr.Job);
            }
        }

        public void OnJobEnd(JobRunner jbr)
        {
            if (jbr == null)
            {
                return;
            }

            _logger.Debug("OnJobEnd callback Enter");
            lock (_jobRunnerList)
            {
                _curRunningJobRunner = null;

                if (_jobList.Contains(jbr.Job))
                {
                    _jobList.Remove(jbr.Job);
                }

                if (_jobRunnerList.Contains(jbr))
                {
                    _jobRunnerList.Remove(jbr);
                }
                _rescheduleEvent.Set();
            }

            if (JobRunningStateChanged != null)
            {
                JobRunningStateChanged(this, jbr.Job);
            }
            
            _logger.Debug("OnJobEnd callback Leave");
        }

        public void OnJobInit(JobRunner jbr)
        {
            _logger.Debug("OnJobInit callback Enter");
            _logger.Debug("OnJobInit callback Leave");
        }

        public void OnJobUninit(JobRunner jbr)
        {
            _logger.Debug("OnJobInit callback Enter");
            _logger.Debug("OnJobInit callback Leave");
        }
    }
}
