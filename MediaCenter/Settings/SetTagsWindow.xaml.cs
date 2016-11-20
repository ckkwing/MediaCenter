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
        [Import]
        private IEventAggregator EventAggregator;

        private string folderPath = string.Empty;
        private IList<MonitoredFile> monitoredFiles;
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

        #region Command
        public ICommand CreateTagCommand { get; private set; }

        #endregion

        public SetTagsWindow(string folderPath, IList<MonitoredFile> monitoredFiles)
            : base(null)
        {
            InitializeComponent();
            this.DataContext = this;
            this.folderPath = folderPath;
            this.monitoredFiles = monitoredFiles;
            CreateTagCommand = new DelegateCommand(OnCreateTag);
        }

        private void OnCreateTag()
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

            UpdateTagsJob job = UpdateTagsJob.Create(folderPath, monitoredFiles);
            job.TagsToUpdate = tagToUpdate;
            JobManager.Instance.AddJob(job);
            JobManager.Instance.ForceStart(job);

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
    }
}
