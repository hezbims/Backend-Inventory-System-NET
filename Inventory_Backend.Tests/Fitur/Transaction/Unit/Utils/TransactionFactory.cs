using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public record TransactionFactory(
    int Id,
    long TransactionTime,
    int StakeholderId,
    TransactionType Type,
    TransactionStatus Status,
    int CreatorId,
    int AssignedUserId,
    IReadOnlyList<TransactionItemFactory> TransactionItems,
    string Notes)
{
    public Transaction Build()
    {
        return new Transaction(
            id: Id,
            type: Type,
            transactionTime: TransactionTime,
            stakeholderId: StakeholderId,
            status: Status,
            creatorId: CreatorId,
            assignedUserId: AssignedUserId,
            notes: Notes,
            transactionItems: TransactionItems.Select(item =>
            {
                if (Status is TransactionStatus.Waiting or TransactionStatus.Rejected)
                    return (item with
                    {
                        PreparedQuantity = null,
                    }).Build();
                return item.Build();
            }).ToList());
    }
}