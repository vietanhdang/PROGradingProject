using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Enumeration.Enumeration;

namespace Common.Models
{
    /// <summary>
    /// Update Role hàng loạt
    /// </summary>
    public class RoleUpdate
    {
        public int AccountId { get; set; }
        public Role role { get; set; }
    }
}
