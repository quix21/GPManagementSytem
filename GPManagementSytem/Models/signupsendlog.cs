using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("signupsendlog")]
    public class Signupsendlog
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string SendCode { get; set; }
        public string AcademicYear { get; set; }
        public int UserId { get; set; }
        public int PracticeId { get; set; }
        public string Guid { get; set; }
        public bool NoChangesClicked { get; set; }
        public bool DetailsUpdated { get; set; }
        public DateTime DateSent { get; set; }
        public DateTime? DateActionTaken { get; set; }
        public int SentBy { get; set; }


    }
}