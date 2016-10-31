using MediaCenter.Infrastructure.Core.Model;
using NLogger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core
{
    public class Core
    {
        public static Core Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly Core instance = new Core();
        }

        private JobScheduler _scheduler;

        Core()
        {

        }

        private const string LOG_TAG = "MeidaCenter.Core";
        protected ILog _logger;
        public event JobRunningStateChangedEventHandler JobRunningStateChanged;

        public void Init()
        {
            _logger = LogManager.GetLogger(LOG_TAG);
            _scheduler = new JobScheduler();
            _scheduler.JobRunningStateChanged += OnJobRunningStateChanged;
            _scheduler.Init();
        }

        public void Uninit()
        {
            _scheduler.JobRunningStateChanged -= OnJobRunningStateChanged;
            _scheduler.Destory();
        }

        public bool ForceStart(Job job)
        {
            _logger.DebugFormat("ForceStart {0}", job);
            return _scheduler.StartJob(job);
        }

        //Manully stop a Job, //backup, restore or sync
        public bool ForceStop(Job job)
        {
            _logger.DebugFormat("ForceStop {0}", job);
            return _scheduler.StopJob(job);
        }

        public bool AddJob(Job job)
        {
            _logger.DebugFormat("AddJob {0}", job);
            return _scheduler.AddJob(job);
        }

        public bool UpdateJob(Job job)
        {
            _logger.DebugFormat("UpdateJob {0}", job);
            return _scheduler.UpdateJob(job);
        }

        //user delete a Job from UI
        public bool RemoveJob(Job job)
        {
            _logger.DebugFormat("RemoveJob {0}", job);
            return _scheduler.RemoveJob(job);
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
