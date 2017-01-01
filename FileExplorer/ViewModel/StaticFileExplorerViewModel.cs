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

        public void UpdateFolderTree(IList<string> resourcePaths)
        {
            IFolder parent = RootFolder;
            foreach (string resourcePath in resourcePaths)
            {
                parent = RootFolder;
                string[] splitedFolders = resourcePath.Split(new string[] { ExplorerFactoryBase.PATH_SPLITER }, StringSplitOptions.RemoveEmptyEntries);
                string path = string.Empty;
                for (int i = 0; i < splitedFolders.Length; i++)
                {
                    path += splitedFolders[i] + ExplorerFactoryBase.PATH_SPLITER;
                    DirectoryInfo directory = new DirectoryInfo(path);
                    if (directory.IsNull())
                        break;
                    IFolder node = FindInFolder(directory.FullName, parent.Folders);
                    if (node.IsNull())
                    {
                        LocalFolder currentFolder = new LocalFolder(directory, parent);
                        currentFolder.Folders.Clear();
                        parent.Folders.Add(currentFolder);
                        parent = currentFolder;
                    }
                    else
                    {
                        parent = node;
                    }
                }
            }
        }

        public void RemoveFolderTree(IList<string> resourcePaths)
        {
            IFolder parent = RootFolder;
            foreach (string resourcePath in resourcePaths)
            {
                IFolder node = FindInFolder(resourcePath + ExplorerFactoryBase.PATH_SPLITER, parent.Folders);
                if (!node.IsNull())
                {
                    node.Parent.Folders.Remove(node);
                }
            }
        }

        private IFolder FindInFolder(string folderPathToFind, IList<IFolder> folders)
        {
            IFolder folder = null;
            foreach(IFolder iFolder in folders)
            {
                if (0 == string.Compare(iFolder.FullPath, folderPathToFind, true))
                {
                    folder = iFolder;
                    break;
                }
                folder = FindInFolder(folderPathToFind, iFolder.Folders);
                if (!folder.IsNull())
                    break;
            }
            return folder;
        }
    }
}
