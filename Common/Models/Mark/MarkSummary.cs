using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Mark
{
    public class MarkSummary
    {
        public int TotalQuestion { get; set; }
        public int TotalTestCase { get; set; }
        public List<MarkLog> MarkLogs { get; set; }
        public float Score { get; set; }
    }
}
