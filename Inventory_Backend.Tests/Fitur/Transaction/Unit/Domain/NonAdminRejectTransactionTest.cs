using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.RejectTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class NonAdminRejectTransactionTest
{
    [Theory]
    [MemberData(memberName: nameof(TransactionStatusesGenerate.All), MemberType  = typeof(TransactionStatusesGenerate))]
    public void Non_Admin_Can_Not_Allowed_To_Reject_Any_Transaction(TransactionStatus status)
    {
        UserDto nonAdmin = new UserDto(Id: 5, IsAdmin: false);
        Transaction transaction = new TransactionFactory(
            Id: 1,
            Status: status,
            Type: TransactionType.Out,
            TransactionTime: 25_000_000_000L,
            StakeholderId: 3,
            CreatorId: 4,
            AssignedUserId: nonAdmin.Id,
            Notes: "",
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 2, ProductId: 3, ExpectedQuantity: 5, PreparedQuantity: 5, Notes: "")
            ]).Build();

        var errors = transaction.Reject(new RejectTransactionDto(
            Rejector: nonAdmin, Notes: "sebuah notes")).GetError();

        Assert.Single(errors, error => error is NonAdminIsNotAllowedToRejectTransactionError);
    }
}