namespace DocumentApi.Models.DTOs;

public class DocumentResponse
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; }
    public string Rolle { get; set; } = string.Empty;
}