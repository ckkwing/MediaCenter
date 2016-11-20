using DBUtility;
using FileExplorer.Model;
using IDAL;
using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDAL
{
    public class MonitorFolder : IMonitorFolder
    {
        public bool InsertFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        public int InsertPatchFolders(IList<MonitoredFolderInfo> folders)
        {
            int iSuccessRows = 0;
            if (0 == folders.Count)
                return iSuccessRows;
            //string sqlDelete = "DELETE FROM MonitoredFolder";
            string sqlInsertFormat = "INSERT INTO {0} (ID, PATH, NAME) VALUES (NULL, @PATH, @NAME);";
            string sqlInsert = string.Format(sqlInsertFormat, DataAccess.TABLE_NAME_MONITOREDFOLDER);

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@PATH", DbType.String),
                new SQLiteParameter("@NAME", DbType.String)
                    };

            try
            {
                //SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms);

                foreach (MonitoredFolderInfo folder in folders)
                {
                    parms[0].Value = folder.Path;
                    parms[1].Value = folder.Name;
                    iSuccessRows += SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlInsert, parms);
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
        
        public IList<MonitoredFolderInfo> GetMonitoredFolderList()
        {
            IList<MonitoredFolderInfo> folderList = new List<MonitoredFolderInfo>();
            string sqlSelect = "SELECT * FROM " + DataAccess.TABLE_NAME_MONITOREDFOLDER;
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                int id = dr.GetInt32(0);
                string path = dr.GetString(1);
                string name = dr.GetString(2);
                MonitoredFolderInfo folderInfo = new MonitoredFolderInfo() { ID = id, Path = path, Name = name };
                folderList.Add(folderInfo);
            }
            dr.Close();
            return folderList;
        }


    }
}
