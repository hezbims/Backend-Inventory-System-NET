namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;

internal sealed class TransactionItemsShouldNotContainsNegativeQuantity : IBaseTransactionDomainError
{
    public required int Index { get; init; }
}