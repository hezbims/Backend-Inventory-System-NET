using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.RejectTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminRejectTransactionTest
{
    private readonly TransactionFactory _baseTransactionFactory;
    private readonly UserDto _admin = new(Id: 2, IsAdmin: true);
    private readonly UserDto _nonAdmin = new(Id: 3, IsAdmin: false);

    public AdminRejectTransactionTest()
    {
        _baseTransactionFactory = new TransactionFactory(
            Id: 1,
            Status: TransactionStatus.Waiting,
            Type: TransactionType.Out,
            TransactionTime: 25_000_000_000L,
            StakeholderId: 3,
            CreatorId: _admin.Id,
            AssignedUserId: _nonAdmin.Id,
            Notes: "",
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 2, ProductId: 3, ExpectedQuantity: 5, PreparedQuantity: 5, Notes: ""),
                new TransactionItemFactory(
                    Id: 3, ProductId: 4, ExpectedQuantity: 3, PreparedQuantity: 0, Notes: "sorry udah diambil sama divisi lain :("),
                new TransactionItemFactory(
                    Id:9, ProductId: 12, ExpectedQuantity: 3, PreparedQuantity: 2, Notes: ""),
            ]);
    }

    [Fact]
    public void Rejected_Transaction_Should_Result_In_Correct_Transaction_Data_And_Correct_Side_Effects()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Prepared,
        }).Build();

        var sideEffects = transaction.Reject(new RejectTransactionDto(
            Rejector: _admin, Notes: "pasti berhasil")).GetData();
        transaction.AssertTransactionFullData(
            id: 1,
            status: TransactionStatus.Rejected,
            type: TransactionType.Out,
            transactionTime: 25_000_000_000L,
            stakeholderId: 3,
            creatorId: _admin.Id,
            assignedUserId: _nonAdmin.Id,
            notes: "pasti berhasil",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 3, ExpectedQuantity: 5, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 4, ExpectedQuantity: 3, PreparedQuantity: null, Notes: "sorry udah diambil sama divisi lain :("),
                new TransactionItemAssertionDto(
                    ProductId: 12, ExpectedQuantity: 3, PreparedQuantity: null, Notes: ""),
            ]);
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 3, Quantity: 5, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 12, Quantity: 2, Type: TransactionType.In),
        ]);
    }
    
    [Fact]
    public void Rejected_Waiting_Transaction_Should_Result_In_Correct_Transaction_Data_And_Empty_Side_Effects()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = TransactionStatus.Waiting,
        }).Build();

        var sideEffects = transaction.Reject(new RejectTransactionDto(
            Rejector: _admin, Notes: "pasti berhasil")).GetData();
        transaction.AssertTransactionFullData(
            id: 1,
            status: TransactionStatus.Rejected,
            type: TransactionType.Out,
            transactionTime: 25_000_000_000L,
            stakeholderId: 3,
            creatorId: _admin.Id,
            assignedUserId: _nonAdmin.Id,
            notes: "pasti berhasil",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 3, ExpectedQuantity: 5, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 4, ExpectedQuantity: 3, PreparedQuantity: null, Notes: "sorry udah diambil sama divisi lain :("),
                new TransactionItemAssertionDto(
                    ProductId: 12, ExpectedQuantity: 3, PreparedQuantity: null, Notes: ""),
            ]);
        Assert.Empty(sideEffects);
    }
    
    [Theory]
    [MemberData(nameof(TransactionStatusesGenerate.NoWaitingAndPrepared) , MemberType = typeof(TransactionStatusesGenerate))]
    public void Admin_Can_Not_Reject_Transaction_With_Status_Other_Than_Waiting_And_Prepared(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = status,
        }).Build();

        var errors = transaction.Reject(new RejectTransactionDto(
            Rejector: _admin, Notes: "Coba Reject")).GetError();
        Assert.Single(errors, error => error is OnlyWaitingAndPreparedTransactionCanBeRejectedError);
    }
    
    [Theory]
    [MemberData(nameof(TransactionStatusesGenerate.All) , MemberType = typeof(TransactionStatusesGenerate))]
    public void Admin_Can_Not_Reject_Transaction_With_Empty_Rejection_Notes(TransactionStatus status)
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Status = status,
        }).Build();

        var errors = transaction.Reject(new RejectTransactionDto(
            Rejector: _admin, Notes: "")).GetError();
        Assert.Single(errors, error => error is RejectionNotesMustNotBeEmptyError);
    }
}