using Microsoft.EntityFrameworkCore;

namespace PharmacyChainBe.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<MedicineBatch> MedicineBatches { get; set; }
        public DbSet<SalesInvoice> SalesInvoices { get; set; }
        public DbSet<SalesInvoiceDetail> SalesInvoiceDetails { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PurchaseRequestDetail> PurchaseRequestDetails { get; set; }
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration for relationships to avoid multiple cascade paths
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Supplier)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            // SalesInvoice relationships
            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.CreatedByUser)
                .WithMany(u => u.SalesInvoices)
                .HasForeignKey(si => si.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<SalesInvoice>()
                .HasOne(si => si.Branch)
                .WithMany(b => b.SalesInvoices)
                .HasForeignKey(si => si.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseRequest relationships
            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.CreatedByUser)
                .WithMany(u => u.CreatedPurchaseRequests)
                .HasForeignKey(pr => pr.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.ReviewedByUser)
                .WithMany(u => u.ReviewedPurchaseRequests)
                .HasForeignKey(pr => pr.ReviewedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.Branch)
                .WithMany(b => b.PurchaseRequests)
                .HasForeignKey(pr => pr.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            // PurchaseOrder relationships
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.CreatedByUser)
                .WithMany(u => u.CreatedPurchaseOrders)
                .HasForeignKey(po => po.CreatedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.ReceivedByUser)
                .WithMany(u => u.ReceivedPurchaseOrders)
                .HasForeignKey(po => po.ReceivedByUserID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Branch)
                .WithMany(b => b.PurchaseOrders)
                .HasForeignKey(po => po.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.Supplier)
                .WithMany(s => s.PurchaseOrders)
                .HasForeignKey(po => po.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            // MedicineBatch relationships
            modelBuilder.Entity<MedicineBatch>()
                .HasOne(mb => mb.Branch)
                .WithMany(b => b.MedicineBatches)
                .HasForeignKey(mb => mb.BranchID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicineBatch>()
                .HasOne(mb => mb.Supplier)
                .WithMany(s => s.MedicineBatches)
                .HasForeignKey(mb => mb.SupplierID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicineBatch>()
                .HasOne(mb => mb.Medicine)
                .WithMany(m => m.MedicineBatches)
                .HasForeignKey(mb => mb.MedicineID)
                .OnDelete(DeleteBehavior.Restrict);

            // Inventory relationships
            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.Branch)
                .WithMany(b => b.Inventories)
                .HasForeignKey(i => i.BranchID)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
