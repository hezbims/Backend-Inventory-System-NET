using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.Common;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminCreateNewTransactionTest
{
    private readonly UserDto _userAdmin = new UserDto(Id: 99, IsAdmin: true);
    private readonly UserDto _assignedNonAdminUser = new UserDto(Id: 64, IsAdmin: false);
    
    [Theory]
    [InlineData(TransactionType.In)]
    [InlineData(TransactionType.Out)]
    public void ShouldResultingInCorrectSideEffects(TransactionType transactionType)
    {
        IReadOnlyList<CreateTransactionItemDto> transactionItems =
        [
            new CreateTransactionItemDto(
                ProductId: 23, Quantity: 5, Notes: "Kuambil 5"),
            new CreateTransactionItemDto(
                ProductId: 24, Quantity: 3, Notes: ""),
            new CreateTransactionItemDto(
                ProductId: 25, Quantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: transactionType, 
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
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
            transactionItems: transactionItems.Select(item =>
                new TransactionItemAssertionDto(
                    ProductId: item.ProductId,
                    ExpectedQuantity: item.Quantity,
                    PreparedQuantity: item.Quantity,
                    Notes: item.Notes
                )
            ).ToList());
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
            AssignedUser: null));
        
        List<IBaseTransactionDomainError> errors = result.GetError();
        Assert.Contains(errors, error => error is TransactionItemsShouldNotBeEmptyError);
    }
}