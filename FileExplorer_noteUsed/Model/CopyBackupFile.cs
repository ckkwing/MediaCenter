using FileExplorer.Helper;
using System;
using System.IO;
using FS = FileExplorer.FileStatistic;

namespace FileExplorer.Model
{
    public class CopyBackupFile : LocalFile
    {
        public CopyBackupFile(string path, IFolder parent)
            : base(path, parent)
        {
           
        }

        public CopyBackupFile(FileInfo fi, long itemSize, IFolder parent)
            : base(fi.FullName, parent)
        {
            this.Size = itemSize;
        }

        public override object Clone()
        {
            CopyBackupFile file = new CopyBackupFile(this.FullPath, this.Parent);
            CloneMembers(file);
            return file;
        }
    }
}
