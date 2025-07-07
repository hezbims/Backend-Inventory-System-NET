using System.Net;
using Inventory_Backend.Tests.Fitur._Preparation;
using Inventory_Backend.Tests.Fitur.Transaction.Integration.GetPengajuanTest._Dto;
using Inventory_Backend.Tests.Helper;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Transaction.Integration.GetPengajuanTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class GetPengajuansByUserTypeTests : BaseIntegrationTest
{
    public GetPengajuansByUserTypeTests(TestWebAppFactory testWebAppFactory, ITestOutputHelper output) : base(
        testWebAppFactory, output)
    {
        Get<BasicDataset>().Run();
    }
    
    [Fact]
    public async Task AdminShouldSeeAllTransactions()
    {
        HttpResponseMessage response = await AdminClient.GetAsync(
            TestConstant.ApiEndpoints.GetPengajuansV2);
        Output.WriteLine($"QQQ {await response.Content.ReadAsStringAsync()}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response
            .ToModel<GetPengajuansResponseTestDto>();
       
        Assert.Equal(9, body.Data.Count);
    }

    [Fact]
    public async Task NonAdminShouldSeeOnlyTheirTransactions()
    {
        HttpResponseMessage response = await NonAdminClient.GetAsync(
            TestConstant.ApiEndpoints.GetPengajuansV2);
        Output.WriteLine($"QQQ {await response.Content.ReadAsStringAsync()}");
    
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response
            .ToModel<GetPengajuansResponseTestDto>();
   
        Assert.Equal(3, body.Data.Count);
        Assert.All(body.Data , data => Assert.True(!data.User.IsAdmin));
    }
}