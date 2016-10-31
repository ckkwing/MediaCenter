using FileExplorer.Model;
using IDAL.Model;
using MediaCenter.Infrastructure;
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
                        OpenTagSettingWindow(eventArgs.Content as MonitoredFile);
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
                DBHelper.InsertFoldersToMonitor(selectedFiles);
                //this.eventAggregator.GetEvent<MonitoredFoldersChangedEvent>().Publish("");
                DataManager.Instance.FileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = DBHelper.GetExistMonitoredFolderStringList() };
                DataManager.Instance.FileScanner.Start();
            }
        }

        private void OpenTagSettingWindow(MonitoredFile monitoredFile)
        {
            if (null == monitoredFile)
                return;

            SetTagsWindow window = new Settings.SetTagsWindow(monitoredFile);
            window.ShowDialog();
        }
        
    }
}
