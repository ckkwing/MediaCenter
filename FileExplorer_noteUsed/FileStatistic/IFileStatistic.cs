using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer.FileStatistic
{
    public class FileStatisticEventArgs : EventArgs
    {
        public bool IsCompleted { get; protected set; }
        /// <summary>
        /// Total folder count
        /// </summary>
        public long FolderCount { get; protected set; }

        /// <summary>
        /// Total file count
        /// </summary>
        public long FileCount { get; protected set; }

        /// <summary>
        /// Total file size(unit:KB)
        /// </summary>
        public long Size { get; protected set; }

        public FileStatisticEventArgs(long fileCount, long folderCount, long size, bool isCompleted = false)
        {
            FileCount = fileCount;
            FolderCount = folderCount;
            Size = size;
            IsCompleted = isCompleted;
        }
    }

    public interface IFileStatistic : IDisposable
    {
        event EventHandler<FileStatisticEventArgs> ScanProgressChanged;

        /// <summary>
        /// Add new file path to statistic 
        /// </summary>
        /// <param name="newPaths"></param>
        void Add(bool isFolder, params string[] newPaths);

        /// <summary>
        /// Remove pending file path
        /// </summary>
        /// <param name="newPaths"></param>
        void Remove(bool isFolder, params string[] newPaths);

        /// <summary>
        /// Cancel file scan
        /// </summary>
        void Cancel();

        /// <summary>
        /// Reset file scan, cancel all the pending scan and clean the cache
        /// </summary>
        void Reset();
    }
}
