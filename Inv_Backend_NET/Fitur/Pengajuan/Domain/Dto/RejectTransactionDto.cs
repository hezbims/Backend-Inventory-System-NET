namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;

public record RejectTransactionDto(
    UserDto Rejector,
    List<string> Notes);