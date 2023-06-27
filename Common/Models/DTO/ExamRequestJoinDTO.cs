using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class ExamRequestJoinDTO
    {
        /// <summary>
        /// Code of exam
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code not empty")]
        public string ExamCode { get; set; }

        /// <summary>
        /// Password of exam
        /// </summary>
        public string Password { get; set; }
    }
}
