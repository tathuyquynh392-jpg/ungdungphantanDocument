using Document.API.Data;
using Document.API.DTOs;
using Document.API.Hubs;
using DocumentModel = global::Document.API.Models.Document;
using DocumentStatus = global::Document.API.Models.DocumentStatus;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Document.API.Endpoints;

public static class DocumentsEndpoints
{
    public static IEndpointRouteBuilder MapDocumentsEndpoints(
        this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/Documents");

        // POST: /api/Documents
        group.MapPost("/", async (
            CreateDocumentDto dto,
            ApplicationDbContext db,
            IHubContext<DocumentHub> hub) =>
        {
            var document = new DocumentModel
            {
                Title = dto.Title,
                Description = dto.Description,
                FileName = dto.FileName,
                CreatedBy = dto.CreatedBy,
                Status = DocumentStatus.Draft,
                CreatedAt = DateTime.UtcNow
            };

            db.Documents.Add(document);
            await db.SaveChangesAsync();

            // Phát sự kiện realtime
            await hub.Clients.All.SendAsync("DocumentCreated", document);

            return Results.Created(
                $"/api/Documents/{document.Id}",
                document);
        });

        // GET: /api/Documents
        group.MapGet("/", async (ApplicationDbContext db) =>
        {
            var documents = await db.Documents
                .OrderByDescending(d => d.CreatedAt)
                .ToListAsync();

            return Results.Ok(documents);
        });

        // GET: /api/Documents/{id}
        group.MapGet("/{id:int}", async (
            int id,
            ApplicationDbContext db) =>
        {
            var document = await db.Documents.FindAsync(id);

            return document is null
                ? Results.NotFound(new
                {
                    message = "Không tìm thấy tài liệu."
                })
                : Results.Ok(document);
        });

        // POST: /api/Documents/{id}/submit
        group.MapPost("/{id:int}/submit", async (
            int id,
            ApplicationDbContext db,
            IHubContext<DocumentHub> hub) =>
        {
            var document = await db.Documents.FindAsync(id);

            if (document is null)
                return Results.NotFound(new
                {
                    message = "Không tìm thấy tài liệu."
                });

            if (document.Status != DocumentStatus.Draft)
                return Results.BadRequest(new
                {
                    message = "Chỉ tài liệu ở trạng thái Draft mới được gửi duyệt."
                });

            document.Status = DocumentStatus.Pending;
            document.SubmittedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            // Phát sự kiện realtime
            await hub.Clients.All.SendAsync("DocumentSubmitted", document);

            return Results.Ok(document);
        });

        // POST: /api/Documents/{id}/approve
        group.MapPost("/{id:int}/approve", async (
            int id,
            ApplicationDbContext db,
            IHubContext<DocumentHub> hub) =>
        {
            var document = await db.Documents.FindAsync(id);

            if (document is null)
                return Results.NotFound(new
                {
                    message = "Không tìm thấy tài liệu."
                });

            if (document.Status != DocumentStatus.Pending)
                return Results.BadRequest(new
                {
                    message = "Chỉ tài liệu đang chờ duyệt mới được phê duyệt."
                });

            document.Status = DocumentStatus.Approved;
            document.ApprovedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            // Phát sự kiện realtime
            await hub.Clients.All.SendAsync("DocumentApproved", document);

            return Results.Ok(document);
        });

        // POST: /api/Documents/{id}/reject
        group.MapPost("/{id:int}/reject", async (
            int id,
            RejectDocumentDto dto,
            ApplicationDbContext db,
            IHubContext<DocumentHub> hub) =>
        {
            var document = await db.Documents.FindAsync(id);

            if (document is null)
                return Results.NotFound(new
                {
                    message = "Không tìm thấy tài liệu."
                });

            if (document.Status != DocumentStatus.Pending)
                return Results.BadRequest(new
                {
                    message = "Chỉ tài liệu đang chờ duyệt mới được từ chối."
                });

            if (string.IsNullOrWhiteSpace(dto.Reason))
                return Results.BadRequest(new
                {
                    message = "Lý do từ chối không được để trống."
                });

            document.Status = DocumentStatus.Rejected;
            document.RejectionReason = dto.Reason;

            await db.SaveChangesAsync();

            // Phát sự kiện realtime
            await hub.Clients.All.SendAsync("DocumentRejected", document);

            return Results.Ok(document);
        });

        return app;
    }
}