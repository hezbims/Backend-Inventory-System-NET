namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;

public record UpdateTransactionDto(
    long TransactionTime,
    int StakeholderId,
    int UpdaterId, // User ID
    IReadOnlyList<TransactionItemDto> TransactionItems);