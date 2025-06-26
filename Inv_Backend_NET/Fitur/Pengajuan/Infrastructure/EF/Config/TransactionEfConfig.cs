using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF.Config;

public class TransactionEfConfig : IEntityTypeConfiguration<TransactionEf>
{
    public void Configure(EntityTypeBuilder<TransactionEf> builder)
    {
        builder
            .Property(x => x.Status)
            .HasConversion(
                status => (int)status,
                statusCode => (TransactionStatus)statusCode);
        
        builder
            .HasIndex(x => new { x.Priorities , x.UpdatedAt });
        
        builder.HasIndex(x => x.UpdatedAt);
    }
}