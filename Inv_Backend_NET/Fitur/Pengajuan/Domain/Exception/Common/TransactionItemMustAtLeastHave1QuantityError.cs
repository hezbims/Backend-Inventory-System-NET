namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;

internal sealed class TransactionItemMustAtLeastHave1QuantityError : IBaseTransactionDomainError
{
    public required int Index { get; init; }
}