using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Group;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.UpdateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminUpdateTransactionTest
{
    private readonly UserDto _nonAdminPrimary;
    private readonly UserDto _nonAdminSecondary;
    private readonly Transaction _waitingTransaction;

    public NonAdminUpdateTransactionTest()
    {
        _nonAdminSecondary = new UserDto(IsAdmin: false, Id: 2);
        _nonAdminPrimary = new UserDto(IsAdmin: false, Id: 1);
        _nonAdminSecondary = new UserDto(IsAdmin: false, Id: 2);
        _waitingTransaction = new TransactionFactory(
            Id: 1,
            Type: TransactionType.Out,
            TransactionTime: 2,
            StakeholderId: 3,
            CreatorId: _nonAdminPrimary.Id,
            AssignedUserId: _nonAdminPrimary.Id,
            Notes: "Sebelum di update",
            Status: TransactionStatus.Waiting,
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 1, ProductId: 1, ExpectedQuantity: 3, PreparedQuantity: null, Notes: "Tolong ini dijadikan 2 karung"),
                new TransactionItemFactory(
                    Id: 2, ProductId: 2, ExpectedQuantity: 2, PreparedQuantity: null, Notes: "Tolong ini diplastikin ya"),
            ]
        ).Build();
    }
    
    [Fact]
    public void Non_Admin_Must_Be_Able_To_Update_Waiting_Transaction_Resulting_Correct_Transaction_Data()
    {
        _waitingTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 200,
            Group: new GroupDto(Id: 99, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "Saya datang nanti jam 9",
            TransactionItems: [
                new UpdateTransactionItemDto(ProductId: 45, Quantity: 25, Notes: ""),
                new UpdateTransactionItemDto(ProductId: 2, Quantity: 3, Notes: "Tolong ini diplastikin ya"),
                new UpdateTransactionItemDto(ProductId: 34, Quantity:25, Notes: "Hati-hati"),
            ]));
        
        _waitingTransaction.AssertTransactionFullData(
            id: 1,
            transactionTime: 200,
            stakeholderId: 99,
            type: TransactionType.Out,
            status: TransactionStatus.Waiting,
            assignedUserId: _nonAdminPrimary.Id,
            creatorId: _nonAdminPrimary.Id,
            notes: "Saya datang nanti jam 9",
            transactionItems: [
                new TransactionItemAssertionDto(ProductId: 45, ExpectedQuantity: 25, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(ProductId: 2, ExpectedQuantity: 3, PreparedQuantity: null, Notes: "Tolong ini diplastikin ya"),
                new TransactionItemAssertionDto(ProductId: 34, ExpectedQuantity:25, PreparedQuantity: null, Notes: "Hati-hati")]);
    }
    
    [Fact]
    public void Non_Admin_Must_Be_Able_To_Update_Waiting_Transaction_Without_Resulting_Any_Side_Effect()
    {
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = _waitingTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 200,
            Group: new GroupDto(Id: 99, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "Saya datang nanti jam 9",
            TransactionItems: [
                new UpdateTransactionItemDto(ProductId: 45, Quantity: 25, Notes: ""),
                new UpdateTransactionItemDto(ProductId: 2, Quantity: 3, Notes: "Tolong ini diplastikin ya"),
                new UpdateTransactionItemDto(ProductId: 34, Quantity:25, Notes: "Hati-hati"),
            ])).GetData();
        
        Assert.Empty(sideEffects);
    }

    [Theory]
    [InlineData(TransactionStatus.Canceled)]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Prepared)]
    public void Non_Admin_Must_Not_Be_Able_Update_Transaction_That_Has_Status_Other_Than_Waiting(
        TransactionStatus transactionStatus)
    {
        Transaction transaction = new TransactionFactory(
            Id: 24,
            Type: TransactionType.Out,
            TransactionTime: 25,
            StakeholderId: 99,
            Status: transactionStatus,
            CreatorId: _nonAdminPrimary.Id,
            AssignedUserId: _nonAdminPrimary.Id,
            Notes: "",
            TransactionItems: [
                new TransactionItemFactory(
                    ProductId: 23, 
                    ExpectedQuantity: 4, 
                    PreparedQuantity: transactionStatus == TransactionStatus.Canceled ? null : 3,
                    Notes: "",
                    Id: 34)
            ]).Build();

        var errors = transaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 32,
            Group: new GroupDto(Id: 37, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "Di update",
            TransactionItems:
            [
                new UpdateTransactionItemDto(ProductId: 23, Quantity: 25, Notes: ""),
            ])).GetError();

        Assert.Contains(errors, error => error is NonAdminCanNotUpdateNonWaitingTransactionError);
    }

    [Fact]
    public void Non_Admin_Must_Not_Be_Able_To_Update_Other_User_Transaction()
    {
        Transaction transaction = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            TransactionTime: 21,
            StakeholderId: 99,
            Creator: _nonAdminSecondary,
            Notes: "Punya secondary non-admin",
            TransactionItems: [
                new CreateTransactionItemDto(ProductId: 3, ExpectedQuantity: 34, PreparedQuantity: 234, Notes: "")
            ])).GetData().Item1;

        var errors = transaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 23,
            Group: new GroupDto(Id: 31, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "",
            TransactionItems:
            [
                new UpdateTransactionItemDto(ProductId: 3, Quantity: 56, Notes: ""),
            ]
        )).GetError();
        
        Assert.Contains(errors, error => error is NonAdminCanOnlyUpdateTheirOwnTransactionError);
    }

    [Fact]
    public void Non_Admin_Should_Not_Be_Able_To_Update_Transaction_With_Supplier_Group()
    {
        var errors = _waitingTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 32,
            Group: new GroupDto(Id: 37, IsSupplier: true),
            Updater: _nonAdminPrimary,
            Notes: "",
            TransactionItems:
            [
                new UpdateTransactionItemDto(ProductId: 3, Quantity: 34, Notes: "")
            ])).GetError();

        Assert.Contains(errors, error => error is NonAdminCanNotAssignSupplierGroupError);
    }

    [Fact]
    public void Non_Admin_Must_Not_Be_Able_To_Update_Waiting_Transaction_With_Empty_Transaction_Items()
    {
        var errors = _waitingTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 32,
            Group: new GroupDto(Id: 37, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "",
            TransactionItems: [])).GetError();

        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }

    [Fact]
    public void
        Non_Admin_Must_Not_Be_Able_To_Update_Waiting_Transaction_With_Transaction_Item_With_Less_Than_1_Quantity()
    {
        var errors = _waitingTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 32,
            Group: new GroupDto(Id: 37, IsSupplier: false),
            Updater: _nonAdminPrimary,
            Notes: "",
            TransactionItems: [
                new UpdateTransactionItemDto(ProductId: 1, Quantity: 1, Notes: ""),
                new UpdateTransactionItemDto(ProductId: 1, Quantity: 0, Notes: ""),
                new UpdateTransactionItemDto(ProductId: 1, Quantity: -2, Notes: ""),
            ])).GetError();

        var lessThan1QuantityErrors = errors.Where(error => 
            error is ExpectedQuantityMustGreaterThanZeroError)
            .Cast<ExpectedQuantityMustGreaterThanZeroError>()
            .ToList();
        
        Assert.Contains(lessThan1QuantityErrors, e => e.Index == 1);
        Assert.Contains(lessThan1QuantityErrors, e => e.Index == 2);
    }
}