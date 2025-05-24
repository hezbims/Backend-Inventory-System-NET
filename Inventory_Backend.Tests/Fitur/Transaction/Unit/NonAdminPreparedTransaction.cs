using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.TransactionItem;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.PrepareTransaction;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminPreparedTransaction
{
    [Fact]
    public void Non_Admin_Should_Not_Be_Able_To_Prepare_Transaction()
    {
        UserDto nonAdminUser = new UserDto(IsAdmin: false, Id: 2);
        Transaction transaction = Transaction.CreateNew(new CreateNewTransactionDto(
            TransactionType: TransactionType.Out, 
            TransactionTime: 25,
            StakeholderId: 2,
            Creator: nonAdminUser,
            Notes: "",
            TransactionItems: [
                new CreateTransactionItemDto(ProductId: 2, Quantity: 3, Notes: ""),
            ]
        )).GetData().Item1;
        
        var errors = transaction.PrepareTransaction(new PrepareTransactionDto(
                Notes: "Seharusnya aku tak mampu 😭",
                Preparator: nonAdminUser, TransactionItems: [
                    new PrepareTransactionItemDto(PreparedQuantity: 1),]))
            .GetError();
        Assert.Contains(errors, error => error is UserNonAdminShouldNotPrepareTransactionError);
    }
}