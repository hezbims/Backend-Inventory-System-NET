using Inventory_Backend_NET.Common.Domain.Event;
using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

using Transaction = Inventory_Backend_NET.Fitur.Pengajuan.Domain.Entity.Transaction;

public class AdminCancelTransactionTest
{
    private readonly UserDto _admin = new UserDto(Id: 1, IsAdmin: true);
    private readonly UserDto _nonAdmin = new UserDto(Id: 2, IsAdmin: false);

    private UserDto _currentTransactionCreator;
    private readonly TransactionFactory _baseTransactionFactory;

    public AdminCancelTransactionTest()
    {
        _currentTransactionCreator = _admin;
        _baseTransactionFactory = new TransactionFactory(
            Id: 1,
            TransactionTime: 23_000_563_329L,
            StakeholderId: 2,
            Type: TransactionType.Out,
            Status: TransactionStatus.Prepared,
            CreatorId: _admin.Id,
            AssignedUserId: _nonAdmin.Id,
            TransactionItems: [
                new TransactionItemFactory(
                    Id: 1, ProductId: 32, ExpectedQuantity: 5, PreparedQuantity: 3, Notes: ""),
                new TransactionItemFactory(
                    Id: 2, ProductId: 33, ExpectedQuantity: 4, PreparedQuantity: 0, Notes: ""),
                new TransactionItemFactory(
                    Id: 3, ProductId: 34, ExpectedQuantity: 1, PreparedQuantity: 1, Notes: "")
            ],
            Notes: "sebelum berubah");
    }

    [Fact]
    public void Admin_Can_Cancel_Waiting_Transaction()
    {
        _currentTransactionCreator = _nonAdmin;
        Transaction transaction = (_baseTransactionFactory with
        {
            CreatorId = _currentTransactionCreator.Id,
            Status = TransactionStatus.Waiting
        }).Build();
    }
    
    [Fact]
    public void Admin_Can_Cancel_Prepared_Transaction()
    {
        _currentTransactionCreator = _nonAdmin;
        Transaction transaction = (_baseTransactionFactory with
        {
            CreatorId = _currentTransactionCreator.Id,
            Status = TransactionStatus.Prepared
        }).Build();
        
        
    }

    [Fact]
    public void Admin_Can_Cancel_Confirmed_Transaction_That_Created_By_Their_Own()
    {
        _currentTransactionCreator = _admin;
        Transaction transaction = (_baseTransactionFactory with
        {
            CreatorId = _currentTransactionCreator.Id,
            Status = TransactionStatus.Confirmed 
        }).Build();
    }

    [Fact]
    public void Admin_Can_Not_Cancel_Confirmed_Transaction_That_Created_By_Other_User()
    {
        _currentTransactionCreator = _nonAdmin;
        Transaction transaction = (_baseTransactionFactory with
        {
            CreatorId = _currentTransactionCreator.Id,
            Status = TransactionStatus.Confirmed
        }).Build();
    }

    [Fact]
    public void Admin_Can_Not_Cancel_Canceled_Transaction()
    {
        IEnumerable<TransactionStatus> statuses = Enum.GetValues(typeof(TransactionStatus))
            .Cast<TransactionStatus>()
            .Where(status => status != TransactionStatus.Prepared && 
                             status != TransactionStatus.Confirmed);

        foreach (var status in statuses)
        {
            Transaction transaction = (_baseTransactionFactory with
            {
                Status = status
            }).Build();
            
            
        }
    }

    [Fact]
    public void Admin_Can_Not_Cancel_Transaction_With_Empty_Notes()
    {
        Transaction transaction = (_baseTransactionFactory with
        {
            Notes = ""
        }).Build();
    }

    private void Assert_Correct_Canceled_Transaction_Data(
        Transaction transaction,
        string expectedNotes)
    {
        transaction.AssertTransactionFullData(
            id: 1,
            transactionTime: 23_000_563_329L,
            stakeholderId: 2,
            type: TransactionType.Out,
            status: TransactionStatus.Canceled,
            creatorId: _currentTransactionCreator.Id,
            assignedUserId: _nonAdmin.Id,
            notes: expectedNotes,
            transactionItems: [
                new TransactionItemAssertionDto(
                    ProductId: 32, ExpectedQuantity: 5, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 33, ExpectedQuantity: 4, PreparedQuantity: null, Notes: ""),
                new TransactionItemAssertionDto(
                    ProductId: 34, ExpectedQuantity: 1, PreparedQuantity: null, Notes: "")
            ]);
    }

    private void Assert_Correct_Side_Effects(
        IReadOnlyList<ProductQuantityChangedEvent> actualSideEffects,
        IReadOnlyList<ProductQuantityChangedEvent>? expectedSideEffects = null)
    {
        if (expectedSideEffects == null)
            actualSideEffects.AssertAll([
                new ProductQuantityChangedEventAssertionDto(
                    ProductId: 32, Quantity: 3, Type: TransactionType.In),
                new ProductQuantityChangedEventAssertionDto(
                    ProductId: 34, Quantity: 1, Type: TransactionType.In),
            ]);
        else
            actualSideEffects.AssertAll(expectedSideEffects.Select(sideEffect =>
                new ProductQuantityChangedEventAssertionDto(
                    ProductId: sideEffect.ProductId, 
                    Quantity: sideEffect.Quantity, 
                    Type: sideEffect.Type)).ToList());
    }
}