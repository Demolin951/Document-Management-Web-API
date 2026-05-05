namespace DocumentApi.Models.DTOs;

public class AddDocumentVersionRequest
{
    public string UserName { get; set; } = string.Empty;
    public IFormFile File { get; set; } = null!;
}