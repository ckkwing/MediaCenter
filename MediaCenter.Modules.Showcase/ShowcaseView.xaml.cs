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

namespace MediaCenter.Modules.Showcase
{
    /// <summary>
    /// Interaction logic for ShowcaseView.xaml
    /// </summary>
    /// 
    [ViewExport(RegionName = RegionNames.MainRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class ShowcaseView : UserControl
    {
        [Import]
        ShowcaseViewModel ViewModel
        {
            get
            {
                return this.DataContext as ShowcaseViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public ShowcaseView()
        {
            InitializeComponent();
        }
    }
}
