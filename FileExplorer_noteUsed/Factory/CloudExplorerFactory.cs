using BIUCore.Models;
using FileExplorer.Helper;
using FileExplorer.Model;
using System;
using System.Collections.Generic;

namespace FileExplorer.Factory
{
    class CloudExplorerFactory : ExplorerFactoryBase
    {
        protected OnlineTarget onlineTarget;

        internal CloudExplorerFactory(OnlineTarget onlineTarget)
        {
            this.onlineTarget = onlineTarget;
        }

        IFolder[] roots=null;
        public override void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            Action action = () =>
            {
                 roots = new IFolder[] { new CloudRootFolder(onlineTarget) };
            };

            action.BeginInvoke((ar) =>
            {
                action.EndInvoke(ar);
                if (!callback.IsNull())
                {
                    callback(roots);
                }

            }, action);

            //IFolder[] roots = new IFolder[] { new CloudRootFolder(onlineTarget) };
            //if (!callback.IsNull())
            //{
            //    callback(roots);
            //}
        }
    }
}
