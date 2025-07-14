using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

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
        _transaction = new TransactionFactory(
            Id: 1,
            Type: TransactionType.Out,
            Status: TransactionStatus.Waiting,
            TransactionTime: 25,
            StakeholderId: 2,
            CreatorId: _nonAdminUser.Id,
            AssignedUserId: _nonAdminUser.Id,
            Notes: "Saya ambil 15 menit sebelum istirahat",
            TransactionItems: [
                new TransactionItemFactory(Id: 1, ProductId: 2, ExpectedQuantity: 3, PreparedQuantity: null, Notes: ""),
                new TransactionItemFactory(Id: 2, ProductId: 3, ExpectedQuantity: 4, PreparedQuantity: null, Notes: "ini notes"),
            ]
        ).Build();
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Prepare_Waiting_Transaction_And_Produce_Correct_Side_Effects()
    {
        var sideEffects = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Notes: "",
            Preparator: _adminUser, TransactionItems: [
                new PrepareTransactionItemDto(PreparedQuantity: 1),
                new PrepareTransactionItemDto(PreparedQuantity: 0)]))
            .GetData();
        
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 2, Quantity: 1, Type: TransactionType.Out),
        ]);
    }
    
    [Fact]
    public void Admin_Should_Be_Able_To_Prepare_Transaction_And_Produce_Correct_Transaction_Data()
    {
        _transaction.PrepareTransaction(new PrepareTransactionDto(
            Notes: "Beberapa barang udah diambil grup lain",
            Preparator: _adminUser, TransactionItems:
            [
                new PrepareTransactionItemDto(PreparedQuantity: 1),
                new PrepareTransactionItemDto(PreparedQuantity: 0)
            ]));
        
        _transaction.AssertTransactionFullData(
            id: 1, 
            transactionTime: 25,
            stakeholderId: 2, 
            type: TransactionType.Out,
            status: TransactionStatus.Prepared, 
            creatorId: _nonAdminUser.Id, 
            assignedUserId: _nonAdminUser.Id,
            notes: "Beberapa barang udah diambil grup lain",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 2, ExpectedQuantity: 3, PreparedQuantity: 1, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 3, ExpectedQuantity: 4, PreparedQuantity: 0, Notes: "ini notes"),
            ]);
    }

    [Theory]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Prepared)]
    [InlineData(TransactionStatus.Canceled)]
    public void Admin_Should_Not_Be_Able_Prepare_Transaction_With_Status_Other_Than_Waiting(
        TransactionStatus status)
    {
        Transaction nonWaitingTransaction = new TransactionFactory(
            Id: 1,
            Type: TransactionType.Out,
            TransactionTime: 25_000_000L,
            StakeholderId: 2,
            Status: status,
            CreatorId: _nonAdminUser.Id,
            AssignedUserId: _nonAdminUser.Id,
            Notes: "Semuanya dikemas dalam karung",
            TransactionItems: 
            [
                new TransactionItemFactory(
                    ProductId: 2, 
                    ExpectedQuantity: 12,
                    PreparedQuantity: status == TransactionStatus.Prepared ? 12 : null,
                    Notes: "", 
                    Id: 4)
            ]
        ).Build();

        var errors = nonWaitingTransaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            Notes: "Hmm...",
            TransactionItems:
            [
                new PrepareTransactionItemDto(PreparedQuantity: 12)
            ])).GetError();

        Assert.Contains(errors, error => error is OnlyWaitingTransactionCanBePreparedError);
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Prepare_Transaction_With_Different_Transaction_Item_Size()
    {
        var errors = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            Notes: "humm",
            TransactionItems:
            [
                new PrepareTransactionItemDto(PreparedQuantity: 1),
                new PrepareTransactionItemDto(PreparedQuantity: 0),
                new PrepareTransactionItemDto(PreparedQuantity: 2)
            ])).GetError();

        Assert.Contains(errors , error => error is TransactionItemsSizeMustBeSameError);
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Prepare_Transaction_With_Negative_Transaction_Item_Quantity()
    {
        var errors = _transaction.PrepareTransaction(new PrepareTransactionDto(
            Preparator: _adminUser,
            Notes: "seharusnya error nih",
            TransactionItems:
            [
                new PrepareTransactionItemDto(PreparedQuantity: -1),
                new PrepareTransactionItemDto(PreparedQuantity: 0),
            ])).GetError();

        var error = (PreparedQuantityMustNotNegativeError) errors.Single();
        Assert.Equal(0, error.Index);
    }
}