using FileExplorer.Model;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL.Model
{
    public class DBFolderInfo : DBFileInfo
    {
        public new static DBFolderInfo Create()
        {
            return new DBFolderInfo();
        }

        public static DBFolderInfo Convert(IFolder iFolder)
        {
            if (null == iFolder)
                return Create();

            return new DBFolderInfo() { Name = iFolder.Name, Path = iFolder.FullPath };
        }

        public static DBFolderInfo Convert(DirectoryInfo directoryInfo)
        {
            if (null == directoryInfo)
                return Create();

            return new DBFolderInfo() { Name = directoryInfo.Name, Path = directoryInfo.FullName };
        }
    }
}
