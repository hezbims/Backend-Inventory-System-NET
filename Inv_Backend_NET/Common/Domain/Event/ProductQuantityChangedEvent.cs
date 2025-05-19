using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Common.Domain.Event;

public record ProductQuantityChangedEvent(
    int ProductId, int Quantity, TransactionType Type);