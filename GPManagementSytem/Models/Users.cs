﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    public enum UserTypes :int
    {
        Administrator = 1,
        Practice = 2,
        StandardUser = 3
    }

    [Table("User")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        public int UserType { get; set; }
        public bool Year2 { get; set; }
        public bool Year3 { get; set; }
        public bool Year4 { get; set; }
        public bool Year5 { get; set; }
        public int PracticeId { get; set; }
        [DisplayName("Accound Enabled")]
        public bool IsActive { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdatedBy { get; set; }
    }
}