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
            //this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Subscribe(OnLoadDataCompleted, ThreadOption.UIThread);
            TagSelectedCommand = new DelegateCommand<TagInfo>(OnTagSelected, CanExcute);
            LoadData();
        }

        ~NavigationViewModel()
        {
            //this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Unsubscribe(OnLoadDataCompleted);
        }

        private void OnLoadDataCompleted(LoadMediasJob obj)
        {
            throw new NotImplementedException();
        }

        private void LoadData()
        {
            //DataManager.Instance.DBCache.RefreshTagInfos();
            RunOnUIThreadAsync(() =>
            {
                Tags.Clear();
                foreach (TagInfo tag in DataManager.Instance.DBCache.TagInfos)
                {
                    Tags.Add(tag);
                }
            });

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
    }
}
