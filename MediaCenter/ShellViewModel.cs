using FileExplorer.Model;
using IDAL.Model;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Core.Model;
using MediaCenter.Infrastructure.Core.Runner;
using MediaCenter.Infrastructure.Event;
using MediaCenter.Settings;
using MediaCenter.Settings.FolderManager;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.FileScan;

namespace MediaCenter
{
    [Export]
    public class ShellViewModel : ViewModelBase
    {
        private IEventAggregator eventAggregator;

        [ImportingConstructor]
        public ShellViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<SettingsEvent>().Subscribe(OnSetting, ThreadOption.UIThread);
            JobManager.Instance.JobRunningStateChanged += Instance_JobRunningStateChanged;
        }

        ~ShellViewModel()
        {
            JobManager.Instance.JobRunningStateChanged -= Instance_JobRunningStateChanged;
        }

        private void Instance_JobRunningStateChanged(object sender, Infrastructure.Core.Model.Job job)
        {
            ;
        }

        private void OnSetting(SettingEventArgs eventArgs)
        {
            switch (eventArgs.SettingType)
            {
                case SettingType.FolderManager:
                    {
                        OpenFolderManagerWindow();
                    }
                    break;
                case SettingType.SetTags:
                    {
                        OpenTagSettingWindow(eventArgs.Content);
                    }
                    break;
            }
        }

        private void OpenFolderManagerWindow()
        {
            FolderManagerWindow window = new FolderManagerWindow();
            window.ShowDialog();
            
            IList<IFolder> selectedFiles = window.SelectedFileList;
            IList<string> selectedFilesPath = selectedFiles.Select(item => item.FullPath).ToList();

            if (window.ResultButtonType == Theme.CustomControl.Dialog.CommonDialog.ResultButtonType.OK)
            {
                FileScannerJob job = FileScannerJob.Create(selectedFilesPath);
                JobManager.Instance.AddJob(job);
                JobManager.Instance.ForceStart(job);
                return;

                DataManager.Instance.FileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = selectedFilesPath };
                DataManager.Instance.FileScanner.StartAsync();
            }
        }

        private void OpenTagSettingWindow(object obj)
        {
            if (null == obj)
                return;
            
            SetTagsWindow window = new Settings.SetTagsWindow(obj);
            window.ShowDialog();
        }
        
    }
}
