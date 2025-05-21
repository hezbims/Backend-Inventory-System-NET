using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CreateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminCreateNewTransactionTest
{
    private readonly UserDto _nonAdmin = new UserDto(Id: 1, IsAdmin: false);    

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
                new CreateTransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new CreateTransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
            ]
        ));

        var data = result.GetData();
        Assert.Empty(data.Item2);
    }

    [Fact]  
    public void TransactionDataShouldCreatedCorrectly()
    {
        IReadOnlyList<CreateTransactionItemDto> transactionItems =
        [
            new CreateTransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
            new CreateTransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
        ];
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            TransactionTime: 1_000_200L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            TransactionItems: transactionItems
        ));

        var data = result.GetData();
        Transaction transaction = data.Item1;
        transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 1_000_200L, 
            stakeholderId: 1, 
            type: TransactionType.Out, 
            status: TransactionStatus.Waiting, 
            creatorId: _nonAdmin.Id, 
            assignedUserId: _nonAdmin.Id, 
            transactionItems: transactionItems.ToAssertionDtos());
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
                new CreateTransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new CreateTransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
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