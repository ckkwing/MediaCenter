using FileExplorer.Model;
using FileExplorer.ViewModel;
using System;
using System.Collections.Generic;
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

namespace FileExplorer
{
    /// <summary>
    /// Interaction logic for UITreeView.xaml
    /// </summary>
    public partial class UITreeView : UserControl
    {
        public FileExplorerViewModel ViewModel
        {
            get
            {
                return this.DataContext as FileExplorerViewModel;
            }
        }

        public UITreeView()
        {
            InitializeComponent();
        }

        private void treeExplorer_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            IFolder folder = e.NewValue as IFolder;
            if (folder.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }

            this.ViewModel.SetCurrentFolder(folder);
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

            this.ViewModel.LoadFolderChildren(folder);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            IFolder folder = chk.DataContext as IFolder;
            if (folder.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox chk = sender as CheckBox;
            IFolder folder = chk.DataContext as IFolder;
            if (folder.IsNull() || this.ViewModel.IsNull())
            {
                return;
            }
        }
    }
}
