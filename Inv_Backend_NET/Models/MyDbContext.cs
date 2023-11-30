using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options): base(options)
        { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Kategori> Kategoris { get; set; } = null!;
        public DbSet<Barang> Barangs { get; set; } = null!;
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Barang>()
                .HasIndex(barang => barang.Nama)
                .IsUnique();
            modelBuilder.Entity<Barang>()
                .HasIndex(barang => barang.KodeBarang)
                .IsUnique();
            modelBuilder.Entity<Barang>()
                .HasIndex(barang => new
                {
                    barang.NomorRak,
                    barang.NomorLaci,
                    barang.NomorKolom
                })
                .IsUnique();
            
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "123",
                    IsAdmin = true,
                },
                new User
                {
                    Id = 2,
                    Username = "hezbi",
                    Password = "123",
                    IsAdmin = false,
                }
            );
        }
    }
}
