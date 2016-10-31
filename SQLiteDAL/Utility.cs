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
    class Utility
    {
        internal static bool TabbleIsExist(String tableName)
        {
            if (tableName == null)
            {
                return false;
            }
            string sql = "select count(*) from sqlite_master where type ='table' and name ='" + tableName + "' ";
            object objResult = SqliteHelper.ExecuteScalar(DataAccess.ConnectionStringProfile, CommandType.Text, sql, null);
            int iCount = 0;
            try
            {
                iCount = Convert.ToInt32(objResult);
            }
            catch
            {
                return false;
            }
            return iCount > 0 ? true : false;
        }
    }
}
