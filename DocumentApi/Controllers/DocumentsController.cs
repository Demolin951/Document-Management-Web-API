using DocumentApi.Common;
using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using DocumentApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Microsoft.AspNetCore.Http;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/document")]
public class DocumentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly AccessService _accessService;
    public DocumentController(AppDbContext context, AccessService accessService)
    {
        _context = context;
        _accessService = accessService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DocumentAccess>> UploadDocument([FromForm] UploadDocumentRequest request)
    {
        var user = await _accessService.FindUserByUserName(request.UserName);

        if (user == null)
            return ApiResponse.UserNotFound();

        if (request.File == null || request.File.Length == 0)
        {
            return ApiResponse.FileIsRequired();
        }

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var document = new Document
        {
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            CreatedAtUtc = DateTime.UtcNow
        };

        var version = new DocumentVersion
        {
            Document = document,
            VersionNumber = 1,
            Data = memoryStream.ToArray(),
            UploadedAtUtc = DateTime.UtcNow,
            UploadedByUserId = user.Id
        };

        _context.Documents.Add(document);
        _context.Versions.Add(version);

        var access = new DocumentAccess
        {
            User = user,
            Document = document,
            Role = Role.Owner
        };

        _context.Accesses.Add(access);

        await _context.SaveChangesAsync();

        return Ok(access);
    }

    /// <summary>
    /// Gibt alle Dokumente zurück, auf die der angegebene Benutzer Zugriff hat.
    /// </summary>
    /// <param name="username">
    /// Der Benutzername des Users, dessen Dokumentzugriffe abgefragt werden.
    /// </param>
    /// <param name="docId">
    /// Optional: Filtert die Ergebnisse auf ein bestimmtes Dokument.
    /// Wenn nicht angegeben, werden alle zugänglichen Dokumente zurückgegeben.
    /// </param>
    /// <returns>
    /// Eine Liste von Dokumenten inklusive Metadaten und der jeweiligen Zugriffsrolle des Benutzers.
    /// </returns>
    /// <response code="200">
    /// Dokument(e) erfolgreich gefunden.
    /// </response>
    /// <response code="403">
    /// Der Benutzer hat keinen Zugriff auf das angeforderte Dokument.
    /// </response>
    /// <response code="404">
    /// Der angegebene Benutzer wurde nicht gefunden.
    /// </response>
    [HttpGet]
    [Produces("application/json")]
    [ProducesResponseType(typeof(List<DocumentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetDocuments([FromQuery] string username, [FromQuery] int? docId)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return ApiResponse.UserNotFound();

        var query = _context.Accesses
            .Include(x => x.Document)
            .Where(x => x.UserId == user.Id);

        if (docId != null)
        {
            query = query.Where(x => x.DocumentId == docId.Value);
        }

        var documents = await query
            .Select(x => new DocumentResponse
            {
                Id = x.Document.Id,
                FileName = x.Document.FileName,

                Owner = _context.Accesses
                .Where(ownerAccess =>
                    ownerAccess.DocumentId == x.Document.Id &&
                    ownerAccess.Role == Role.Owner)
                .Select(ownerAccess => ownerAccess.User.Name)
                .FirstOrDefault() ?? string.Empty,

                CreatedAtUtc = x.Document.CreatedAtUtc,
                Role = x.Role.ToString()
            })
            .ToListAsync();

        if (docId != null && documents.Count == 0)
            return ApiResponse.AccessDenied();

        return Ok(documents);
    }

}