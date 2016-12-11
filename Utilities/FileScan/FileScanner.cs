using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.FileScan
{
    public delegate void FileScannerProcessEventHandler(object sender, FileScannerProcessEventArgs e);
    public class FileScanner
    {
        private readonly IList<string> defaultMusicExtension = new List<string>(StandardFileExtensions.GetAudioExtensions());
        private readonly IList<string> defaultVideoExtension = new List<string>(StandardFileExtensions.GetVideoExtensions());
        private readonly IList<string> defaultPhotoExtension = new List<string>(StandardFileExtensions.GetImageExtensions());

        public class FileScannerConfiguration
        {
            private IList<string> pathsToScan = new List<string>();

            public IList<string> PathsToScan
            {
                get
                {
                    return pathsToScan;
                }

                set
                {
                    pathsToScan = value;
                }
            }
        }

        private FileScannerConfiguration config = new FileScannerConfiguration();
        public FileScannerConfiguration Config
        {
            get
            {
                return config;
            }

            set
            {
                config = value;
            }
        }

        private bool isCancel = false;
        public bool IsCancel
        {
            get
            {
                return isCancel;
            }

            private set
            {
                isCancel = value;
            }
        }

        private List<FileInfo> filesInDirectory = new List<FileInfo>();
        public List<FileInfo> FilesInDirectory
        {
            get
            {
                return filesInDirectory;
            }

            private set
            {
                filesInDirectory = value;
            }
        }

        #region Event

        public event FileScannerProcessEventHandler ProcessEvent;

        #endregion

        public FileScanner() { }

        public FileScanner(FileScannerConfiguration config)
        {
            this.Config = config;
        }

        private void Reset()
        {
            FilesInDirectory.Clear();
        }

        private void NotifyEvent(ProcessType processType)
        {
            ProcessEvent?.Invoke(this, new FileScannerProcessEventArgs(processType));
        }

        private void NotifyEvent(FileScannerProcessEventArgs args)
        {
            ProcessEvent?.Invoke(this, args);
        }

        public void StartAsync()
        {
            Reset();
            NLogger.LogHelper.UILogger.Debug("FileScanner async start");
            NotifyEvent(ProcessType.Start);
            Thread runnerThread = new Thread(StartScanAsync);
            runnerThread.IsBackground = true;
            runnerThread.Name = "File Scanner Thread + ";
            runnerThread.Start();
        }

        public void StartSync()
        {
            Reset();
            NLogger.LogHelper.UILogger.Debug("FileScanner sync start");
            GetFiles();
            NLogger.LogHelper.UILogger.Debug("FileScanner sync end");
        }

        public void Stop()
        {
            IsCancel = true;
            NotifyEvent(ProcessType.Cancelling);
            NLogger.LogHelper.UILogger.Debug("FileScanner async cancelling...");
        }

        private async void StartScanAsync()
        {
            NotifyEvent(ProcessType.InProcess);
            await GetFiles();
            NotifyEvent(ProcessType.End);
            NLogger.LogHelper.UILogger.Debug("FileScanner async end");
        }

        private bool CheckMediaType(FileInfo fileInfo)
        {
            bool bRel = true;
            if (null == fileInfo)
                return false;
            
            string ext = fileInfo.Extension.ToUpper();
            if (defaultPhotoExtension.Contains(ext))
            {
                //ImageCount++;
            }
            else if (defaultVideoExtension.Contains(ext))
            {
                //VideoCount++;
            }
            else if (defaultMusicExtension.Contains(ext))
            {
                //MusicCount++;
            }
            else
            {
                //bRel = false;
            }

            return bRel;
        }

        private Task<List<FileInfo>> GetFiles()
        {
            foreach (string pathToScan in Config.PathsToScan)
            {
                if (IsCancel)
                    break;
                if (string.IsNullOrEmpty(pathToScan))
                    continue;
                DirectoryInfo directoryInfo = new DirectoryInfo(pathToScan);
                if (null == directoryInfo || !directoryInfo.Exists)
                    continue;
                IList<FileInfo> searchedFiles = new List<FileInfo>();
                ScanFilesInDirectory(directoryInfo, ref searchedFiles);
                FilesInDirectory.AddRange(searchedFiles);
                NotifyEvent(new FileScannerProcessEventArgs(ProcessType.InProcess) { CurrentDir = directoryInfo, Files = searchedFiles });
            }

            return Task.Run(() =>
            {
                return FilesInDirectory;
            });
        }

        private void ScanFilesInDirectory(DirectoryInfo directoryInfo, ref IList<FileInfo> searchedFiles)
        {
            if (null == directoryInfo || !directoryInfo.Exists)
                return;

            try
            {
                FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
                for (int i = 0; i < fileSystemInfos.Length; i++)
                {
                    if (IsCancel)
                        break;
                    FileSystemInfo fileSystemInfo = fileSystemInfos[i];
                    if (null == fileSystemInfo)
                        continue;
                    if (fileSystemInfo is DirectoryInfo)
                    {
                        ScanFilesInDirectory(fileSystemInfo as DirectoryInfo, ref searchedFiles);
                    }
                    else
                    {
                        FileInfo fileInfo = fileSystemInfo as FileInfo;
                        if (null != fileInfo)
                        {
                            if (CheckMediaType(fileInfo))
                                searchedFiles.Add(fileInfo);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                NLogger.LogHelper.UILogger.Debug("ScanFilesInDirectory", e);
            }
        }
    }
}
