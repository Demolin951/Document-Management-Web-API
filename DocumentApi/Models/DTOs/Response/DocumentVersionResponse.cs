namespace DocumentApi.Models.DTOs;
 
public class DocumentVersionResponse
{
    public int Id { get; set; }
 
    public int DocumentId { get; set; }
 
    public int VersionNumber { get; set; }
 
    public DateTime UploadedAtUtc { get; set; }
 
    public string UploadedBy { get; set; } = string.Empty;
}