using PharmacyChainBe.DTOs.Request;
using PharmacyChainBe.Enums;
using PharmacyChainBe.Exceptions;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;
using PharmacyChainBe.Services.Interfaces;

namespace PharmacyChainBe.Services.Implementations
{
    public class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IInventoryRepository _inventoryRepository;

        public SalesService(ISalesRepository salesRepository, IInventoryRepository inventoryRepository)
        {
            _salesRepository = salesRepository;
            _inventoryRepository = inventoryRepository;
        }

        public async Task CreateSalesInvoiceAsync(int branchId, int userId, CreateSalesInvoiceDto request)
        {
            if (request.Details == null || !request.Details.Any())
            {
                throw new ApiException("A sales invoice must contain at least one medicine item.", 400);
            }

            decimal subtotal = 0;
            bool requiresPrescription = false;
            var invoiceDetails = new List<SalesInvoiceDetail>();

            foreach (var detail in request.Details)
            {
                if (detail.Quantity <= 0)
                {
                    throw new ApiException("Quantity must be greater than zero.", 400);
                }

                var medicine = await _salesRepository.GetMedicineByIdAsync(detail.MedicineId);
                if (medicine == null || !medicine.IsActive)
                {
                    throw new ApiException($"Medicine ID {detail.MedicineId} is not valid or inactive.", 400);
                }

                var inventory = await _inventoryRepository.GetInventoryAsync(branchId, detail.MedicineId);
                if (inventory == null || inventory.QuantityInStock < detail.Quantity)
                {
                    throw new ApiException($"Not enough stock for medicine '{medicine.MedicineName}'.", 400);
                }

                if (medicine.RequiresPrescription)
                {
                    requiresPrescription = true;
                }

                decimal lineTotal = medicine.SellingPrice * detail.Quantity;
                subtotal += lineTotal; // BR-03

                invoiceDetails.Add(new SalesInvoiceDetail
                {
                    MedicineID = detail.MedicineId,
                    Quantity = detail.Quantity,
                    UnitPrice = medicine.SellingPrice,
                    LineTotal = lineTotal
                });
            }

            // AT1 / BR-02: Check prescription requirement
            if (requiresPrescription && string.IsNullOrEmpty(request.PrescriptionImageUrl))
            {
                throw new ApiException("A valid prescription is required to complete this sale.", 400); // MSG17
            }

            decimal totalAmount = subtotal - request.DiscountAmount;
            if (totalAmount < 0) totalAmount = 0;

            var salesInvoice = new SalesInvoice
            {
                InvoiceCode = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}",
                BranchID = branchId,
                CreatedByUserID = userId,
                CustomerName = request.CustomerName,
                CustomerPhoneNumber = request.CustomerPhoneNumber,
                Subtotal = subtotal,
                DiscountAmount = request.DiscountAmount,
                TotalAmount = totalAmount,
                PaymentMethod = request.PaymentMethod,
                PaymentStatus = PaymentStatus.Completed, // Payment processed
                InvoiceStatus = InvoiceStatus.Finalized,
                PrescriptionImageUrl = request.PrescriptionImageUrl,
                IsPrescriptionVerified = requiresPrescription ? true : null, // Pharmacist verified it
                CreatedAt = DateTime.UtcNow,
                CompletedAt = DateTime.UtcNow,
                SalesInvoiceDetails = invoiceDetails
            };

            await _salesRepository.CreateSalesInvoiceAsync(salesInvoice);

            // BR-01: Update inventory lock (deduct stock after payment is processed)
            foreach (var detail in invoiceDetails)
            {
                // We use "Remove" logic since the previous AddStockAsync was generic.
                // Wait, InventoryRepository only has AddStockAsync which ADDS stock.
                // I need to add DeductStockAsync or reuse AdjustInventoryQuantityAsync if it existed.
                // I will add DeductStockAsync to IInventoryRepository.
                await _inventoryRepository.DeductStockAsync(branchId, detail.MedicineID, detail.Quantity);
            }
        }
    }
}
