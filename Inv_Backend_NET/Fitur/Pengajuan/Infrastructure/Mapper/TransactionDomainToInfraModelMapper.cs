using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.Mapper;

internal static class TransactionDomainToInfraModelMapper
{
    internal static TransactionEf ToEfModel(
        this Transaction transaction,
        TimeProvider timeProvider)
    {
        return new TransactionEf
        {
            Id = transaction.Id,
            TransactionTime = transaction.TransactionTime,
            GroupId = transaction.StakeholderId,
            Status = transaction.Status,
            CreatorId = transaction.CreatorId,
            AssignedUserId = transaction.AssignedUserId,
            Notes = transaction.Notes,
            Priorities = transaction.Status == TransactionStatus.Waiting ? 1 : 0,
            UpdatedAt = timeProvider.GetUtcNow().Millisecond,
            CreatedAt = timeProvider.GetUtcNow().Millisecond,
        };
    }
}