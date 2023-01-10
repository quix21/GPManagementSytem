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
        public Practices practice { get; set; }


        [NotMapped]
        public bool Year2Requested4Checked { get; set; }
        public bool Year2Wk1RequestedChecked { get; set; }
        public bool Year2Wk2RequestedChecked { get; set; }
        public bool Year2Wk3RequestedChecked { get; set; }
        public bool Year2Wk4RequestedChecked { get; set; }
        public bool Year2Wk5RequestedChecked { get; set; }
        public bool Year2Wk6RequestedChecked { get; set; }

        public bool Year3B1RequestedChecked { get; set; }
        public bool Year3B2RequestedChecked { get; set; }
        public bool Year3B3RequestedChecked { get; set; }
        
        public bool Year3B4RequestedChecked { get; set; }
        
        public bool Year3B5RequestedChecked { get; set; }
        
        public bool Year3B6RequestedChecked { get; set; }
        
        public bool Year3B7RequestedChecked { get; set; }
        
        public bool Year4B1RequestedChecked { get; set; }
        
        public bool Year4B2RequestedChecked { get; set; }
        
        public bool Year4B3RequestedChecked { get; set; }
        
        public bool Year4B4RequestedChecked { get; set; }
        
        public bool Year4B5RequestedChecked { get; set; }
        
        public bool Year4B6RequestedChecked { get; set; }
        
        public bool Year4B7RequestedChecked { get; set; }
        
        public bool Year4B8RequestedChecked { get; set; }
        
        public bool Year5B1RequestedChecked { get; set; }
        
        public bool Year5B2RequestedChecked { get; set; }
        
        public bool Year5B3RequestedChecked { get; set; }
        
        public bool Year5B4RequestedChecked { get; set; }
       
        public bool Year5B5RequestedChecked { get; set; }
        
        public bool Year5B6RequestedChecked { get; set; }
        

        //TODO - copy allocation properties, remove allocated and add checked. Reflect this in the razor page like Year2Wk1Requested
    }
}