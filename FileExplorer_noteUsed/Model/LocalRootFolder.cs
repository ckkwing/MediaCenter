using FileExplorer.Factory;
using FileExplorer.Helper;
using FileExplorer.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileExplorer.Model
{
    public class LocalRootFolder : LocalFolder
    {
        DriverWatcher driverWatcher = null;
        /// <summary>
        /// notify drive change
        /// </summary>
        public event DriverChangedEventhandler DriverChanged;

        public LocalRootFolder()
            : base(string.Empty, null)
        {
            using (DataSourceShell shellItem = LocalExplorerFactory.GetPCRootShellItem())
            {
                this.Name = shellItem.DisplayName;
                this.Icon = shellItem.Icon;
            }

            this.IsCheckVisible = false;
            this.IsExpanded = true;
            this.IsSelected = true;
            this.Parent = this;
        }

        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                var drivers = DriverWatcher.GetFixedDrivers().OrderBy(item => item.Name);
                var list = drivers.Select(item => new LocalDriver(item.Name, this));
                IsFolderLoaded = AddItemsByChunk(list, this.Folders, this.Items);

                driverWatcher = new DriverWatcher();
                driverWatcher.DriverChanged += driverWatcher_DriverChanged;
                driverWatcher.Start(drivers.Select(item => item.Name).ToList());
            }
            return this.Folders;
        }

        void driverWatcher_DriverChanged(List<string> drivers, bool isAdd)
        {
            if (drivers.IsNullOrEmpty())
            {
                return;
            }

            RunOnUIThreadAsync(() =>
            {
                if (isAdd)
                {
                    foreach (var item in drivers)
                    {
                        LocalDriver newDriver = new LocalDriver(item, this);
                        var firstDriver = this.Folders.FirstOrDefault(f => f.Name.CompareTo(newDriver.Name) > 0);
                        if (!firstDriver.IsNull())
                        {
                            this.Folders.Insert(this.Folders.IndexOf(firstDriver), newDriver);
                        }
                        else
                        {
                            this.Folders.Add(newDriver);
                        }

                        this.Items.Add(newDriver);
                    }
                }
                else
                {
                    foreach (var item in drivers)
                    {
                        var driver = this.Folders.FirstOrDefault(d => d.Name.EqualIgnoreCase(item));
                        if (!driver.IsNull())
                        {
                            driver.Dispose();
                            this.Folders.Remove(driver);
                            this.Items.Remove(driver);
                        }
                    }
                }

                if (!DriverChanged.IsNull())
                {
                    DriverChanged(drivers, isAdd);
                }
            });
        }

        protected override void OnDisposing(bool isDisposing)
        {
            base.OnDisposing(isDisposing);
            if (!driverWatcher.IsNull())
            {
                driverWatcher.DriverChanged -= driverWatcher_DriverChanged;
                driverWatcher.Stop();
            }
        }
    }

    public class LocalDriver : LocalFolder
    {
        public LocalDriver(string path, IFolder parent)
            : base(path, parent)
        {
            /// CD-ROM is not existed 
            /// Virtual folder is not existed

            this.FullPath = path;
            this.Parent = parent;
            this.AddPlaceHolder();
            fileAttr = FileAttributes.Directory;
        }
    }

    public class LocalVirtualFolder : LocalFolder
    {
        public LocalVirtualFolder(DataSourceShell shellItem)
            : base(string.Empty, null)
        {
            if (shellItem.IsNull())
            {
                throw new ArgumentNullException();
            }
            this.Name = shellItem.DisplayName;
            this.Icon = shellItem.Icon;
            this.FullPath = shellItem.Path;
            this.IsCheckVisible = false;
            this.Parent = this;
        }

        private IFolder realFolder;
        public IFolder RealFolder
        {
            get { return realFolder; }
            set
            {
                if (realFolder != value)
                {
                    if (!realFolder.IsNull())
                    {
                        realFolder.VirtualParent = null;
                    }

                    realFolder = value;
                    if (!realFolder.IsNull())
                    {
                        realFolder.VirtualParent = this;
                    }
                }
            }
        }

        public override void GetFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            if (RealFolder.IsNull())
            {
                if (!callback.IsNull())
                {
                    callback(this.Folders);
                }
                return;
            }

            this.RealFolder.GetFoldersAsync((folders) =>
            {
                if (!IsFolderLoaded)
                {
                    IsFolderLoaded = AddItemsByChunk(folders, this.Folders, this.Items);
                }
                if (!callback.IsNull())
                {
                    callback(this.Folders);
                }
            });
        }

        public override void GetFilesAsync(Action<IEnumerable<IFile>> callback)
        {
            if (RealFolder.IsNull())
            {
                if (!callback.IsNull())
                {
                    callback(this.Files);
                }
                return;
            }

            this.RealFolder.GetFilesAsync((files) =>
            {
                if (!IsFileLoaded)
                {
                    IsFileLoaded = AddItemsByChunk(files, this.Files, this.Items);
                }

                if (!callback.IsNull())
                {
                    callback(this.Files);
                }
            });
        }
    }
}
