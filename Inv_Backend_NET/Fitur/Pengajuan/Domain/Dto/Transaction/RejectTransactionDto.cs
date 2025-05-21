using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;

public record RejectTransactionDto(
    UserDto Rejector,
    List<string> Notes);