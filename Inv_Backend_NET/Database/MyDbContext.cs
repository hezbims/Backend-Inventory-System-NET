﻿using Inventory_Backend_NET.Database.Interceptor;
using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Database
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

        public DbSet<TotalPengajuanByTanggal> TotalPengajuanByTanggals { get; set; } = null!;
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StatusPengajuan>().HasData(
                StatusPengajuan.Diterima,
                StatusPengajuan.Menunggu,
                StatusPengajuan.Ditolak
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.AddInterceptors(new CreateNewPengajuanInterceptor());
            base.OnConfiguring(optionsBuilder);
        }
    }
}
