namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;

public record TransactionItemsShouldNotContainsNegativeQuantity(
    List<int> ErrorIndices) : IBaseTransactionDomainError;