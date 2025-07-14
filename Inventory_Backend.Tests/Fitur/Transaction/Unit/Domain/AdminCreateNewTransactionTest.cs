using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction.Create;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.CreateTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminCreateNewTransactionTest
{
    private readonly UserDto _userAdmin = new UserDto(Id: 99, IsAdmin: true);
    private readonly UserDto _assignedNonAdminUser = new UserDto(Id: 64, IsAdmin: false);
    
    #region Positive Case
    [Fact]
    public void ShouldResultingInCorrectSideEffects_When_Transaction_Type_Is_OUT()
    {
        IReadOnlyList<CreateOutTypeTransactionItemDto> transactionItems =
        [
            new (ProductId: 23, ExpectedQuantity: 6, PreparedQuantity: 5, Notes: "Kuambil 5"),
            new (ProductId: 24, ExpectedQuantity: 3, PreparedQuantity: 3, Notes: ""),
            new (ProductId: 25, ExpectedQuantity: 8, PreparedQuantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            Notes: "Ini buatan admin",
            TransactionItems: transactionItems,
            AssignedUser: null));
        
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = result.GetData().Item2;

        sideEffects.AssertAll(expectedEvents: [
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 23, Quantity: 5, TransactionType.Out),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 24, Quantity: 3, TransactionType.Out),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 25, Quantity: 7, TransactionType.Out),
        ]);
    }
    
    [Fact]
    public void ShouldResultingInCorrectSideEffects_When_Transaction_Type_Is_IN()
    {
        IReadOnlyList<CreateInTypeTransactionItemDto> transactionItems =
        [
            new (ProductId: 23, Quantity: 6, Notes: "Kuambil 5"),
            new (ProductId: 24, Quantity: 3, Notes: ""),
            new (ProductId: 25, Quantity: 8, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateInTypeTransaction(new CreateInTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            Notes: "Ini buatan admin",
            TransactionItems: transactionItems));
        
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = result.GetData().Item2;

        sideEffects.AssertAll(expectedEvents: [
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 23, Quantity: 6, TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 24, Quantity: 3, TransactionType.In),
            new ProductQuantityChangedEventAssertionDto(
                ProductId: 25, Quantity: 8, TransactionType.In),
        ]);
    }

    
    /// <summary>
    /// Status harus confirmed dan assigned user harus admin
    /// </summary>
    [Fact]
    public void Should_Create_Correct_Transaction_Data_When_Assigned_User_Is_Not_Exists_And_Transaction_Type_Is_OUT()
    {
        IReadOnlyList<CreateOutTypeTransactionItemDto> transactionItems = [
            new(
                ProductId: 25, 
                ExpectedQuantity: 8,
                PreparedQuantity: 7, 
                Notes: "Humm.. 😐")];
        
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems,
            Notes: "Ini buatan admin",
            AssignedUser: null));

        Transaction transaction = result.GetData().Item1;
        
        transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 12,
            stakeholderId: 1, 
            type: TransactionType.Out,
            creatorId: _userAdmin.Id, 
            assignedUserId: _userAdmin.Id,
            status: TransactionStatus.Confirmed,
            notes: "Ini buatan admin",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 25,
                    ExpectedQuantity: 8,
                    PreparedQuantity:  7,
                    Notes: "Humm.. 😐"
                )
            ]);
    }
    
    
    /// <summary>
    /// Status harus confirmed dan assigned user harus balik ke creatornya lagi, selain itu prepared quantitynya bakal ikut expected quantity
    /// </summary>
    [Fact]
    public void ShouldCreateCorrectTransactionData_When_Assigned_User_Is_Non_Admin_And_Transaction_Type_Is_In()
    {
        IReadOnlyList<CreateInTypeTransactionItemDto> transactionItems = [
            new(
                ProductId: 25,
                Quantity: 8,
                Notes: "Humm.. 😐")];
        
        var result = Transaction.CreateInTypeTransaction(new CreateInTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems,
            Notes: "Ini buatan admin"));

        Transaction transaction = result.GetData().Item1;
        
        transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 12,
            stakeholderId: 1, 
            type: TransactionType.In,
            creatorId: _userAdmin.Id, 
            assignedUserId: _userAdmin.Id,
            status: TransactionStatus.Confirmed,
            notes: "Ini buatan admin",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 25,
                    ExpectedQuantity: 8,
                    PreparedQuantity: 8,
                    Notes: "Humm.. 😐"
                )
            ]);
    }
    
    /// <summary>
    /// Status harus prepared dan assigned user harus terassign non-admin
    /// </summary>
    [Fact]
    public void ShouldCreateCorrectTransactionData_When_Assigned_User_Is_Non_Admin_And_Transaction_Type_Is_Out()
    {
        IReadOnlyList<CreateOutTypeTransactionItemDto> transactionItems = [
            new(
                ProductId: 25, 
                ExpectedQuantity: 8,
                PreparedQuantity: 7, 
                Notes: "Humm.. 😐")];
        
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems,
            Notes: "Ini buatan admin",
            AssignedUser: _assignedNonAdminUser));

        Transaction transaction = result.GetData().Item1;
        
        transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 12,
            stakeholderId: 1, 
            type: TransactionType.Out,
            creatorId: _userAdmin.Id, 
            assignedUserId: _assignedNonAdminUser.Id,
            status: TransactionStatus.Prepared,
            notes: "Ini buatan admin",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 25,
                    ExpectedQuantity: 8,
                    PreparedQuantity: 7,
                    Notes: "Humm.. 😐"
                )
            ]);
    }
    #endregion

    #region Negative Case
    [Fact]
    public void Must_Not_Be_Able_To_Assign_To_Admin()
    {
        IReadOnlyList<CreateOutTypeTransactionItemDto> transactionItems =
        [
            new (ProductId: 23, ExpectedQuantity: 5, PreparedQuantity: 5, Notes: "Kuambil 5"),
            new (ProductId: 24, ExpectedQuantity: 3, PreparedQuantity: 3, Notes: ""),
            new (ProductId: 25, ExpectedQuantity: 7, PreparedQuantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            AssignedUser: _userAdmin,
            Creator: _userAdmin,
            Notes: "Ini buatan admin",
            TransactionItems: transactionItems));

        var errors = result.GetError();

        Assert.Contains(errors, error => error is AdminMustNotAssignTransactionToAdminUserError);

    }
    
    [Fact]
    public void ShouldNotAbleCreateTransactionWithEmptyTransactionItem()
    {
        var result = Transaction.CreateOutTypeTransaction(new CreateOutTypeTransactionDto(
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: [],
            Notes: "seharusnya ini gk bisa",
            AssignedUser: null));
        
        List<IBaseTransactionDomainError> errors = result.GetError();
        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }
    #endregion
    
}