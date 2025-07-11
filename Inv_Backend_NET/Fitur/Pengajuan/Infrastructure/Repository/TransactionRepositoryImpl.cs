using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.Repository;

internal sealed class TransactionRepositoryImpl(
    MyDbContext dbContext,
    TimeProvider timeProvider)
{
    public void Save(Transaction transaction)
    {
        TransactionEf transactionEf = new TransactionEf
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
            TransactionItems = transaction.TransactionItems.Select(item => new TransactionItemEf
            {
                Id = item.Id,
                ProductId = item.ProductId,
                Notes = item.Notes,
                ExpectedQuantity = item.ExpectedQuantity,
                PreparedQuantity = item.PreparedQuantity,
                TransactionId = transaction.Id,
            }).ToList(),
        };
        
        dbContext.Transactions.Add(transactionEf);
    }
}