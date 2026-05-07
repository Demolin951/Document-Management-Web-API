namespace DocumentApi.Common;

public static class ErrorMessage
{
    public const string UserNotFound = "User not found";
    public const string DocumentNotFound = "Document not found";
    public const string AccessDenied = "Access denied";
    public const string CurrentOwnerNotFound = "Current owner not found";
    public const string CurrentUserHasNoAccess = "Current user has no access";
    public const string OnlyOwnerCanTransferOwnership = "Only owner can transer ownership";
    public const string FileIsRequired = "File is required";
    public const string InvalidFileType = "Only PDF files are allowed";
    public const string NewOwnerHasNoAccess = "New owner has no access to this document";

    public const string OnlyOwnerCanAddAccess = "Only owner can add access";
    public const string TargetUserNotFound = "Target user not found";
    public const string UserAlreadyHasAccess = "User already has access";
    public const string OnlyOwnerCanChangeRole = "Only owner can change roles";
    public const string TargetUserHasNoAccess = "Target user has no access";
    public const string TargetUserIsAlreadyOwner = "Target user is already owner";
    public const string OnlyOwnerCanDeleteAccess = "Only owner can delete access";
    public const string CanNotDeleteOwnerAccess = "Can not delete owner access";
    public const string NewOwnerNotFound = "New owner not found";
    public const string OnlyOwnerOrEditor = "Only owner or editor can upload new versions";
    public const string NoVersionFound = "No version found";
    public const string OwnerRoleCanOnlyBeAssighnedByTransfer = "Owner role can only be assigned by ownership transfer";
}