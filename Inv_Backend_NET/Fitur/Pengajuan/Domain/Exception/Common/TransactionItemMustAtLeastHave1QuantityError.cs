namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;

public record TransactionItemMustAtLeastHave1QuantityError(
    List<int> ErrorIndices) : IBaseTransactionDomainError;