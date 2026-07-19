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
        public List<ReceiveMedicineBatchDto> ReceivedBatches { get; set; } = new List<ReceiveMedicineBatchDto>();
    }

    public class ReceiveMedicineBatchDto
    {
        [Required]
        public int MedicineBatchId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Received quantity must be greater than zero.")]
        public int ReceivedQuantity { get; set; }
    }
}
