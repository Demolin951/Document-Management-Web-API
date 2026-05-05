namespace DocumentApi.Models;

public class UploadDocumentRequest
{
    public IFormFile File {get; set;} = default!;
    public int UserId {get; set;}
}