using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    public interface IDBTag
    {
        IList<DBTagInfo> GetTags();
        bool InsertTag(DBTagInfo tagInfo);
        int InsertPatchTags(IList<DBTagInfo> tagInfos);
    }
}
