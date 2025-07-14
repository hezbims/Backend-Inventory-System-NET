using System.Reflection;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

internal sealed record TransactionItemFactory(
    int Id,
    int ProductId,
    int ExpectedQuantity,
    int? PreparedQuantity,
    string Notes)
{
    public TransactionItem Build(TransactionStatus transactionStatus)
    {
        var transactionItemConstructor = typeof(TransactionItem).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            [
                typeof(int),
                typeof(int),
                typeof(int),
                typeof(string),
                typeof(int)
            ],
            null
        )!;

        return (TransactionItem)transactionItemConstructor.Invoke([
            ProductId,
            ExpectedQuantity,
            transactionStatus is TransactionStatus.Waiting or TransactionStatus.Rejected ?
                null : PreparedQuantity,
            Notes,
            Id,
        ]);
    }
}