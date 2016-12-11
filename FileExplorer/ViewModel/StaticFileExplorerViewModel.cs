using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Model;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities.Extension;

namespace FileExplorer.ViewModel
{
    [Export(typeof(StaticFileExplorerViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class StaticFileExplorerViewModel : FileExplorerViewModel
    {
        IList<string> resourcePaths = new List<string>();

        #region Command
        //public ICommand OnLoadedCommand { get; private set; }

        #endregion

        public StaticFileExplorerViewModel()
        {
        }

        public void LoadExplorerByFolderPaths(IList<string> paths)
        {
            resourcePaths = paths;
            this.InitialExplorer(new StaticLocalExplorerFactory(resourcePaths));
        }

        public override void SetCurrentFolder(IFolder folder)
        {
            if (folder.IsNull() || this.CurrentFolder == folder)
            {
                return;
            }

            if (folder is LocalRootFolder)
                return;

            //if (null == resourcePaths.FirstOrDefault(path => path.Contains(folder.FullPath)))
            //    return;

            this.RunOnUIThread(() =>
            {
                ///If the new folder is current folder's child
                ///current folder need load all childrens
                ///Can not be canceled
                if (!this.CurrentFolder.IsNull() && folder.Parent != this.CurrentFolder)
                {
                    this.CurrentFolder.Cancel();
                }
                //ClearSortOptions();
                this.CurrentFolder = folder;
                //this.ContentView = CollectionViewSource.GetDefaultView(this.CurrentFolder.Items);
                //this.SetSortOrder(SortPropertyName, ListSortDirection.Ascending);
                //this.InvokeSortOrderCallback();
            });

            folder.GetChildrenAsync((items) =>
            {
                // this.SetCheckedPaths();
            });
        }

        public override void LoadFolderChildren(IFolder folder)
        {
            if (folder.IsNull() || this.CurrentFolder == folder)
            {
                return;
            }

            if (folder is LocalRootFolder)
                return;
            
            //if (null == resourcePaths.FirstOrDefault(path => path.Contains(folder.FullPath)))
            //    return;

            folder.GetChildrenAsync((folders) =>
            {
                //this.SetCheckedPaths();
            });
        }


    }
}
