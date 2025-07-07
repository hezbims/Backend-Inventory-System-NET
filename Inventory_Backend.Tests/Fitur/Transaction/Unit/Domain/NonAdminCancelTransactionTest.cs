using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CancelTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminCancelTransactionTest
{
    private readonly UserDto _primaryNonAdmin = new UserDto(Id: 1, IsAdmin: false);
    private readonly UserDto _secondaryNonAdmin = new UserDto(Id: 2, IsAdmin: false);
    private readonly TransactionFactory _baseTransactionFactory;

    public NonAdminCancelTransactionTest()
    {
        _baseTransactionFactory = new TransactionFactory(
            Id: 1,
            TransactionTime: 23_000_563_329L,
            StakeholderId: 2,
            Type: TransactionType.Out,
            Status: TransactionStatus.Confirmed,
            CreatorId: _primaryNonAdmin.Id,
            AssignedUserId: _primaryNonAdmin.Id,
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 1, ProductId: 32, ExpectedQuantity: 5, PreparedQuantity: 3, Notes: "apa ya.."),
                new TransactionItemFactory(
                    Id: 2, ProductId: 33, ExpectedQuantity: 4, PreparedQuantity: 0, Notes: ""),
                new TransactionItemFactory(
                    Id: 3, ProductId: 34, ExpectedQuantity: 1, PreparedQuantity: 1, Notes: "")
            ],
            Notes: "sebelum berubah");
    }
        
    #region Positive Case
    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Prepared)]
    public void Non_Admin_Canceling_Transaction_Must_Result_In_Correct_Transaction_Data_When_Transaction_Status(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = status,
        }).Build();
        
        transaction.CancelTransaction(
            new CancelTransactionDto(Cancelator: _primaryNonAdmin, Notes: "Tidak jadi, sudah dapat")).GetData();
        
        transaction.AssertTransactionFullData(
            id: 1,
            transactionTime: 23_000_563_329L,
            stakeholderId: 2,
            type: TransactionType.Out,
            status: TransactionStatus.Canceled,
            creatorId: _primaryNonAdmin.Id,
            assignedUserId: _primaryNonAdmin.Id,
            notes: "Tidak jadi, sudah dapat",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 32, 
                    ExpectedQuantity: 5,
                    PreparedQuantity: 3,
                    Notes: "apa ya.."),
                new TransactionItemAssertionDto(
                    ProductId: 33,
                    ExpectedQuantity: 4,
                    PreparedQuantity: 0,
                    Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 34,
                    ExpectedQuantity: 1,
                    PreparedQuantity: 1,
                    Notes: ""),
            ]);
    }
    
    [Fact]
    public void Non_Admin_Canceling_Transaction_Must_Result_In_Correct_Transaction_Data_When_Transaction_Status_Is_Waiting()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Waiting,
        }).Build();
        
        transaction.CancelTransaction(
            new CancelTransactionDto(Cancelator: _primaryNonAdmin, Notes: "Tidak jadi, sudah dapat")).GetData();
        
        transaction.AssertTransactionFullData(
            id: 1,
            transactionTime: 23_000_563_329L,
            stakeholderId: 2,
            type: TransactionType.Out,
            status: TransactionStatus.Canceled,
            creatorId: _primaryNonAdmin.Id,
            assignedUserId: _primaryNonAdmin.Id,
            notes: "Tidak jadi, sudah dapat",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 32, 
                    ExpectedQuantity: 5,
                    PreparedQuantity: null,
                    Notes: "apa ya.."),
                new TransactionItemAssertionDto(
                    ProductId: 33,
                    ExpectedQuantity: 4,
                    PreparedQuantity: null,
                    Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 34,
                    ExpectedQuantity: 1,
                    PreparedQuantity: null,
                    Notes: ""),
            ]);
    }

    [Fact]
    public void
        Non_Admin_Cancel_Transaction_Should_Have_No_Side_Effects_When_Transaction_Have_No_Prepared_Quantities_Status_Is_Waiting()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Waiting,
        }).Build();
        
        var sideEffects = transaction.CancelTransaction(
            new CancelTransactionDto(Cancelator: _primaryNonAdmin, Notes: "some notes")).GetData();
        Assert.Empty(sideEffects);
    }
    
    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Prepared)]
    public void
        Non_Admin_Cancel_Transaction_Should_Have_Side_Effects_When_Transaction_Have_Prepared_Quantities_And_Has_Status(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = status,
        }).Build();
        
        var sideEffects = transaction.CancelTransaction(
            new CancelTransactionDto(Cancelator: _primaryNonAdmin, Notes: "some notes")).GetData();
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 32, Quantity: 3, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 34, Quantity: 1, Type: TransactionType.In),
        ]);
    }
    #endregion

    #region Negative Case
    [Fact]
    public void Non_Admin_Can_Not_Cancel_Canceled_Transaction()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Canceled,
        }).Build();

        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _primaryNonAdmin, Notes: "coba cancel canceled")).GetError();

        Assert.Single(errors, error => error is TransactionCanNotCanceledTwiceError);
    }

    [Fact]
    public void Non_Admin_Can_Not_Cancel_Rejected_Transaction()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Rejected,
        }).Build();

        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _primaryNonAdmin, Notes: "coba cancel rejected")).GetError();

        Assert.Single(errors, error => error is RejectedTransactionCanNotCanceled);
    }

    [Fact]
    public void Non_Admin_Can_Not_Cancel_Transaction_With_Empty_Notes()
    {
        Transaction transaction = _baseTransactionFactory.Build();
        
        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _primaryNonAdmin, Notes: "")).GetError();
        Assert.Single(errors, error => error is CanNotCancelTransactionWithEmptyNotesError);
    }

    
    [Theory]
    [MemberData(nameof(TransactionStatusesGenerate.All), MemberType = typeof(TransactionStatusesGenerate))]
    public void Non_Admin_Can_Not_Cancel_Other_User_Transaction(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            AssignedUserId = _secondaryNonAdmin.Id,
            Status = status,
        }).Build();

        var errors = transaction.CancelTransaction(new CancelTransactionDto(
            Cancelator: _primaryNonAdmin, Notes: "Some cancelation notes...")).GetError();

        Assert.Single(errors, error => error is CanNotCancelOtherUserTransaction);
    }
    #endregion
}