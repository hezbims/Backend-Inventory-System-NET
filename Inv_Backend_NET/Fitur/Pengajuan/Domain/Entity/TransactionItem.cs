namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

public class TransactionItem
{
    public int Id { get; private init; }
    public int ProductId { get; private init; }
    public int Quantity { get; private init; }
    public string Notes { get; private init; }

    public TransactionItem(
        int productId,
        int quantity,
        string notes,
        int id = 0)
    {
        Id = id;
        ProductId = productId;
        Quantity = quantity;
        Notes = notes;
    }

    public static TransactionItem CreateNew(
        int productId, int quantity, string notes)
    {
        return new TransactionItem(
            id: 0, 
            productId: productId,
            quantity: quantity,
            notes: notes);
    }
}