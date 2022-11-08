using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    public enum VisitTypes : int
    {
        Routine =1,
        ReVisit = 2,
        Triggered = 3,
        Check = 4,
        Induction = 5,
        Welcome = 6

    }

    [Table("QualityVisit")]
    public class QualityVisit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int PracticeId { get; set; }
        [Required]
        public int VisitTypeId { get; set; }
        public DateTime DateOfVisit { get; set; }
        public int VisitNumber { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdatedBy { get; set; }
    }
}