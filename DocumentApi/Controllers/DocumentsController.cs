using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    [HttpGet]
    public async Task<ActionResult<List<View>>> GetAll()
    {
        var documents = await _context.Documents
            .Select(d => new View
            {
                Id = d.Id,
                FileName = d.FileName,
                ContentType = d.ContentType,
                //UploadedAtUtc = d.UploadedAtUtc,
                //UserId = d.UserId

            })
            .ToListAsync();

        return Ok(documents);
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<View>> Upload([FromForm] UploadDocumentRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(ApiMessages.FileNotProvided);
        }

        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var document = new Document
        {
            FileName = request.File.FileName,
            ContentType = request.File.ContentType,
            //Data = memoryStream.ToArray(),
            //UploadedAtUtc = DateTime.UtcNow,
            //UserId = request.UserId
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        var owner = await _context.Users.FindAsync(request.UserId);

        if (owner is null)
        {
            return BadRequest("User not found");
        }

        var documentView = new View
        {
            Id = document.Id,
            FileName = document.FileName,
            ContentType = document.ContentType,
            //UploadedAtUtc = document.UploadedAtUtc,
            //UserId = document.UserId
        };

        return Ok(documentView);
    }
}
//[HttpGet("download/{id}")]
//public async Task<ActionResult> Download(int id)
//{
//    var document = await _context.Documents.FindAsync(id);

//    if (document == null)
//    {
//        return NotFound(ApiMessages.DocumentNotFound);
//    }

//return File(document.Data, document.ContentType, document.FileName);

//}
//}