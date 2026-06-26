using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }

        public int CustomerID { get; set; }

        public int? PharmacistID { get; set; }

        public string PrescriptionImage { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public DateTime? ApprovedDate { get; set; }

        public User? Customer { get; set; }

        public User? Pharmacist { get; set; }
    }
}
