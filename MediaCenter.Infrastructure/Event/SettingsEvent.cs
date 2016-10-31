using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Event
{
    public enum SettingType
    {
        FolderManager,
        SetTags,
    }

    public class SettingEventArgs : EventArgs
    {
        private object content = null;
        public object Content
        {
            get
            {
                return content;
            }

            private set
            {
                content = value;
            }
        }

        private SettingType settingType = SettingType.FolderManager;
        public SettingType SettingType
        {
            get
            {
                return settingType;
            }

            set
            {
                settingType = value;
            }
        }

        public SettingEventArgs(SettingType settingType, object obj)
        {
            this.SettingType = settingType;
            this.Content = obj;
        }
    }

    public class SettingsEvent : PubSubEvent<SettingEventArgs>
    {
    }
}
