namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

internal sealed class ExpectedQuantityMustGreaterThanZeroError : TransactionItemError
{
    internal ExpectedQuantityMustGreaterThanZeroError(int index)
    {
        Index = index;
    }
}