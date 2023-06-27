using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Common.Models
{
    public class ExamStudentSubmitDTO
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Exam")]
        [Required(ErrorMessage = "ExamId is required")]
        public int ExamId { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Student")]
        [Required(ErrorMessage = "StudentId is required")]
        public int StudentId { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Submit time
        /// </summary>
        public DateTime? SubmitedTime { get; set; }

        /// <summary>
        /// Submit folder
        /// </summary>
        public string? SubmitedFolder { get; set; }

        /// <summary>
        /// Student's status
        /// 0: Not submit
        /// 1: Start
        /// 2: Submit
        /// 3: Submit late
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Student's Log
        /// </summary>
        public string? MarkLog { get; set; }

        /// <summary>
        /// Student's score
        /// </summary>
        public int? Score { get; set; }

        /// <summary>
        /// Count time student submit exam
        /// </summary>
        public int? CountTimeSubmit { get; set; }
    }
}
