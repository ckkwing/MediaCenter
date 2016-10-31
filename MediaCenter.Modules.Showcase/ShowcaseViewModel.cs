using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using IDAL.Model;
using System.Collections.ObjectModel;
using System.IO;
using MediaCenter.Infrastructure;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using System.Diagnostics;
using System.ComponentModel;
using Utilities.Extension;
using System.Threading;
using FileExplorer.Model;
using MediaCenter.Infrastructure.Core.Model;
using MediaCenter.Infrastructure.Core;

namespace MediaCenter.Modules.Showcase
{
    [Export(typeof(ShowcaseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShowcaseViewModel : ViewModelBase
    {
        private IEventAggregator eventAggregator;

        private ObservableCollection<MonitoredFile> files = new ObservableCollection<MonitoredFile>();
        public ObservableCollection<MonitoredFile> Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
                OnPropertyChanged("Files");
            }
        }

        private Job currentJob = null;

        #region Command
        public ICommand PlayCommand { get; private set; }
        public ICommand OpenLocationCommand { get; private set; }
        public ICommand SetTagCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public ShowcaseViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Subscribe(this.MonitoredFoldersSelected, ThreadOption.UIThread);
            this.eventAggregator.GetEvent<TagChangedEvent>().Subscribe(this.OnTagSelected, ThreadOption.UIThread);
            JobManager.Instance.JobRunningStateChanged += Instance_JobRunningStateChanged;
            PlayCommand = new DelegateCommand<MonitoredFile>(OnPlay, CanExcute);
            OpenLocationCommand = new DelegateCommand<MonitoredFile>(OnOpenLocation, CanExcute);
            SetTagCommand = new DelegateCommand<MonitoredFile>(OnSetTag, CanExcute);
        }

        ~ShowcaseViewModel()
        {
            this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Unsubscribe(this.MonitoredFoldersSelected);
            this.eventAggregator.GetEvent<TagChangedEvent>().Unsubscribe(this.OnTagSelected);
            JobManager.Instance.JobRunningStateChanged -= Instance_JobRunningStateChanged;
        }
        
        private void OnSetTag(MonitoredFile monitoredFile)
        {
            if (null == monitoredFile)
                return;
            this.eventAggregator.GetEvent<SettingsEvent>().Publish(new SettingEventArgs(SettingType.SetTags, monitoredFile));
        }

        private void OnOpenLocation(MonitoredFile monitoredFile)
        {
            if (null == monitoredFile)
                return;
            FileInfo file = new FileInfo(monitoredFile.Path);
            if (file.IsNull())
                return;
            Process.Start("explorer.exe", file.DirectoryName);
        }

        private void OnPlay(MonitoredFile monitoredFile)
        {
            if (null == monitoredFile)
                return;
            Process.Start(monitoredFile.Path);
        }

        private void MonitoredFoldersSelected(IFolder iFolder)
        {
            Files.Clear();
            //StartBackgroundWork(iFolder);

            if (!currentJob.IsNull())
            {
                JobManager.Instance.ForceStop(currentJob);
                currentJob = null;
            }
            currentJob = LoadMediasJob.Create(new LoadMediasJob.LoadPattern() { Category = LoadMediasJob.Category.Folder, keyword = string.Empty });
            JobManager.Instance.AddJob(currentJob);
            JobManager.Instance.ForceStart(currentJob);
        }

        private void OnTagSelected(TagInfo tag)
        {
            Files.Clear();
            if (!currentJob.IsNull())
            {
                JobManager.Instance.ForceStop(currentJob);
                currentJob = null;
            }
            currentJob = LoadMediasJob.Create(new LoadMediasJob.LoadPattern() { Category = LoadMediasJob.Category.Tag, keyword = tag.Name });
            JobManager.Instance.AddJob(currentJob);
            JobManager.Instance.ForceStart(currentJob);
        }

        private void Instance_JobRunningStateChanged(object sender, Job job)
        {
            if (sender.IsNull() || job.IsNull() || currentJob.IsNull())
                return;
            if (0 != string.Compare(job.JobID, currentJob.JobID))
                return;
            switch (job.Type)
            {
                case JobType.LoadDBMedias:
                    {
                        LoadMediasJob loadMediasJob = job as LoadMediasJob;
                        RunOnUIThread(() => {
                            foreach (MonitoredFile file in loadMediasJob.Files)
                            {
                                Files.Add(file);
                            }
                        });
                        
                    }
                    break;
            }
        }
        
    }
}
