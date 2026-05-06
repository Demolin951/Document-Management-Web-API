namespace DocumentApi.Models.DTOs;

public class TransferOwnerRequest
{
    public string CurrentOwnerUserName { get; set; } = string.Empty;
    public string NewOwnerUserName { get; set; } = string.Empty;
}