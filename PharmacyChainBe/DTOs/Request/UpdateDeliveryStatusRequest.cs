using PharmacyChainBe.Enums;
using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class UpdateDeliveryStatusRequest
    {
        [Required(ErrorMessage = "Trạng thái giao hàng không được để trống.")]
        public DeliveryStatus DeliveryStatus { get; set; }

        [MaxLength(255, ErrorMessage = "Ghi chú phản hồi không được vượt quá 255 ký tự.")]
        public string? SupplierResponseNote { get; set; }
    }
}
