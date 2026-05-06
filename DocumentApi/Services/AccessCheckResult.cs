using DocumentApi.Models;

namespace DocumentApi.Services;

public class AccessCheckResult
{
    public bool UserExists { get; set; }
    public bool HasAccess { get; set; }
    public User? User { get; set; }
    public DocumentAccess? Access { get; set; }
}