using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Backend_NET.Database.Configuration;

public class BarangConfiguration : IEntityTypeConfiguration<Barang>
{
    public void Configure(EntityTypeBuilder<Barang> builder)
    {
        builder
            .HasIndex(barang => barang.Nama)
            .IsUnique();
        
        builder
            .HasIndex(barang => barang.KodeBarang)
            .IsUnique();
        
        builder
            .HasIndex(barang => new
            {
                barang.NomorRak,
                barang.NomorLaci,
                barang.NomorKolom
            })
            .IsUnique();
    }
}