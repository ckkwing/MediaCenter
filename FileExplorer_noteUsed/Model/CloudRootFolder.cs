using BIUCore;
using BIUCore.Cloud;
using BIUCore.Models;
using FileExplorer.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Utilities.Common;

namespace FileExplorer.Model
{
    class CloudRootFolder : CloudFolder
    {
        private IMyCloud onlineCloud;
        public CloudRootFolder(OnlineTarget onlineTarget)
        {
            switch (onlineTarget)
            {
                case OnlineTarget.NeroCloud:
                    {
                        onlineCloud = DataManager.MyCloudInstance;
                        this.Title = ResourceProvider.LoadString("IDS_ONLINEJOB_RESTORE_ROOT_NAME");
                        this.Icon = ResourceProvider.LoadImageSource("IMG_CLOUDTREEICON_NERO");
                    }
                    break;
                case OnlineTarget.GoogleDrive:
                    {
                        onlineCloud = DataManager.MyCloudInstance.GoogleCloud;
                        this.Title = ResourceProvider.LoadString("IDS_ONLINEJOB_RESTORE_ROOT_NAME_GOOGLE");
                        this.Icon = ResourceProvider.LoadImageSource("IMG_CLOUDTREEICON_GOOGLE");
                    }
                    break;
                case OnlineTarget.AmazonCloud:
                    {
                        onlineCloud = DataManager.MyCloudInstance.AmazonCloud;
                        this.Title = ResourceProvider.LoadString("IDS_ONLINEJOB_RESTORE_ROOT_NAME_AMAZON");
                        this.Icon = ResourceProvider.LoadImageSource("IMG_CLOUDTREEICON_AMAZON");
                    }
                    break;
            }
            if (null == onlineCloud)
                throw new ArgumentNullException(onlineTarget.ToString() + " IMyCloud is NULL!");
            CloudItem = onlineCloud.RootFolder;

            this.IsExpanded = true;
            this.IsSelected = true;
            this.IsChecked = true;
            this.Parent = this;
            this.FullPath = Path.DirectorySeparatorChar.ToString();
        }

        public override string FolderPath
        {
            get
            {
                return Path.DirectorySeparatorChar.ToString();
            }
        }

        public override void GetChildrenAsync(Action<IEnumerable<IFile>> callback)
        {
            Action action = () =>
            {
                if (!Utility.IsConnectedToInternet())
                {
                    return;
                }

                bool isSyncMode = onlineCloud.CheckAndRefreshSubscriptionSyncMode();
                if (!onlineCloud.IsSignedIn)
                {
                    LogHelper.Debug("OnlineFileExplorerDataFactory, Is SignedIn false, return empty list");
                }

                ///Check is cloud root folder is loaded
                int i = 0;
                while (i++ < 10)
                {
                    if (CloudItem.IsNull())
                    {
                        Thread.Sleep(2 * 1000);
                    }

                    CloudItem = onlineCloud.RootFolder;
                    if (!CloudItem.IsNull())
                    {
                        break;
                    }
                }
            };

            action.BeginInvoke((ar) =>
            {
                action.EndInvoke(ar);
                base.GetChildrenAsync(callback);
            }, action);
        }
    }
}
