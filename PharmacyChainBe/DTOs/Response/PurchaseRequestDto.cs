using System;
using System.Collections.Generic;

namespace PharmacyChainBe.DTOs.Response
{
    public class PurchaseRequestDto
    {
        public int PurchaseRequestID { get; set; }
        public string RequestCode { get; set; } = string.Empty;
        public int BranchID { get; set; }
        public string? BranchName { get; set; }
        public int CreatedByUserID { get; set; }
        public string? CreatedByUserName { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? ReviewNote { get; set; }
        public int? ReviewedByUserID { get; set; }
        public string? ReviewedByUserName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ReviewedAt { get; set; }
        
        public List<PurchaseRequestDetailDto> Details { get; set; } = new List<PurchaseRequestDetailDto>();
        public List<PurchaseRequestOrderDto> PurchaseOrders { get; set; } = new List<PurchaseRequestOrderDto>();
    }

    public class PurchaseRequestOrderDto
    {
        public int PurchaseOrderID { get; set; }
        public string PurchaseOrderCode { get; set; } = string.Empty;
        public string SupplierName { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public string DeliveryStatus { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<PurchaseRequestOrderItemDto> Items { get; set; } = new List<PurchaseRequestOrderItemDto>();
    }

    public class PurchaseRequestOrderItemDto
    {
        public string MedicineName { get; set; } = string.Empty;
        public int OrderedQuantity { get; set; }
        public string Unit { get; set; } = string.Empty;
    }
}
