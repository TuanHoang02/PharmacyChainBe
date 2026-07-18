using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class SupplierResponseRequestDto
    {
        // Optional for accept; required (validated in service) for reject.
        // MaxLength must match PurchaseOrder.SupplierResponseNote column (nvarchar(255)).
        [MaxLength(255, ErrorMessage = "Lý do từ chối không được vượt quá 255 ký tự.")]
        public string? RejectionReason { get; set; }
    }
}
