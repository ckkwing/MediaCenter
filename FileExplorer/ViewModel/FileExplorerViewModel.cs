﻿using FileExplorer.Event;
using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extension;

namespace FileExplorer.ViewModel
{
    public class FileExplorerViewModel : ViewModelBase
    {
        #region Property
        protected ExplorerFactoryBase RootFactory { get; set; }
        protected ObservableCollection<IFolder> items = new ObservableCollection<IFolder>();
        /// <summary>
        /// Root items
        /// </summary>
        public ObservableCollection<IFolder> Items
        {
            get { return items; }
        }
        public bool IsInitialFailed
        {
            get
            {
                return this.RootFolder.IsNull();
            }
        }

        protected IFolder rootFolder;
        public IFolder RootFolder
        {
            get { return rootFolder; }
            protected set
            {
                SetProperty(ref rootFolder, value, "IsInitialFailed");
            }
        }

        protected IFolder currentFolder = null;
        public IFolder CurrentFolder
        {
            get { return currentFolder; }
            protected set
            {
                if (SetProperty(ref currentFolder, value, "CurrentFolder"))
                {
                    //Not implement now
                    //this.SearchViewModel.InitialSearch(currentFolder);
                }
            }
        }

        protected IList<string> checkedPaths;
        public IList<string> CheckedPaths
        {
            get
            {
                checkedPaths = checkedPaths ?? new List<string>();
                return checkedPaths;
            }
            protected set
            {
                if (checkedPaths != value)
                {
                    checkedPaths = value;
                }
            }
        }

        protected bool isChecking = false;
        /// <summary>
        /// Is current checking item in treeview
        /// </summary>
        public bool IsChecking
        {
            get { return isChecking; }
            set
            {
                SetProperty(ref isChecking, value, "IsChecking");
            }
        }

        #endregion

        public FileExplorerViewModel()
        {

        }

        public void LoadLocalExplorer()
        {
            this.InitialExplorer(new LocalExplorerFactory());
        }

        virtual internal void InitialExplorer(ExplorerFactoryBase factory)
        {
            if (factory.IsNull())
            {
                throw new ArgumentNullException("factory");
            }

            RemoveExplorer();
            this.RootFactory = factory;
            this.RootFactory.GetRootFoldersAsync((items) =>
            {
                this.RunOnUIThread(() =>
                {
                    try
                    {
                        foreach (var item in items)
                        {
                            if (RootFolder.IsNull())
                            {
                                LogHelper.Debug("RootFolder is NULL, add root");
                                RootFolder = item;
                                if (RootFolder is LocalRootFolder)
                                {
                                    (RootFolder as LocalRootFolder).DriverChanged += FileExplorerViewModel_DriverChanged;
                                }
                            }
                            else
                            {
                                //Not implement now
                                //if (item is CloudRootFolder)
                                //{
                                //    if (0 == string.Compare(item.FolderPath, RootFolder.FolderPath))
                                //    {
                                //        LogHelper.Debug("Duplicate root for CloudRootFolder, so ignore this callback");
                                //        continue;
                                //    }
                                //}
                            }
                            this.Items.Add(item);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Debug(string.Format("InitialExplorer error:{0}", ex.Message));
                    }
                });

                FileBase.RaiseCheckedChanged(RootFolder, false);
            });

        }

        protected void RemoveExplorer()
        {
            if (this.RootFactory.IsNull())
            {
                return;
            }

            RunOnUIThread(() =>
            {
                foreach (var item in this.Items)
                {
                    item.Dispose();
                }
                this.Items.Clear();
                //this.ContentView = null;
            });

            this.RootFactory = null;
            if (!RootFolder.IsNull())
            {
                if (RootFolder is LocalRootFolder)
                {
                    (RootFolder as LocalRootFolder).DriverChanged -= FileExplorerViewModel_DriverChanged;
                }
                this.RootFolder.Dispose();
                this.RootFolder = null;
            }

            this.CurrentFolder = null;
            //this.SearchViewModel.UninitialSearch();
        }

        public virtual void LoadFolderChildren(IFolder folder)
        {
            if (folder.IsNull() || this.CurrentFolder == folder)
            {
                return;
            }

            folder.GetChildrenAsync((folders) =>
            {
                //this.SetCheckedPaths();
            });
        }

        public virtual void SetCurrentFolder(IFolder folder)
        {
            if (folder.IsNull() || this.CurrentFolder == folder)
            {
                return;
            }

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

        public IEnumerable<string> GetCheckedPaths()
        {
            IEnumerable<IFile> checkedItems = GetCheckedFiles();
            IEnumerable<string> result = checkedItems.Select(item => item.FullPath);

            return result;
        }

        public IList<IFolder> GetCheckedFolders()
        {
            IList<IFolder> checkedItems = new List<IFolder>();
            if (!this.RootFolder.IsNull() && this.RootFolder is IFolderCheck)
            {
                IEnumerable<IFile> checkedFiles = (this.RootFolder as IFolderCheck).GetCheckedItems(this.RootFolder);
                if (null != checkedFiles)
                {
                    foreach (IFile ifile in checkedFiles)
                    {
                        if (ifile.IsFolder)
                        {
                            IFolder ifolder = ifile as IFolder;
                            if (null != ifolder)
                                checkedItems.Add(ifolder);
                        }

                    }
                }
            }

            return checkedItems;
        }

        public IEnumerable<IFile> GetCheckedFiles()
        {
            IEnumerable<IFile> checkedItems = new IFile[0];
            //if (!this.SearchViewModel.IsNull() && this.SearchViewModel.IsSearchEnabled)
            //{
            //    checkedItems = this.SearchViewModel.GetCheckedItems();
            //}
            //else if (!this.RootFolder.IsNull() && this.RootFolder is IFolderCheck)
            //{
            //    checkedItems = (this.RootFolder as IFolderCheck).GetCheckedItems(this.RootFolder);
            //}

            //Temp implement
            if (!this.RootFolder.IsNull() && this.RootFolder is IFolderCheck)
            {
                checkedItems = (this.RootFolder as IFolderCheck).GetCheckedItems(this.RootFolder);
            }
            
            return checkedItems;
        }

        public void SetCheckedPaths(IList<string> pathList)
        {
            if (pathList.IsNullOrEmpty())
            {
                return;
            }
            this.CheckedPaths = pathList.OrderBy(item => item.Length).ToList();
            SetCheckedPaths();
        }

        public void SetCheckedPaths()
        {
            SetPathChecked(CheckedPaths);
        }

        public void SetPathChecked(IEnumerable<string> list, bool isChecked = true)
        {
            if (list.IsNullOrEmpty() || this.RootFolder.IsNull()) //this.RootFolder.IsLoading
            {
                return;
            }

            IsChecking = true;
            int totalItems = list.Count();
            int checkProcessing = 0;

            Action action = () =>
            {
                int i = 0;
                const int timeout = 60;
                //Add timeout for checking mask view can't be removed
                // minutes
                while (true)
                {
                    Thread.Sleep(1 * 1000);
                    if (!IsChecking)
                    {
                        return;
                    }
                    if (i++ > timeout)
                    {
                        if (IsChecking)
                        {
                            IsChecking = false;
                        }
                        return;
                    }
                }
            };
            action.BeginInvoke((ar) => action.EndInvoke(ar), action);

            foreach (string path in list)
            {
                ///Maybe change explore factory during checking
                if (RootFolder.IsNull())
                {
                    IsChecking = false;
                    break;
                }

                this.RootFolder.GetItemAsync(path, (file) =>
                {
                    checkProcessing++;
                    LogHelper.DebugFormat("/****　set checked index:{0}, total:{1}", checkProcessing, totalItems);
                    if (checkProcessing == totalItems)
                    {
                        IsChecking = false;
                    }
                    if (!file.IsNull())
                    {
                        file.IsChecked = isChecked;
#if DEBUG
                        //if (file is CloudFile || file is CloudFolder)
                        //{
                        //    LogHelper.DebugFormat("/++++　set checked path:{0}", file.FolderPath + file.Name);
                        //}
                        //else
                        //{
                        //    LogHelper.DebugFormat("/++++　set checked path:{0}", file.FullPath);
                        //}
#endif
                    }
                });
            }
        }

        protected void FileExplorerViewModel_DriverChanged(List<string> drives, bool isAdd)
        {
            //Not implement now
            //SetCheckedPaths();
        }
    }
}
