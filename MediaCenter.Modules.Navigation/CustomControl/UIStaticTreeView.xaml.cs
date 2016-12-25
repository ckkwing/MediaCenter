using FileExplorer.Model;
using FileExplorer.ViewModel;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Utilities.Extension;
using MediaCenter.Infrastructure.Core.Model;
using System.Windows.Threading;
using Utilities.FileScan;
using System.Diagnostics;

namespace MediaCenter.Modules.Navigation.CustomControl
{
    /// <summary>
    /// Interaction logic for UIStaticTreeView.xaml
    /// </summary>
    public partial class UIStaticTreeView : UserControl
    {
        private readonly IEventAggregator eventAggregator;

        public StaticFileExplorerViewModel ViewModel
        {
            get { return this.DataContext as StaticFileExplorerViewModel; }
            private set { this.DataContext = value; }
        }

        public UIStaticTreeView()
        {
            InitializeComponent();
            this.eventAggregator = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEventAggregator>();
            if (null == eventAggregator)
                throw new ArgumentNullException("IEventAggregator");
            this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Subscribe(OnLoadDataCompleted, ThreadOption.UIThread);
            DataManager.Instance.FileScanner.ProcessEvent += FileScanner_ProcessEvent;
            ViewModel = new StaticFileExplorerViewModel();
            ViewModel.LoadExplorerByFolderPaths(DataManager.Instance.DBCache.MonitoredFolderStrings);
        }

        ~UIStaticTreeView()
        {
            DataManager.Instance.FileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Unsubscribe(OnLoadDataCompleted);
        }

        private void OnLoadDataCompleted(LoadMediasJob obj)
        {
            //ViewModel.LoadExplorerByFolderPaths(DataManager.Instance.DBCache.MonitoredFileStrings);
        }

        private void treeExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IFolder folder = e.NewValue as IFolder;
            if (folder.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }

            //this.ViewModel.SetCurrentFolder(folder);
            this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Publish(folder);
        }

        private void treeExplorer_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = e.OriginalSource as TreeViewItem;
            if (item == null)
            {
                return;
            }
            IFolder folder = item.DataContext as IFolder;
            if (folder.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }

            //this.ViewModel.LoadFolderChildren(folder);
        }

        private void treeExplorer_PreviewMouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var treeViewItem = VisualUpwardSearch<TreeViewItem>(e.OriginalSource as DependencyObject) as TreeViewItem;
            if (treeViewItem != null)
            {
                treeViewItem.Focus();
                e.Handled = true;
            }
        }

        static DependencyObject VisualUpwardSearch<T>(DependencyObject source)
        {
            while (source != null && source.GetType() != typeof(T))
                source = VisualTreeHelper.GetParent(source);

            return source;
        }

        private void btnSetTags_Click(object sender, RoutedEventArgs e)
        {
            IFolder folder = treeExplorer.SelectedItem as IFolder;
            if (folder.IsNull())
                return;
            this.eventAggregator.GetEvent<SettingsEvent>().Publish(new SettingEventArgs(SettingType.SetTags, folder));
        }

        private void btnOpenLocation_Click(object sender, RoutedEventArgs e)
        {
            IFolder folder = treeExplorer.SelectedItem as IFolder;
            if (folder.IsNull())
                return;

            Process.Start("explorer.exe", folder.FullPath);
        }

        private void FileScanner_ProcessEvent(object sender, FileScannerProcessEventArgs e)
        {
            if (e.IsNull())
                return;
            switch (e.ProcessType)
            {
                case ProcessType.InProcess:
                    break;
                    {
                        if (e.IsOneDirScanned)
                        {

                        }
                    }
                case ProcessType.End:
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            ViewModel.LoadExplorerByFolderPaths(DataManager.Instance.DBCache.MonitoredFolderStrings);
                        }));
                    }
                    break;
            }
        }
    }
}
