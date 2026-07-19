using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class ReceiveMedicinesDto
    {
        [Required]
        public List<ReceiveMedicineDetailDto> Details { get; set; } = new List<ReceiveMedicineDetailDto>();
    }

    public class ReceiveMedicineDetailDto
    {
        [Required]
        public int MedicineId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Received quantity must be greater than zero.")]
        public int ReceivedQuantity { get; set; }

        [Required]
        [MaxLength(50)]
        public string BatchNumber { get; set; } = string.Empty;

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public DateTime ManufacturingDate { get; set; }

        [Required]
        public int SupplierId { get; set; }
    }
}
