using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Reflection.Metadata.Ecma335;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/document")]
public class VersionsController : ControllerBase
{
    private readonly AppDbContext _context;
    public VersionsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost("{documentId:int}/versions")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> AddVersion([FromRoute] int documentId, [FromForm] AddDocumentVersionRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Name == request.UserName);

        if (user == null)
            return NotFound("User not found");

        var access = await _context.Accesses
            .FirstOrDefaultAsync(x =>
                x.UserId == user.Id &&
                x.DocumentId == documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        if (access.Role != Role.Owner && access.Role != Role.Editor)
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