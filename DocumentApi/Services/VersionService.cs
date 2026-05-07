using System.Reflection.Metadata.Ecma335;
using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DocumentApi.Services;

public class VersionService
{
    private readonly AppDbContext _context;

    public VersionService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetLatestVersionNumber(int documentId)
    {
        return await _context.Versions
            .Where(x => x.DocumentId == documentId)
            .MaxAsync(x => (int?)x.VersionNumber) ?? 0;
    }

    public async Task<DocumentVersion?> GetLatestVersionWithDocument(int documentId)
    {
        return await _context.Versions
            .Include(x => x.Document)
            .Where(x => x.DocumentId == documentId)
            .OrderByDescending(x => x.VersionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentVersionResponse?> GetLatestVersion(int documentId)
    {
        return await _context.Versions
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
    }

    public async Task<DocumentVersion?> GetVersionWithDocument(int documentId, int versionNumber)
    {
        return await _context.Versions
            .Include(x => x.Document)
            .Where(x => x.DocumentId == documentId && x.VersionNumber == versionNumber)
            .FirstOrDefaultAsync();
    }

    public async Task<DocumentVersionResponse?> GetVersion(int documentId, int versionNumber)
    {
        return await _context.Versions
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
    }

    public async Task<List<DocumentVersionResponse>> GetVersions(int documentId)
    {
        return await _context.Versions
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
    }
}