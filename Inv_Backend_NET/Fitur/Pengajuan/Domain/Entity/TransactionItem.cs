using Inventory_Backend_NET.Common.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

namespace Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;

using CreateResult = Result<TransactionItem, List<TransactionItemError>>;

internal sealed class TransactionItem
{
    public int Id { get; private init; }
    public int ProductId { get; private init; }
    public int ExpectedQuantity { get; private init; }
    public int? PreparedQuantity { get; private init; }
    public string Notes { get; private init; }

    private TransactionItem(
        int productId,
        int expectedQuantity,
        int? preparedQuantity,
        string notes,
        int id = 0)
    {
        Id = id;
        ProductId = productId;
        ExpectedQuantity = expectedQuantity;
        PreparedQuantity = preparedQuantity;
        Notes = notes;
    }

    internal static CreateResult CreateNew(
        int productId, int expectedQuantity, int? preparedQuantity, string notes, int index)
    {
        List<TransactionItemError> errors = [];
        if (expectedQuantity <= 0)
            errors.Add(new ExpectedQuantityMustGreaterThanZeroError(index));
        if (preparedQuantity < 0)
            errors.Add(new PreparedQuantityMustNotNegativeError(index));
        if (errors.Count == 0 && preparedQuantity > expectedQuantity)
            errors.Add(new PreparedQuantityCantBeGreaterThanExpectedQuantityError(index));
        
        if (errors.Count != 0)
            return new CreateResult.Failed(errors);
        
        return new CreateResult.Succeed(new TransactionItem(
            id: 0, 
            productId: productId,
            expectedQuantity: expectedQuantity,
            preparedQuantity: preparedQuantity,
            notes: notes));
    }
}