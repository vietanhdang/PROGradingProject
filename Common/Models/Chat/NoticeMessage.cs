using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.Chat
{
    /// <summary>
    /// This class is used to send message to group chat
    /// </summary>
    public class NoticeMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string? Message { get; set; }
        public int? GroupId { get; set; }
        public string? SenderName { get; set; }
        public int SenderId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string Type { get; set; }

    }
}
