using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction.Create;

internal sealed record CreateInTypeTransactionDto(
    long TransactionTime,
    int StakeholderId,
    UserDto Creator,
    string Notes,
    IReadOnlyList<CreateInTypeTransactionItemDto> TransactionItems);