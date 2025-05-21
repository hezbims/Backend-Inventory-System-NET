using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public static class TransactionDomainAssertionUtils
{
    public static void AssertTransactionFullData(
        this Transaction transaction,
        int id, long transactionTime, int stakeholderId,
        TransactionType type, TransactionStatus status, int creatorId,
        int assignedUserId, IReadOnlyList<TransactionItemAssertionDto> transactionItems)
    {
        Assert.Equal(id, transaction.Id);
        Assert.Equal(transactionTime, transaction.TransactionTime);
        Assert.Equal(stakeholderId, transaction.StakeholderId);
        Assert.Equal(type, transaction.Type);
        Assert.Equal(status, transaction.Status);
        Assert.Equal(creatorId, transaction.CreatorId);
        Assert.Equal(assignedUserId, transaction.AssignedUserId);
        Assert.Equal(transactionItems.Count, transaction.TransactionItems.Count);
        for (int i = 0 ; i < transactionItems.Count; i++)
        {
           var expectedItem = transactionItems[i];
           var actualItem = transaction.TransactionItems[i];
           Assert.Equal(expectedItem.ProductId, actualItem.ProductId);
           Assert.Equal(expectedItem.Quantity, actualItem.Quantity);
           Assert.Equal(expectedItem.Notes, actualItem.Notes);
        }
    }

    public static void AssertAll(
        this IReadOnlyList<ProductQuantityChangedEvent> actualEvents,
        IReadOnlyList<ProductQuantityChangedEventAssertionDto> expectedEvents)
    {
        Assert.Equal(expectedEvents.Count(), actualEvents.Count());
        for (int i = 0; i < actualEvents.Count(); i++)
        {
            var actualEvent = actualEvents[i];
            var expectedEvent = expectedEvents[i];
            Assert.Equal(expectedEvent.ProductId, actualEvent.ProductId);
            Assert.Equal(expectedEvent.Quantity, actualEvent.Quantity);
            Assert.Equal(expectedEvent.Type, actualEvent.Type);
        }
    }
}

public static class TransactionAssertionMapper
{
    public static List<ProductQuantityChangedEventAssertionDto> ToAssertionDtos(
        this IEnumerable<CreateTransactionItemDto> transactionItemDtos,
        TransactionType transactionType)
    {
        return transactionItemDtos.Select(item =>
            new ProductQuantityChangedEventAssertionDto(
                ProductId: item.ProductId, Quantity: item.Quantity, Type: transactionType))
            .ToList();
    }
    
    public static List<TransactionItemAssertionDto> ToAssertionDtos(
        this IEnumerable<CreateTransactionItemDto> transactionItemDtos)
    {
        return transactionItemDtos.Select(item =>
                new TransactionItemAssertionDto(
                    ProductId: item.ProductId, Quantity: item.Quantity, Notes: item.Notes))
            .ToList();
    }
}

public record TransactionItemAssertionDto(
    int ProductId, int Quantity, string Notes);

public record ProductQuantityChangedEventAssertionDto(
    int ProductId, int Quantity, TransactionType Type);