﻿using IDAL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaCenter.Infrastructure.Core.Model
{
    public class LoadMediasJob : Job
    {
        public enum Category
        {
            Folder,
            Tag,
        }

        public class LoadPattern
        {
            public Category Category { get; set; }
            public string keyword = string.Empty;
        }

        public LoadPattern Pattern { get; set; }

        private IList<DBFileInfo> files = new List<DBFileInfo>();
        public IList<DBFileInfo> Files
        {
            get
            {
                return files;
            }

            set
            {
                files = value;
            }
        }

        public LoadMediasJob()
            :base()
        {
            this.Type = JobType.LoadDBMedias;
        }

        public LoadMediasJob(JobType type)
           : base(type)
        {
        }

        public LoadMediasJob(LoadPattern pattern)
            :this()
        {
            this.Pattern = pattern;
        }

        public static LoadMediasJob Create(LoadPattern pattern)
        {
            return new LoadMediasJob(pattern);
        }
    }
}
