using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MediaCenter.Theme.CustomControl
{
    public class BasicWindow : Window, INotifyPropertyChanged
    {
        public BasicWindow()
            : base()
        {

        }

        static BasicWindow()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(BasicWindow), new FrameworkPropertyMetadata(typeof(BasicWindow)));
        }

        public BasicWindow(Window owner = null)
            : this()
        {
            SetOwner(owner);
            this.Style = Application.Current.Resources["StyleBasicWindow"] as Style;
        }

        protected void SetOwner(Window owner)
        {
            if (null == owner)
            {
                if (null != Application.Current && null != Application.Current.MainWindow)
                    this.Owner = Application.Current.MainWindow;
            }
            else
            {
                this.Owner = owner;
            }
        }
        
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
