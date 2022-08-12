using GPManagementSytem.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.ViewModel
{
    public class ApprovePracticeChangesViewModel
    {
        public Practices originalRecord { get; set; } 
        public PracticesExternal changedRecord { get; set; }
    }
}