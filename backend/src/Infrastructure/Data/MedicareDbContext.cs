using Medicare.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Medicare.Infrastructure.Data
{
    public class MedicareDbContext : DbContext
    {
        public MedicareDbContext(DbContextOptions<MedicareDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<OrderRequest> OrderRequests { get; set; }
        public DbSet<OrderRequestItem> OrderRequestItems { get; set; }
        public DbSet<SiteContent> SiteContents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);
            modelBuilder.Entity<User>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Menu configuration
            modelBuilder.Entity<Menu>()
                .HasKey(m => m.Id);
            modelBuilder.Entity<Menu>()
                .Property(m => m.DateApplied)
                .IsRequired();
            modelBuilder.Entity<Menu>()
                .HasMany(m => m.Items)
                .WithOne(mi => mi.Menu)
                .HasForeignKey(mi => mi.MenuId)
                .OnDelete(DeleteBehavior.Cascade);

            // MenuItem configuration
            modelBuilder.Entity<MenuItem>()
                .HasKey(mi => mi.Id);
            modelBuilder.Entity<MenuItem>()
                .Property(mi => mi.Name)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<MenuItem>()
                .Property(mi => mi.Price)
                .HasPrecision(10, 2);

            // OrderRequest configuration
            modelBuilder.Entity<OrderRequest>()
                .HasKey(or => or.Id);
            modelBuilder.Entity<OrderRequest>()
                .Property(or => or.CustomerName)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<OrderRequest>()
                .Property(or => or.CustomerPhone)
                .IsRequired()
                .HasMaxLength(20);
            modelBuilder.Entity<OrderRequest>()
                .Property(or => or.TotalPrice)
                .HasPrecision(10, 2);
            modelBuilder.Entity<OrderRequest>()
                .HasMany(or => or.Items)
                .WithOne(oi => oi.OrderRequest)
                .HasForeignKey(oi => oi.OrderRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            // OrderRequestItem configuration
            modelBuilder.Entity<OrderRequestItem>()
                .HasKey(oi => oi.Id);
            modelBuilder.Entity<OrderRequestItem>()
                .Property(oi => oi.UnitPrice)
                .HasPrecision(10, 2);
            modelBuilder.Entity<OrderRequestItem>()
                .Property(oi => oi.Subtotal)
                .HasPrecision(10, 2);

            // SiteContent configuration
            modelBuilder.Entity<SiteContent>()
                .HasKey(sc => sc.Id);
            modelBuilder.Entity<SiteContent>()
                .Property(sc => sc.Key)
                .IsRequired()
                .HasMaxLength(255);
            modelBuilder.Entity<SiteContent>()
                .HasIndex(sc => sc.Key)
                .IsUnique();
        }
    }
}
