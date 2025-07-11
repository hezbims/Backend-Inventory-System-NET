using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
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
    [Theory]
    [InlineData(TransactionType.In)]
    [InlineData(TransactionType.Out)]
    public void ShouldResultingInCorrectSideEffects(TransactionType transactionType)
    {
        IReadOnlyList<CreateTransactionItemDto> transactionItems =
        [
            new (ProductId: 23, Quantity: 5, Notes: "Kuambil 5"),
            new (ProductId: 24, Quantity: 3, Notes: ""),
            new (ProductId: 25, Quantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: transactionType, 
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            Notes: "Ini buatan admin",
            TransactionItems: transactionItems));
        
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = result.GetData().Item2;

        sideEffects.AssertAll(expectedEvents: 
            transactionItems.Select(item =>
                new ProductQuantityChangedEventAssertionDto(
                    ProductId: item.ProductId,
                    Quantity: item.Quantity,
                    Type: transactionType
                )
            ).ToList());
    }

    [Theory]
    [InlineData(TransactionType.In, true)]
    [InlineData(TransactionType.In, false)]
    [InlineData(TransactionType.Out, true)]
    [InlineData(TransactionType.Out, false)]
    public void ShouldCreateCorrectTransactionData(
        TransactionType transactionType,
        bool useAssignedUser)
    {
        IReadOnlyList<CreateTransactionItemDto> transactionItems = [
            new CreateTransactionItemDto(ProductId: 25, Quantity: 7, Notes: "Humm.. 😐")];
        
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: transactionType,
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems,
            Notes: "Ini buatan admin",
            AssignedUser: useAssignedUser ? _assignedNonAdminUser : null));

        Transaction transaction = result.GetData().Item1;
        int expectedAssignedUserId;
        TransactionStatus expectedTransactionStatus;
        if (!useAssignedUser)
        {
            expectedAssignedUserId = transaction.CreatorId;
            expectedTransactionStatus = TransactionStatus.Confirmed;
        }
        else
        {
            // assigned user must be same as creator when transaction type is IN
            if (transactionType == TransactionType.In)
            {
               expectedAssignedUserId = transaction.CreatorId;
               expectedTransactionStatus = TransactionStatus.Confirmed;
            }
            else
            {
                expectedAssignedUserId = _assignedNonAdminUser.Id;
                expectedTransactionStatus = TransactionStatus.Prepared;
            }
        }
        
        transaction.AssertTransactionFullData(
            id: 0, 
            transactionTime: 12,
            stakeholderId: 1, 
            type: transactionType,
            creatorId: _userAdmin.Id, 
            assignedUserId: expectedAssignedUserId,
            status: expectedTransactionStatus,
            notes: "Ini buatan admin",
            transactionItems: transactionItems.Select(item =>
                new TransactionItemAssertionDto(
                    ProductId: item.ProductId,
                    ExpectedQuantity: item.Quantity,
                    PreparedQuantity: item.Quantity,
                    Notes: item.Notes
                )
            ).ToList());
    }
    #endregion

    #region Negative Case
    [Fact]
    public void Must_Not_Be_Able_To_Assign_To_Admin()
    {
        IReadOnlyList<CreateTransactionItemDto> transactionItems =
        [
            new (ProductId: 23, Quantity: 5, Notes: "Kuambil 5"),
            new (ProductId: 24, Quantity: 3, Notes: ""),
            new (ProductId: 25, Quantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out, 
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
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: [],
            Notes: "seharusnya ini gk bisa",
            AssignedUser: null));
        
        List<IBaseTransactionDomainError> errors = result.GetError();
        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }

    [Fact]
    public void Should_Not_Create_Transaction_Item_With_Negative_Quantity()
    {
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out,
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: [
                new (ProductId: 3, Quantity: -1, Notes: "test"),
                new (ProductId: 4, Quantity: 0, Notes: "test"),
                new (ProductId: 5, Quantity: -2, Notes: "test"),
                
            ],
            Notes: "seharusnya ini gk bisa",
            AssignedUser: null));
        
        List<IBaseTransactionDomainError> errors = result.GetError();
        var negativeQuantityErrors =  errors.Where(error => 
                error is TransactionItemsShouldNotContainsNegativeQuantity)
                .Cast<TransactionItemsShouldNotContainsNegativeQuantity>()
                .ToList();
        Assert.Equal(2, negativeQuantityErrors.Count);
        Assert.Contains(negativeQuantityErrors , e => e.Index == 2);
        Assert.Contains(negativeQuantityErrors , e => e.Index == 0);
    }
    #endregion
    
}