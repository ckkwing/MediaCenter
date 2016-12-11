using FileExplorer.Model;
using IDAL.Model;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities;
using MediaCenter.Infrastructure.Core.Model;

namespace MediaCenter.Modules.Navigation
{
    [Export(typeof(NavigationViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class NavigationViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;

        private ObservableCollection<TagInfo> tags = new ObservableCollection<TagInfo>();
        public ObservableCollection<TagInfo> Tags
        {
            get
            {
                return tags;
            }

            set
            {
                tags = value;
                OnPropertyChanged("Tags");
            }
        }

        //private ObservableCollection<MonitoredFolderInfo> monitoredFolders = new ObservableCollection<MonitoredFolderInfo>();
        //public ObservableCollection<MonitoredFolderInfo> MonitoredFolders
        //{
        //    get
        //    {
        //        return monitoredFolders;
        //    }

        //    set
        //    {
        //        monitoredFolders = value;
        //        OnPropertyChanged("MonitoredFolders");
        //    }
        //}

        #region Command
        public ICommand TagSelectedCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public NavigationViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            TagSelectedCommand = new DelegateCommand<TagInfo>(OnTagSelected, CanExcute);
            Infrastructure.Core.Core.Instance.JobRunningStateChanged += Instance_JobRunningStateChanged;
            LoadData();
        }

        ~NavigationViewModel()
        {
            Infrastructure.Core.Core.Instance.JobRunningStateChanged -= Instance_JobRunningStateChanged;
        }

        private void OnLoadDataCompleted(LoadMediasJob obj)
        {
            throw new NotImplementedException();
        }

        private void LoadData()
        {
            //DataManager.Instance.DBCache.RefreshTagInfos();
            RefreshTags();

            //DataManager.Instance.DBCache.RefreshMonitoredFolders();
            //RunOnUIThreadAsync(() =>
            //{
            //    MonitoredFolders.Clear();
            //    foreach (MonitoredFolderInfo info in DataManager.Instance.DBCache.MonitoredFolders)
            //    {
            //        MonitoredFolders.Add(info);
            //    }
            //});
        }

        private void RefreshTags()
        {
            RunOnUIThreadAsync(() =>
            {
                Tags.Clear();
                foreach (TagInfo tag in DataManager.Instance.DBCache.TagInfos)
                {
                    Tags.Add(tag);
                }
            });
        }

        private void OnTagSelected(TagInfo tag)
        {
            this.eventAggregator.GetEvent<TagChangedEvent>().Publish(tag);
        }

        private void OnMonitoredFolderSelected(MonitoredFolderInfo monitoredFolderInfo)
        {
            //this.eventAggregator.GetEvent<TagChangedEvent>().Publish(tag);
        }

        //public void NotifySelectedMonitoredFolderChanged(IFolder iFolder)
        //{
        //    this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Publish(iFolder);
        //}

        private void Instance_JobRunningStateChanged(object sender, Job job)
        {
            if (null == job)
                return;
            switch(job.Type)
            {
                case JobType.UpdateTags:
                    {
                        RunOnUIThreadAsync(() => {
                            this.Tags.Clear();
                            foreach (TagInfo tagInfo in DataManager.Instance.DBCache.TagInfos)
                            {
                                this.Tags.Add(tagInfo);
                            }
                        });
                    }
                    break;
                case JobType.LoadDBMedias:
                    break;
                case JobType.FileScanner:
                    {
                        RunOnUIThreadAsync(() => {
                        });
                    }
                    break;
            }
        }
    }
}
