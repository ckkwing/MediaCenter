using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using MediaCenter.Infrastructure;
using MediaCenter.Infrastructure.Behaviors;
using Microsoft.Practices.Prism.PubSubEvents;
using MediaCenter.Infrastructure.Event;

namespace MediaCenter.Modules.Header
{
    /// <summary>
    /// Interaction logic for HeaderView.xaml
    /// </summary>
    //[Export]

    [ViewExport(RegionName = RegionNames.HeaderRegion)]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class HeaderView : UserControl
    {
        [Import]
        public IEventAggregator EventAggregator;

        [Import]
        HeaderViewModel ViewModel
        {
            get
            {
                return this.DataContext as HeaderViewModel;
            }
            set
            {
                this.DataContext = value;
            }
        }

        public HeaderView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.EventAggregator.GetEvent<SettingsEvent>().Publish(new SettingEventArgs(SettingType.FolderManager, null));
        }
    }
}
