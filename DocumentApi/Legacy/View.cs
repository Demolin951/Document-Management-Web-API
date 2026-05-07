namespace DocumentApi.Models;

public class View
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public DateTime UploadedAtUtc { get; set; }
}