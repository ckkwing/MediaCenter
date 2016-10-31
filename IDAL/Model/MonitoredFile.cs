using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        protected string extension = string.Empty;
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
        
        public static MonitoredFile Create()
        {
            return new MonitoredFile();
        }

        public static MonitoredFile Convert(FileInfo fileInfo)
        {
            if (null == fileInfo)
                return Create();

            return new MonitoredFile() { Name = fileInfo.Name, Path = fileInfo.FullName, Extension = fileInfo.Extension };
        }
    }
}
