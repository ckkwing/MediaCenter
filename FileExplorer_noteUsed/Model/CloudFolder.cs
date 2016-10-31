using BackItUp.OnlineService;
using FileExplorer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileExplorer.Model
{
    public class CloudFolder : FolderBase
    {
        protected CloudFolder()
        {
        }

        public CloudFolder(ICloudFolder folder, IFolder parent)
            : base(string.Empty, parent)
        {
            if (folder.IsNull() || parent.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.CloudItem = folder;
            this.Parent = parent;
            this.Name = folder.Name;
            this.LastModifyTime = folder.ModificationTime;
            this.Size = folder.Size;
            this.FullPath = Path.Combine(parent.FullPath, folder.CloudId);
            this.AddPlaceHolder();
        }

        protected ICloudFolder CloudItem { get; set; }

        protected override string IconKey
        {
            get
            {
                return this.Name;
            }
        }

        public override string FolderPath
        {
            get
            {
                folderPath = folderPath.IsNullOrEmpty() && !Parent.IsNull() ? Path.Combine(this.Parent.FolderPath, this.Parent.Name) : folderPath;
                return folderPath;
            }
        }

        public string GetCloudPath
        {
            get
            {
                string cloudPath = Path.Combine(FolderPath, this.Name);
                return cloudPath;
            }
        }

        #region IFolderAsync

        public override void GetChildrenAsync(Action<IEnumerable<IFile>> callback)
        {
            IsCanceled = false;
            ///If all subitem got, don't need show loading
            if (!(IsFolderLoaded && IsFileLoaded))
            {
                IsLoading = true;
            }

            this.GetFoldersAsync((folders) =>
            {
                this.GetFilesAsync((files) =>
                {
                    if (!callback.IsNull())
                    {
                        callback(this.Items);
                    }
                });
            });
        }

        public override void GetFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            Action action = () =>
            {
                if (!IsFolderLoaded)
                {
                    StartLoadCloudData();
                }
            };

            action.BeginInvoke((ar) =>
            {
                EndInvokeAction(ar);
                if (!callback.IsNull())
                {
                    callback(this.Folders);
                }

            }, action);
        }

        public override void GetFilesAsync(Action<IEnumerable<IFile>> callback)
        {
            Action action = () =>
            {
                if (!IsFileLoaded)
                {
                    StartLoadCloudData();
                }
            };

            action.BeginInvoke((ar) =>
            {
                EndInvokeAction(ar);
                if (!callback.IsNull())
                {
                    callback(this.Files);
                }

            }, action);
        }

        AutoResetEvent autoEvent = new AutoResetEvent(false);
        private void StartLoadCloudData()
        {
            lock (lockObj)
            {
                LoadCloudChildren();
            }
        }

        private void LoadCloudChildren()
        {
            if (this.IsFolderLoaded && this.IsFileLoaded)
            {
                this.IsLoading = false;
                autoEvent.Set();
                return;
            }

            IsFolderLoaded = false;
            IsFileLoaded = false;

            RunOnUIThread(() =>
            {
                this.Folders.Clear();
                this.Files.Clear();
            });

            Action<CloudServiceWithDataEventArgs<ChildrenParameters>> cloudFolderChildrenReadyAction = null;

            const int retryTime = 10;
            int currentRetry = 0;

            cloudFolderChildrenReadyAction = (e) =>
            {
                ///May request multi-times
                ///so need ignore cancled request
                if (!e.IsNull() && e.IsSucceeded &&
                    !e.Data.IsNull() && !e.Data.Children.IsNull() &&
                    !IsCanceled)
                {
                    try
                    {
                        IEnumerable<IFolder> cloudFolders = e.Data.Children.Where(item => item.IsDir &&
                                                                                  !this.Folders.Any(i => (i as CloudFolder).CloudItem.CloudId == item.CloudId))
                                                                           .Select(item => new CloudFolder(item as ICloudFolder, this));
                        IEnumerable<IFile> cloudFiles = e.Data.Children.Where(item => !item.IsDir &&
                                                                              !this.Files.Any(i => (i as CloudFile).CloudItem.CloudId == item.CloudId))
                                                                       .Select(item => new CloudFile(item, this));

                        if (null != e && null != e.Data)
                        {
                            this.AddItemsByChunk(cloudFolders, this.Folders, this.Items);
                            this.AddItemsByChunk(cloudFiles, this.Files, this.Items);
                        }
                    }
                    catch { }
                    currentRetry = 0;
                }
                else
                {
                    if (currentRetry++ < retryTime)
                    {
                        Thread.Sleep(2 * 1000);
                    }
                    else
                    {
                        ///Timeout, retry failed too
                        this.IsLoading = false;
                        autoEvent.Set();
                        return;
                    }
                }

                if (IsCanceled || e.Data.IsNull() || (e.IsSucceeded && !e.Data.IsNull() && e.Data.NextPagingEntry.IsNull()))
                {
                    this.IsLoading = false;
                    this.IsFolderLoaded = true;
                    this.IsFileLoaded = true;
                    autoEvent.Set();
                    return;
                }
                else
                {
                    //Read next page
                    this.CloudItem.GetChildrenAsync(cloudFolderChildrenReadyAction, e.Data.NextPagingEntry);
                }
            };

            ///TODO:Files loaded before folders 
            ///this need CLOUD API support for sort or filter

            this.CloudItem.GetChildrenAsync(cloudFolderChildrenReadyAction, null);
            autoEvent.WaitOne();
        }

        public override void Cancel()
        {
            base.Cancel();
            autoEvent.Set();
        }

        protected override void AddPlaceHolder()
        {
            if (this != LocalFolder.PlackHolderItem &&
                  !this.Folders.Contains(LocalFolder.PlackHolderItem))
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Add(LocalFolder.PlackHolderItem);
                });
            }

        }

        public override object Clone()
        {
            CloudFolder file = new CloudFolder();
            CloneMembers(file);
            return file;
        }
        #endregion
    }
}
