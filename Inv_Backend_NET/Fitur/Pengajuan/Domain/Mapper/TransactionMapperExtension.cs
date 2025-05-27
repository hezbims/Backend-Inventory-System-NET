using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;

public static class TransactionMapperExtension
{
    public static List<ProductQuantityChangedEvent> ToProductQuantityChangedEvents(
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
}