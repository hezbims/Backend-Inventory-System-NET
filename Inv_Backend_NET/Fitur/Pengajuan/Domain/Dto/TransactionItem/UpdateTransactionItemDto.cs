namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

public record UpdateTransactionItemDto(int? ProductId, int Quantity, string? Notes);