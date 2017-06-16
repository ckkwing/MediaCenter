using FileExplorer.Model;
using FileExplorer.ViewModel;
using MediaCenter.Infrastructure;
using MediaCenter.Theme.CustomControl;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Utilities.Extension;
using static MediaCenter.Theme.CustomControl.Dialog.CommonDialog;
using MediaCenter.Theme.CustomControl.Dialog;

namespace MediaCenter.Settings.FolderManager
{
    /// <summary>
    /// Interaction logic for FolderManagerWindow.xaml
    /// </summary>
    public partial class FolderManagerWindow : BasicWindow
    {
        //public int Test { get; set; } = 1;
        //public override string ToString() => $"{Test}";

        private IList<IFolder> selectedFileList = new List<IFolder>();
        public IList<IFolder> SelectedFileList
        {
            get
            {
                return selectedFileList;
            }

            private set
            {
                selectedFileList = value;
            }
        }

        private ResultButtonType resultButtonType = ResultButtonType.Cancel;
        public ResultButtonType ResultButtonType
        {
            get
            {
                return resultButtonType;
            }

            private set
            {
                resultButtonType = value;
            }
        }

        public FileExplorerViewModel ViewModel
        {
            get { return this.DataContext as FileExplorerViewModel; }
            private set { this.DataContext = value; }
        }

        public FolderManagerWindow(Window owner = null)
            : base(owner)
        {
            InitializeComponent();
            ViewModel = new FileExplorerViewModel();
        }

        private void BasicWindow_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadLocalExplorer();
            if (!ViewModel.RootFolder.IsNull() &&
                             ViewModel.RootFolder.IsChecked == false)
            {
                ViewModel.SetCheckedPaths(DBHelper.GetExistMonitoredFolderStringList());
            }
        }

        private void BasicWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }
        
        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            this.ResultButtonType = ResultButtonType.OK;
            SelectedFileList = ViewModel.GetCheckedFolders();
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        
    }
}
