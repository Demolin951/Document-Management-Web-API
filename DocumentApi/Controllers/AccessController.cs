using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using DocumentApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/document")]
public class AccessController : ControllerBase
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

        if (!accessResult.UserExists)
            return NotFound("User not found");

        if (!accessResult.HasAccess)
            return StatusCode(403, "Access denied");

        if (!_accessService.IsOwner(accessResult.Access!))
            return StatusCode(403, "Only owner can view access list");

        var accesses = await _context.Accesses
            .Include(x => x.User)
            .Where(x => x.DocumentId == documentId)
            .Select(x => new DocumentAccessResponse
            {
                DocumentId = x.DocumentId,
                UserId = x.UserId,
                UserName = x.User.Name,
                Role = x.Role
            })
            .ToListAsync();

        return Ok(accesses);
    }

    [HttpPost("{documentId:int}/access")]
    public async Task<IActionResult> AddAccess([FromRoute] int documentId, [FromBody] ChangeDocumentRoleRequest request)
    {
        var ownerCheck = await _accessService.CheckAccess(request.UserName, documentId);

        if (!ownerCheck.UserExists)
            return NotFound("User not found");

        if (!ownerCheck.HasAccess)
            return StatusCode(403, "Access denied");

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return StatusCode(403, "Only owner can add access");

        var targetUser = await _accessService.FindUserByUserName(request.TargetUserName);

        if (targetUser == null)
            return NotFound("Target user not found");

        var existingAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (existingAccess != null)
            return BadRequest("User already has access");

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

        if (!ownerCheck.UserExists)
            return NotFound("User not found");

        if (!ownerCheck.HasAccess)
            return StatusCode(403, "Access denied");

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return StatusCode(403, "Only owner can change roles");

        var targetUser = await _accessService.FindUserByUserName(request.TargetUserName);

        if (targetUser == null)
            return NotFound("Target user not found");

        var targetAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (targetAccess == null)
            return NotFound("Target user has no access");

        if (targetAccess.Role == Role.Owner)
            return BadRequest("Target user is already owner");

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

        if (!ownerCheck.UserExists)
            return NotFound("User not found");

        if (!ownerCheck.HasAccess)
            return StatusCode(403, "Access denied");

        if (!_accessService.IsOwner(ownerCheck.Access!))
            return StatusCode(403, "Only owner can delete access");

        var targetUser = await _accessService.FindUserByUserName(targetUserName);

        if (targetUser == null)
            return NotFound("Target user not found");

        var targetAccess = await _accessService.FindAccess(targetUser.Id, documentId);

        if (targetAccess == null)
            return NotFound("Target user has no access");

        if (targetAccess.Role == Role.Owner)
            return BadRequest("Can not delete owner access");

        _context.Accesses.Remove(targetAccess);
        await _context.SaveChangesAsync();

        return Ok("Access deleted");
    }

    [HttpPut("{documentId:int}/owner")]
    public async Task<IActionResult> TransferOwner([FromRoute] int documentId, [FromBody] TransferOwnerRequest request)
    {
        var currentOwnerCheck = await _accessService.CheckAccess(request.CurrentOwnerUserName, documentId);

        if (!currentOwnerCheck.UserExists)
            return NotFound("Current owner not found");

        if (!currentOwnerCheck.HasAccess)
            return StatusCode(403, "Current user has no access");

        if (!_accessService.IsOwner(currentOwnerCheck.Access!))
            return StatusCode(403, "Only owner can trasfer ownership");

        var newOwner = await _accessService.FindUserByUserName(request.NewOwnerUserName);

        if (newOwner == null)
            return NotFound("New owner not found");

        var newOwnerAccess = await _accessService.FindAccess(newOwner.Id, documentId);

        if (newOwnerAccess == null)
        {
            newOwnerAccess = new DocumentAccess
            {
                UserId = newOwner.Id,
                DocumentId = documentId,
                Role = Role.Owner
            };

            _context.Accesses.Add(newOwnerAccess);
        }
        else
        {
            newOwnerAccess.Role = Role.Owner;
        }

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