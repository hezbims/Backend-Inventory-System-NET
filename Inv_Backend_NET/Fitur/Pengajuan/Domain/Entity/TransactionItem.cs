namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

public class TransactionItem
{
    public int Id { get; private init; }
    public int ProductId { get; private init; }
    public int ExpectedQuantity { get; private init; }
    public int? PreparedQuantity { get; private init; }
    public string Notes { get; private init; }

    public TransactionItem(
        int productId,
        int expectedQuantity,
        int? preparedQuantity,
        string notes,
        int id = 0)
    {
        Id = id;
        ProductId = productId;
        ExpectedQuantity = expectedQuantity;
        PreparedQuantity = preparedQuantity;
        Notes = notes;
    }

    public static TransactionItem CreateNew(
        int productId, int expectedQuantity, int? preparedQuantity, string notes)
    {
        return new TransactionItem(
            id: 0, 
            productId: productId,
            expectedQuantity: expectedQuantity,
            preparedQuantity: preparedQuantity,
            notes: notes);
    }
}