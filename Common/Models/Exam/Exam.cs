using Common.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Common.Models
{
    /// <summary>
    /// Exam
    /// </summary>
    [Table("Exams")]
    public class Exam : BaseEntity
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExamId { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }

        /// <summary>
        /// Exam's name
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name not empty")]
        public string ExamName { get; set; }

        /// <summary>
        /// Exam's Code
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Code not empty")]
        public string ExamCode { get; set; }

        /// <summary>
        /// Exam's Password
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Exam's source 
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "QuestionFolder not empty")]
        public string QuestionFolder { get; set; }

        /// <summary>
        /// Exam's source 
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FolderTestCase not empty")]
        public string? TestCaseFolder { get; set; }

        /// <summary>
        /// TotalScore
        /// </summary>
        public float? TotalScore { get; set; }

        /// <summary>
        /// Exam's answer
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "AnswerFolder not empty")]
        public string? AnswerFolder { get; set; }

        /// <summary>
        /// Total questions
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TotalQuestions not empty")]
        public int? TotalQuestions { get; set; }

        /// <summary>
        /// Time start
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TimeStart not empty")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// Time end
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TimeEnd not empty")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Exam's status
        /// 0: not start
        /// 1: started
        /// 2: ended
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status not empty")]
        public int Status { get; set; }

        public bool IsShowScore { get; set; }

        /// <summary>
        /// Teacher's account
        /// </summary>
        [JsonIgnore]
        public virtual Teacher Teacher { get; set; }

        /// <summary>
        /// Exam's students
        /// </summary>
        [JsonIgnore]
        public virtual ICollection<ExamStudent>? ExamStudents { get; set; }
    }
}
