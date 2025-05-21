using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;

public record PrepareTransactionDto(
    UserDto Preparator,
    IReadOnlyList<PrepareTransactionItemDto> TransactionItems);