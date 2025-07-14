using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;

internal static class TransactionMapperExtension
{
    internal static List<ProductQuantityChangedEvent> ToProductQuantityChangedEvents(
        this IEnumerable<TransactionItem> transactionItems,
        TransactionType transactionType)
    {
        return transactionItems
            .Where(transactionItem =>
                transactionItem.PreparedQuantity is > 0)
            .Select(
                transactionItem =>
                    new ProductQuantityChangedEvent(
                        ProductId: transactionItem.ProductId,
                        Quantity: transactionItem.PreparedQuantity!.Value,
                        Type: transactionType)).ToList();
    }

    internal static List<TransactionItem> ToTransactionItems(
        this List<Result<TransactionItem, List<TransactionItemError>>> createTransactionItemsResult)
    {
        return createTransactionItemsResult.Select(
            item => item.GetData()).ToList();
    }

    internal static List<TransactionItemError> GetErrors(
        this List<Result<TransactionItem, List<TransactionItemError>>> createTransactionItemsResult)
    {
        return createTransactionItemsResult
            .Where(item => item.IsFailed())
            .SelectMany(
            item => item.GetError()).ToList();
    }

}