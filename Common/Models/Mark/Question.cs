using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class Question
    {
        public int QuestionId { get; set; }
        public List<TestCase>? TestCases { get; set; }
    }
}
