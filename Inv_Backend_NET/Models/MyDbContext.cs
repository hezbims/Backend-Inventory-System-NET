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
        public DbSet<Pengajuan> Pengajuans { get; set; } = null!;
        public DbSet<Pengaju> Pengajus { get; set; } = null!;
        public DbSet<BarangAjuan> BarangAjuans { get; set; } = null!;
        public DbSet<StatusPengajuan> StatusPengajuans { get; set; } = null!;
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Ngekonfigurasi Barang
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

            modelBuilder.Entity<Pengajuan>()
                .Property(e => e.Status)
                .HasConversion(
                    e => e.Value,
                    e => StatusPengajuan.From(e)
                );


            modelBuilder.Entity<StatusPengajuan>().HasData(
                StatusPengajuan.Diterima,
                StatusPengajuan.Menunggu,
                StatusPengajuan.Ditolak
            );
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
