using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Mark
{
    public class MarkLog
    {
        public DateTime MarkTime { get; set; } = DateTime.Now;
        public string LogType { get; set; } = "Info"; // "Error", "Warning", "Info", "Success
        public object Log { get; set; }
        public MarkLog(DateTime markTime, object log)
        {
            MarkTime = markTime;
            Log = log;
        }
        public MarkLog(object log)
        {
            Log = log;
        }

        public MarkLog()
        {
        }

        public MarkLog(object log, string logType)
        {
            Log = log;
            LogType = logType;
        }

    }
}
