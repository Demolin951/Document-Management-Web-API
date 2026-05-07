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
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
    {
        var user = await _accessService.FindUserByUserName(request.UserName);

        if (user == null)
            return NotFound("User not found");

        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest("File is required");
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

        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetDocuments([FromQuery] string username, [FromQuery] int? docId)
    {
        var user = await _accessService.FindUserByUserName(username);

        if (user == null)
            return NotFound("User not found");

        var query = _context.Accesses
            .Include(x => x.Document)
            .Where(x => x.UserId == user.Id);

        if (docId == null)
        {
            var documents = await query
                .Select(x => new DocumentResponse
                {
                    Id = x.Document.Id,
                    FileName = x.Document.FileName,
                    CreatedAtUtc = x.Document.CreatedAtUtc,
                    Rolle = x.Role.ToString()
                })
                .ToListAsync();
            return Ok(documents);
        }

        var access = await query
            .FirstOrDefaultAsync(x => x.DocumentId == docId.Value);

        if (access == null)
            return StatusCode(403, "Access denied");

        var response = new DocumentResponse
        {
            Id = access.DocumentId,
            FileName = access.Document.FileName,
            CreatedAtUtc = access.Document.CreatedAtUtc,
            Rolle = access.Role.ToString()
        };

        return Ok(response);
    }

}