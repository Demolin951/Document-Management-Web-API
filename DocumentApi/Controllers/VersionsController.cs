using DocumentApi.Common;
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
public class VersionsController : ApiControllerBase
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
        var accessResult = await _accessService.CheckAccess(request.UserName, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        if (!_accessService.IsEditorOrOwner(accessResult.Access!))
            return ApiResponse.OnlyOwnerOrEditor();

        var user = accessResult.User!;

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
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        var versions = await _versionService.GetVersions(documentId);

        return Ok(versions);
    }

    [HttpGet("{documentId:int}/version/latest")]
    public async Task<IActionResult> GetLatestVersion([FromRoute] int documentId, [FromQuery] string username)
    {
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        var latestVersion = await _versionService.GetLatestVersion(documentId);

        if (latestVersion == null)
            return ApiResponse.NoVersionFound();

        return Ok(latestVersion);
    }

    [HttpGet("{documentId:int}/version/{versionNumber:int}")]
    public async Task<IActionResult> GetVersion([FromRoute] int documentId, [FromRoute] int versionNumber, [FromQuery] string username)
    {
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        var version = await _versionService.GetVersion(documentId, versionNumber);

        if (version == null)
            return ApiResponse.NoVersionFound();

        return Ok(version);
    }

    [HttpGet("{documentId:int}/version/download/latest")]
    public async Task<IActionResult> DownloadLatestVersion([FromRoute] int documentId, [FromQuery] string username)
    {
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        var version = await _versionService.GetLatestVersionWithDocument(documentId);

        if (version == null)
            return ApiResponse.NoVersionFound();

        var result = File(version.Data, version.Document.ContentType, version.Document.FileName);

        return result;
    }

    [HttpGet("{documentId:int}/version/{versionNumber:int}/download")]
    public async Task<IActionResult> DownloadVersion([FromRoute] int documentId, [FromRoute] int versionNumber, [FromQuery] string username)
    {
        var accessResult = await _accessService.CheckAccess(username, documentId);

        if (TryGetAccessError(accessResult, out var error))
        {
            return error!;
        }

        var version = await _versionService.GetVersionWithDocument(documentId, versionNumber);

        if (version == null)
            return ApiResponse.NoVersionFound();

        var result = File(version.Data, version.Document.ContentType, version.Document.FileName);

        return result;
    }
}