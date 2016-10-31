using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileExplorer.Helper
{
    static class LogHelper
    {
        internal static void Debug(string s)
        {
            NLogger.LogHelper.UILogger.Debug(s);
        }

        internal static void Debug(string s, Exception ex)
        {
            NLogger.LogHelper.UILogger.Debug(s, ex);
        }

        internal static void Debug(Exception ex)
        {
            NLogger.LogHelper.UILogger.Debug("FileExplorer exception:", ex);
        }

        internal static void DebugFormat(string format, params object[] values)
        {
            NLogger.LogHelper.UILogger.DebugFormat(format, values);
        }

        internal static void Info(string s)
        {
            NLogger.LogHelper.UILogger.Info(s);
        }

        internal static void Error(string s)
        {
            NLogger.LogHelper.UILogger.Error(s);
        }
    }
}
