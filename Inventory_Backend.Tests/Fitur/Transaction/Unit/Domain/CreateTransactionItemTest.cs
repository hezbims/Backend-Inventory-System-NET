using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

public class CreateTransactionItemTest
{
    [Fact]
    public void Prepared_Quantity_Must_Not_Negative()
    {
        var result = TransactionItem.CreateNew(
            productId: 1, expectedQuantity: 0, preparedQuantity: -1, notes: "", index: 0);
        
        Assert.True(result.IsFailed());
        Assert.Single(result.GetError(), error => 
            error is PreparedQuantityMustNotNegativeError convertedError &&
            convertedError.Index == 0);
    }
    
    [Fact]
    public void Expected_Quantity_Must_At_Least_One()
    {
        var result = TransactionItem.CreateNew(
            productId: 1, expectedQuantity: 0, preparedQuantity: -1, notes: "", index: 0);
        
        Assert.True(result.IsFailed());
        Assert.Single(result.GetError(), error => 
            error is ExpectedQuantityMustGreaterThanZeroError convertedError &&
            convertedError.Index == 0);
    }
    
    [Fact]
    public void Prepared_Quantity_Cant_Greater_Than_Expected_Quantity()
    {
        var result = TransactionItem.CreateNew(
            productId: 1, expectedQuantity: 1, preparedQuantity: 2, notes: "", index: 0);
        
        Assert.True(result.IsFailed());
        Assert.Single(result.GetError(), error => 
            error is PreparedQuantityCantBeGreaterThanExpectedQuantityError convertedError &&
            convertedError.Index == 0);
    }
}