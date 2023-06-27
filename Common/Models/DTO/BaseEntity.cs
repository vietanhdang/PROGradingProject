using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Common.Models
{
    /// <summary>
    /// Base entity for all entities
    /// </summary>
    /// Author: ANHDVHE151359 - 01.06.23
    public class BaseEntity
    {

        /// <summary>
        /// Created date of entity
        /// </summary>
        public DateTime? CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Updated date of entity
        /// </summary>
        public DateTime? UpdatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Created by of entity
        /// </summary>
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Updated by of entity
        /// </summary>
        public string? UpdatedBy { get; set; }
    }
}
