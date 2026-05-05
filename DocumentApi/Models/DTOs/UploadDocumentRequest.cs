using Microsoft.AspNetCore.Http;

namespace DocumentApi.Models.DTOs;

public class UploadDocumentRequest
{
    public IFormFile File { get; set; } = null!;
    public string UserName { get; set; } = string.Empty;
}