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

            ViewModel = new StaticFileExplorerViewModel();
            //IList<string> paths = new List<string>();
            //DataManager.Instance.DBCache.MonitoredFolders.ToList().ForEach(item => {
            //    paths.Add(item.Path);
            //});
            ViewModel.LoadExplorerByFolderPaths(DataManager.Instance.DBCache.MonitoredFolderStrings);
        }

        ~UIStaticTreeView()
        {
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

            this.eventAggregator.GetEvent<MonitoredFoldersSelectedEvent>().Publish(folder);
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
    }
}
