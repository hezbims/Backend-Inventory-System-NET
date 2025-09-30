using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;

namespace Inventory_Backend.Tests.Fitur.Transaction.Integration._Utils;

public static class TransactionDbAssertions
{
    public static void AssertData(
        this TransactionEf transaction,
        long transactionTime,    
        int groupId,
        TransactionStatus status,
        int creatorId,
        int assignedUserId,
        string notes,
        // Supaya Transaction yang statusnya waiting, dilihat paling atas oleh admin
        int priorities,
        long updatedAt,
        long createdAt
    )
    {
        Assert.NotEqual(0, transaction.Id);
        Assert.Equal(transactionTime, transaction.TransactionTime);
        Assert.Equal(groupId, transaction.GroupId);
        Assert.Equal(status, transaction.Status);
        Assert.Equal(creatorId, transaction.CreatorId);
        Assert.Equal(assignedUserId, transaction.AssignedUserId);
        Assert.Equal(notes, transaction.Notes);
        Assert.Equal(priorities, transaction.Priorities);
        Assert.Equal(updatedAt, transaction.UpdatedAt);
        Assert.Equal(createdAt, transaction.CreatedAt);
    }

    public static void AssertContains(
        this IEnumerable<TransactionItemEf> actualTransactionItems,
        params TransactionItemEf[] expectedTransactionItems)
    {
        var actualTransactionItemList = actualTransactionItems.ToList();
        foreach (TransactionItemEf expectedItem in expectedTransactionItems)
        {
            Assert.NotNull(actualTransactionItemList.FirstOrDefault(
                item =>
                    item.Id != 0
                    && item.TransactionId == expectedItem.TransactionId
                    && item.ProductId == expectedItem.ProductId
                    && item.ExpectedQuantity == expectedItem.ExpectedQuantity
                    && item.PreparedQuantity == expectedItem.PreparedQuantity
                    && item.Notes == expectedItem.Notes));
        }
    }
}