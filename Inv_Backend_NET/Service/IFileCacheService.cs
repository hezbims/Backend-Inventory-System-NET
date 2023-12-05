namespace Inventory_Backend_NET.Service;

public interface IFileCacheService
{
    string? Get(string key);
    void Set(string key, string value);
}