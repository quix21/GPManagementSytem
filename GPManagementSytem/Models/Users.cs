using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        [DisplayName("First name")]
        public string Firstname { get; set; }
        [Required]
        public string Surname { get; set; }
        [Required]
        public string Email { get; set; }
        public string Telephone { get; set; }
        [Required]
        [DisplayName("Unit")]
        public int UnitId { get; set; }
        [DisplayName("Is Administrator")]
        public bool IsAdmin { get; set; }
        public bool Approved { get; set; }
        [DisplayName("Accound Enabled")]
        public bool IsLive { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
    }
}