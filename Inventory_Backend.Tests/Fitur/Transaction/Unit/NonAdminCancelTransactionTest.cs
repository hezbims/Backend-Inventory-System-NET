using Inventory_Backend_NET.Common.Domain.ValueObject;
using Inventory_Backend_NET.Fitur.Pengajuan.Domain.Dto.User;
using Inventory_Backend.Tests.Fitur.Transaction.Unit.Utils;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

public class NonAdminCancelTransactionTest
{
    private readonly UserDto _primaryNonAdmin = new UserDto(Id: 1, IsAdmin: false);
    private readonly UserDto _secondaryNonAdmin = new UserDto(Id: 2, IsAdmin: false);
    private readonly TransactionFactory _transactionFactory;

    public NonAdminCancelTransactionTest()
    {
        _transactionFactory = new TransactionFactory(
            Id: 1,
            TransactionTime: 2_333_444_555L,
            StakeholderId: 1,
            Type: TransactionType.Out);
    }
    
    
    [Fact]
    public void Non_Admin_Can_Cancel_All_Transaction_With_Status_Other_Than_Canceled()
    {
        
    }

    [Fact]
    public void Non_Admin_Can_Not_Cancel_Canceled_Transaction()
    {
        
    }

    [Fact]
    public void Non_Admin_Can_Not_Cancel_Transaction_With_Empty_Notes()
    {
        
    }

    [Fact]
    public void Non_Admin_Can_Not_Cancel_Other_User_Transaction()
    {
        
    }
}