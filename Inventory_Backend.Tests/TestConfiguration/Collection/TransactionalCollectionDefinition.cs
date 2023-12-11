using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;

namespace Inventory_Backend.Tests.TestConfiguration.Collection;

[CollectionDefinition(TestConstant.CollectionName)]
public class TransactionalCollectionDefinition : 
    ICollectionFixture<TransactionalMyDbFixture>
{
    
}