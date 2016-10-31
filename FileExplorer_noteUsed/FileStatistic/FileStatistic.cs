using FileExplorer.Helper;
using FileExplorer.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace FileExplorer.FileStatistic
{
    public class FileStatistic : IFileStatistic
    {
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        object lockListObj = new object();
        object lockDicObj = new object();

        IList<string> pendingList = new List<string>();
        IDictionary<string, bool> allScanPathDic = new Dictionary<string, bool>();
        IDictionary<string, StatisticItem> cacheDic = new Dictionary<string, StatisticItem>();
        IDictionary<string, StatisticItem> currentScannedDic = new Dictionary<string, StatisticItem>();

        public FileStatistic()
        {
        }

        #region IFileStatistic

        public event EventHandler<FileStatisticEventArgs> ScanProgressChanged;

        protected void RaiseScanProgressChanged(bool isCompleted = false)
        {
            long fileCount, folderCount, size;
            lock (lockDicObj)
            {
                var folders = currentScannedDic.Values.Where(item => item.ItemCount > 0);
                fileCount = folders.Sum(item => item.ItemCount);
                folderCount = folders.Count();
                size = folders.Sum(item => item.Size);
            }

            if (!ScanProgressChanged.IsNull())
            {
                FileStatisticEventArgs args = new FileStatisticEventArgs(fileCount, folderCount, size, isCompleted);
                ScanProgressChanged(this, args);
            }
        }

        public void Add(bool isFolder, params string[] newPaths)
        {
            if (newPaths.IsNullOrEmpty())
            {
                return;
            }

            var lowerPaths = newPaths.Where(item => !item.IsNullOrEmpty()).Select(item => item);

            foreach (var item in lowerPaths)
            {
                lock (lockListObj)
                {
                    IList<string> keys = GetMatchPaths(item, allScanPathDic.Keys, isFolder);
                    foreach (var key in keys)
                    {
                        //allScanPathDic.Remove(key);
                        allScanPathDic[key] = true;
                    }

                    if (isFolder)
                    {
                        string[] parts = item.Split(Path.DirectorySeparatorChar);
                        string tempPath = string.Empty;
                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (i == 0)
                            {
                                tempPath += parts[i] + Path.DirectorySeparatorChar;
                            }
                            else
                            {
                                tempPath = Path.Combine(tempPath, parts[i]);
                            }

                            allScanPathDic[tempPath] = true;
                        }
                    }

                    //LogHelper.DebugFormat("/++++ FileStatistic add path :{0}", item);
                }

                lock (lockDicObj)
                {
                    var folderItem = GetCacheItem(item, cacheDic, isFolder);
                    if (!pendingList.Contains(item) &&
                        (folderItem.IsNull() || (!folderItem.IsNull() && !folderItem.IsCompleted)))
                    {
                        lock (lockListObj)
                        {
                            pendingList.Add(item);
                        }
                    }

                    IList<string> keys = GetMatchPaths(item, cacheDic.Keys, isFolder);
                    foreach (var key in keys)
                    {
                        if (cacheDic[key].IsCompleted)
                        {
                            currentScannedDic[key] = cacheDic[key];
                        }
                        else
                        {
                            ///If item is not completed
                            ///do scan again
                            lock (lockListObj)
                            {
                                pendingList.Add(key);
                            }
                        }
                    }
                }
            }

            bool isEmpty = false;
            lock (lockListObj)
            {
                isEmpty = pendingList.IsNullOrEmpty();
            }

            if (isEmpty)
            {
                RaiseScanProgressChanged(true);
            }
            else
            {
                RaiseScanProgressChanged();
            }

            StartScanThread();
        }

        private IList<string> GetMatchPaths(string path, IEnumerable<string> pathList, bool isFolder)
        {
            IList<string> result = new List<string>();
            if (pathList.IsNullOrEmpty() || path.IsNullOrEmpty())
            {
                return result;
            }

            if (isFolder)
            {
                string folderPath = GetPathWithDirSeparatorChar(path);
                result = pathList.Where(item => item == path || item.StartsWith(folderPath)).ToList();
            }
            else
            {
                result = pathList.Where(item => item == path).ToList();
            }
            return result;
        }

        private StatisticItem GetCacheItem(string path, IDictionary<string, StatisticItem> dic, bool isFolder)
        {
            StatisticItem result = null;
            if (path.IsNullOrEmpty() || dic.IsNullOrEmpty())
            {
                return result;
            }

            dic.TryGetValue(path, out result);
            if (isFolder && result.IsNull())
            {
                string folderPath = GetPathWithDirSeparatorChar(path);
                dic.TryGetValue(folderPath, out result);
            }
            return result;
        }

        public void Remove(bool isFolder, params string[] pathList)
        {
            if (pathList.IsNullOrEmpty())
            {
                return;
            }

            var lowerPaths = pathList.Where(item => !item.IsNullOrEmpty()).Select(item => item);

            foreach (var path in lowerPaths)
            {
                lock (lockListObj)
                {
                    var matchPaths = GetMatchPaths(path, pendingList, isFolder);
                    foreach (var item in matchPaths)
                    {
                        pendingList.Remove(item);
                    }

                    allScanPathDic[path] = false;
                    var keys = GetMatchPaths(path, allScanPathDic.Keys, isFolder);
                    foreach (var item in keys)
                    {
                        allScanPathDic[item] = false;
                        //LogHelper.DebugFormat("/----- FileStatistic.Remove() canceled path :{0}", item);
                    }
                }

                lock (lockDicObj)
                {
                    if (isFolder)
                    {
                        var keys = GetMatchPaths(path, currentScannedDic.Keys, isFolder);
                        foreach (var key in keys)
                        {
                            currentScannedDic.Remove(key);
                        }
                    }
                    else
                    {
                        string folderPath = Path.GetDirectoryName(path);
                        StatisticItem folderItem = GetCacheItem(folderPath, cacheDic, true);
                        if (!folderItem.IsNull())
                        {
                            try
                            {
                                FileInfo fi = new FileInfo(path);
                                folderItem.ItemCount--;
                                folderItem.Size -= fi.Length;
                            }
                            catch (Exception ex)
                            {
                                LogHelper.DebugFormat("FileInfo exception:{0}", ex.Message);
                            }
                        }
                    }
                }
            }

            bool isEmpty = false;
            lock (lockListObj)
            {
                isEmpty = pendingList.IsNullOrEmpty();
            }
            RaiseScanProgressChanged(isEmpty);
        }

        bool isCanceled = false;
        public void Cancel()
        {
            isCanceled = true;
            lock (lockListObj)
            {
                pendingList.Clear();
                allScanPathDic.Clear();
            }

            lock (lockDicObj)
            {
                currentScannedDic.Clear();
            }
            resetEvent.Set();
        }

        public void Reset()
        {
            Cancel();
            lock (lockDicObj)
            {
                cacheDic.Clear();
            }
            isCanceled = false;
        }

        #endregion IFileStatistic

        #region Scan files

        bool isRunning = true;
        Thread thread = null;

        private void StartScanThread()
        {
            if (!thread.IsNull())
            {
                resetEvent.Set();
                return;
            }

            lock (this)
            {
                if (thread.IsNull())
                {
                    thread = new Thread(Scan);
                    thread.Name = "File_statistic_thread";
                    thread.IsBackground = true;
                    thread.Start();
                }
                resetEvent.Set();
            }
        }

        private void Scan()
        {
            while (isRunning)
            {
                string path = string.Empty;
                lock (lockListObj)
                {
                    if (pendingList.Count > 0)
                    {
                        path = pendingList[0];
                        pendingList.RemoveAt(0);
                    }
                }

                if (path.IsNullOrEmpty())
                {
                    RaiseScanProgressChanged(true);
                    resetEvent.WaitOne();
                    continue;
                }

                RaiseScanProgressChanged();
                Scan(path);
                RemoveCanceldItems(path);
                RaiseScanProgressChanged();
            }
        }

        private void RemoveCanceldItems(string path)
        {
            if (path.IsNullOrEmpty())
            {
                return;
            }

            bool isFolderCanceled = GetFolderIsCanceled(path);

            if (isCanceled || isFolderCanceled)
            {
                IList<string> keys = null;
                lock (lockDicObj)
                {
                    keys = GetMatchPaths(path, cacheDic.Keys, true);
                    foreach (var item in keys)
                    {
                        if (cacheDic.ContainsKey(item) && !cacheDic[item].IsCompleted)
                        {
                            cacheDic.Remove(item);
                        }

                        if (currentScannedDic.ContainsKey(item))
                        {
                            currentScannedDic.Remove(item);
                        }
                    }
                }

                if (!keys.IsNullOrEmpty())
                {
                    lock (lockListObj)
                    {
                        foreach (var item in keys)
                        {
                            if (allScanPathDic.ContainsKey(item))
                            {
                                allScanPathDic.Remove(item);
                            }
                        }
                    }
                }

                // LogHelper.DebugFormat("/***** FileStatistic.Thread_Scan() canceled in thread path :{0}", path);
            }
        }

        private void Scan(string path)
        {
            if (path.IsNullOrEmpty() || isCanceled)
            {
                return;
            }

            if (File.Exists(path))
            {
                GetFileInfo(path);
            }
            else if (Directory.Exists(path))
            {
                GetFolderInfo(path);
            }
        }

        private void GetFileInfo(string path)
        {
            if (path.IsNullOrEmpty() || isCanceled)
            {
                return;
            }

            FileInfo fi = null;
            try
            {
                fi = new FileInfo(path);
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("FileInfo exception:{0}", ex.Message);
                return;
            }

            if (fi.IsNull() || CheckIsFileIgnored(fi))
            {
                return;
            }
            AddFileItem(fi);
        }

        private void GetFolderInfo(string path)
        {
            if (path.IsNullOrEmpty() || LocalFolder.GetIsExcludeFolder(path) || isCanceled)
            {
                return;
            }

            DirectoryInfo di = null;
            try
            {
                di = new DirectoryInfo(path);
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("DirectoryInfo exception:{0}", ex.Message);
                return;
            }

            GetFolderInfo(di);
        }

        private StatisticItem GetFolderInfo(DirectoryInfo dirInfo)
        {
            StatisticItem result = null;
            string folderPath = string.Empty;
            try
            {
                if (dirInfo.IsNull() ||
                    //Hard driver will be ignored by Attributes
                   (dirInfo.Root.FullName != dirInfo.FullName &&
                   (LocalFolder.GetIsExcludeFolder(dirInfo.FullName) || LocalFolder.GetIsExcludeAttribute(dirInfo.Attributes))) ||
                    isCanceled)
                {
                    return result;
                }
                folderPath = dirInfo.FullName;
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("GetFiles exception:{0},path={1}", ex.Message, dirInfo.Name);
                return result;
            }

            StatisticItem folderItem = null;
            lock (this.lockDicObj)
            {
                folderItem = GetCacheItem(folderPath, cacheDic, true);
            }

            if (!folderItem.IsNull() && folderItem.IsCompleted)
            {
                return folderItem;
            }

            folderItem = new StatisticItem();
            folderItem.Path = folderPath;
            AddToCache(folderItem);

            bool isFolderCanceled = GetFolderIsCanceled(folderPath);
            FileInfo[] files = null;
            try
            {
                files = dirInfo.GetFiles();
                if (!files.IsNullOrEmpty() &&
                    (folderItem.IsNull() || (!folderItem.IsNull() && !folderItem.IsCompleted)))
                {
                    foreach (var fi in files)
                    {
                        if (isCanceled || isFolderCanceled)
                        {
                            return result;
                        }

                        if (fi.IsNull() || CheckIsFileIgnored(fi))
                        {
                            continue;
                        }

                        fileScannCount++;
                        folderItem.ItemCount++;
                        folderItem.Size += fi.Length;
                        if (ReportProgressForFolder(folderPath))
                        {
                            isFolderCanceled = GetFolderIsCanceled(folderPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("GetFiles exception:{0}", ex.Message);
            }

            DirectoryInfo[] folders = null;
            try
            {
                folders = dirInfo.GetDirectories();
                if (!folders.IsNullOrEmpty())
                {
                    foreach (var f in folders)
                    {
                        try
                        {
                            ///Folder too long path
                            isFolderCanceled = GetFolderIsCanceled(f.FullName);
                            ///Check is cancel for every folder
                            if (isCanceled)
                            {
                                return folderItem;
                            }
                            if (isFolderCanceled)
                            {
                                continue;
                            }
                            StatisticItem subItem = GetFolderInfo(f);
                        }
                        catch (Exception ex)
                        {
                            LogHelper.DebugFormat("GetDirectories exception:{0}", ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("GetDirectories exception:{0}", ex.Message);
            }

            if (!folderItem.IsNull())
            {
                isFolderCanceled = GetAllFoldersIsCanceled(folderPath);
                if (!isFolderCanceled)
                {
                    folderItem.IsCompleted = true;
                }
                result = folderItem;
            }
            return folderItem;
        }

        private void AddFileItem(FileInfo fi)
        {
            if (fi.IsNull())
            {
                return;
            }

            try
            {
                string folderPath = fi.Directory.FullName;
                StatisticItem folderItem;
                lock (lockDicObj)
                {
                    cacheDic.TryGetValue(folderPath, out folderItem);
                }

                if (folderItem.IsNull())
                {
                    folderItem = new StatisticItem();
                    folderItem.Path = folderPath;
                }

                lock (lockListObj)
                {
                    bool isScanEnabled = false;
                    bool isContained = allScanPathDic.TryGetValue(folderPath, out isScanEnabled);
                    if (!isScanEnabled || folderItem.IsCompleted)
                    {
                        ///folderItem.IsCompleted
                        ///BIUPC-2222: If folder is statisticed already, then to add the sub files
                        ///this will be duplicate operation
                        allScanPathDic[folderPath] = true;
                        folderItem.ItemCount = 0;
                        folderItem.Size = 0;
                    }
                }
                folderItem.IsCompleted = false;
                folderItem.ItemCount++;
                folderItem.Size += fi.Length;

                AddToCache(folderItem);
            }
            catch (Exception ex)
            {
                LogHelper.DebugFormat("FileInfo exception:{0}", ex.Message);
            }
        }

        FileAttributes ignoredAttrs = FileAttributes.Hidden | FileAttributes.System | FileAttributes.Temporary;
        private bool CheckIsFileIgnored(FileInfo fi)
        {
            bool result = false;
            if (!fi.IsNull() && (fi.Attributes & ignoredAttrs) > 0)
            {
                result = true;
            }

            return result;
        }

        private void AddToCache(StatisticItem item)
        {
            if (item.IsNull())
            {
                return;
            }

            lock (lockDicObj)
            {
                cacheDic[item.Path] = item;
                currentScannedDic[item.Path] = item;
            }
        }

        int fileScannCount = 0;
        const int refreshCapacity = 10000;

        private bool ReportProgressForFolder(string folderPath)
        {
            bool result = false;
            ///Report progress changed ,but control the frequency
            if (fileScannCount > refreshCapacity && !folderPath.IsNullOrEmpty())
            {
                RaiseScanProgressChanged();
                fileScannCount = 0;
                result = true;
            }
            return result;
        }

        private bool GetFolderIsCanceled(string path)
        {
            bool result = false;
            if (path.IsNullOrEmpty())
            {
                return result;
            }

            ///Check is cancel for every folder
            lock (lockListObj)
            {
                if (allScanPathDic.IsNullOrEmpty())
                {
                    result = true;
                }

                string folderPath = GetPathWithDirSeparatorChar(path);

                foreach (var kv in allScanPathDic)
                {
                    if (kv.Value)
                    {
                        continue;
                    }

                    if (kv.Key == path)
                    {
                        result = !kv.Value;
                        break;
                    }
                    else
                    {
                        string tempPath = GetPathWithDirSeparatorChar(kv.Key);
                        //if parent is canceled
                        if (folderPath.StartsWith(tempPath) && !kv.Value)
                        {
                            result = !kv.Value;
                            break;
                        }
                    }
                }
            }
            //LogHelper.DebugFormat("GetIsCanceled path:{0}, isCanceled:{1} --/", path, result);
            ///Maybe is canceled , the cache is cleared
            ///so add isCanceled
            return result || isCanceled;
        }

        private string GetPathWithDirSeparatorChar(string path)
        {
            string result = path;
            if (result.IsNullOrEmpty() || result.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                return result;
            }
            result = result + Path.DirectorySeparatorChar;
            return result;
        }

        private bool GetAllFoldersIsCanceled(string path)
        {
            bool isFolderCanceled = false;
            bool isSubFolderCanceled = false;

            if (path.IsNullOrEmpty())
            {
                return isFolderCanceled;
            }

            ///Check is cancel for every folder
            lock (lockListObj)
            {
                if (allScanPathDic.IsNullOrEmpty())
                {
                    isFolderCanceled = true;
                    isSubFolderCanceled = true;
                }

                string folderPath = GetPathWithDirSeparatorChar(path);

                foreach (var kv in allScanPathDic)
                {
                    if (kv.Value)
                    {
                        continue;
                    }
                    if (kv.Key == path)
                    {
                        isFolderCanceled = !kv.Value;
                    }
                    else
                    {
                        string tempPath = GetPathWithDirSeparatorChar(kv.Key);
                        ////if parent or children is canceled
                        if (folderPath.StartsWith(tempPath) || tempPath.StartsWith(folderPath))
                        {
                            isSubFolderCanceled = !kv.Value;
                            break;
                        }
                    }
                }
            }
            //LogHelper.DebugFormat("GetIsCanceled path:{0}, isCanceled:{1} --/", path, isFolderCanceled);
            ///Maybe is canceled , the cache is cleared
            ///so add isCanceled
            if (isSubFolderCanceled)
            {
                return isSubFolderCanceled || isCanceled;
            }
            return isFolderCanceled || isCanceled;
        }

        #endregion

        #region IDisposable

        protected void OnDisposing(bool isDisposed)
        {
            Reset();
            isRunning = false;
            resetEvent.Set();
            //resetEvent.Dispose();
            thread = null;
        }

        public void Dispose()
        {
            OnDisposing(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable
    }
}
