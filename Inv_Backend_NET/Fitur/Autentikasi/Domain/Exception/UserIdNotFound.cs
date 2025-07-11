namespace Inventory_Backend_NET.Fitur.Autentikasi.Domain.Exception;

internal sealed class UserIdNotFound : BaseAuthError
{
    public required int Id { get; init; }
}