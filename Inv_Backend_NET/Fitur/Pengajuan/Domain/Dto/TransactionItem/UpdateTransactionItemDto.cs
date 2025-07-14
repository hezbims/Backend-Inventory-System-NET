namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

internal sealed record UpdateTransactionItemDto(int ProductId, int Quantity, string Notes);