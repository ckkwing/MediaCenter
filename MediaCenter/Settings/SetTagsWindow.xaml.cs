using FileExplorer.Model;
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

        private ObservableCollection<DBTagInfo> totalTags = new ObservableCollection<DBTagInfo>();
        public ObservableCollection<DBTagInfo> TotalTags
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

        private ObservableCollection<DBTagInfo> tags = new ObservableCollection<DBTagInfo>();
        public ObservableCollection<DBTagInfo> Tags
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
        public ICommand RealActionCommand { get; private set; }

        #endregion

        public SetTagsWindow(object objToAddTags)
            : base(null)
        {
            InitializeComponent();
            this.DataContext = this;
            this.objToAddTags = objToAddTags;
            AddTagCommand = new DelegateCommand(OnAddTag);
            RealActionCommand = new DelegateCommand(OnRealAction);
            GetExistTags();
        }

        private void GetExistTags()
        {
            TotalTags = new ObservableCollection<DBTagInfo>(DataManager.Instance.DBCache.TagInfos);
            if (objToAddTags is DBFileInfo)
            {
                Tags = new ObservableCollection<DBTagInfo>(DataManager.Instance.DBCache.GetContainTags(objToAddTags as DBFileInfo));
            }
        }

        private void OnAddTag()
        {
            string[] array = TagArray.Split(';');
            for (int i = 0; i < array.Length; i++)
            {
                if (string.IsNullOrEmpty(array[i]))
                    continue;
                DBTagInfo tagInfo = null;
                tagInfo = TotalTags.FirstOrDefault(item => (0 == string.Compare(array[i], item.Name)));
                if (tagInfo.IsNull())
                {
                    tagInfo = new DBTagInfo() { Name = array[i] };
                    TotalTags.Add(tagInfo);
                }

                if (!tagInfo.IsNull())
                {
                    AddTag(tagInfo);
                }
            }
        }

        private void OnRealAction()
        {
            string tagToUpdate = string.Empty;
            var newTags = from info in Tags where info.ID == -1 select info;
            DBHelper.InsertTags(newTags.ToList());
            foreach (DBTagInfo tagInfo in Tags)
            {
                tagToUpdate += tagInfo.ID + DataAccess.DB_MARK_SPLIT;
            }
            string folderPath = string.Empty;
            List<DBFileInfo> files = new List<DBFileInfo>();
            if (objToAddTags is DBFileInfo)
            {
                files.Add(objToAddTags as DBFileInfo);
            }
            else
            {
                IFolder iFolder = objToAddTags as IFolder;
                folderPath = iFolder.FullPath;
            }
            UpdateTagsJob job = UpdateTagsJob.Create(folderPath, files);
            job.TagsToUpdate = tagToUpdate;
            JobManager.Instance.AddJob(job);
            JobManager.Instance.ForceStart(job);
            this.Close();
        }

        private void OnListBoxItemDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem listBoxItem = e.Source as ListBoxItem;
            if (listBoxItem.IsNull())
                return;
            DBTagInfo tagInfo = listBoxItem.DataContext as DBTagInfo;
            if (tagInfo.IsNull())
                return;

           switch(listBoxItem.Tag.ToString())
            {
                case "Add":
                    AddTag(tagInfo);
                    break;
                case "Remove":
                    RemoveTag(tagInfo);
                    break;
            }
        }

        private void AddTag(DBTagInfo tagInfo)
        {
            if (Tags.Contains(tagInfo))
                return;
            Tags.Add(tagInfo);
        }

        private void RemoveTag(DBTagInfo tagInfo)
        {
            if (Tags.Contains(tagInfo))
                Tags.Remove(tagInfo);
        }
    }
}
