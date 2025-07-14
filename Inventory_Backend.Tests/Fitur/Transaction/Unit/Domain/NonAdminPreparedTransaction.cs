using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminPreparedTransaction
{
    [Fact]
    public void Non_Admin_Should_Not_Be_Able_To_Prepare_Transaction()
    {
        UserDto nonAdminUser = new UserDto(IsAdmin: false, Id: 2);
        Transaction transaction = new TransactionFactory(
            Type: TransactionType.Out, 
            TransactionTime: 25,
            StakeholderId: 2,
            CreatorId: nonAdminUser.Id,
            Notes: "",
            Status: TransactionStatus.Waiting,
            AssignedUserId: nonAdminUser.Id,
            Id: 1,
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 1, ProductId: 2, ExpectedQuantity: 3, PreparedQuantity: null, Notes: ""),
            ]
        ).Build();
        
        var errors = transaction.PrepareTransaction(new PrepareTransactionDto(
                Notes: "Seharusnya aku tak mampu 😭",
                Preparator: nonAdminUser, TransactionItems: [
                    new PrepareTransactionItemDto(PreparedQuantity: 1),]))
            .GetError();
        Assert.Contains(errors, error => error is UserNonAdminShouldNotPrepareTransactionError);
    }
}