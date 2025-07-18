using Inventory_Backend_NET.Common.Presentation.Model;
using Inventory_Backend_NET.Fitur.Autentikasi.Domain.Exception;

namespace Inventory_Backend_NET.Fitur.Autentikasi.ErrorTranslator;

internal sealed class AuthDomainErrorTranslator
{
    internal MyValidationError Translate(BaseAuthError error)
    {
        return error switch
        {
            UserIdNotFound err =>
                new MyValidationError(
                    Code: "DOM_AUTH_USER_ID_NOT_FOUND",
                    Message: String.Format(Resources.Auth.user_id_not_found, err.Id)),
            _ => throw new ArgumentOutOfRangeException(
                $"Untranslated error : {error.GetType().FullName}")
        };
    }
}