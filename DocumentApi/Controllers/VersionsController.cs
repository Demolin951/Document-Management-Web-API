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
    private readonly VersionService _versionService;
    public VersionsController(AppDbContext context, AccessService accessService, VersionService versionService)
    {
        _context = context;
        _accessService = accessService;
        _versionService = versionService;
    }

    [HttpPost("{documentId:int}/version")]
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

        var latestVersionNumber = await _versionService.GetLatestVersionNumber(documentId);

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
            UploadedBy = user.Name,
        };

        return Ok(response);
    }

    [HttpGet("{documentId:int}/version/all")]
    public async Task<IActionResult> GetVersions([FromRoute] int documentId, [FromQuery] string username)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        var versions = await _context.Versions
            .Include(x => x.UploadedByUser)
            .Where(x => x.DocumentId == documentId)
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => new DocumentVersionResponse
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                VersionNumber = x.VersionNumber,
                UploadedAtUtc = x.UploadedAtUtc,
                UploadedBy = x.UploadedByUser.Name
            })
            .ToListAsync();

        return Ok(versions);
    }

    [HttpGet("{documentId:int}/version/latest")]
    public async Task<IActionResult> GetLatestVersion([FromRoute] int documentId, [FromQuery] string username)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        var latestVersion = await _context.Versions
            .Include(x => x.UploadedByUser)
            .Where(x => x.DocumentId == documentId)
            .OrderByDescending(x => x.VersionNumber)
            .Select(x => new DocumentVersionResponse
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                VersionNumber = x.VersionNumber,
                UploadedAtUtc = x.UploadedAtUtc,
                UploadedBy = x.UploadedByUser.Name
            })
            .FirstOrDefaultAsync();

        if (latestVersion == null)
            return NotFound("No version found");

        return Ok(latestVersion);
    }

    [HttpGet("{documentId:int}/version/{versionNumber:int}")]
    public async Task<IActionResult> GetVersion([FromRoute] int documentId, [FromRoute] int versionNumber, [FromQuery] string username)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        var version = await _context.Versions
            .Include(x => x.UploadedByUser)
            .Where(x => x.DocumentId == documentId && x.VersionNumber == versionNumber)
            .Select(x => new DocumentVersionResponse
            {
                Id = x.Id,
                DocumentId = x.DocumentId,
                VersionNumber = x.VersionNumber,
                UploadedAtUtc = x.UploadedAtUtc,
                UploadedBy = x.UploadedByUser.Name
            })
            .FirstOrDefaultAsync();

        if (version == null)
            return NotFound("No version found");

        return Ok(version);
    }

    [HttpGet("{documentId:int}/version/download/latest")]
    public async Task<IActionResult> DownloadLatestVersion([FromRoute] int documentId, [FromQuery] string username)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        var version = await _versionService.GetLatestVersionWithDocument(documentId);

        if (version == null)
            return NotFound("No version found");

        var result = File(version.Data, version.Document.ContentType, version.Document.FileName);

        return result;
    }

    [HttpGet("{documentId:int}/version/{versionNumber:int}/download")]
    public async Task<IActionResult> DownloadVersion([FromRoute] int documentId, [FromRoute] int versionNumber, [FromQuery] string username)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var access = await _accessService.FindAccess(user.Id, documentId);

        if (access == null)
            return StatusCode(403, "Access denied");

        var version = await _versionService.GetVersionWithDocument(documentId, versionNumber);

        if (version == null)
            return NotFound("No version found");

        var result = File(version.Data, version.Document.ContentType, version.Document.FileName);

        return result;
    }
}