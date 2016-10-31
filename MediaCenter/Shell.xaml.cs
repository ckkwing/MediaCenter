using MediaCenter.Theme.CustomControl;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MediaCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [Export]
    public partial class Shell : BasicWindow
    {
        [Import]
        public ShellViewModel ViewModel
        {
            get { return this.DataContext as ShellViewModel; }
            set { this.DataContext = value; }
        }

        public Shell()
        {
            InitializeComponent();
        }
    }
}
