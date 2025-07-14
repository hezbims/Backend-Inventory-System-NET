namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

internal sealed class PreparedQuantityMustNotNegativeError : TransactionItemError
{
    internal PreparedQuantityMustNotNegativeError(int index)
    {
        Index = index;
    }
}