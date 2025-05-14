using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.PostTransactionTests.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminCreateNewTransactionTest
{
    private UserDto _nonAdmin = new UserDto(Id: 1, IsAdmin: false);    

    #region Positive Case
    [Fact]
    public void ShouldNotHaveSideEffects()
    {
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            TransactionItems: [
                new TransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new TransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
            ]
        ));

        var data = result.GetData();
        Assert.Empty(data.Item2);
    }

    [Fact]  
    public void TransactionDataShouldCreatedCorrectly()
    {
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            TransactionTime: 1_000_200L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            TransactionItems: [
                new TransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new TransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
            ]
        ));

        var data = result.GetData();
        Transaction transaction = data.Item1;
        Assert.Equal(0, transaction.Id);
        Assert.Equal(1_000_200L, transaction.TransactionTime);
        Assert.Equal(1, transaction.StakeholderId);
        Assert.Equal(TransactionType.Out, transaction.TransactionType);
        Assert.Equal(TransactionStatus.Waiting, transaction.Status);
        Assert.Equal(_nonAdmin.Id, transaction.CreatorId);
        
        Assert.Equal(0, transaction.TransactionItems[0].Id);
        Assert.Equal(1, transaction.TransactionItems[0].ProductId);
        Assert.Equal(3, transaction.TransactionItems[0].Quantity);
        Assert.Equal("", transaction.TransactionItems[0].Notes);
        
        Assert.Equal(0, transaction.TransactionItems[1].Id);
        Assert.Equal(2, transaction.TransactionItems[1].ProductId);
        Assert.Equal(4, transaction.TransactionItems[1].Quantity);
        Assert.Equal("Tolong diplastikin", transaction.TransactionItems[1].Notes);
    }
    #endregion

    #region Negative Case
    [Fact]
    public void ShouldNotCreateInTypeTransaction()
    {
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.In,
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            TransactionItems: [
                new TransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new TransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
            ]
        ));

        var errors = result.GetError();
        Assert.Contains(errors, error => error is UserNonAdminShouldNotCreateTransactionOfTypeInError);
    }

    [Fact]
    public void ShouldNotCreateTransactionWithEmptyTransactionItems()
    {
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.In,
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            TransactionItems: []
        ));

        var errors = result.GetError();
        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }
    #endregion
}