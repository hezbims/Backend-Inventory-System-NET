using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminConfirmTransactionTest
{
    private readonly UserDto _admin = new (Id: 10234, IsAdmin: true);
    private readonly UserDto _nonAdmin = new (Id: 10, IsAdmin: false);
    [Fact]
    public void Admin_Must_Not_Be_Able_To_Confirm_Transaction_For_All_Status()
    {
        foreach (var transactionStatus in (TransactionStatus[]) Enum.GetValues(typeof(TransactionStatus)))
        {
            Transaction transaction = new TransactionFactory(
                Id: 25,
                Type: TransactionType.Out,
                Status: transactionStatus,
                StakeholderId: 24,
                TransactionTime: 123459789237L,
                CreatorId: _nonAdmin.Id,
                AssignedUserId: _nonAdmin.Id,
                Notes: "",
                TransactionItems:
                [
                    new TransactionItemFactory(
                        ProductId: 23,
                        ExpectedQuantity: 5,
                        PreparedQuantity: transactionStatus == TransactionStatus.Waiting ? null : 3,
                        Notes: "",
                        Id: 1)
                ]).Build(); 
            var errors = transaction.ConfirmTransaction(
                new ConfirmTransactionDto(User: _admin, Notes: "")).GetError();
            Assert.Single(errors, error => error is AdminCanNotConfirmTransactionError);
        }
    }
}