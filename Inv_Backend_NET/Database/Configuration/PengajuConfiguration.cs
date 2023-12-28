using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Backend_NET.Database.Configuration;

public class PengajuConfiguration : IEntityTypeConfiguration<Pengaju>
{
    public void Configure(EntityTypeBuilder<Pengaju> builder)
    {
        builder
            .HasIndex(e => e.Nama)
            .IsUnique();
    }
}