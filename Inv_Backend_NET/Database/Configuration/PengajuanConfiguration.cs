using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Backend_NET.Database.Configuration;

public class PengajuanConfiguration : IEntityTypeConfiguration<Pengajuan>
{
    public void Configure(EntityTypeBuilder<Pengajuan> builder)
    {
        // SETTING PENGAJUAN
        builder
            .Property(e => e.Status)
            .HasConversion(
                e => e.Value,
                e => StatusPengajuan.From(e)
            );
        builder
            .HasIndex(e => e.KodeTransaksi)
            .IsUnique();
    }
}