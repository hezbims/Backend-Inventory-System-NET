using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.Transaction;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity;
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
        Transaction transaction = new Transaction(
            id: 25,
            type: TransactionType.Out,
            status: TransactionStatus.Prepared,
            stakeholderId: 24,
            transactionTime: 123459789237L,
            creatorId: _primaryNonAdmin.Id,
            assignedUserId: _primaryNonAdmin.Id,
            notes: "",
            transactionItems:
            [
                new TransactionItem(
                    productId: 23,
                    expectedQuantity: 5,
                    preparedQuantity: 5,
                    notes: "",
                    id: 1)
            ]);

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
            Transaction nonPreparedTransaction = new Transaction(
                id: 25,
                type: TransactionType.Out,
                status: transactionStatus,
                stakeholderId: 24,
                transactionTime: 123459789237L,
                creatorId: _primaryNonAdmin.Id,
                assignedUserId: _primaryNonAdmin.Id,
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
        Transaction otherUserTransaction = new Transaction(
            id: 25,
            type: TransactionType.Out,
            status: TransactionStatus.Prepared,
            stakeholderId: 24,
            transactionTime: 123459789237L,
            creatorId: _secondaryNonAdmin.Id,
            assignedUserId: _secondaryNonAdmin.Id,
            notes: "",
            transactionItems:
            [
                new TransactionItem(
                    productId: 23,
                    expectedQuantity: 5,
                    preparedQuantity: 5,
                    notes: "",
                    id: 1)
            ]);

        var errors = otherUserTransaction.ConfirmTransaction(new ConfirmTransactionDto(
            User: _primaryNonAdmin, Notes: "")).GetError();
        Assert.Single(errors, error => error is NonAdminCanNotConfirmOtherUserTransaction);
    }
}