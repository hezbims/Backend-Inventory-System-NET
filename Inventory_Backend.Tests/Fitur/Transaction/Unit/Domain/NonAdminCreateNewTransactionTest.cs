using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction.Create;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CreateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminCreateNewTransactionTest
{
    private readonly UserDto _nonAdmin = new UserDto(Id: 1, IsAdmin: false);    

    #region Positive Case
    [Fact]
    public void ShouldNotHaveSideEffects()
    {
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            Notes: "seharusnya gk ada side effect",
            TransactionItems: [
                new CreateOutTypeTransactionItemDto(ProductId: 1, ExpectedQuantity: 3, PreparedQuantity: 2, Notes: ""),
                new CreateOutTypeTransactionItemDto(ProductId: 2, ExpectedQuantity: 4, PreparedQuantity: null, Notes: "Tolong diplastikin")
            ],
            AssignedUser: null
        ));

        var data = result.GetData();
        Assert.Empty(data.Item2);
    }

    [Fact]  
    public void TransactionDataShouldCreatedCorrectly()
    {
        IReadOnlyList<CreateOutTypeTransactionItemDto> transactionItems =
        [
            new (ProductId: 1, ExpectedQuantity: 3, PreparedQuantity: 234324, Notes: ""),
            new (ProductId: 2, ExpectedQuantity: 4, PreparedQuantity: null, Notes: "Tolong diplastikin")
        ];
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            TransactionTime: 1_000_200L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            Notes: "non-admin ngebuat",
            TransactionItems: transactionItems,
            AssignedUser: null
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
            notes: "non-admin ngebuat",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 1, ExpectedQuantity: 3, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 2, ExpectedQuantity: 4, PreparedQuantity: null, Notes: "Tolong diplastikin"),
            ]);
    }
    #endregion

    #region Negative Case
    [Fact]
    public void ShouldNotCreateInTypeTransaction()
    {
        var result = Transaction.CreateInTypeTransaction(new CreateInTypeTransactionDto(
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            Notes: "seharusnya gagal",
            TransactionItems: [
                new CreateInTypeTransactionItemDto(ProductId: 1, Quantity: 3, Notes: ""),
                new CreateInTypeTransactionItemDto(ProductId: 2, Quantity: 4, Notes: "Tolong diplastikin")
            ]
        ));

        var errors = result.GetError();
        Assert.Contains(errors, error => error is UserNonAdminShouldNotCreateTransactionOfTypeInError);
    }

    [Fact]
    public void ShouldNotCreateTransactionWithEmptyTransactionItems()
    {
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            Notes: "seharusnya enggak bisa",
            TransactionItems: [],
            AssignedUser: null
        ));

        var errors = result.GetError();
        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }

    [Fact]
    public void Must_Not_Create_Transaction_Item_With_Expected_Quantity_Less_Than_1()
    {
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            TransactionTime: 0L,
            StakeholderId: 1,
            Creator: _nonAdmin,
            Notes: "seharusnya enggak bisa",
            TransactionItems: 
            [
                new CreateOutTypeTransactionItemDto(ProductId: 3, ExpectedQuantity: 0, PreparedQuantity: 23234, Notes: ""),
                new CreateOutTypeTransactionItemDto(ProductId: 4, ExpectedQuantity: -1, PreparedQuantity: null,Notes: ""),
                new CreateOutTypeTransactionItemDto(ProductId: 5, ExpectedQuantity: 5, PreparedQuantity: 47289347, Notes: "Yang ini valid"),
            ],
            AssignedUser: null
        ));

        var errors = result.GetError();
        var lessThan1QuantityErrors = errors.Where(error => 
                error is ExpectedQuantityMustGreaterThanZeroError)
            .Cast<ExpectedQuantityMustGreaterThanZeroError>()
            .ToList();
        Assert.Equal(2, lessThan1QuantityErrors.Count);
        Assert.Contains(lessThan1QuantityErrors , e => e.Index == 1);
        Assert.Contains(lessThan1QuantityErrors , e => e.Index == 0);
    }
    #endregion
}