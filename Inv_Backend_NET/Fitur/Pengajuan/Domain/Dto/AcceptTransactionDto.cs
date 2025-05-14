namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;

public record AcceptTransactionDto(
    long TransactionTime,
    int StakeholderId,
    UserDto Acceptor,
    IReadOnlyList<TransactionItemDto> TransactionItems);