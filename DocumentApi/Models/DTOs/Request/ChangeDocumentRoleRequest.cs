namespace DocumentApi.Models.DTOs;

public class ChangeDocumentRoleRequest
{
    public string UserName { get; set; } = string.Empty;
    public string TargetUserName { get; set; } = string.Empty;
    public Role Role { get; set; }
}