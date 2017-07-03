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
    public class DBFolder : IDBFolder
    {
        private readonly string sqlUpdateFormat = @"UPDATE " + DataAccess.TABLE_NAME_MONITOREDFOLDER + " SET PATH = @PATH, NAME = @NAME, ISSCANNED = @ISSCANNED WHERE ID = {0};";

        public bool InsertFolder(string folderPath)
        {
            throw new NotImplementedException();
        }

        public int InsertPatchFolders(IList<DBFolderInfo> folders)
        {
            int iSuccessRows = 0;
            if (0 == folders.Count)
                return iSuccessRows;
            //string sqlDelete = "DELETE FROM MonitoredFolder";
            string sqlInsertFormat = "INSERT INTO {0} (ID, PATH, NAME, ISSCANNED) VALUES (NULL, @PATH, @NAME, @ISSCANNED);" + DataAccess.SQL_SELECT_ID_LAST;
            string sqlInsert = string.Format(sqlInsertFormat, DataAccess.TABLE_NAME_MONITOREDFOLDER);

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@PATH", DbType.String),
                new SQLiteParameter("@NAME", DbType.String),
                new SQLiteParameter("@ISSCANNED", DbType.Int32)
                    };

            try
            {
                //SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms);

                foreach (DBFolderInfo folder in folders)
                {
                    parms[0].Value = folder.Path;
                    parms[1].Value = folder.Name;
                    parms[2].Value = folder.IsScanned ? 1 : 0;
                    object objRel = SqliteHelper.ExecuteScalar(DataAccess.ConnectionStringProfile, CommandType.Text, sqlInsert, parms);
                    if (null != objRel)
                    {
                        iSuccessRows++;
                        int id = Convert.ToInt32(objRel);
                        folder.ID = id;
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

        public int UpdateFolders(IList<DBFolderInfo> folders)
        {
            int iSuccessRows = 0;
            if (0 == folders.Count)
                return iSuccessRows;

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@PATH", DbType.String),
                new SQLiteParameter("@NAME", DbType.String),
                new SQLiteParameter("@ISSCANNED", DbType.Int32)
                    };

            try
            {
                //SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms);

                foreach (DBFolderInfo folder in folders)
                {
                    string sqlUpdate = string.Format(sqlUpdateFormat, folder.ID);
                    parms[0].Value = folder.Path;
                    parms[1].Value = folder.Name;
                    parms[2].Value = folder.IsScanned ? 1 : 0;
                    object objRel = SqliteHelper.ExecuteScalar(DataAccess.ConnectionStringProfile, CommandType.Text, sqlUpdate, parms);
                    if (null != objRel)
                    {
                        iSuccessRows++;
                        int id = Convert.ToInt32(objRel);
                        folder.ID = id;
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

        public IList<DBFolderInfo> GetMonitoredFolderList()
        {
            IList<DBFolderInfo> folderList = new List<DBFolderInfo>();
            string sqlSelect = "SELECT * FROM " + DataAccess.TABLE_NAME_MONITOREDFOLDER;
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                int id = dr.GetInt32(0);
                string path = dr.GetString(1);
                string name = dr.GetString(2);
                int iScanned = dr.GetInt32(3);
                DBFolderInfo folderInfo = new DBFolderInfo() { ID = id, Path = path, Name = name, IsScanned = (1 == iScanned) ? true : false };
                folderList.Add(folderInfo);
            }
            dr.Close();
            return folderList;
        }

        public void DeletePatchFolders(IList<DBFolderInfo> folders)
        {
            if (0 == folders.Count)
                return;
            string sqlDeleteFormat = "DELETE FROM {0} WHERE ID=@ID";
            string sqlDelete = string.Format(sqlDeleteFormat, DataAccess.TABLE_NAME_MONITOREDFOLDER);

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@ID", DbType.Int32)
                    };

            try
            {
                foreach (DBFolderInfo folder in folders)
                {
                    parms[0].Value = folder.ID;
                    SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms);
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
        }
    }
}
