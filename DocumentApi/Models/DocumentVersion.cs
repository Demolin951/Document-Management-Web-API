namespace DocumentApi.Models;

public class DocumentVersion
{
    public int Id { get; set; }
    public int VersionNumber { get; set; }
    public int DocumentId { get; set; }
    public DateTime UploadedAtUtc { get; set; }
    public int UploadedByUserId { get; set; }
    public Document Document { get; set; } = null!;
    public User UploadedByUser { get; set; } = null!;
}