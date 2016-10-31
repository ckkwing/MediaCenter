using FileExplorer.Model;
using IDAL.Model;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Behaviors;
using SQLiteDAL;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Utilities.Extension;

namespace MediaCenter.Modules.Navigation
{
    /// <summary>
    /// Interaction logic for NavigationView.xaml
    /// </summary>
    //[Export]

    [ViewExport(RegionName = RegionNames.NavigationRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class NavigationView : UserControl
    {
        public NavigationView()
        {
            InitializeComponent();
        }

        [Import]
        NavigationViewModel ViewModel
        {
            get
            {
                return this.DataContext as NavigationViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        //private void treeExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        //{
        //    IFolder folder = e.NewValue as IFolder;
        //    if (folder.IsNull() || this.ViewModel.IsNull())
        //    {
        //        return;
        //    }
        //    ViewModel.NotifySelectedMonitoredFolderChanged(folder);
        //}
    }
}
