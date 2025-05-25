using Inventory_Backend_NET.Fitur.Pengajuan.Domain.ValueObject;

namespace Inventory_Backend.Tests.Fitur.Transaction.Unit;

public class AdminUpdateTransactionTest
{
    [Fact]
    public void Admin_Should_Be_Able_To_Update_Prepared_Transaction_Resulting_In_Correct_Transaction_Data()
    {
        
    }

    [Fact]
    public void Admin_Should_Be_Able_To_Update_Prepared_Transaction_Resulting_Correct_Side_Effects()
    {
        
    }

    [Theory]
    [InlineData(TransactionStatus.Canceled)]
    [InlineData(TransactionStatus.Confirmed)]
    [InlineData(TransactionStatus.Waiting)]
    public void Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Status_Other_Than_Prepared(
        TransactionStatus status)
    {
        
    }

    [Fact]
    public void Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Different_Size_Of_Transaction_Items()
    {
        
    }

    [Fact]
    public void
        Admin_Should_Not_Be_Able_To_Update_Prepared_Transaction_With_Transaction_Item_That_Has_Less_Than_0_Quantity()
    {
        
    }
}