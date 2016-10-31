using IDAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileExplorer.Model;
using System.Data.SQLite;
using System.Data;
using DBUtility;
using System.IO;
using IDAL.Model;
using System.Diagnostics;

namespace SQLiteDAL
{
    public class ScannedFile : IScannedFile
    {
        public int InsertPatchFiles(IList<FileInfo> files)
        {
            int iSuccessRows = 0;
            if (0 == files.Count)
                return iSuccessRows;
            string sqlDelete = "DELETE FROM File";
            string sqlInsertFormat = "INSERT INTO {0} (ID, PATH, NAME, EXTENSION, TAG) VALUES (NULL, @PATH, @NAME, @EXTENSION, @TAG);";
            string sqlInsert = string.Format(sqlInsertFormat, DataAccess.TABLE_NAME_FILE);

            SQLiteConnection conn = new SQLiteConnection(DataAccess.ConnectionStringProfile);
            conn.Open();
            SQLiteTransaction trans = conn.BeginTransaction(IsolationLevel.ReadCommitted);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@PATH", DbType.String),
                new SQLiteParameter("@NAME", DbType.String),
                new SQLiteParameter("@EXTENSION", DbType.String),
                new SQLiteParameter("@TAG", DbType.String)
                    };

            try
            {
                SqliteHelper.ExecuteNonQuery(trans, CommandType.Text, sqlDelete, parms);

                foreach (FileInfo file in files)
                {
                    parms[0].Value = file.FullName;
                    parms[1].Value = file.Name;
                    parms[2].Value = file.Extension;
                    parms[3].Value = string.Empty;
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

        public IList<MonitoredFile> GetFilesByTags(string tags)
        {
            IList<MonitoredFile> fileList = new List<MonitoredFile>();
            string sqlSelectFormat = "SELECT * FROM " + DataAccess.TABLE_NAME_FILE + (string.IsNullOrEmpty(tags) ? string.Empty : " WHERE TAG LIKE '%{0}%'");
            string sqlSelect = string.Format(sqlSelectFormat, tags);
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                MonitoredFile monitoredFile = MonitoredFile.Create();
                monitoredFile.ID = dr.GetInt32(0);
                monitoredFile.Path = dr.GetString(1);
                monitoredFile.Name = dr.GetString(2);
                fileList.Add(monitoredFile);
            }
            dr.Close();
            return fileList;
        }

        public IList<MonitoredFile> GetFilesUnderFolder(string folderPath)
        {
            IList<MonitoredFile> fileList = new List<MonitoredFile>();
            string sqlSelectFormat = "SELECT * FROM " + DataAccess.TABLE_NAME_FILE + (string.IsNullOrEmpty(folderPath) ? string.Empty:" WHERE PATH LIKE '%{0}%'");
            string sqlSelect = string.Format(sqlSelectFormat, folderPath);
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                MonitoredFile monitoredFile = MonitoredFile.Create();
                monitoredFile.ID = dr.GetInt32(0);
                monitoredFile.Path = dr.GetString(1);
                monitoredFile.Name = dr.GetString(2);
                fileList.Add(monitoredFile);
            }
            dr.Close();
            return fileList;
        }

        public void GetFilesUnderFolderAsync(string folderPath, Action<IList<MonitoredFile>> callback,ref bool isCancel)
        {
            IList<MonitoredFile> fileList = new List<MonitoredFile>();
            string sqlSelectFormat = "SELECT * FROM " + DataAccess.TABLE_NAME_FILE + (string.IsNullOrEmpty(folderPath) ? string.Empty : " WHERE PATH LIKE '%{0}%'");
            string sqlSelect = string.Format(sqlSelectFormat, folderPath);
            SQLiteDataReader dr = SqliteHelper.ExecuteReader(DataAccess.ConnectionStringProfile, CommandType.Text, sqlSelect, null);
            int iCount = 0;
            
            while (dr.Read())
            {
                if (dr.IsDBNull(1))
                    continue;
                MonitoredFile monitoredFile = MonitoredFile.Create();
                monitoredFile.ID = dr.GetInt32(0);
                monitoredFile.Path = dr.GetString(1);
                monitoredFile.Name = dr.GetString(2);
                fileList.Add(monitoredFile);
                iCount++;
                if (iCount >= 100)
                {
                    if (null != callback)
                    {
                        callback(fileList);
                        fileList.Clear();
                    }
                    iCount = 0;
                }
                Debug.WriteLine(isCancel);
                if (isCancel)
                    break;
            }
            dr.Close();
            if (null != callback)
            {
                callback(fileList);
            }
        }

        public bool UpdateFile(MonitoredFile file)
        {
            int iRel = 0;
            string sqlUpdateFormat = @"UPDATE "+ DataAccess.TABLE_NAME_FILE + " SET PATH = @PATH, NAME = @NAME, EXTENSION = @EXTENSION,  TAG = @TAG WHERE ID = {0};";
            string sqlUpdate = string.Format(sqlUpdateFormat, file.ID);
            SQLiteParameter[] parms = {
                new SQLiteParameter("@PATH", DbType.String),
                new SQLiteParameter("@NAME", DbType.String),
                new SQLiteParameter("@EXTENSION", DbType.String),
                new SQLiteParameter("@TAG", DbType.String)
                    };
            parms[0].Value = file.Path;
            parms[1].Value = file.Name;
            parms[2].Value = file.Extension;
            parms[3].Value = file.Tag;
            iRel = SqliteHelper.ExecuteNonQuery(DataAccess.ConnectionStringProfile, CommandType.Text, sqlUpdate, parms);

            return iRel > 0 ? true : false;
        }
    }
}
