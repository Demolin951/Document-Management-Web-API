namespace DocumentApi.Models;

public class Document
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public List<DocumentAccess> Accesses { get; set; } = new();
    public List<DocumentVersion> Versions { get; set; } = new();

    //public int UserId { get; set; }
    //public User? Owner { get; set; }
    //public byte[] Data { get; set; } = Array.Empty<byte>();
}