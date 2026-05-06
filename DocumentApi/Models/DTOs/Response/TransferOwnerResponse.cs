namespace DocumentApi.Models.DTOs;

public class TransferOwnerResponse
{
    public string PreviousOwner { get; set; } = string.Empty;
    public string NewOwner { get; set; } = string.Empty;
    public int DocumentId { get; set; }
}