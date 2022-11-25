using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("signupdates")]
    public class SignupDates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime? Year2Wk1From { get; set; }
        public DateTime? Year2Wk1To { get; set; }
        public DateTime? Year2Wk2From { get; set; }
        public DateTime? Year2Wk2To { get; set; }
        public DateTime? Year2Wk3From { get; set; }
        public DateTime? Year2Wk3To { get; set; }
        public DateTime? Year2Wk4From { get; set; }
        public DateTime? Year2Wk4To { get; set; }
        public DateTime? Year2Wk5From { get; set; }
        public DateTime? Year2Wk5To { get; set; }
        public DateTime? Year2Wk6From { get; set; }
        public DateTime? Year2Wk6To { get; set; }
        public DateTime? Year3B1From { get; set; }
        public DateTime? Year3B1To { get; set; }
        public DateTime? Year3B2From { get; set; }
        public DateTime? Year3B2To { get; set; }
        public DateTime? Year3B3From { get; set; }
        public DateTime? Year3B3To { get; set; }
        public DateTime? Year3B4From { get; set; }
        public DateTime? Year3B4To { get; set; }
        public DateTime? Year3B5From { get; set; }
        public DateTime? Year3B5To { get; set; }
        public DateTime? Year3B6From { get; set; }
        public DateTime? Year3B6To { get; set; }
        public DateTime? Year3B7From { get; set; }
        public DateTime? Year3B7To { get; set; }
        public DateTime? Year4B1From { get; set; }
        public DateTime? Year4B1To { get; set; }
        public DateTime? Year4B2From { get; set; }
        public DateTime? Year4B2To { get; set; }
        public DateTime? Year4B3From { get; set; }
        public DateTime? Year4B3To { get; set; }
        public DateTime? Year4B4From { get; set; }
        public DateTime? Year4B4To { get; set; }
        public DateTime? Year4B5From { get; set; }
        public DateTime? Year4B5To { get; set; }
        public DateTime? Year4B6From { get; set; }
        public DateTime? Year4B6To { get; set; }
        public DateTime? Year4B7From { get; set; }
        public DateTime? Year4B7To { get; set; }
        public DateTime? Year4B8From { get; set; }
        public DateTime? Year4B8To { get; set; }
        public DateTime? Year5B1From { get; set; }
        public DateTime? Year5B1To { get; set; }
        public DateTime? Year5B2From { get; set; }
        public DateTime? Year5B2To { get; set; }
        public DateTime? Year5B3From { get; set; }
        public DateTime? Year5B3To { get; set; }
        public DateTime? Year5B4From { get; set; }
        public DateTime? Year5B4To { get; set; }
        public DateTime? Year5B5From { get; set; }
        public DateTime? Year5B5To { get; set; }
        public DateTime? Year5B6From { get; set; }
        public DateTime? Year5B6To { get; set; }
        public string AcademicYear { get; set; }
        public DateTime? DateUpdated { get; set; }
        public int? UpdatedBy { get; set; }
    }
}