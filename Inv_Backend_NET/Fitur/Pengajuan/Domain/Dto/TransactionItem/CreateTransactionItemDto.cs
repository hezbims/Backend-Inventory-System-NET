namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

internal sealed record CreateTransactionItemDto(int ProductId, int Quantity, string Notes);

internal static class TransactionItemDtoExtensions
{
    internal static List<Entity.TransactionItem> ToTransactionItemEntities(
        this IEnumerable<CreateTransactionItemDto> items,
        bool isAdminCreation)
    {
        return items.Select(item => Entity.TransactionItem.CreateNew(
            productId: item.ProductId, 
            expectedQuantity: item.Quantity,
            preparedQuantity: isAdminCreation ? item.Quantity : null,
            notes: item.Notes)).ToList();
    }
}