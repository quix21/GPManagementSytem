using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GPManagementSytem.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        [Required]
        [DisplayName("First name")]
        public string Firstname { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        public string Pwd { get; set; }
        [Required]
        [DisplayName("User Type")]
        public string UserType { get; set; }
        public bool Year2 { get; set; }
        public bool Year3 { get; set; }
        public bool Year4 { get; set; }
        public bool Year5 { get; set; }
        [DisplayName("Practice")]
        public string Practice { get; set; }
        [DisplayName("Accound Enabled")]
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdatedBy { get; set; }
    }
}