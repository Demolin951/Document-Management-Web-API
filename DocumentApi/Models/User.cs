namespace DocumentApi.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DocumentVersion> Versions { get; set; } = new();
    public List<DocumentAccess> Accesses { get; set; } = new();

}