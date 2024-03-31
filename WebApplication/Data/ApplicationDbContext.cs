using WebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }

        public DbSet<Branch> Branches { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Retailer> Retailers { get; set; }
        public DbSet<CategorySubType> CategorySubTypes { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.firstName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.lastName)
                .HasMaxLength(250);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.isActive)
                .HasColumnName("isActive")
                .HasColumnType("bit")
                .IsRequired(false);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Transaction)
                .WithOne()
                .HasForeignKey<Order>(o => o.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ApplicationUser>()
                .Property(e => e.GroupId)
                .HasColumnName("GroupId")
                .HasColumnType("int")
                .IsRequired(false);

        }
    }
}