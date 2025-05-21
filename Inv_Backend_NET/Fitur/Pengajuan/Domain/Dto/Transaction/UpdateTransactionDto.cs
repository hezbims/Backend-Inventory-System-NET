using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;

public record UpdateTransactionDto(
    long TransactionTime,
    int StakeholderId,
    int UpdaterId, // User ID
    IReadOnlyList<CreateTransactionItemDto> TransactionItems);