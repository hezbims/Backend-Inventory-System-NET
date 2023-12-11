using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Backend_NET.Database.Configuration;

public class KategoriConfiguration : IEntityTypeConfiguration<Kategori>
{
    public void Configure(EntityTypeBuilder<Kategori> builder)
    {
        // Ngekonfigurasi Kategori
        builder
            .HasIndex(kategori => kategori.Nama)
            .IsUnique();
    }
}