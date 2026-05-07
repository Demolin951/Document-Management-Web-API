using DocumentApi.Data;
using DocumentApi.Models;
using DocumentApi.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace DocumentApi.Services;

public class AccessService
{
    private readonly AppDbContext _context;

    public AccessService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<User?> FindUserByUserName(string userName)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x => x.Name == userName);
    }

    public async Task<DocumentAccess?> FindAccess(int userId, int documentId)
    {
        return await _context.Accesses
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.DocumentId == documentId);
    }

    public bool IsOwner(DocumentAccess access)
    {
        return access.Role == Role.Owner;
    }

    public bool IsEditorOrOwner(DocumentAccess access)
    {
        return access.Role == Role.Owner || access.Role == Role.Editor;
    }

    public async Task<AccessCheckResult> CheckAccess(string userName, int documentId)
    {
        var user = await FindUserByUserName(userName);

        if (user == null)
        {
            return new AccessCheckResult
            {
                UserExists = false,
                HasAccess = false
            };
        }

        var access = await FindAccess(user.Id, documentId);

        return new AccessCheckResult
        {
            UserExists = true,
            HasAccess = access != null,
            User = user,
            Access = access
        };
    }

    public async Task<List<DocumentAccessResponse>> GetAccesses(int documentId)
    {
        return await _context.Accesses
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
    }
}