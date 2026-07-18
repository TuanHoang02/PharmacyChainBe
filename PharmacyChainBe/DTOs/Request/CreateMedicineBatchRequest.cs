using System.ComponentModel.DataAnnotations;

namespace PharmacyChainBe.DTOs.Request
{
    public class CreateMedicineBatchRequest
    {
        [Required(ErrorMessage = "Mã chi tiết đơn đặt hàng không được để trống.")]
        public int PurchaseOrderDetailID { get; set; }

        [Required(ErrorMessage = "Số lô sản xuất không được để trống.")]
        [MaxLength(50, ErrorMessage = "Số lô sản xuất không được vượt quá 50 ký tự.")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ngày sản xuất không được để trống.")]
        public DateTime ManufacturingDate { get; set; }

        [Required(ErrorMessage = "Ngày hết hạn không được để trống.")]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Số lượng nhận không được để trống.")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng nhận phải lớn hơn 0.")]
        public int ReceivedQuantity { get; set; }
    }
}
