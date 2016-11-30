using IDAL;
using IDAL.Model;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Core.Model;
using MediaCenter.Infrastructure.Event;
using MediaCenter.Theme.CustomControl;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Utilities.Extension;

namespace MediaCenter.Settings
{
    /// <summary>
    /// Interaction logic for SetTagsWindow.xaml
    /// </summary>
    public partial class SetTagsWindow : BasicWindow
    {
        //[Import]
        //private IEventAggregator EventAggregator;
        
        private object objToAddTags;
        private string tagArray = string.Empty;
        public string TagArray
        {
            get
            {
                return tagArray;
            }

            set
            {
                tagArray = value;
            }
        }

        private ObservableCollection<TagInfo> totalTags = new ObservableCollection<TagInfo>();
        public ObservableCollection<TagInfo> TotalTags
        {
            get
            {
                return totalTags;
            }

            set
            {
                totalTags = value;
                OnPropertyChanged("TotalTags");
            }
        }

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

        #region Command
        public ICommand AddTagCommand { get; private set; }
        public ICommand RemoveTagCommand { get; private set; }
        public ICommand RealActionCommand { get; private set; }

        #endregion

        public SetTagsWindow(object objToAddTags)
            : base(null)
        {
            InitializeComponent();
            this.DataContext = this;
            this.objToAddTags = objToAddTags;
            AddTagCommand = new DelegateCommand(OnAddTag);
            RemoveTagCommand = new DelegateCommand<TagInfo>(OnRemoveTag);
            RealActionCommand = new DelegateCommand(OnRealAction);
            GetExistTags();

        }

        private void GetExistTags()
        {
            if (objToAddTags is MonitoredFile)
            {
                Tags = new ObservableCollection<TagInfo>(DataManager.Instance.DBCache.GetContainTags(objToAddTags as MonitoredFile));
                for(int i =0; i<40;i++)
                {
                    Tags.Add(new TagInfo() { Name = i.ToString() });
                }
            }
        }

        private void OnAddTag()
        {
            bool isNewTagCreated = false;
            string tagToUpdate = string.Empty;
            string[] array = TagArray.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrEmpty(array[i]))
                    continue;
                TagInfo tagInfo = null;
                tagInfo = DataManager.Instance.DBCache.TagInfos.FirstOrDefault(item => (0 == string.Compare(array[i], item.Name)));
                if (tagInfo.IsNull())
                {
                    tagInfo = new TagInfo() { Name = array[i] };
                    if (!DBHelper.InsertTag(tagInfo))
                    {
                        break;
                    }
                    isNewTagCreated = true;
                    DataManager.Instance.DBCache.TagInfos.Add(tagInfo);
                }

                if (!tagInfo.IsNull())
                    tagToUpdate += tagInfo.ID + DataAccess.DB_MARK_SPLIT;
            }

            //UpdateTagsJob job = UpdateTagsJob.Create(folderPath, new List<MonitoredFile>() { monitoredFile });
            //job.TagsToUpdate = tagToUpdate;
            //JobManager.Instance.AddJob(job);
            //JobManager.Instance.ForceStart(job);

            //this.monitoredFile.Tag = tagToUpdate; //need clone
            //if (DBHelper.UpdateFile(this.monitoredFile))
            //{

            //}

            //Notify ui refresh tag region
            if (isNewTagCreated)
            {
                //this.EventAggregator.GetEvent<SettingsEvent>().Publish(new SettingEventArgs(SettingType.SetTags, null));
                
            }
        }

        private void OnRemoveTag(TagInfo tagInfo)
        {
        }

        private void OnRealAction()
        {

        }
    }
}
