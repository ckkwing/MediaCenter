using FileExplorer.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FS = FileExplorer.FileStatistic;

namespace FileExplorer.Model
{
    public class CopyBackupFolder : LocalFolder
    {
        private const long SIZE_4GB = ((long)4) * 1024 * 1024 * 1024;

        protected CopyBackupFolder()
        {
        }

        public CopyBackupFolder(string path, string name, IFolder parent)
            : base(path, parent)
        {
            this.Name = name;
        }


        public CopyBackupFolder(DirectoryInfo dirInfo, string name, IFolder parent)
            : base(dirInfo.FullName, parent)
        {
            this.Name = name;
        }
        protected override IEnumerable<IFolder> GetFolders()
        {
            if (!IsFolderLoaded)
            {
                RunOnUIThread(() =>
                {
                    this.Folders.Clear();
                });

                IEnumerable<IFolder> folders = SearchFolders();
                IsFolderLoaded = AddItemsByChunk(folders, this.Folders, this.Items);
            }
            return this.Folders;
        }

        private IEnumerable<IFolder> SearchFolders(string searchPattern = searchAllWildChar)
        {
            IEnumerable<IFolder> result = new IFolder[0];

            ///Check is folder is existed before query
            ///CD-ROM will block query for a while without check existed
            try
            {
                //The specified path, file name, or both are too long. 
                //The fully qualified file name must be less than 260 characters, 
                //and the directory name must be less than 248 characters.
                if (Directory.Exists(this.FullPath) && this.FullPath.Length < folderPathMaxLen)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.FullPath);
                    result = dirInfo.GetDirectories().Where(item =>
                    {
                        try
                        {
                            return item.FullName.Length < folderPathMaxLen && (item.Attributes & ExcludeFileAttributes) == 0;
                        }
                        catch (Exception ex)
                        {
                            ///Access denied exception
                            LogHelper.Debug(ex);
                            return false;
                        }
                    }).Select(item => new CopyBackupFolder(item, item.Name, this));
                    result = SetFolderOrder(result);
                }
            }
            catch (Exception ex)
            {
                ///Access denied exception
                LogHelper.Debug(ex);
            }
            return result;
        }

        protected override IEnumerable<IFile> GetFiles()
        {
            if (!IsFileLoaded)
            {
                IEnumerable<IFile> files = SearchFiles();
                IsFileLoaded = AddItemsByChunk(files, this.Files, this.Items);
            }
            return Files;
        }

        private IEnumerable<IFile> SearchFiles(string searchPattern = searchAllWildChar)
        {
            IEnumerable<IFile> result = new IFile[0];
            try
            {
                //The specified path, file name, or both are too long. 
                //The fully qualified file name must be less than 260 characters, 
                //and the directory name must be less than 248 characters.
                if (Directory.Exists(this.FullPath) && this.FullPath.Length < folderPathMaxLen)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(this.FullPath);
                    result = dirInfo.GetFiles().Where(item =>
                    {
                        try
                        {
                            bool ret = item.FullName.Length < filePathMaxLen && (item.Attributes & ExcludeFileAttributes) == 0;
                            if (ret)
                            {
                                if (item.Extension.StartsWith(".nbiu"))
                                {
                                    ret = false;
                                }
                            }
                            return ret;
                        }
                        catch (Exception ex)
                        {
                            ///Access denied exception
                            LogHelper.Debug(ex);
                            return false;
                        }
                    }).Select(item => 
                    {
                        long len = item.Length;
                        if (item.Length >= SIZE_4GB)
                        {
                            len = GetSplitFileSize(item.FullName);
                        }
                        return new CopyBackupFile(item, len, this);
                    }
                    );

                    result = SetFolderOrder(result);
                }
            }
            catch (Exception ex)
            {
                ///Access denied exception
                LogHelper.Debug(ex);
            }
            return result;
        }

        private long GetSplitFileSize(string filePath)
        {
            long len = 0;
            IList<string> splitFileList = new List<string>();
            string fileDir = Path.GetDirectoryName(filePath);
            string fileName = Path.GetFileName(filePath);
            if (File.Exists(filePath))
            {
                FileInfo f = new FileInfo(filePath);
                len += f.Length;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(fileDir);
            string searchPattern = fileName + ".nbiu*";
            FileInfo[] splitFiles = dirInfo.GetFiles(searchPattern);
            if (splitFiles != null && splitFiles.Length > 0)
            {
                foreach (FileInfo f in splitFiles)
                {
                    len += f.Length;
                }
            }

            return len;
        }
    }
}
