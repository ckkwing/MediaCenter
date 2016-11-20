﻿using FileExplorer.Model;
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
                IList<string> existedFileList = DBHelper.GetExistMonitoredFolderStringList();
                IEnumerable<string> newFolders = selectedFilesPath.Where(path => !existedFileList.Contains(path));
                IEnumerable<IFolder> foldersToAdd = selectedFiles.Where(item => newFolders.Contains(item.FullPath));
                DBHelper.InsertFoldersToMonitor(foldersToAdd.ToList());
                DataManager.Instance.DBCache.RefreshMonitoredFolders();
                //this.eventAggregator.GetEvent<MonitoredFoldersChangedEvent>().Publish("");
                DataManager.Instance.FileScanner.Config = new FileScanner.FileScannerConfiguration() { PathsToScan = DataManager.Instance.DBCache.MonitoredFolderStrings };
                DataManager.Instance.FileScanner.Start();
            }
        }

        private void OpenTagSettingWindow(object obj)
        {
            if (null == obj)
                return;

            string folderPath = string.Empty;
            IList<MonitoredFile> monitoredFiles = new List<MonitoredFile>();
            if (obj is IFolder)
                folderPath = (obj as IFolder).FullPath;
            else if (obj is MonitoredFile)
                monitoredFiles.Add(obj as MonitoredFile);
            else
                return;

            SetTagsWindow window = new Settings.SetTagsWindow(folderPath, monitoredFiles);
            window.ShowDialog();
        }
        
    }
}
