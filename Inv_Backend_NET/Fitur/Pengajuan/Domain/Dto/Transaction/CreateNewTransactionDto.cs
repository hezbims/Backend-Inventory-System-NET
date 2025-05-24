using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;

public record CreateNewTransactionDto(
    TransactionType TransactionType,
    long TransactionTime,
    int StakeholderId,
    UserDto Creator,
    string Notes,
    IReadOnlyList<CreateTransactionItemDto> TransactionItems,
    UserDto? AssignedUser = null);
