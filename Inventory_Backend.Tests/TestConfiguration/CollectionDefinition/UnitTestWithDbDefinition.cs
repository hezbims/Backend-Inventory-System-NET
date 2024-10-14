using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;

namespace Inventory_Backend.Tests.TestConfiguration.CollectionDefinition;

/// <summary>
/// Collection definition digunakan apabila ada banyak Test Class yang punya dependensi yang sama dan harus dijalankan
/// secara serial (non-paralel). Fixture adalah dependency yang dapat di inject pada Test Class.
/// </summary>
[CollectionDefinition(TestConstant.UnitTestWithDbCollection)]
public class UnitTestWithDbDefinition : 
    ICollectionFixture<MyDbFixture>
{
    
}