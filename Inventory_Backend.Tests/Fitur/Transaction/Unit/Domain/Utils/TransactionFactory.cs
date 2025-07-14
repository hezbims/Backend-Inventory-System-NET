using System.Reflection;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

internal sealed record TransactionFactory(
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
    internal Transaction Build()
    {
        var transactionConstructor = typeof(Transaction).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [
                typeof(int), 
                typeof(TransactionType), 
                typeof(long), 
                typeof(int),
                typeof(TransactionStatus), 
                typeof(int), 
                typeof(int), 
                typeof(string),
                typeof(List<TransactionItem>)
            ], 
            null
        )!;

        return (Transaction) transactionConstructor.Invoke([
            Id, 
            Type, 
            TransactionTime, 
            StakeholderId,
            Status, 
            CreatorId, 
            AssignedUserId, 
            Notes,
            TransactionItems.Select(item => item.Build(Status)).ToList(),
        ]);
    }
}