using FileExplorer.Helper;
using FileExplorer.Model;
using FileExplorer.Shell;
using NBEngineLib;
using System;
using System.Collections.Generic;

namespace FileExplorer.Factory
{
    class CDRomExplorerFactory : ExplorerFactoryBase
    {
        public string JobPath { get; private set; }

        public CDRomExplorerFactory(string jobPath)
        {
            if (jobPath.IsNullOrEmpty())
            {
                throw new ArgumentNullException();
            }
            JobPath = jobPath;
        }

        public override void GetRootFoldersAsync(Action<IEnumerable<IFolder>> callback)
        {
            IFolder[] roots = new IFolder[0];
            try
            {
                READ_DATA_TYPES types = READ_DATA_TYPES.RD_OPEN_BACKUP_FOR_RESTORE |
                                   READ_DATA_TYPES.RD_NOT_CHANGE_VERSION |
                                   READ_DATA_TYPES.RD_READ_BACKUP_TREE;

                JobData jobData = new JobData();
                NB_ERROR_CODE errorCode = jobData.Load(types, JobPath, false, true);
                if (errorCode == NB_ERROR_CODE.EC_SUCCESS)
                {
                    roots = new IFolder[] { new CDRomRootFolder(JobPath) };
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("Load cd-rom file failed:{0}", ex.Message);
            }

            if (!callback.IsNull())
            {
                callback(roots);
            }
        }
    }
}
