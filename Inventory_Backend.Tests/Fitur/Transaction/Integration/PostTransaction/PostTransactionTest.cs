using System.Net;
using System.Net.Http.Json;
using Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF;
using Inventory_Backend.Tests.Fitur._Preparation;
using Inventory_Backend.Tests.Fitur.Transaction.Integration._Utils;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Xunit.Abstractions;
using TransactionItemEf = Inventory_Backend_NET.Fitur.Pengajuan.Infrastructure.EF.TransactionItemEf;

namespace Inventory_Backend.Tests.Fitur.Transaction.Integration.PostTransaction;

/// <summary>
/// User membuat transaksi baru
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class PostTransactionTest : BaseIntegrationTest
{
    private readonly BasicDataset.TestData _testData;
    
    public PostTransactionTest(
        TestWebAppFactory webApp,
        ITestOutputHelper output) : base(webApp, output)
    {
        _testData = Get<BasicDataset>().Run(seedPengajuan: false);
    }

    [Fact]
    public async Task Should_Create_Correct_Data_In_Transaction_Table_And_Product_Table_When_Admin_Create_In_Type_Transaction()
    {
        var body = new { message = 5 };
        var response = await AdminClient.PostAsJsonAsync(
            "transaction", body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        TransactionEf newTransaction = Db.Transactions.Single();
        newTransaction.AssertData(
            transactionTime: newTransaction.TransactionTime,
            groupId: newTransaction.GroupId,
            status: newTransaction.Status,
            creatorId: newTransaction.CreatorId,
            assignedUserId: newTransaction.AssignedUserId,
            notes: newTransaction.Notes,
            priorities: newTransaction.Priorities,
            updatedAt: newTransaction.UpdatedAt,
            createdAt: newTransaction.CreatedAt);
        
        var transactionItems = Db.TransactionItems.ToList();
        Assert.Equal(2, transactionItems.Count);
        transactionItems.AssertContains(
            new TransactionItemEf
            {
                Id = -1,
                ExpectedQuantity = 5,
                PreparedQuantity = 5,
                Notes = "",
                ProductId = 3,
                TransactionId = newTransaction.Id,
            },
            new TransactionItemEf
            {
                Id = -1,
                ExpectedQuantity = 3,
                PreparedQuantity = 3,
                Notes = "",
                ProductId = 1,
                TransactionId = newTransaction.Id,
            });
    }

    [Fact]
    public async Task
        Should_Display_Error_Response_When_Non_Admin_Create_In_Type_Transaction()
    {
        throw new NotImplementedException();
    }
}