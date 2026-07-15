using Microsoft.EntityFrameworkCore;
using PharmacyChainBe.Models;

namespace PharmacyChainBe.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var context = new AppDbContext(serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

            // Add Roles
            if (!await context.Roles.AnyAsync())
            {
                var roles = new List<Role>
                {
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "OperationsManager" },
                    new Role { RoleName = "Pharmacist" },
                    new Role { RoleName = "BranchManager" },
                    new Role { RoleName = "Supplier" }
                };

                await context.Roles.AddRangeAsync(roles);
                await context.SaveChangesAsync();
            }

            var rolesList = await context.Roles.ToListAsync();

            // Add Admin User first because we need it for CreatedBy in Branches
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@gmail.com");
            if (adminUser == null)
            {
                var adminRole = rolesList.FirstOrDefault(r => r.RoleName == "Admin");
                if (adminRole != null)
                {
                    adminUser = new User
                    {
                        FullName = "System Administrator",
                        Email = "admin@gmail.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                        Phone = "0123456789",
                        RoleID = adminRole.RoleID,
                        BranchID = null, // Explicitly ensure Admin does not belong to any branch
                        Status = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();
                }
            }

            // Add Branches
            if (adminUser != null && !await context.Branches.AnyAsync())
            {
                var branches = new List<Branch>
                {
                    new Branch
                    {
                        BranchName = "Cau Giay Branch",
                        Address = "123 Trần Duy Hưng, Phường Yên Hòa, Quận Cầu Giấy, Hà Nội",
                        Phone = "02437651234",
                        Status = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = adminUser.UserID
                    },
                    new Branch
                    {
                        BranchName = "Thanh Xuan Branch",
                        Address = "88 Nguyễn Trãi, Phường Thanh Xuân Trung, Quận Thanh Xuân, Hà Nội",
                        Phone = "02435551234",
                        Status = true,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = adminUser.UserID
                    }
                };

                await context.Branches.AddRangeAsync(branches);
                await context.SaveChangesAsync();
            }

            var branchesList = await context.Branches.ToListAsync();
            var mainBranch = branchesList.FirstOrDefault(b => b.BranchName == "Cau Giay Branch");
            var secondBranch = branchesList.FirstOrDefault(b => b.BranchName == "Thanh Xuan Branch");

            // Add other Users
            var seedUsers = new List<(string Email, string Name, string Password, string RoleName, string Phone, int? BranchId)>
            {
                ("branchmanager@gmail.com", "Store Manager", "123456", "BranchManager", "0987654321", mainBranch?.BranchID),
                ("pharmacist@gmail.com", "Senior Pharmacist", "123456", "Pharmacist", "0112233445", mainBranch?.BranchID),
                ("pharmacist2@gmail.com", "Junior Pharmacist", "123456", "Pharmacist", "0223344556", secondBranch?.BranchID),
                ("opsmanager@gmail.com", "Operations Manager", "123456", "OperationsManager", "0556677889", null),
                ("supplieruser@gmail.com", "Supplier Rep", "123456", "Supplier", "0667788990", null)
            };

            foreach (var u in seedUsers)
            {
                if (!await context.Users.AnyAsync(x => x.Email == u.Email))
                {
                    var role = rolesList.FirstOrDefault(r => r.RoleName == u.RoleName);
                    if (role != null)
                    {
                        var newUser = new User
                        {
                            FullName = u.Name,
                            Email = u.Email,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(u.Password),
                            Phone = u.Phone,
                            RoleID = role.RoleID,
                            BranchID = u.BranchId,
                            Status = true,
                            CreatedAt = DateTime.UtcNow
                        };
                        await context.Users.AddAsync(newUser);
                    }
                }
            }
            await context.SaveChangesAsync();
        }
    }
}
