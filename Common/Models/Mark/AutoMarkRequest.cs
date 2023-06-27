using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Mark
{
    public class AutoMarkRequest
    {
        public string StudentFolder { get; set; }
        public string TestCaseFolder { get; set; }
        public int ExamId { get; set; }
        public int StudentId { get; set; }
    }
}
