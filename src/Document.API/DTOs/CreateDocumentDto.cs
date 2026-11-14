namespace Document.API.DTOs;

public class CreateDocumentDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;
}