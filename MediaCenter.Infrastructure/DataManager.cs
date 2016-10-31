using IDAL.Model;
using System.Collections.Generic;
using Utilities.FileScan;

namespace MediaCenter.Infrastructure
{
    public sealed class DataManager
    {
        public static DataManager Instance
        {
            get { return Nested.instance; }
        }

        class Nested
        {
            static Nested() { }
            internal static readonly DataManager instance = new DataManager();
        }

        private FileScanner fileScanner = new FileScanner();
        public FileScanner FileScanner
        {
            get
            {
                return fileScanner;
            }
        }

        private DBCache dbCache = new DBCache();
        public DBCache DBCache
        {
            get
            {
                return dbCache;
            }

            set
            {
                dbCache = value;
            }
        }

        DataManager()
        {

        }

        public void Init()
        {
            DBCache.Init();
            FileScanner.ProcessEvent += FileScanner_ProcessEvent;
        }

        public void Uninit()
        {
            FileScanner.ProcessEvent -= FileScanner_ProcessEvent;
            DBCache.Uninit();
        }

        private void FileScanner_ProcessEvent(object sender, FileScannerProcessEventArgs e)
        {
            switch(e.ProcessType)
            {
                case ProcessType.End:
                    {
                        DBHelper.InsertFilesToDB(FileScanner.FilesInDirectory);
                    }
                    break;
            }
        }

    }
}
