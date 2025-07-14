namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

internal abstract class TransactionItemError : IBaseTransactionDomainError
{
    public int Index { get; protected init; }
}