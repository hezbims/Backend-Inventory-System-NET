namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

internal sealed class PreparedQuantityCantBeGreaterThanExpectedQuantityError : TransactionItemError
{
    internal PreparedQuantityCantBeGreaterThanExpectedQuantityError(int index)
    {
        Index = index;
    }
}