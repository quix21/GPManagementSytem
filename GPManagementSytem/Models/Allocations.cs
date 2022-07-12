using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("Allocated")]
    public class Allocations
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int PracticeId { get; set; }
        public string Year2Wk1Requested { get; set; }
        public string Year2Wk1Allocated { get; set; }
        public int Year2Wk2Requested { get; set; }
        public int Year2Wk2Allocated { get; set; }
        public int Year2Wk3Requested { get; set; }
        public int Year2Wk3Allocated { get; set; }
        public int Year2Wk4Requested { get; set; }
        public int Year2Wk4Allocated { get; set; }
        public int Year2Wk5Requested { get; set; }
        public int Year2Wk5Allocated { get; set; }
        public int Year2Wk6Requested { get; set; }
        public int Year2Wk6Allocated { get; set; }
        public int Year3B1Requested { get; set; }
        public int Year3B1Allocated { get; set; }
        public int Year3B2Requested { get; set; }
        public int Year3B2Allocated { get; set; }
        public int Year3B3Requested { get; set; }
        public int Year3B3Allocated { get; set; }
        public int Year3B4Requested { get; set; }
        public int Year3B4Allocated { get; set; }
        public int Year3B5Requested { get; set; }
        public int Year3B5Allocated { get; set; }
        public int Year3B6Requested { get; set; }
        public int Year3B6Allocated { get; set; }
        public int Year3B7Requested { get; set; }
        public int Year3B7Allocated { get; set; }
        public int Year4B1Requested { get; set; }
        public int Year4B1Allocated { get; set; }
        public int Year4B2Requested { get; set; }
        public int Year4B2Allocated { get; set; }
        public int Year4B3Requested { get; set; }
        public int Year4B3Allocated { get; set; }
        public int Year4B4Requested { get; set; }
        public int Year4B4Allocated { get; set; }
        public int Year4B5Requested { get; set; }
        public int Year4B5Allocated { get; set; }
        public int Year4B6Requested { get; set; }
        public int Year4B6Allocated { get; set; }
        public int Year4B7Requested { get; set; }
        public int Year4B7Allocated { get; set; }
        public int Year4B8Requested { get; set; }
        public int Year4B8Allocated { get; set; }
        public int Year5B1Requested { get; set; }
        public int Year5B1Allocated { get; set; }
        public int Year5B2Requested { get; set; }
        public int Year5B2Allocated { get; set; }
        public int Year5B3Requested { get; set; }
        public int Year5B3Allocated { get; set; }
        public int Year5B4Requested { get; set; }
        public int Year5B4Allocated { get; set; }
        public int Year5B5Requested { get; set; }
        public int Year5B5Allocated { get; set; }
        public int Year5B6Requested { get; set; }
        public int Year5B6Allocated { get; set; }
        public string AcademicYear { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }

    }
}