using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Behaviors;
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

namespace MediaCenter.Modules.Footer
{
    /// <summary>
    /// FooterView.xaml 的交互逻辑
    /// </summary>
    [ViewExport(RegionName = RegionNames.FooterRegion)]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public partial class FooterView : UserControl
    {
        [Import]
        FooterViewModel ViewModel
        {
            get
            {
                return this.DataContext as FooterViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public FooterView()
        {
            InitializeComponent();
        }
    }
}
