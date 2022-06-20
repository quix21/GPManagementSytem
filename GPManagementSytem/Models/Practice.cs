using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GPManagementSytem.Models
{
    [Table("Practice")]
    public class Practice
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int OriginalID { get; set;}
        public string Surgery { get; set; }
        public int SurgeryNotInUse { get; set; }
        public string GP1 { get; set; }
        public string GP1Email { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string PracticeManager { get; set; }
        public string PMEmail { get; set; }
        public string GP2 { get; set; }
        public string GP2Email { get; set; }
        public string Website { get; set; }
        public string GP3 { get; set; }
        public string GP3Email { get; set; }
        public string GP4 { get; set; }
        public string GP4Email { get; set; }
        public string SupplierNumber { get; set; }
        public int DoNotContactSurvey { get; set; }
        public string AttachmentsAllocated { get; set; }
        public int ContractReceived { get; set; }
        public string UCCTNotes { get; set; }
        public DateTime QualityVisitDateR1 { get; set; }
        public string QualityVisitNotes { get; set; }
        public int Active { get; set; }
        public int Disabled { get; set; }
        public int Queried { get; set; }
        public string ListSize { get; set; }
        public int NewPractice { get; set; }
        public string AcademicYear { get; set; }
        public DateTime QualityVisitDate { get; set; }
        public int OKToProceed { get; set; }
        public DateTime DataReviewDate { get; set; }
        public string TutorTrainingGPName { get; set; }
        public DateTime TutorTrainingDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public int UpdatedBy { get; set; }


    }
}