using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    public enum EmailTypes: int
    {
        SignUpInvite = 1
    }

    [Table("emailtemplates")]
    public class EmailTemplates
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int EmailTypeId { get; set; }
        [Required]
        public string Subject { get; set; }
        [Required]
        public string Body { get; set; }
        public string AttachmentName { get; set; }
        public string AttachmentName2 { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdatedBy { get; set; }

        [NotMapped]
        [Required]
        public int SendList { get; set; }
    }
}