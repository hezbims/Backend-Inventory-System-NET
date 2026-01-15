namespace Inventory_Backend_NET.Startup.Constant;

internal static class Env
{
    internal const string Local = "Local";
    
    internal const string E2E = "E2E";
    
    /// <summary>
    /// Ignore DB Connection when generating OpenAPI Spec
    /// </summary>
    internal const string ApiSpecGen = "OpenAPI-Spec-Gen";
}