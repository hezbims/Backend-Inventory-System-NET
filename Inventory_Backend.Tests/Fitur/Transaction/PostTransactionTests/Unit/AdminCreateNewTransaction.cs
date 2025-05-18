using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.PostTransactionTests.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminCreateNewTransaction
{
    private readonly UserDto _userAdmin = new UserDto(Id: 99, IsAdmin: true);
    private readonly UserDto _assignedNonAdminUser = new UserDto(Id: 64, IsAdmin: false);
    
    [Theory]
    [InlineData(TransactionType.In)]
    [InlineData(TransactionType.Out)]
    public void ShouldResultingInCorrectSideEffects(TransactionType transactionType)
    {
        List<TransactionItemDto> transactionItems =
        [
            new TransactionItemDto(
                ProductId: 23, Quantity: 5, Notes: "Kuambil 5"),
            new TransactionItemDto(
                ProductId: 24, Quantity: 3, Notes: ""),
            new TransactionItemDto(
                ProductId: 25, Quantity: 7, Notes: "Humm.. 😐"),
        ];
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: transactionType, 
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems));
        
        IReadOnlyList<ProductQuantityChangedEvent> sideEffects = result.GetData().Item2;
        
        Assert.Equal(3, sideEffects.Count);
        for (int i = 0; i < sideEffects.Count; i++)
        {
            Assert.Equal(transactionItems[i].Quantity, sideEffects[i].Quantity);
            Assert.Equal(transactionItems[i].ProductId, sideEffects[i].ProductId);
            Assert.Equal(transactionType, sideEffects[i].Type);
        }
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
        List<TransactionItemDto> transactionItems = [
            new TransactionItemDto(
                ProductId: 25, Quantity: 7, Notes: "Humm.. 😐")];
        
        var result = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: transactionType,
            StakeholderId: 1,
            TransactionTime: 12,
            Creator: _userAdmin,
            TransactionItems: transactionItems,
            AssignedUser: useAssignedUser ? _assignedNonAdminUser : null));

        Transaction transaction = result.GetData().Item1;
        Assert.Equal(0, transaction.Id);
        Assert.Equal(12, transaction.TransactionTime);
        Assert.Equal(1, transaction.StakeholderId);
        Assert.Equal(transactionType, transaction.Type);
        Assert.Equal(_userAdmin.Id, transaction.CreatorId);
        Assert.Single(transaction.TransactionItems);
        Assert.Equal(25, transaction.TransactionItems[0].ProductId);
        Assert.Equal(7, transaction.TransactionItems[0].Quantity);
        Assert.Equal("Humm.. 😐", transaction.TransactionItems[0].Notes);

        if (!useAssignedUser)
        {
            Assert.Equal(transaction.CreatorId, transaction.AssignedUserId);
            Assert.Equal(TransactionStatus.Confirmed, transaction.Status);
        }
        else
        {
            // assigned user must be same as creator when transaction type is IN
            if (transactionType == TransactionType.In)
            {
                Assert.Equal(transaction.CreatorId, transaction.AssignedUserId);
                Assert.Equal(TransactionStatus.Confirmed, transaction.Status);
            }
            else
            {
                Assert.Equal(_assignedNonAdminUser.Id, transaction.AssignedUserId);
                Assert.Equal(TransactionStatus.Prepared, transaction.Status);
            }
        }
    }
}