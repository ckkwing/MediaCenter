using IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.Model;
using System.Data.SQLite;
using DBUtility;
using System.Data;

namespace SQLiteDAL
{
    public class Tag : ITag
    {
        public IList<TagInfo> GetTags()
        {
            IList<TagInfo> tagList = new List<TagInfo>();
            string sqlSelect = "SELECT * FROM " + DataAccess.TABLE_NAME_TAG;
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                int id = dr.GetInt32(0);
                string name = dr.GetString(1);
                TagInfo tagInfo = new TagInfo() { ID = id, Name = name };
                tagList.Add(tagInfo);
            }
            dr.Close();
            return tagList;
        }

        public bool InsertTag(TagInfo tagInfo)
        {
            bool bRel = false;
            if (null == tagInfo)
                return bRel;
            string sqlInsertFormat = "INSERT INTO {0} (ID, VALUE) VALUES (NULL, @VALUE); " + DataAccess.SQL_SELECT_ID_LAST;
            string sqlInsert = string.Format(sqlInsertFormat, DataAccess.TABLE_NAME_TAG);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@VALUE", DbType.String)
                    };
            parms[0].Value = tagInfo.Name;
            object objRel = SqliteHelper.ExecuteScalar(DataAccess.ConnectionStringProfile, CommandType.Text, sqlInsert, parms);
            if (null != objRel)
            {
                int id = Convert.ToInt32(objRel);
                tagInfo.ID = id;
                bRel = true;
            }
            return bRel;
        }
    }
}
