using DBUtility;
using IDAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLiteDAL
{
    public class DBEntity : IDBEntity
    {
        public bool InitDBProfile()
        {
            if (!System.IO.File.Exists(DataAccess.DBFilePath))
            {
                SQLiteConnection.CreateFile(DataAccess.DBFilePath);
            }
            CreateMonitoredFolderTable();
            CreateTagTable();
            CreateFileTable();
            return true;
        }

        private void CreateMonitoredFolderTable()
        {
            if (Utility.TabbleIsExist(DataAccess.TABLE_NAME_MONITOREDFOLDER))
                return;
            string sqlFormat = @"CREATE TABLE {0}(
                                                                    ID INTEGER PRIMARY KEY   AUTOINCREMENT,
                                                                    PATH TEXT NOT NULL,
                                                                    NAME TEXT NOT NULL
                                                                    );";
            string sql = string.Format(sqlFormat, DataAccess.TABLE_NAME_MONITOREDFOLDER);
            SqliteHelper.ExecuteNonQuery(DataAccess.ConnectionStringProfile, CommandType.Text, sql, null);
        }

        private void CreateTagTable()
        {
            if (Utility.TabbleIsExist(DataAccess.TABLE_NAME_TAG))
                return;
            string sqlFormat = @"CREATE TABLE {0}(
                                                                    ID INTEGER PRIMARY KEY   AUTOINCREMENT,
                                                                    VALUE TEXT NOT NULL
                                                                    );";
            string sql = string.Format(sqlFormat, DataAccess.TABLE_NAME_TAG);
            SqliteHelper.ExecuteNonQuery(DataAccess.ConnectionStringProfile, CommandType.Text, sql, null);
        }

        private void CreateFileTable()
        {
            if (Utility.TabbleIsExist(DataAccess.TABLE_NAME_FILE))
                return;
            string sqlFormat = @"CREATE TABLE {0}(
                                                                    ID INTEGER PRIMARY KEY   AUTOINCREMENT,
                                                                    PATH TEXT NOT NULL,
                                                                    NAME TEXT NOT NULL,
                                                                    EXTENSION TEXT,
                                                                    TAG TEXT
                                                                    );";
            string sql = string.Format(sqlFormat, DataAccess.TABLE_NAME_FILE);
            SqliteHelper.ExecuteNonQuery(DataAccess.ConnectionStringProfile, CommandType.Text, sql, null);
        }

    }
}
