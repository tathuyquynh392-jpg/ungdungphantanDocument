using System;

namespace Document.API.Models;

public class Document
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string FileName { get; set; } = string.Empty;

    public string CreatedBy { get; set; } = string.Empty;

    public DocumentStatus Status { get; set; } = DocumentStatus.Draft;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? SubmittedAt { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public string? RejectionReason { get; set; }
}
