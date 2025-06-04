using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

public record TransactionItemFactory(
    int Id,
    int ProductId,
    int ExpectedQuantity,
    int? PreparedQuantity,
    string Notes)
{
    public TransactionItem Build()
    {
        return new TransactionItem(
            id: Id,
            productId: ProductId,
            expectedQuantity: ExpectedQuantity,
            preparedQuantity: PreparedQuantity,
            notes: Notes);
    }
}