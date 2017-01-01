using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.FileScan;
using static Utilities.StandardFileExtensions;
using Utilities;

namespace IDAL.Model
{
    public class MonitoredFile : BindableBase
    {
        protected int id = -1;
        public int ID
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
                OnPropertyChanged("ID");
            }
        }

        protected string path = string.Empty;
        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
                OnPropertyChanged("Path");
            }
        }

        protected string name = string.Empty;
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private string extension = string.Empty;
        public string Extension
        {
            get
            {
                return extension;
            }

            set
            {
                extension = value;
                OnPropertyChanged("Extension");
            }
        }

        private FileCategory category = FileCategory.Unknown;
        public FileCategory Category
        {
            get
            {
                return category;
            }

            set
            {
                category = value;
                OnPropertyChanged("Category");
            }
        }

        protected string tag = string.Empty;
        public string Tag
        {
            get
            {
                return tag;
            }

            set
            {
                tag = value;
                OnPropertyChanged("Tag");
            }
        }

        private int parentID = -1;
        public int ParentID
        {
            get
            {
                return parentID;
            }

            set
            {
                parentID = value;
                OnPropertyChanged("ParentID");
            }
        }

        private bool isScanned = false;
        public bool IsScanned
        {
            get
            {
                return isScanned;
            }

            set
            {
                isScanned = value;
                OnPropertyChanged("IsScanned");
            }
        }

        public static MonitoredFile Create()
        {
            return new MonitoredFile();
        }

        public static MonitoredFile Convert(ScannedFileInfo scannedFileInfo)
        {
            if (null == scannedFileInfo || null == scannedFileInfo.File)
                return Create();

            return new MonitoredFile() { Name = scannedFileInfo.File.Name, Path = scannedFileInfo.File.FullName, Extension = scannedFileInfo.File.Extension, Category = scannedFileInfo.Category };
        }
    }
}
