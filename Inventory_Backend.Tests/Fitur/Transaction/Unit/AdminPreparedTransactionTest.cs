using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminPreparedTransactionTest
{
    private readonly UserDto _adminUser;
    private readonly UserDto _nonAdminUser;
    private readonly Transaction _transaction;

    public AdminPreparedTransactionTest()
    {
        _adminUser = new UserDto(IsAdmin: true, Id: 1);
        _nonAdminUser = new UserDto(IsAdmin: false, Id: 2);
        _transaction = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out, 
            TransactionTime: 25,
            StakeholderId: 2,
            Creator: _nonAdminUser,
            TransactionItems: [
                new CreateTransactionItemDto(ProductId: 2, Quantity: 3, Notes: ""),
                new CreateTransactionItemDto(ProductId: 3, Quantity: 4, Notes: "ini notes"),
            ]
        )).GetData().Item1;
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Prepare_Transaction_And_Produce_Correct_Side_Effects()
    {
        var sideEffects = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser, TransactionItems: [
                new PrepareTransactionItemDto(Quantity: 1),
                new PrepareTransactionItemDto(Quantity: 0)]))
            .GetData();
        
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 2, Quantity: 3, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 3, Quantity: 4, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 2, Quantity: 1, Type: TransactionType.Out),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 3, Quantity: 0, Type: TransactionType.Out)
        ]);
    }
    
    [Fact]
    public void Admin_Should_Be_Able_To_Prepare_Transaction_And_Produce_Correct_Transaction_Data()
    {
        _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser, TransactionItems:
            [
                new PrepareTransactionItemDto(Quantity: 1),
                new PrepareTransactionItemDto(Quantity: 0)
            ]));
        
        _transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 25,
            stakeholderId: 2, 
            type: TransactionType.Out,
            status: TransactionStatus.Prepared, 
            creatorId: _nonAdminUser.Id, 
            assignedUserId: _nonAdminUser.Id, 
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 2, Quantity: 1, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 3, Quantity: 0, Notes: "ini notes"),
            ]);
    }

    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Prepared)]
    [InlineData(TransactionStatus.Canceled)]
    public void Admin_Should_Not_Be_Able_Prepare_Transaction_With_Status_Other_Than_Waiting(
        TransactionStatus status)
    {
        Transaction nonWaitingTransaction = new Transaction(
            id: 1,
            type: TransactionType.Out,
            transactionTime: 25_000_000L,
            stakeholderId: 2,
            status: status,
            creatorId: _nonAdminUser.Id,
            assignedUserId: _nonAdminUser.Id,
            transactionItems: 
            [
                new TransactionItem(productId: 2, quantity: 12, notes: "", id: 4)
            ]
        );

        var errors = nonWaitingTransaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            TransactionItems:
            [
                new PrepareTransactionItemDto(Quantity: 12)
            ])).GetError();

        Assert.Contains(errors, error => error is OnlyWaitingTransactionCanBePreparedError);
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Prepare_Transaction_With_Different_Transaction_Item_Size()
    {
        var errors = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            TransactionItems:
            [
                new PrepareTransactionItemDto(Quantity: 1),
                new PrepareTransactionItemDto(Quantity: 0),
                new PrepareTransactionItemDto(Quantity: 2)
            ])).GetError();

        Assert.Contains(errors , error => error is PreparedTransactionItemsSizeMustBeSameError);
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Prepare_Transaction_With_Negative_Transaction_Item_Quantity()
    {
        var errors = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            TransactionItems:
            [
                new PrepareTransactionItemDto(Quantity: -1),
                new PrepareTransactionItemDto(Quantity: 0),
            ])).GetError();

        Assert.Contains(errors , error => error is TransactionItemsShouldNotContainsNegativeQuantity);
    }
}