using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Model
{
    public class MediaInfo
    {
        public enum MediaType
        {
            Video = 0x0001,
            Music = 0x0002,
            Document = 0x0004,
            Image = 0x0008,
        }
    }
}
