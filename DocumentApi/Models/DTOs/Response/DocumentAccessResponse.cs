using DocumentApi.Models;

namespace DocumentApi.Models.DTOs;

public class DocumentAccessResponse
{
    public int DocumentId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Role Role { get; set; }
}