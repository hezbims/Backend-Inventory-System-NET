using FluentAssertions;
using Inventory_Backend.Tests.Fitur._Preparation;
using Inventory_Backend.Tests.Fitur.Pengajuan.GetPengajuanTest._Dto;
using Inventory_Backend.Tests.Helper;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.Fitur.Pengajuan.GetPengajuanTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class GetPengajuansByUserTypeTests : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly BasicDataset _basicDataset;
    private readonly IServiceScope _scope;
    private readonly ITestOutputHelper _output;

    public GetPengajuansByUserTypeTests(
        TestWebAppFactory webApp,
        ITestOutputHelper output)
    {
        _webApp = webApp;
        _output = output;
        _webApp.ConfigureLoggingToTestOutput(output);
        
        _scope = _webApp.Services.CreateScope();
        _basicDataset = _scope.ServiceProvider.GetRequiredService<BasicDataset>();
    }

    [Fact]
    public async Task AdminShouldSeeAllTransactions()
    {
        var testData = _basicDataset.Run();
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);
        HttpResponseMessage response = await adminClient.GetAsync(
            TestConstant.ApiEndpoints.GetPengajuansV2);
        _output.WriteLine($"QQQ {await response.Content.ReadAsStringAsync()}");
        
        response.IsSuccessStatusCode.Should().BeTrue(); 
        GetPengajuansResponseTestDto body = await response
            .ToModel<GetPengajuansResponseTestDto>();
       
        body.Data.Count.Should().Be(9);
    }

    [Fact]
    public async Task NonAdminShouldSeeOnlyTheirTransactions()
    {
        var testData = _basicDataset.Run();
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);
        HttpResponseMessage response = await nonAdminClient.GetAsync(
            TestConstant.ApiEndpoints.GetPengajuansV2);
        _output.WriteLine($"QQQ {await response.Content.ReadAsStringAsync()}");
    
        response.IsSuccessStatusCode.Should().BeTrue(); 
        GetPengajuansResponseTestDto body = await response
            .ToModel<GetPengajuansResponseTestDto>();
   
        body.Data.Count.Should().Be(3);
        body.Data.All(data => data.User.IsAdmin).Should().BeFalse();
    }

    public void Dispose()
    {
        _webApp.Cleanup(_scope);
    }
}