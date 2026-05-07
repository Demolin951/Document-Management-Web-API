using DocumentApi.Services;
using DocumentApi.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.MicrosoftExtensions;

namespace DocumentApi.Controllers;

public abstract class ApiController : ControllerBase
{
    protected bool TryGetAccessError(
        AccessCheckResult accessResult,
        out IActionResult? error,
        string userNameNotFoundMessage = ErrorMessage.UserNotFound,
        string accessDeniedMessage = ErrorMessage.AccessDenied)
    {
        error = null;

        if (!accessResult.UserExists)
        {
            error = ApiResponse.NotFound(userNameNotFoundMessage);
            return true;
        }


        if (!accessResult.HasAccess)
        {
            error = ApiResponse.Forbidden(accessDeniedMessage);
            return true;
        }


        return false;
    }
}