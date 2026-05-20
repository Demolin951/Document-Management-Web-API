namespace DocumentApi.Models.DTOs;

public class DocumentResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
}