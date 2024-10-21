using System.Net;

namespace Inventory_Backend_NET.Fitur._Model;

public interface ResultObject
{
    public string Message { get; }
    public string Type { get; }
    public HttpStatusCode StatusCode { get; }
}