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
using Utilities.Extension;

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

            ViewModel = new StaticFileExplorerViewModel();
            IList<string> paths = new List<string>();
            DataManager.Instance.DBCache.MonitoredFolders.ToList().ForEach(item => {
                paths.Add(item.Path);
            });
            ViewModel.LoadExplorerByFolderPaths(paths);
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
    }
}
