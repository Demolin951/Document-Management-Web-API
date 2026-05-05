namespace DocumentApi.Models;

public class DocumentAccess
{

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int DocumentId { get; set; }
    public Document Document { get; set; } = null!;

    public Role Role { get; set; }

}