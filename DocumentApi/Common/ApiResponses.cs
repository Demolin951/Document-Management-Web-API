using Microsoft.AspNetCore.Mvc;

namespace DocumentApi.Common;

public static class ApiResponse
{
    public static NotFoundObjectResult UserNotFound()
    {
        return new NotFoundObjectResult(ErrorMessage.UserNotFound);
    }

    public static NotFoundObjectResult CurrentOwnerNotFound()
    {
        return new NotFoundObjectResult(ErrorMessage.CurrentOwnerNotFound);
    }

    public static NotFoundObjectResult NoVersionFound()
    {
        return new NotFoundObjectResult(ErrorMessage.NoVersionFound);
    }

    public static NotFoundObjectResult NewOwnerNotFound()
    {
        return new NotFoundObjectResult(ErrorMessage.NewOwnerNotFound);
    }

    public static NotFoundObjectResult DocumentNotFound()
    {
        return new NotFoundObjectResult(ErrorMessage.DocumentNotFound);
    }

    public static NotFoundObjectResult TargetUserNotFound()
    {
        return new NotFoundObjectResult(ErrorMessage.TargetUserNotFound);
    }

    public static NotFoundObjectResult TargetUserHasNoAccess()
    {
        return new NotFoundObjectResult(ErrorMessage.TargetUserHasNoAccess);
    }

    public static ObjectResult AccessDenied()
    {
        return new ObjectResult(ErrorMessage.AccessDenied)
        {
            StatusCode = 403
        };
    }

        public static ObjectResult OnlyOwnerOrEditor()
    {
        return new ObjectResult(ErrorMessage.OnlyOwnerOrEditor)
        {
            StatusCode = 403
        };
    }

    public static ObjectResult CurrentUserHasNoAccess()
    {
        return new ObjectResult(ErrorMessage.CurrentUserHasNoAccess)
        {
            StatusCode = 403
        };
    }

    public static ObjectResult OnlyOwnerCanTransferOwnership()
    {
        return new ObjectResult(ErrorMessage.OnlyOwnerCanTransferOwnership)
        {
            StatusCode = 403
        };
    }

    public static ObjectResult OnlyOwnerCanAddAccess()
    {
        return new ObjectResult(ErrorMessage.OnlyOwnerCanAddAccess)
        {
            StatusCode = 403
        };
    }

    public static ObjectResult OnlyOwnerCanChangeRole()
    {
        return new ObjectResult(ErrorMessage.OnlyOwnerCanChangeRole)
        {
            StatusCode = 403
        };
    }

    public static ObjectResult OnlyOwnerCanDeleteAccess()
    {
        return new ObjectResult(ErrorMessage.OnlyOwnerCanDeleteAccess)
        {
            StatusCode = 403
        };
    }

    public static BadRequestObjectResult FileIsRequired()
    {
        return new BadRequestObjectResult(ErrorMessage.FileIsRequired);
    }

    public static BadRequestObjectResult OwnerRoleCanOnlyBeAssighnedByTransfer()
    {
        return new BadRequestObjectResult(ErrorMessage.OwnerRoleCanOnlyBeAssighnedByTransfer);
    }

    public static BadRequestObjectResult CanNotDeleteOwnerAccess()
    {
        return new BadRequestObjectResult(ErrorMessage.CanNotDeleteOwnerAccess);
    }

    public static BadRequestObjectResult TargetUserIsAlreadyOwner()
    {
        return new BadRequestObjectResult(ErrorMessage.TargetUserIsAlreadyOwner);
    }

    public static BadRequestObjectResult UserAlreadyHasAccess()
    {
        return new BadRequestObjectResult(ErrorMessage.UserAlreadyHasAccess);
    }

    public static BadRequestObjectResult InvalidFileType()
    {
        return new BadRequestObjectResult(ErrorMessage.InvalidFileType);
    }

    public static BadRequestObjectResult NewOwnerHasNoAccess()
    {
        return new BadRequestObjectResult(ErrorMessage.NewOwnerHasNoAccess);
    }

    public static ObjectResult Forbidden(string message)
    {
        return new ObjectResult(message)
        {
            StatusCode = 403
        };
    }

    public static NotFoundObjectResult NotFound(string message)
    {
        return new NotFoundObjectResult(message);
    }

    public static BadRequestObjectResult BadRequest(string message)
    {
        return new BadRequestObjectResult(message);
    }
}