using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Presentation.Dto;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Presentation;

public class PostTransactionRequestBodyValidationTest
{
    private readonly PostTransactionRequestBody _baseTestData = new();
    
    [Fact]
    public void Creator_Must_Exist()
    {
        PostTransactionRequestBody emptyBody = new();

        
        throw new NotImplementedException();
    }

    [Theory]
    [InlineData(true, TransactionType.Out, 1)]
    [InlineData(true, TransactionType.In, 0)]
    [InlineData(false, TransactionType.Out, 0)]
    [InlineData(false, TransactionType.In, 0)]
    public void Prepared_Quantity_Must_Exist_If_Creator_Is_Admin_And_Transaction_Type_Is_Out(
        bool isAdmin, TransactionType transactionType, int countError)
    {
        _baseTestData.SetUserCreator(isAdmin: isAdmin, id: 1);
        _baseTestData.TransactionType = transactionType == TransactionType.In ? "IN" : "OUT";
        _baseTestData.TransactionItems =
        [
            new PostTransactionItemReqeustBody()
        ];
        
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);
        
        Assert.Equal(countError, validationResults.Count(val =>
            val.Code == "PRES_TRANSACTION_ITEM_PREPARED_QUANTITY_MUST_EXIST"));
    }
    
    [Fact]
    public void Expected_Quantity_Is_Required()
    {
        _baseTestData.TransactionItems =
        [
            new PostTransactionItemReqeustBody(),
            new PostTransactionItemReqeustBody
            {
                ExpectedQuantity = 2,
            },
            new PostTransactionItemReqeustBody
            {
                PreparedQuantity = 3,
            }
        ];
        
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);
        
        Assert.Equal(2, validationResults.Count(val => 
                val.Code == "PRES_TRANSACTION_ITEM_EXPECTED_QUANTITY_MUST_EXIST"));
    }

    [Fact]
    public void Transaction_Item_Must_Not_Empty()
    {
        _baseTestData.TransactionItems = [];
        
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);

        Assert.Single(validationResults.Where(val => 
            val.Code == "PRES_TRANSACTION_ITEM_IS_EMPTY"));
    }

    [Fact]
    public void Transaction_Item_Must_Contains_Product_Id()
    {
        _baseTestData.TransactionItems = [
            new PostTransactionItemReqeustBody()
        ];
        
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);

        Assert.Single(validationResults.Where(val => 
            val.Code == "PRES_TRANSACTION_ITEM_PRODUCT_ID_IS_EMPTY"));
    }

    [Theory]
    [InlineData("IN", 0)]
    [InlineData("OUT", 0)]
    [InlineData("OTHER", 1)]
    public void Transaction_Type_Must_Either_IN_Or_OUT(string transactionType, int invalidTransactionTypeErrorCount)
    {
        _baseTestData.TransactionType = transactionType;
        
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);

        Assert.Equal(
            invalidTransactionTypeErrorCount,
            validationResults.Count(validationResult => 
                validationResult.Code == "PRES_TRANASCTION_TYPE_IS_INVALID")
        );
    }

    [Fact]
    public void Transaction_Type_Must_Be_Filled()
    {
        bool isValid = _baseTestData.TryValidate(out var validationResults);
        Assert.False(isValid);

        Assert.Single(validationResults.Where(val =>
            val.Code == "PRES_TRANSACTION_TYPE_IS_EMPTY"));
    }
}