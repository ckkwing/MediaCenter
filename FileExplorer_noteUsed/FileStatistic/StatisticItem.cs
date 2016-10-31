using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileExplorer.FileStatistic
{
    class StatisticItem
    {
        internal string Path { get; set; }
        internal long ItemCount { get; set; }
        internal long Size { get; set; }
        internal bool IsCompleted { get; set; }

    }
}
