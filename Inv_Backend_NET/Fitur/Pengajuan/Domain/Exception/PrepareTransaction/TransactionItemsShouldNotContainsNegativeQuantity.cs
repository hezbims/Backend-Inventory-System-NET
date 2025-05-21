namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;

public class TransactionItemsShouldNotContainsNegativeQuantity(
    List<int> ErrorIndices) : IBaseTransactionDomainError;