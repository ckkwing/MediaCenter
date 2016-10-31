using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace IDAL
{
    public sealed class DataAccess
    {
        private static readonly string path = ConfigurationManager.AppSettings["DALEntry"];
        public static readonly string DBFolder = FolderDefine.DatabaseFolder;
        public static readonly string DBName = @"maindb.sqlite";
        public static readonly string DBFilePath = string.Format(@"{0}\{1}", DBFolder, DBName);
        public static readonly string ConnectionStringProfile = string.Format("Data Source={0};Version=3;", DBFilePath);
        public static readonly string TABLE_NAME_MONITOREDFOLDER = @"MonitoredFolder";
        public static readonly string TABLE_NAME_FILE = @"File";
        public static readonly string TABLE_NAME_TAG = @"Tag";
        public static readonly string DB_MARK_SPLIT = @";";
        public static readonly string SQL_SELECT_ID_LAST = @"SELECT last_insert_rowid();";

        private DataAccess() { }

        public static IDBEntity CreateDBEntity()
        {
            string className = path + ".DBEntity";
            return (IDBEntity)Assembly.Load(path).CreateInstance(className);
        }
        
        public static IMonitorFolder CreateMonitorFolder()
        {
            string className = path + ".MonitorFolder";
            return (IMonitorFolder)Assembly.Load(path).CreateInstance(className);
        }

        public static IScannedFile CreateScannedFile()
        {
            string className = path + ".ScannedFile";
            return (IScannedFile)Assembly.Load(path).CreateInstance(className);
        }

        public static ITag CreateTag()
        {
            string className = path + ".Tag";
            return (ITag)Assembly.Load(path).CreateInstance(className);
        }
    }
}
