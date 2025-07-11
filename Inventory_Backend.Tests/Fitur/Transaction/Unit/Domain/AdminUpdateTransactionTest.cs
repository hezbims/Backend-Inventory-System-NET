using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Group;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.UpdateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminUpdateTransactionTest
{
    private readonly UserDto _admin = new(Id: 23, IsAdmin: true);
    private readonly Transaction _preparedTransaction = new(
        id: 25,
        transactionTime: 25_000_000L,
        stakeholderId: 24,
        type: TransactionType.Out,
        status: TransactionStatus.Prepared,
        creatorId: 55,
        assignedUserId: 55,
        notes: "",
        transactionItems:  [
            new TransactionItem(
                productId: 4, expectedQuantity: 5, preparedQuantity: 3, notes: ""),
            new TransactionItem(
                productId: 5, expectedQuantity: 1, preparedQuantity: 1, notes: "kode-738")
        ]
    );

    [Fact]
    public void Admin_Should_Be_Able_To_Update_Prepared_Transaction_Resulting_In_Correct_Transaction_Data()
    {
        _preparedTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: 999, // seharusnya diabaikan
            Group: new GroupDto(Id: 999, IsSupplier: true), // seharusnya diabaikan
            Updater: _admin,
            Notes: "Barang ke-2 sudah diambil divisi lain. Barang ke-1 salah ketik quantitynya barusan",
            TransactionItems: [
                new UpdateTransactionItemDto(
                    ProductId: -1, Quantity: 4, Notes: "seharusnya diabaikan"),
                new UpdateTransactionItemDto(
                    ProductId: -1, Quantity: 0, Notes: "seharusnya diabaikan"),
            ]));
        
        _preparedTransaction.AssertTransactionFullData(
            id: 25,
            transactionTime: 25_000_000L,
            stakeholderId: 24,
            type: TransactionType.Out,
            status: TransactionStatus.Prepared,
            creatorId: 55,
            assignedUserId: 55,
            notes: "Barang ke-2 sudah diambil divisi lain. Barang ke-1 salah ketik quantitynya barusan",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 4, ExpectedQuantity: 5, PreparedQuantity: 4, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 5, ExpectedQuantity: 1, PreparedQuantity: 0, Notes: "kode-738"),
            ]);
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Update_Prepared_Transaction_Resulting_Correct_Side_Effects()
    {
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = 
            _preparedTransaction.UpdateTransaction(new UpdateTransactionDto(
                TransactionTime: null,
                Group: null,
                Updater: _admin,
                Notes: "Seharusnya menghasilkan side effect yang benar",
                TransactionItems:
                [
                    new UpdateTransactionItemDto(
                        ProductId: null, Quantity: 1, Notes: null),
                    new UpdateTransactionItemDto(
                        ProductId: null, Quantity: 0, Notes: null)
                ])).GetData();
        
        Assert.Equal(3 , sideEffects.Count);
        sideEffects.AssertAll([
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 4, Quantity: 3, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 5, Quantity: 1, Type: TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 4, Quantity: 1, Type: TransactionType.Out),
        ]);
    }
    
    [Theory]
    [InlineData(TransactionStatus.Canceled)]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Waiting)]
    public void Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Status_Other_Than_Prepared(
        TransactionStatus status)
    {
        Transaction nonPreparedTransaction = new Transaction(
            id: 98,
            type: TransactionType.Out,
            stakeholderId: 98,
            transactionTime: 23_987_897_999L,
            status: status,
            creatorId: 79,
            assignedUserId: 403,
            notes: "",
            transactionItems:
            [
                new TransactionItem(
                    productId: 4, expectedQuantity: 5, preparedQuantity: 3, notes: ""),
                new TransactionItem(
                    productId: 5, expectedQuantity: 1, preparedQuantity: 1, notes: "kode-738")
            ]);

        var errors = nonPreparedTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: null,
            Group: null,
            Updater: _admin,
            Notes: "seharusnya gagal",
            TransactionItems:
            [
                new UpdateTransactionItemDto(
                    ProductId: null, Quantity: 3, Notes: null),
                new UpdateTransactionItemDto(
                    ProductId: null, Quantity: 1, Notes: null)
            ])).GetError();

        Assert.Contains(errors, error => error is AdminCanOnlyUpdatePreparedTransaction);
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Different_Size_Of_Transaction_Items()
    {
        var errors = _preparedTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: null,
            Group: null,
            Updater: _admin,
            Notes: "seharusnya gagal",
            TransactionItems: [])).GetError();

        Assert.Contains(errors, error => error is TransactionItemsSizeMustBeSameError);
    }

    [Fact]
    public void
        Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Transaction_Item_That_Has_Less_Than_0_Quantity()
    {
        var errors = _preparedTransaction.UpdateTransaction(new UpdateTransactionDto(
            TransactionTime: null,
            Group: null,
            Updater: _admin,
            Notes: "seharusnya gagal",
            TransactionItems:
            [
                new UpdateTransactionItemDto(
                    ProductId: null, Quantity: -1, Notes: null),
                new UpdateTransactionItemDto(
                    ProductId: null, Quantity: 0, Notes: null)
            ])).GetError();

        var error = (TransactionItemsShouldNotContainsNegativeQuantity) errors.Single(
            error => error is TransactionItemsShouldNotContainsNegativeQuantity);
        Assert.Equal(0, error.Index);
    }
}