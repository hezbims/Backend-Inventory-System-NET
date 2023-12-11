using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Inventory_Backend.Tests.TestConfiguration.Mock;

public class MockDistributedCache : MemoryDistributedCache
{
    public MockDistributedCache() : base(
        optionsAccessor: Options.Create(new MemoryDistributedCacheOptions()) 
    ){}
}