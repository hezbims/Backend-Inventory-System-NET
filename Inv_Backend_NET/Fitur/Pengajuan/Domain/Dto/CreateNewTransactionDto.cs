using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;

public record CreateNewTransactionDto(
    TransactionType TransactionType,
    long TransactionTime,
    int StakeholderId,
    UserDto Creator,
    IReadOnlyList<TransactionItemDto> TransactionItems);