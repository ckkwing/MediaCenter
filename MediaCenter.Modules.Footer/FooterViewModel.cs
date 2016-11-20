using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using MediaCenter.Infrastructure.Core.Model;
using Utilities.Extension;

namespace MediaCenter.Modules.Footer
{
    [Export(typeof(FooterViewModel))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class FooterViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;

        private string contentString = "Footer";
        public string ContentString
        {
            get
            {
                return contentString;
            }

            set
            {
                contentString = value;
                OnPropertyChanged("ContentString");
            }
        }

        [ImportingConstructor]
        public FooterViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
            this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Subscribe(OnLoadDataCompleted, ThreadOption.UIThread);
        }

        ~FooterViewModel()
        {
            this.eventAggregator.GetEvent<LoadDataCompletedEvent>().Unsubscribe(OnLoadDataCompleted);
        }

        private void OnLoadDataCompleted(LoadMediasJob loadMediasJob)
        {
            if (loadMediasJob.IsNull())
                return;
            this.ContentString = string.Format("Included {0} files", loadMediasJob.Files.Count);
        }
    }
}
