using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("PracticeExternal")]
    public class PracticesExternal: Practices
    {
        public int PrimaryId { get; set; }
        public DateTime DateRequested { get; set; }
        public int RequestedBy { get; set; }
        public DateTime DateApproved { get; set; }
        public int ApprovedBy { get; set; }
        public bool ChangesApproved { get; set; }
    }
}