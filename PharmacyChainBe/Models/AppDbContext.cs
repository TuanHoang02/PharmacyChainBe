using Microsoft.EntityFrameworkCore;

namespace PharmacyChainBe.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Branch> Branches => Set<Branch>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Medicine> Medicines => Set<Medicine>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<Prescription> Prescriptions => Set<Prescription>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
        public DbSet<PurchaseRequestDetail> PurchaseRequestDetails => Set<PurchaseRequestDetail>();
        public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
        public DbSet<PurchaseOrderDetail> PurchaseOrderDetails => Set<PurchaseOrderDetail>();
        public DbSet<MedicineBatch> MedicineBatches => Set<MedicineBatch>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User - Role
            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            // User - Branch
            modelBuilder.Entity<User>()
                .HasOne(u => u.Branch)
                .WithMany(b => b.Users)
                .HasForeignKey(u => u.BranchID)
                .OnDelete(DeleteBehavior.SetNull);

            // User - Supplier
            modelBuilder.Entity<User>()
                .HasOne(u => u.Supplier)
                .WithMany(s => s.Users)
                .HasForeignKey(u => u.SupplierID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.CreatedUser)
                .WithMany()
                .HasForeignKey(b => b.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Branch>()
                .HasOne(b => b.UpdatedUser)
                .WithMany()
                .HasForeignKey(b => b.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.CreatedUser)
                .WithMany()
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.UpdatedUser)
                .WithMany()
                .HasForeignKey(c => c.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.CreatedUser)
                .WithMany()
                .HasForeignKey(m => m.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Medicine>()
                .HasOne(m => m.UpdatedUser)
                .WithMany()
                .HasForeignKey(m => m.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Medicine>()
                .HasIndex(m => m.MedicineCode)
                .IsUnique();

            modelBuilder.Entity<Inventory>()
                .HasOne(i => i.UpdatedUser)
                .WithMany()
                .HasForeignKey(i => i.UpdatedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Pharmacist)
                .WithMany()
                .HasForeignKey(o => o.PharmacistID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Pharmacist)
                .WithMany()
                .HasForeignKey(p => p.PharmacistID)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.RequestedUser)
                .WithMany()
                .HasForeignKey(pr => pr.RequestedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PurchaseRequest>()
                .HasOne(pr => pr.ApprovedUser)
                .WithMany()
                .HasForeignKey(pr => pr.ApprovedBy)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(po => po.CreatedUser)
                .WithMany()
                .HasForeignKey(po => po.CreatedBy)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
