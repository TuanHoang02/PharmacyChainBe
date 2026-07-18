using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.DTOs.Response;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;

        public SalesService(ISalesRepository salesRepository)
        {
            _salesRepository = salesRepository;
        }

        public async Task<PagedResponse<List<SalesHistoryDto>>> GetPagedAsync(SalesHistoryQuery query, CancellationToken cancellationToken = default)
        {
            var paged = await _salesRepository.GetPagedAsync(query, cancellationToken);
            var mappedList = paged.Data.Select(MapToHistoryDto).ToList();

            return new PagedResponse<List<SalesHistoryDto>>
            {
                Data = mappedList,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize,
                TotalRecords = paged.TotalRecords
            };
        }

        public async Task<SalesInvoiceDetailDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var invoice = await _salesRepository.GetByIdAsync(id, cancellationToken);
            if (invoice == null)
            {
                throw new ApiException("Không tìm thấy hóa đơn.", 404);
            }

            return MapToDetailDto(invoice);
        }

        #region Helper Mappings

        private SalesHistoryDto MapToHistoryDto(SalesInvoice invoice)
        {
            return new SalesHistoryDto
            {
                SalesInvoiceID = invoice.SalesInvoiceID,
                InvoiceCode = invoice.InvoiceCode,
                CustomerName = invoice.CustomerName,
                CustomerPhoneNumber = invoice.CustomerPhoneNumber,
                TotalAmount = invoice.TotalAmount,
                PaymentMethod = invoice.PaymentMethod,
                PaymentStatus = invoice.PaymentStatus,
                InvoiceStatus = invoice.InvoiceStatus,
                CreatedAt = invoice.CreatedAt,
                CompletedAt = invoice.CompletedAt
            };
        }

        private SalesInvoiceDetailDto MapToDetailDto(SalesInvoice invoice)
        {
            return new SalesInvoiceDetailDto
            {
                SalesInvoiceID = invoice.SalesInvoiceID,
                InvoiceCode = invoice.InvoiceCode,
                CustomerName = invoice.CustomerName,
                CustomerPhoneNumber = invoice.CustomerPhoneNumber,
                Subtotal = invoice.Subtotal,
                DiscountAmount = invoice.DiscountAmount,
                TotalAmount = invoice.TotalAmount,
                PaymentMethod = invoice.PaymentMethod,
                PaymentStatus = invoice.PaymentStatus,
                InvoiceStatus = invoice.InvoiceStatus,
                PrescriptionImageUrl = invoice.PrescriptionImageUrl,
                IsPrescriptionVerified = invoice.IsPrescriptionVerified,
                CreatedAt = invoice.CreatedAt,
                CompletedAt = invoice.CompletedAt,
                BranchName = invoice.Branch?.BranchName ?? string.Empty,
                PharmacistName = invoice.CreatedByUser?.FullName ?? string.Empty,
                Items = invoice.SalesInvoiceDetails?.Select(MapToItemDto).ToList() ?? new List<SalesInvoiceItemDto>()
            };
        }

        private SalesInvoiceItemDto MapToItemDto(SalesInvoiceDetail detail)
        {
            return new SalesInvoiceItemDto
            {
                MedicineID = detail.MedicineID,
                MedicineName = detail.Medicine?.MedicineName ?? string.Empty,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                LineTotal = detail.LineTotal
            };
        }

        #endregion
    }
}
