namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

public record CreateTransactionItemDto(int ProductId, int Quantity, string Notes);

public static class TransactionItemDtoExtensions
{
    public static List<Entity.TransactionItem> ToTransactionItemEntities(
        this IEnumerable<CreateTransactionItemDto> items)
    {
        return items.Select(item => Entity.TransactionItem.CreateNew(
            productId: item.ProductId, quantity: item.Quantity, notes: item.Notes)).ToList();
    }
}