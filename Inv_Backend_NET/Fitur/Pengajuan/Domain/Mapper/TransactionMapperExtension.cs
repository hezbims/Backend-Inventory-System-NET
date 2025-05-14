using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Mapper;

public static class TransactionMapperExtension
{
    public static List<ProductQuantityChangedEvent> ToProductQuantityChangedEvents(
        this IEnumerable<TransactionItem> transactionItems,
        TransactionType transactionType)
    {
        return transactionItems.Select(
            transactionItem => 
                new ProductQuantityChangedEvent(
                    productId: transactionItem.ProductId,
                    quantity: transactionItem.Quantity,
                    type: transactionType)).ToList();
    }
}