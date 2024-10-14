using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;

namespace Inventory_Backend.Tests.TestConfiguration.CollectionDefinition;

[CollectionDefinition(TestConstant.IntegrationTestDefinition)]
public class IntegrationTestDefinition : ICollectionFixture<TestWebAppFactory>
{
    
}