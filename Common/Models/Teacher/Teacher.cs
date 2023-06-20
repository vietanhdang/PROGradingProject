using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    [Table("Teachers")]
    public class Teacher : BaseEntity
    {
        /// <summary>
        ///  [Key]
        /// </summary>
        [ForeignKey("Account")]
        public int TeacherId { get; set; }

        /// <summary>
        /// Teacher's name
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Name not empty")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Code not empty")]
        public string Code { get; set; }

        /// <summary>
        /// Teacher's phone number
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Teacher's address
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Teacher's account
        /// </summary>
        public virtual Account Account { get; set; }

        /// <summary>
        /// Teacher's exams
        /// </summary>
        [NotMapped]
        public virtual ICollection<Exam>? Exams { get; set; }
    }
}
