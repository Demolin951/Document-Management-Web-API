using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentApi.Models.DTOs;
using System.Runtime.CompilerServices;
using System.Data;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentController : ControllerBase
{
    private readonly AppDbContext _context;
    public DocumentController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument([FromForm] UploadDocumentRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Name == request.UserName);

        if (user == null)
            return NotFound("User not found");

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
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Name == username);

        var query = _context.Accesses
            .Include(x => x.Document)
            .Where(x => x.UserId == user.Id);

        var documents = await query
            .Select(x => new
            {
                x.Document.Id,
                x.Document.FileName,
                x.Document.CreatedAtUtc,
                Rolle = x.Role.ToString()
            })
            .ToListAsync();

        var access = await query
            .FirstOrDefaultAsync(x => x.DocumentId == docId.Value);

        if (access == null)
            return Forbid();

        return Ok(documents);
    }

}