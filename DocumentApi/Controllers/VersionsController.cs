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
public class VersionsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AccessService _accessService;
    public VersionsController(AppDbContext context, AccessService accessService)
    {
        _context = context;
        _accessService = accessService;
    }

    [HttpPost("{documentId:int}/versions")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddVersion([FromRoute] int documentId, [FromForm] AddDocumentVersionRequest request)
    {
        var user = await _accessService.FindUserByUserName(request.UserName);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        if (!_accessService.IsEditorOrOwner(access))
            return StatusCode(403, "Only owner or editor can upload new versions");

        var latestVersionNumber = await _context.Versions
            .Where(x => x.DocumentId == documentId)
            .MaxAsync(x => (int?)x.VersionNumber) ?? 0;

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var version = new DocumentVersion
        {
            DocumentId = documentId,
            VersionNumber = latestVersionNumber + 1,
            Data = memoryStream.ToArray(),
            UploadedAtUtc = DateTime.UtcNow,
            UploadedByUserId = user.Id
        };

        _context.Versions.Add(version);
        await _context.SaveChangesAsync();

        var response = new DocumentVersionResponse
        {
            Id = version.Id,
            DocumentId = version.DocumentId,
            VersionNumber = version.VersionNumber,
            UploadedAtUtc = version.UploadedAtUtc,
            UploadedBy = user.Name
        };

        return Ok(response);
    }

}