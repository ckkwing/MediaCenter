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
    public class DBTag : IDBTag
    {
        public IList<DBTagInfo> GetTags()
        {
            IList<DBTagInfo> tagList = new List<DBTagInfo>();
            string sqlSelect = "SELECT * FROM " + DataAccess.TABLE_NAME_TAG;
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                int id = dr.GetInt32(0);
                string name = dr.GetString(1);
                DBTagInfo tagInfo = new DBTagInfo() { ID = id, Name = name };
                tagList.Add(tagInfo);
            }
            dr.Close();
            return tagList;
        }

        public bool InsertTag(DBTagInfo tagInfo)
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

        public int InsertPatchTags(IList<DBTagInfo> tagInfos)
        {
            int iSuccessRows = 0;
            if (null == tagInfos || 0 == tagInfos.Count)
                return iSuccessRows;
            string sqlInsertFormat = "INSERT INTO {0} (ID, VALUE) VALUES (NULL, @VALUE); " + DataAccess.SQL_SELECT_ID_LAST;
            string sqlInsert = string.Format(sqlInsertFormat, DataAccess.TABLE_NAME_TAG);

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@VALUE", DbType.String)
                    };

            try
            {
                foreach (DBTagInfo tagInfo in tagInfos)
                {
                    parms[0].Value = tagInfo.Name;
                    object objRel = SqliteHelper.ExecuteScalar(DataAccess.ConnectionStringProfile, CommandType.Text, sqlInsert, parms);
                    if (null != objRel)
                    {
                        iSuccessRows++;
                        int id = Convert.ToInt32(objRel);
                        tagInfo.ID = id;
                    }
                }

                trans.Commit();
            }
            catch (Exception e)
            {
                trans.Rollback();
                throw new ApplicationException(e.Message);
            }
            finally
            {
                conn.Close();
            }

            return iSuccessRows;
        }
    }
}
