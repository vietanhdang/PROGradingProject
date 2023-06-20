

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
        /// Student's score
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// Start time
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "StartTime not empty")]
        public int StartTime { get; set; }

        /// <summary>
        /// Submit time
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "SubmitTime not empty")]
        public int SubmitedTime { get; set; }

        /// <summary>
        /// Submit folder
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FolderSubmit not empty")]
        public string SubmitedFolder { get; set; }

        /// <summary>
        /// Student's status
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status not empty")]
        public int Status { get; set; }

        /// <summary>
        /// Student's Log
        /// </summary>
        public string MarkLog { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        /// <summary>
        /// Id of entity
        /// </summary>
        [ForeignKey("Exam")]
        public int ExamId { get; set; }

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
