using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Utilities.StandardFileExtensions;

namespace Utilities.FileScan
{
    public delegate void FileScannerProcessEventHandler(object sender, FileScannerProcessEventArgs e);
    public class FileScanner : BindableBase
    {
        private static readonly IList<string> defaultMusicExtension = new List<string>(StandardFileExtensions.GetAudioExtensions());
        private static readonly IList<string> defaultVideoExtension = new List<string>(StandardFileExtensions.GetVideoExtensions());
        private static readonly IList<string> defaultPhotoExtension = new List<string>(StandardFileExtensions.GetImageExtensions());
        private static readonly IList<string> defaultDocumentExtension = new List<string>(StandardFileExtensions.GetDocumentExtensions());

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

        public IList<DirectoryInfo> NewFoldersToMonitor { get; private set; } = new List<DirectoryInfo>();

        private bool isInProcess = false;
        public bool IsInProcess
        {
            get
            {
                return isInProcess;
            }

            private set
            {
                isInProcess = value;
                OnPropertyChanged("IsInProcess");
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
                OnPropertyChanged("IsCancel");
            }
        }

        private IList<ScannedFileInfo> filesInDirectory = new List<ScannedFileInfo>();
        public IList<ScannedFileInfo> FilesInDirectory
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
            NewFoldersToMonitor.Clear();
            IsCancel = false;
            IsInProcess = false;
        }

        private void NotifyEvent(ProcessType processType, InnerType innerType = InnerType.NA)
        {
            ProcessEvent?.Invoke(this, new FileScannerProcessEventArgs(processType, innerType));
        }

        private void NotifyEvent(FileScannerProcessEventArgs args)
        {
            ProcessEvent?.Invoke(this, args);
        }

        public void StartAsync()
        {
            Reset();
            NLogger.LogHelper.UILogger.Debug("FileScanner async start");
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
            NotifyEvent(ProcessType.Start);
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
            else if (defaultDocumentExtension.Contains(ext))
            {
                //DocumentCount++;
            }
            else
            {
                bRel = false;
            }

            return bRel;
        }

        private bool CheckMediaType(ScannedFileInfo scannedFile)
        {
            bool bRel = true;
            if (null == scannedFile)
                return false;
            scannedFile.Category = GetFileCategory(scannedFile.File);
            if (scannedFile.Category == FileCategory.Unknown)
                bRel = false;
            return bRel;
        }

        private FileCategory GetFileCategory(FileInfo fileInfo)
        {
            FileCategory category = FileCategory.Unknown;
            if (null == fileInfo)
                return category;

            string ext = fileInfo.Extension.ToUpper();
            if (defaultPhotoExtension.Contains(ext))
            {
                //ImageCount++;
                category = FileCategory.Image;
            }
            else if (defaultVideoExtension.Contains(ext))
            {
                //VideoCount++;
                category = FileCategory.Video;
            }
            else if (defaultMusicExtension.Contains(ext))
            {
                //MusicCount++;
                category = FileCategory.Audio;
            }
            else if (defaultDocumentExtension.Contains(ext))
            {
                //DocumentCount++;
                category = FileCategory.Document;
            }

            return category;
        }

        private Task<IList<ScannedFileInfo>> GetFiles()
        {
            IsInProcess = true;
            foreach (string pathToScan in Config.PathsToScan)
            {
                if (IsCancel)
                    break;
                if (string.IsNullOrEmpty(pathToScan))
                    continue;
                DirectoryInfo directoryInfo = new DirectoryInfo(pathToScan);
                if (null == directoryInfo || !directoryInfo.Exists)
                    continue;
                //IList<ScannedFileInfo> searchedFiles = new List<ScannedFileInfo>();
                ScanFilesInDirectory(directoryInfo, ref filesInDirectory);
                //FilesInDirectory.AddRange(searchedFiles);
            }

            return Task.Run(() =>
            {
                IsInProcess = false;
                return FilesInDirectory;
            });
        }

        private void ScanFilesInDirectory(DirectoryInfo directoryInfo, ref IList<ScannedFileInfo> searchedFiles)
        {
            if (null == directoryInfo || !directoryInfo.Exists)
                return;

            NewFoldersToMonitor.Add(directoryInfo);

            IList<ScannedFileInfo> fileList = new List<ScannedFileInfo>();
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
                            ScannedFileInfo scannedFileInfo = new ScannedFileInfo() { File = fileInfo };
                            if (CheckMediaType(scannedFileInfo))
                            {
                                //searchedFiles.Add(scannedFileInfo);
                                fileList.Add(scannedFileInfo);
                                NotifyEvent(new FileScannerProcessEventArgs(ProcessType.InProcess, InnerType.OneFileScanned) { CurrentFile = scannedFileInfo });
                            }
                        }
                    }
                }
            }
            catch(Exception e)
            {
                NLogger.LogHelper.UILogger.Debug("ScanFilesInDirectory", e);
            }
            finally
            {
                ((List<ScannedFileInfo>)searchedFiles).AddRange(fileList);
                NotifyEvent(new FileScannerProcessEventArgs(ProcessType.InProcess, InnerType.OneDirectoryScanned) { CurrentDir = directoryInfo, Files = fileList });
            }
        }
    }
}
