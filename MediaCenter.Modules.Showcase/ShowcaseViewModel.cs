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
using Utilities.FileScan;
using System.Windows.Data;
using static MediaCenter.Infrastructure.Model.MediaInfo;

namespace MediaCenter.Modules.Showcase
{
    [Export(typeof(ShowcaseViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ShowcaseViewModel : ViewModelBase
    {
        private IEventAggregator eventAggregator;
        private int iMediaTypes = (int)MediaType.Document | (int)MediaType.Image | (int)MediaType.Music | (int)MediaType.Video;
        private List<string> defaultExtension = new List<string>();

        private IList<MonitoredFile> files = new List<MonitoredFile>();
        public IList<MonitoredFile> Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
                //OnPropertyChanged("Files");
            }
        }

        private ICollectionView contentView;
        public ICollectionView ContentView
        {
            get
            {
                if (contentView == null)
                {
                    contentView = CollectionViewSource.GetDefaultView(Files);
                    //contentView.CurrentChanged += JobView_CurrentChanged;
                }
                return contentView;
            }
        }

        private string currentFilePath = string.Empty;
        public string CurrentFilePath
        {
            get
            {
                return currentFilePath;
            }

            set
            {
                currentFilePath = value;
                OnPropertyChanged("CurrentFilePath");
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
            this.eventAggregator.GetEvent<MediaFilterChangedEvent>().Subscribe(this.OnFilterChanged, ThreadOption.UIThread);
            JobManager.Instance.JobRunningStateChanged += Instance_JobRunningStateChanged;
            DataManager.Instance.FileScanner.ProcessEvent += FileScanner_ProcessEvent;
            PlayCommand = new DelegateCommand<MonitoredFile>(OnPlay, CanExcute);
            OpenLocationCommand = new DelegateCommand<MonitoredFile>(OnOpenLocation, CanExcute);
            SetTagCommand = new DelegateCommand<MonitoredFile>(OnSetTag, CanExcute);
            ChangeFilterRule();
        }

        ~ShowcaseViewModel()
        {
            DataManager.Instance.FileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Unsubscribe(this.MonitoredFoldersSelected);
            this.eventAggregator.GetEvent<TagChangedEvent>().Unsubscribe(this.OnTagSelected);
            this.eventAggregator.GetEvent<MediaFilterChangedEvent>().Unsubscribe(this.OnFilterChanged);
            JobManager.Instance.JobRunningStateChanged -= Instance_JobRunningStateChanged;
        }

        private void OnFilterChanged(int iFilter)
        {
            this.iMediaTypes = iFilter;
            ChangeFilterRule();
        }

        private void ChangeFilterRule()
        {
            defaultExtension.Clear();
            if ((int)MediaType.Image == (iMediaTypes & (int)MediaType.Image))
            {
                defaultExtension.AddRange(StandardFileExtensions.GetImageExtensions());
            }
            if ((int)MediaType.Video == (iMediaTypes & (int)MediaType.Video))
            {
                defaultExtension.AddRange(StandardFileExtensions.GetVideoExtensions());
            }
            if ((int)MediaType.Music == (iMediaTypes & (int)MediaType.Music))
            {
                defaultExtension.AddRange(StandardFileExtensions.GetAudioExtensions());
            }
            if ((int)MediaType.Document == (iMediaTypes & (int)MediaType.Document))
            {
                defaultExtension.AddRange(StandardFileExtensions.GetDocumentExtensions());
            }

            ContentView.Filter = (item) =>
            {
                if (-1 == iMediaTypes)
                    return false;
                string ext = ((MonitoredFile)item).Extension.ToUpper();
                return defaultExtension.Contains(ext);
            };
            ContentView.Refresh();
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
            currentJob = LoadMediasJob.Create(new LoadMediasJob.LoadPattern() { Category = LoadMediasJob.Category.Folder, keyword = iFolder.FullPath });
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
            currentJob = LoadMediasJob.Create(new LoadMediasJob.LoadPattern() { Category = LoadMediasJob.Category.Tag, keyword = tag.ID.ToString() });
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
                        this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Publish(loadMediasJob);
                        RunOnUIThread(() => {
                            foreach (MonitoredFile file in loadMediasJob.Files)
                            {
                                Files.Add(file);
                            }
                            ContentView.Refresh();
                        });
                        
                    }
                    break;
            }
        }

        private void FileScanner_ProcessEvent(object sender, Utilities.FileScan.FileScannerProcessEventArgs e)
        {
            if (e.IsNull() || e.CurrentFile.IsNull() || e.CurrentFile.File.IsNull())
                return;
            switch (e.ProcessType)
            {
                case ProcessType.Start:
                    break;
                case ProcessType.InProcess:
                    {
                        this.CurrentFilePath = e.CurrentFile.File.FullName;
                    }
                    break;
                case ProcessType.End:
                    break;
            }
        }

    }
}
