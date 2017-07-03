using IDAL.Model;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Modules.Showcase.Model
{
    public class UIFile : BindableBase
    {
        private bool isSelected = false;
        private DBFileInfo monitoredFile;

        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public DBFileInfo MonitoredFile
        {
            get
            {
                return monitoredFile;
            }

            set
            {
                monitoredFile = value;
                OnPropertyChanged("MonitoredFile");
            }
        }

        public UIFile(DBFileInfo monitoredFile)
        {
            this.MonitoredFile = monitoredFile;
        }
    }
}
