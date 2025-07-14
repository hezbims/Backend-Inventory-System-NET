using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Exception.ConfirmTransaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit.Domain;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction; 
public class NonAdminConfirmTransactionTest
{
    private readonly UserDto _primaryNonAdmin = new UserDto(
        Id: 23, IsAdmin: false);

    private readonly UserDto _secondaryNonAdmin = new UserDto(
        Id: 10234, IsAdmin: false);

    [Fact]
    public void Confirmed_Transaction_Should_Has_Correct_Data_And_No_Side_Effects()
    {
        Transaction transaction = new TransactionFactory(
            Id: 25,
            Type: TransactionType.Out,
            Status: TransactionStatus.Prepared,
            StakeholderId: 24,
            TransactionTime: 123459789237L,
            CreatorId: _primaryNonAdmin.Id,
            AssignedUserId: _primaryNonAdmin.Id,
            Notes: "",
            TransactionItems:
            [
                new TransactionItemFactory(
                    ProductId: 23,
                    ExpectedQuantity: 5,
                    PreparedQuantity: 5,
                    Notes: "",
                    Id: 1)
            ]).Build();

        var sideEffects = transaction.ConfirmTransaction(new ConfirmTransactionDto(
            User: _primaryNonAdmin, Notes: "seharusnya berubah")).GetData();
        
        Assert.Empty(sideEffects);
        transaction.AssertTransactionFullData(
            id: 25,
            type: TransactionType.Out,
            status: TransactionStatus.Confirmed,
            stakeholderId: 24,
            transactionTime: 123459789237L,
            creatorId: _primaryNonAdmin.Id,
            assignedUserId: _primaryNonAdmin.Id,
            notes: "seharusnya berubah",
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 23,
                    ExpectedQuantity: 5,
                    PreparedQuantity: 5,
                    Notes: "")
            ]);
    }
    
    [Fact]
    public void Non_Admin_Must_Not_Be_Able_To_Confirm_Transaction_With_Status_Other_Than_Prepared()
    {
        IEnumerable<TransactionStatus> nonPreparedStatus = ((TransactionStatus[])Enum.GetValues(typeof(TransactionStatus)))
            .Where(status => status != TransactionStatus.Prepared);

        foreach (var transactionStatus in nonPreparedStatus)
        {
            Transaction nonPreparedTransaction = new TransactionFactory(
                Id: 25,
                Type: TransactionType.Out,
                Status: transactionStatus,
                StakeholderId: 24,
                TransactionTime: 123459789237L,
                CreatorId: _primaryNonAdmin.Id,
                AssignedUserId: _primaryNonAdmin.Id,
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

            List<IBaseTransactionDomainError> errors = nonPreparedTransaction.ConfirmTransaction(
                new ConfirmTransactionDto(
                    User: _primaryNonAdmin, Notes: "hmm")).
                GetError();
            Assert.Single(errors, error =>
                error is NonAdminCanOnlyConfirmPreparedTransaction e &&
                e.CurrentStatus == transactionStatus);
        }
    }

        
    [Fact]
    public void Non_Admin_Must_Not_Be_Able_Confirm_Other_User_Transaction()
    {
        Transaction otherUserTransaction = new TransactionFactory(
            Id: 25,
            Type: TransactionType.Out,
            Status: TransactionStatus.Prepared,
            StakeholderId: 24,
            TransactionTime: 123459789237L,
            CreatorId: _secondaryNonAdmin.Id,
            AssignedUserId: _secondaryNonAdmin.Id,
            Notes: "",
            TransactionItems:
            [
                new TransactionItemFactory(
                    ProductId: 23,
                    ExpectedQuantity: 5,
                    PreparedQuantity: 5,
                    Notes: "",
                    Id: 1)
            ]).Build();

        var errors = otherUserTransaction.ConfirmTransaction(new ConfirmTransactionDto(
            User: _primaryNonAdmin, Notes: "")).GetError();
        Assert.Single(errors, error => error is NonAdminCanNotConfirmOtherUserTransaction);
    }
}