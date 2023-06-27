using Common.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    /// <summary>
    /// Student's class
    /// </summary>
    [Table("Students")]
    public class Student : BaseEntity
    {
        /// <summary>
        ///  [Key]
        /// </summary>
        [ForeignKey("Account")]
        public int StudentId { get; set; }

        /// <summary>
        /// Student's name
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name not empty")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Code not empty")]
        public string Code { get; set; }

        /// <summary>
        /// Student's phone number
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Student's address
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Student's account
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// exam's student
        /// </summary>
        public virtual ICollection<ExamStudent>? ExamStudents { get; set; }
    }
}
