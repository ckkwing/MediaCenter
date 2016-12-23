using MediaCenter.Infrastructure.Event;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utilities;
using static MediaCenter.Infrastructure.Model.MediaInfo;

namespace MediaCenter.Modules.Header
{
    [Export(typeof(HeaderViewModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HeaderViewModel : ViewModelBase
    {
        private readonly IEventAggregator eventAggregator;

        private bool isImageSelected = true;
        public bool IsImageSelected
        {
            get
            {
                return isImageSelected;
            }

            set
            {
                isImageSelected = value;
                OnPropertyChanged("IsImageSelected");
                NotifyMediaFilterChanged();
            }
        }

        private bool isVideoSelected = true;
        public bool IsVideoSelected
        {
            get
            {
                return isVideoSelected;
            }

            set
            {
                isVideoSelected = value;
                OnPropertyChanged("IsVideoSelected");
                NotifyMediaFilterChanged();
            }
        }
        private bool isMusicSelected = true;
        public bool IsMusicSelected
        {
            get
            {
                return isMusicSelected;
            }

            set
            {
                isMusicSelected = value;
                OnPropertyChanged("IsMusicSelected");
                NotifyMediaFilterChanged();
            }
        }
        private bool isDocumentSelected = true;
        public bool IsDocumentSelected
        {
            get
            {
                return isDocumentSelected;
            }

            set
            {
                isDocumentSelected = value;
                OnPropertyChanged("IsDocumentSelected");
                NotifyMediaFilterChanged();
            }
        }

        #region Command
        //public ICommand FilterMediaCommand { get; private set; }

        #endregion

        [ImportingConstructor]
        public HeaderViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
        }

        private void NotifyMediaFilterChanged()
        {
            int iFilter = -1;
            IList<MediaType> selectedMedias = new List<MediaType>();
            if (IsImageSelected)
                iFilter = (-1 == iFilter) ? (int)MediaType.Image : iFilter | (int)MediaType.Image;
            if (IsVideoSelected)
                iFilter = (-1 == iFilter) ? (int)MediaType.Video : iFilter | (int)MediaType.Video;
            if (IsMusicSelected)
                iFilter = (-1 == iFilter) ? (int)MediaType.Music : iFilter | (int)MediaType.Music;
            if (IsDocumentSelected)
                iFilter = (-1 == iFilter) ? (int)MediaType.Document : iFilter | (int)MediaType.Document;
            this.eventAggregator.GetEvent<MediaFilterChangedEvent>().Publish(iFilter);            
        }
    }
}
