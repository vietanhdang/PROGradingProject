

using Common.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    /// <summary>
    /// Exam's student
    /// </summary>
    [Table("ExamStudents")]
    public class ExamStudent : BaseEntity
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int ExamStudentId { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Student")]
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
        /// 1: Submit
        /// 2: Submit late
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status not empty")]
        public int Status { get; set; }

        /// <summary>
        /// Student's Log
        /// </summary>
        public string? MarkLog { get; set; }

        /// <summary>
        /// Student's score
        /// </summary>
        public float? Score { get; set; }

        /// <summary>
        /// Count time student submit exam
        /// </summary>
        public int? CountTimeSubmit { get; set; }

        /// <summary>
        /// Exam
        /// </summary>
        public virtual Exam Exam { get; set; } = null!;

        /// <summary>
        /// Student
        /// </summary>
        public virtual Student Student { get; set; } = null!;
    }
}
