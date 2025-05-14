using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Common.Domain.Event;

public class ProductQuantityChangedEvent
{
    public int ProductId { get; private set; }
    public int Quantity { get; private set; }
    public TransactionType Type { get; private set; }

    public ProductQuantityChangedEvent(int productId, int quantity, TransactionType type)
    {
        ProductId = productId;
        Type = type;
    }
}