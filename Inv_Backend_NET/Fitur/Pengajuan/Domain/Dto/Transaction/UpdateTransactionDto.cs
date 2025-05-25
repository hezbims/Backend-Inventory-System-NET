using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Group;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;

public record UpdateTransactionDto(
    long TransactionTime,
    GroupDto Group,
    UserDto Updater, // User ID
    string Notes,
    IReadOnlyList<UpdateTransactionItemDto> TransactionItems);