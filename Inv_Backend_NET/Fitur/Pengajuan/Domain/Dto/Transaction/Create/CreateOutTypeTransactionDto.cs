using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction.Create;

internal sealed record CreateOutTypeTransactionDto(
    long TransactionTime,
    int StakeholderId,
    UserDto Creator,
    string Notes,
    IReadOnlyList<CreateOutTypeTransactionItemDto> TransactionItems,
    UserDto? AssignedUser);
