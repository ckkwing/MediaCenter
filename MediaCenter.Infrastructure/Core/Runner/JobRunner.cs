using MediaCenter.Infrastructure.Core.Model;
using NLogger;
using System;
using System.Threading;

namespace MediaCenter.Infrastructure.Core.Runner
{
    public abstract class JobRunner
    {
        protected Thread _runnerThread;
        private const string LOG_TAG = "JobRunner";
        protected ILog _logger;
        protected bool _isCancelled;
        protected IJobRunnerStatusCallback _jobRunnerStatusCallback;

        protected Job _job;
        public Job Job
        {
            get { return _job; }
        }

        private bool _isAppExit = false;

        public bool IsAppExit
        {
            get { return _isAppExit; }
            set
            {
                _isAppExit = value;
            }
        }

        public JobRunner(Job job)
        {
            _logger = LogManager.GetLogger(LOG_TAG);
            _job = job;
        }

        public abstract bool IsReadyToStart();

        public virtual bool Init(IJobRunnerStatusCallback jobRunnerStatusCallback)
        {
            _logger.InfoFormat("Init, JobRunner = {0}, Job = {1}", this, this.Job);
            if ( jobRunnerStatusCallback == null)
            {
                return false;
            }

            if (null != _jobRunnerStatusCallback)
            {
                Uninit();
            }
            
            _jobRunnerStatusCallback = jobRunnerStatusCallback;
            if (null != _jobRunnerStatusCallback)
                _jobRunnerStatusCallback.OnJobInit(this);
            return true;
        }

        public virtual void Uninit()
        {
            _logger.InfoFormat("Uninit, JobRunner = {0}, Job = {1}", this, this.Job);
            if (null != _jobRunnerStatusCallback)
                _jobRunnerStatusCallback.OnJobUninit(this);
            _jobRunnerStatusCallback = null;
        }

        public bool Start()
        {
            if (_job == null || _jobRunnerStatusCallback == null)
            {
                _logger.Error("Start _job == null || _jobRunnerStatusCallback == null, Failed");
                return false;
            }

            _logger.InfoFormat("Start, JobRunner = {0}, Job = {1}", this, this.Job);
            _runnerThread = new Thread(JobRunProc);
            _runnerThread.IsBackground = true;
            _runnerThread.Name = "JobRunner Thread + " + _job.JobID;
            _runnerThread.SetApartmentState(ApartmentState.STA);
            _runnerThread.Start();
            return true;
        }

        public virtual bool Stop(bool isStopForOtherJob)
        {
            _logger.Info("JobRunner Stop Enter");
            _isCancelled = true;
            //_isCancelledForOtherJob = isStopForOtherJob;
            //_job.JobStatus.IsCancelling = true;
            _logger.Info("Stop Leave");
            return true;
        }

        private void JobRunProc()
        {
            _logger.ErrorFormat("============ JobRunner Thread Start, Job:{0}============", _job);
            Utilities.Utility.PreventSleep(true);
            DateTime dtLastRunTime = DateTime.MinValue;

            try
            {
                while (true)
                {
                    _logger.ErrorFormat("Step: JobRunning_Init, WorkingSet:{0}", Environment.WorkingSet);
                    if (!JobRunning_Init())
                    {
                        _logger.Error("==Base JobRunning_Init Failed!");
                        break;
                    }
                    if (_isCancelled)
                    {
                        break;
                    }

                    _logger.ErrorFormat("Step: JobRunning_DoWork, WorkingSet:{0}", Environment.WorkingSet);
                    if (!JobRunning_DoWork())
                    {
                        _logger.Error("==Base JobRunning_DoWork Failed!");
                        break;
                    }
                    if (_isCancelled)
                    {
                        break;
                    }

                    //_logger.ErrorFormat("Step: JobRunning_Compare, WorkingSet:{0}", Environment.WorkingSet);
                    //if (!JobRunning_Compare())
                    //{
                    //    _logger.Error("==Base JobRunning_Compare Failed!");
                    //    break;
                    //}
                    if (_isCancelled)
                    {
                        break;
                    }

                    //_logger.ErrorFormat("Step: JobRunning_Transfer, WorkingSet:{0}", Environment.WorkingSet);
                    //_job.JobStatus.IsTransferring = true;
                    //if (!JobRunning_Transfer())
                    //{
                    //    _logger.Error("==Base JobRunning_Transfer Failed!");
                    //    break;
                    //}

                    dtLastRunTime = DateTime.Now;
                    JobRunning_AfterSuccessfulRunning(dtLastRunTime);
                    break;
                };
            }
            catch (Exception ex)
            {
                _logger.Error("**********Meet Exception **********");
                _logger.Error("JobRunProc", ex);
                _logger.Error("**********Meet Exception **********");
            }

            if (IsAppExit)
            {
                _logger.Error("====***App exited, don't store the job data");
                return;
            }
            
            if (dtLastRunTime == DateTime.MinValue)
            {
                dtLastRunTime = DateTime.Now;
            }

            _logger.ErrorFormat("JobRunning_End, WorkingSet:{0}", Environment.WorkingSet);

            JobRunning_End(dtLastRunTime);
            
            _isCancelled = false;

            if (_jobRunnerStatusCallback != null)
            {
                _jobRunnerStatusCallback.OnJobEnd(this);
            }
            Utilities.Utility.PreventSleep(false);
            _logger.Error("============ JobRunner Thread End ============");
        }

        protected virtual bool JobRunning_Init()
        {
            return true;
        }

        protected virtual bool JobRunning_DoWork()
        {
            return true;
        }
        
        protected virtual void JobRunning_AfterSuccessfulRunning(DateTime dtLastRunTime)
        {
        }

        //you can clean your data here
        protected virtual void JobRunning_End(DateTime dtLastRunTime)
        {
        }

    }
}