using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MediaCenter.Theme.CustomControl
{
    public class BasicWindow : Window, INotifyPropertyChanged
    {
        public Visibility CanMinimize
        {
            get { return (Visibility)GetValue(CanMinimizeProperty); }
            set { SetValue(CanMinimizeProperty, value); }
        }

        public static readonly DependencyProperty CanMinimizeProperty =
            DependencyProperty.Register("CanMinimize", typeof(Visibility),
            typeof(BasicWindow), new UIPropertyMetadata(Visibility.Visible));

        public Visibility CanClose
        {
            get { return (Visibility)GetValue(CanCloseProperty); }
            set { SetValue(CanCloseProperty, value); }
        }

        public static readonly DependencyProperty CanCloseProperty =
            DependencyProperty.Register("CanClose", typeof(Visibility),
            typeof(BasicWindow), new UIPropertyMetadata(Visibility.Visible));

        public Brush TitlebarBackgroundBrush
        {
            get { return (Brush)GetValue(TitlebarBackgroundBrushProperty); }
            set { SetValue(TitlebarBackgroundBrushProperty, value); }
        }

        public static readonly DependencyProperty TitlebarBackgroundBrushProperty =
            DependencyProperty.Register("TitlebarBackgroundBrush", typeof(Brush),
             typeof(BasicWindow), new UIPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString(@"#3cbcb7"))));

        #region Command
        public ICommand CloseWindowCommand
        {
            get;
            set;
        }

        public ICommand MinimizeWindowCommand
        {
            get;
            set;
        }
        #endregion

        public BasicWindow()
        {
            CloseWindowCommand = new DelegateCommand(OnClose, CanExcute);
            MinimizeWindowCommand = new DelegateCommand(OnMinimize, CanExcute);
            //Load Style
            InitializeStyle();

            //Load event delegate
            this.Loaded += delegate { InitializeEvent(); };
        }

        public BasicWindow(Window owner)
            :this()
        {
            SetOwner(owner);
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

    protected virtual void InitializeStyle()
        {
            if (null == Application.Current)
                return;
            if (!Application.Current.Resources.Contains("CommonBaseWindowStyle"))
                return;

            this.Style = (Style)Application.Current.Resources["CommonBaseWindowStyle"];
        }

        private void InitializeEvent()
        {
            try
            {
                ControlTemplate baseWindowTemplate = (ControlTemplate)Application.Current.Resources["CommonBaseWindowControlTemplate"];
                Border tp = (Border)baseWindowTemplate.FindName("topborder", this);
                if (null == tp)
                    return;

                tp.MouseLeftButtonDown += delegate
                {
                    try
                    {
                        this.DragMove();
                    }
                    catch (InvalidOperationException e)
                    {
                        NLogger.LogHelper.UILogger.DebugFormat("DragMove exception: {0}", e.Message);
                    }
                };
            }
            catch (Exception ex)
            {
                NLogger.LogHelper.UILogger.DebugFormat("InitializeEvent exception: {0}", ex.Message);
            }
        }

        virtual protected void OnClose()
        {
            this.Close();
        }

        virtual protected void OnMinimize()
        {
            this.WindowState = WindowState.Minimized;
        }

        protected bool CanExcute()
        {
            return true;
        }

        protected bool CanExcute(object parameter)
        {
            return true;
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
