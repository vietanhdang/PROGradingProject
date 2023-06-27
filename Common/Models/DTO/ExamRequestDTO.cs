using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    /// <summary>
    /// Exam
    /// </summary>
    public class ExamRequestDTO
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
        public int TeacherId { get; set; }

        [NotMapped]
        public string TeacherName { get; set; }

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
        public string Password { get; set; }

        /// <summary>
        /// Exam's source 
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FolderSource not empty")]
        public string QuestionFolder { get; set; }

        public IFormFile? QuestionFile { get; set; }

        public float? TotalScore { get; set; }

        /// <summary>
        /// Exam's answer
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FolderAnswer not empty")]
        public string AnswerFolder { get; set; }

        public IFormFile? AnswerFile { get; set; }

        /// <summary>
        /// Exam's source 
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "FolderTestCase not empty")]
        public string TestCaseFolder { get; set; }
        public IFormFile? TestCaseFile { get; set; }

        /// <summary>
        /// Total questions
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TotalQuestions not empty")]
        public int TotalQuestions { get; set; }

        /// <summary>
        /// Time start
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TimeStart not empty")]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Time end
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "TimeEnd not empty")]
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Exam's status
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Status not empty")]
        public int Status { get; set; }

        public bool? IsStudentTakeExam { get; set; } = false;

    }

    /// <summary>
    /// Exam
    /// </summary>
    public class ExamListDTO
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamCode { get; set; }
        public string? Password { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public bool? IsStudentTakeExam { get; set; } = false;
        public bool IsShowScore { get; set; } = false;
    }

    /// <summary>
    /// Exam
    /// </summary>
    public class ExamDetailDTO
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamCode { get; set; }
        public string QuestionFolder { get; set; }
        public float? TotalScore { get; set; }
        public string? Password { get; set; }
        public string AnswerFolder { get; set; }
        public string TestCaseFolder { get; set; }
        public int TotalQuestions { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Status { get; set; }
        public bool? IsStudentTakeExam { get; set; } = false;
        public bool IsShowScore { get; set; } = false;
        public ICollection<ExamStudentCustomDTO>? ExamStudents { get; set; }
    }

    public class ExamStudentCustomDTO
    {
        public int ExamStudentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? SubmitedTime { get; set; }
        public string? SubmitedFolder { get; set; }
        public int Status { get; set; }
        public string? MarkLog { get; set; }
        public float? Score { get; set; }
        public int? CountTimeSubmit { get; set; }
    }
}
