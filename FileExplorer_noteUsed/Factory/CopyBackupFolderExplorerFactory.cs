using FileExplorer.Helper;
using FileExplorer.Model;
using FileExplorer.Shell;
using System;
using System.Collections.Generic;

namespace FileExplorer.Factory
{
    class CopyBackupFolderExplorerFactory : ExplorerFactoryBase
    {
        public string RootFolder { get; private set; }
        public string JobName { get; private set; }
        public CopyBackupFolderExplorerFactory(string rootFolder, string jobName)
        {
            if (rootFolder.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            RootFolder = rootFolder;
            JobName = jobName;
        }

        public override void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            IFolder[] roots = new IFolder[0];
            try
            {
                roots = new IFolder[] { new CopyBackupRootFolder(RootFolder, JobName) };
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("Load json file failed:{0}", ex.Message);
            }

            if (!callback.IsNull())
            {
                callback(roots);
            }
        }
    }
}
