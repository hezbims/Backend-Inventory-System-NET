using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CancelTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminCancelTransactionTest
{
    private readonly UserDto _admin = new UserDto(Id: 1, IsAdmin: true);
    private readonly UserDto _nonAdmin = new UserDto(Id: 2, IsAdmin: false);

    private readonly TransactionFactory _baseTransactionFactory;

    public AdminCancelTransactionTest()
    {
        _baseTransactionFactory = new TransactionFactory(
            Id: 1,
            TransactionTime: 23_000_563_329L,
            StakeholderId: 2,
            Type: TransactionType.Out,
            Status: TransactionStatus.Confirmed,
            CreatorId: _admin.Id,
            AssignedUserId: _admin.Id,
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 1, ProductId: 32, ExpectedQuantity: 5, PreparedQuantity: 3, Notes: ""),
                new TransactionItemFactory(
                    Id: 2, ProductId: 33, ExpectedQuantity: 4, PreparedQuantity: 0, Notes: ""),
                new TransactionItemFactory(
                    Id: 3, ProductId: 34, ExpectedQuantity: 1, PreparedQuantity: 1, Notes: "")
            ],
            Notes: "sebelum berubah");
    }
    
    [Fact]
    public void Admin_Can_Only_Cancel_Transaction_With_Status_Confirmed()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Confirmed, 
        }).Build();

        var sideEffects = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _admin, Notes: "Ada permintaan dari grup lain")).GetData();
        
        transaction.AssertTransactionFullData(
            id: 1,
            transactionTime: 23_000_563_329L,
            stakeholderId: 2,
            type: TransactionType.Out,
            status: TransactionStatus.Canceled,
            creatorId: _admin.Id,
            assignedUserId: _admin.Id,
            notes: "Ada permintaan dari grup lain",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 32, ExpectedQuantity: 5, 
                    PreparedQuantity: 3, 
                    Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 33, 
                    ExpectedQuantity: 4, 
                    PreparedQuantity: 0, 
                    Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 34, 
                    ExpectedQuantity: 1, 
                    PreparedQuantity: 1, 
                    Notes: "")
            ]);
        
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 32, Quantity: 3, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 34, Quantity: 1, Type: TransactionType.In),
        ]);
    }

    [Theory]
    [MemberData(nameof(TransactionStatusesGenerate.All) , MemberType = typeof(TransactionStatusesGenerate))]
    public void Admin_Can_Not_Cancel_Transaction_That_Assigned_To_Other_User_With_Status(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            AssignedUserId = _nonAdmin.Id,
            Status = status,
        }).Build();
        
        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _admin, Notes: "some notes")).GetError();

        Assert.Single(errors, error => error is CanNotCancelOtherUserTransaction);
    }

    [Fact]
    public void Admin_Can_Not_Cancel_Transaction_With_Status_Canceled()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Canceled,
        }).Build();

        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _admin, Notes: "Ku cancel ini coy")).GetError();

        Assert.Single(errors, error => error is TransactionCanNotCanceledTwiceError);
    }
    
    [Fact]
    public void Admin_Can_Not_Cancel_Rejected_Transaction()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Rejected,
        }).Build();

        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _admin, Notes: "Ku cancel ini coy")).GetError();

        Assert.Single(errors, error => error is RejectedTransactionCanNotCanceled);
    }

    [Fact]
    public void Admin_Can_Not_Cancel_Transaction_With_Empty_Notes()
    {
        Transaction transaction = _baseTransactionFactory.Build();
        
        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _admin, Notes: "")).GetError();
        
        Assert.Single(errors, error => error is CanNotCancelTransactionWithEmptyNotesError);
    }
}