using DocumentApi.Common;
using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using DocumentApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/document")]
public class AccessController : ApiControllerBase
{
    private readonly AppDbContext _context;
    private readonly AccessService _accessService;
    public AccessController(AppDbContext context, AccessService accessService)
    {
        _context = context;
        _accessService = accessService;
    }

    [HttpGet("{documentId:int}/access")]
    public async Task<IActionResult> GetAccesses([FromRoute] int documentId, [FromQuery] string username)
    {
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        if (!_accessService.IsOwner(accessResult.Access!))
            return ApiResponse.OnlyOwnerCanTransferOwnership();

        var accesses = await _accessService.GetAccesses(documentId);

        return Ok(accesses);
    }

    [HttpPost("{documentId:int}/access")]
    public async Task<IActionResult> AddAccess([FromRoute] int documentId, [FromBody] ChangeDocumentRoleRequest request)
    {
        var ownerCheck = await _accessService.CheckAccess(request.UserName, documentId);

        if (TryGetAccessError(ownerCheck, out var error))
        {
            return error!;
        }

        if (request.Role == Role.Owner)
        {
            return ApiResponse.OwnerRoleCanOnlyBeAssighnedByTransfer();
        }

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return ApiResponse.OnlyOwnerCanAddAccess();

        var targetUser = await _accessService.FindUserByUserName(request.TargetUserName);

        if (targetUser == null)
            return ApiResponse.TargetUserNotFound();

        var existingAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (existingAccess != null)
            return ApiResponse.UserAlreadyHasAccess();

        var access = new DocumentAccess
        {
            UserId = targetUser.Id,
            DocumentId = documentId,
            Role = request.Role
        };

        _context.Accesses.Add(access);
        await _context.SaveChangesAsync();

        var response = new DocumentAccessResponse
        {
            DocumentId = access.DocumentId,
            UserId = targetUser.Id,
            UserName = targetUser.Name,
            Role = access.Role
        };

        return Ok(response);
    }

    [HttpPut("{documentId:int}/access")]
    public async Task<IActionResult> ChangeAccessRole([FromRoute] int documentId, [FromBody] ChangeDocumentRoleRequest request)
    {
        var ownerCheck = await _accessService.CheckAccess(request.UserName, documentId);

        if (TryGetAccessError(ownerCheck, out var error))
        {
            return error!;
        }

        if (request.Role == Role.Owner)
        {
            return ApiResponse.OwnerRoleCanOnlyBeAssighnedByTransfer();
        }

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return ApiResponse.OnlyOwnerCanChangeRole();

        var targetUser = await _accessService.FindUserByUserName(request.TargetUserName);

        if (targetUser == null)
            return ApiResponse.TargetUserNotFound();

        var targetAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (targetAccess == null)
            return ApiResponse.TargetUserHasNoAccess();

        if (targetAccess.Role == Role.Owner)
            return ApiResponse.TargetUserIsAlreadyOwner();

        targetAccess.Role = request.Role;

        await _context.SaveChangesAsync();

        var response = new DocumentAccessResponse
        {
            DocumentId = targetAccess.DocumentId,
            UserId = targetUser.Id,
            UserName = targetUser.Name,
            Role = targetAccess.Role
        };

        return Ok(response);

    }

    [HttpDelete("{documentId:int}/access")]
    public async Task<IActionResult> DeleteAccess([FromRoute] int documentId, [FromQuery] string username, [FromQuery] string targetUserName)
    {
        var ownerCheck = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(ownerCheck, out var error))
        {
            return error!;
        }

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return ApiResponse.OnlyOwnerCanDeleteAccess();

        var targetUser = await _accessService.FindUserByUserName(targetUserName);

        if (targetUser == null)
            return ApiResponse.TargetUserNotFound();

        var targetAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (targetAccess == null)
            return ApiResponse.TargetUserHasNoAccess();

        if (targetAccess.Role == Role.Owner)
            return ApiResponse.CanNotDeleteOwnerAccess();

        _context.Accesses.Remove(targetAccess);
        await _context.SaveChangesAsync();

        return Ok("Access deleted");
    }

    [HttpPut("{documentId:int}/owner")]
    public async Task<IActionResult> TransferOwner([FromRoute] int documentId, [FromBody] TransferOwnerRequest request)
    {
        var currentOwnerCheck = await _accessService.CheckAccess(request.CurrentOwnerUserName, documentId);

        if (!currentOwnerCheck.UserExists)
            return ApiResponse.CurrentOwnerNotFound();

        if (!currentOwnerCheck.HasAccess)
            return ApiResponse.CurrentUserHasNoAccess();

        if (!_accessService.IsOwner(currentOwnerCheck.Access!))
            return ApiResponse.OnlyOwnerCanTransferOwnership();

        var newOwner = await _accessService.FindUserByUserName(request.NewOwnerUserName);

        if (newOwner == null)
            return ApiResponse.NewOwnerNotFound();

        var newOwnerAccess = await _accessService.FindAccess(newOwner.Id, documentId);

        if (newOwnerAccess == null)
        {
            return ApiResponse.NewOwnerHasNoAccess();
        }

        newOwnerAccess.Role = Role.Owner;
        currentOwnerCheck.Access!.Role = Role.Editor;

        await _context.SaveChangesAsync();

        var response = new TransferOwnerResponse
        {
            PreviousOwner = request.CurrentOwnerUserName,
            NewOwner = request.NewOwnerUserName,
            DocumentId = documentId
        };

        return Ok(response);

    }
}