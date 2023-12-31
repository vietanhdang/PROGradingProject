﻿using Common.Models.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Common.Models
{
    public class Account : BaseEntity
    {
        /// <summary>
        /// Id of entity
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        /// <summary>
        /// Email of account
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Email not empty")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [RegularExpression(@"^\S+@fpt\.edu\.vn$", ErrorMessage = "Email must be a valid @fpt.edu.vn address.")]
        public string Email { get; set; }

        /// <summary>
        /// Password of account
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Password not empty")]
        public string Password { get; set; }

        /// <summary>
        /// Role of account (1: Admin, 2: Teacher, 3: Student)
        /// </summary>
        [Required(AllowEmptyStrings = false, ErrorMessage = "Role not empty")]
        public virtual int Role { get; set; }

        /// <summary>
        /// Account's teacher
        /// </summary>
        public virtual Teacher? Teacher { get; set; }

        /// <summary>
        /// Account's student
        /// </summary>
        public virtual Student? Student { get; set; }
    }
}
