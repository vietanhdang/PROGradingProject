using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Mark
{
    public class MarkResult
    {
        public int QuestionId { get; set; }
        public int TestCaseId { get; set; }
        public string? InputValue { get; set; }
        public string? ExpectedValue { get; set; }
        public string? YourOutput { get; set; }
        public long Time { get; set; }
        public float Score { get; set; }
        public string? Exception { get; set; }
    }
}
