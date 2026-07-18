using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.DTOs;
using PharmacyChainBe.DTOs.Request;
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

        public async Task<PagedResponse<List<SalesInvoice>>> GetPagedAsync(SalesHistoryQuery query, CancellationToken cancellationToken = default)
        {
            IQueryable<SalesInvoice> dbQuery = _context.SalesInvoices.AsNoTracking();

            // Search by CustomerPhoneNumber
            if (!string.IsNullOrWhiteSpace(query.CustomerPhoneNumber))
            {
                var phonePattern = query.CustomerPhoneNumber.Trim();
                dbQuery = dbQuery.Where(x => x.CustomerPhoneNumber != null && x.CustomerPhoneNumber.Contains(phonePattern));
            }

            // Filter by InvoiceStatus
            if (query.InvoiceStatus.HasValue)
            {
                dbQuery = dbQuery.Where(x => x.InvoiceStatus == query.InvoiceStatus.Value);
            }

            // Sorting
            bool isDesc = query.IsDescending;
            string sortBy = (query.SortBy ?? string.Empty).Trim().ToLower();

            if (sortBy == "totalamount" || sortBy == "amount")
            {
                dbQuery = isDesc ? dbQuery.OrderByDescending(x => x.TotalAmount) : dbQuery.OrderBy(x => x.TotalAmount);
            }
            else
            {
                // Default to CreatedAt descending if not specified or invalid
                dbQuery = isDesc ? dbQuery.OrderByDescending(x => x.CreatedAt) : dbQuery.OrderBy(x => x.CreatedAt);
            }

            int totalRecords = await dbQuery.CountAsync(cancellationToken);
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var data = await dbQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<List<SalesInvoice>>
            {
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalRecords = totalRecords
            };
        }

        public async Task<SalesInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.SalesInvoices
                .AsNoTracking()
                .Include(x => x.Branch)
                .Include(x => x.CreatedByUser)
                .Include(x => x.SalesInvoiceDetails)
                    .ThenInclude(d => d.Medicine)
                .FirstOrDefaultAsync(x => x.SalesInvoiceID == id, cancellationToken);
        }
    }
}
