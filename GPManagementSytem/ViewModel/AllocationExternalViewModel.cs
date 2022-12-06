using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.ViewModel
{
    public class AllocationExternalViewModel
    {
        public Allocations allocations { get; set; }
        public SignupDates signupDates { get; set; }

        [NotMapped]
        public bool Year2Requested4Checked { get; set; }
        public bool Year2Wk1RequestedChecked { get; set; }
    }
}