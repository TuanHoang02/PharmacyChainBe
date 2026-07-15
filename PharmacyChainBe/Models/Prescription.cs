using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.Models
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }

        public string? CustomerName { get; set; }

        public string? CustomerPhone { get; set; }

        public int? PharmacistID { get; set; }

        public string PrescriptionImage { get; set; } = string.Empty;

        public DateTime UploadDate { get; set; }



        public User? Pharmacist { get; set; }
    }
}
