using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminConfirmTransactionTest
{
    private readonly UserDto _admin = new UserDto(Id: 10234, IsAdmin: true);
    private readonly UserDto _nonAdmin = new UserDto(Id: 10, IsAdmin: false);
    [Fact]
    public void Admin_Must_Not_Be_Able_To_Confirm_Transaction_For_All_Status()
    {
        foreach (var transactionStatus in (TransactionStatus[]) Enum.GetValues(typeof(TransactionStatus)))
        {
            Transaction transaction = new Transaction(
                id: 25,
                type: TransactionType.Out,
                status: transactionStatus,
                stakeholderId: 24,
                transactionTime: 123459789237L,
                creatorId: _nonAdmin.Id,
                assignedUserId: _nonAdmin.Id,
                notes: "",
                transactionItems:
                [
                    new TransactionItem(
                        productId: 23,
                        expectedQuantity: 5,
                        preparedQuantity: transactionStatus == TransactionStatus.Waiting ? null : 3,
                        notes: "",
                        id: 1)
                ]); 
            var errors = transaction.ConfirmTransaction(
                new ConfirmTransactionDto(User: _admin, Notes: "")).GetError();
            Assert.Single(errors, error => error is AdminCanNotConfirmTransactionError);
        }
    }
}