using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;

public record TransactionItemDto(int ProductId, int Quantity, string Notes);

public static class TransactionItemDtoExtensions
{
    public static List<TransactionItem> ToTransactionItemEntities(
        this IEnumerable<TransactionItemDto> items)
    {
        return items.Select(item => TransactionItem.CreateNew(
            productId: item.ProductId, quantity: item.Quantity, notes: item.Notes)).ToList();
    }
}