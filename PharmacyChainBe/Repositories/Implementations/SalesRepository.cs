using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;
using PharmacyChainBe.Repositories.Interfaces;

namespace PharmacyChainBe.Repositories.Implementations
{
    public class SalesRepository : ISalesRepository
    {
        private readonly AppDbContext _context;

        public SalesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalesInvoice> CreateSalesInvoiceAsync(SalesInvoice invoice)
        {
            _context.SalesInvoices.Add(invoice);
            await _context.SaveChangesAsync();
            return invoice;
        }

        public async Task<Medicine?> GetMedicineByIdAsync(int medicineId)
        {
            return await _context.Medicines.FirstOrDefaultAsync(m => m.MedicineID == medicineId);
        }
    }
}
