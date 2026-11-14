using Document.API.Data;
using Document.API.DTOs;
using DocumentModel = global::Document.API.Models.Document;
using DocumentModelStatus = global::Document.API.Models.DocumentStatus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Document.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DocumentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/documents
    [HttpPost]
    public async Task<ActionResult<DocumentModel>> Create(
        CreateDocumentDto dto)
    {
        var document = new DocumentModel
        {
            Title = dto.Title,
            Description = dto.Description,
            FileName = dto.FileName,
            CreatedBy = dto.CreatedBy,
            Status = DocumentModelStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        _context.Documents.Add(document);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = document.Id },
            document);
    }

    // GET: api/documents
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DocumentModel>>> GetAll()
    {
        var documents = await _context.Documents
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();

        return Ok(documents);
    }

    // GET: api/documents/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<DocumentModel>> GetById(int id)
    {
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
        {
            return NotFound(new { message = "Không tìm thấy tài liệu." });
        }

        return Ok(document);
    }
    // POST: api/documents/1/submit
    [HttpPost("{id:int}/submit")]
    public async Task<IActionResult> Submit(int id)
    {
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài liệu."
            });
        }

        if (document.Status != DocumentModelStatus.Draft)
        {
            return BadRequest(new
            {
                message = "Chỉ tài liệu ở trạng thái Draft mới được gửi duyệt."
            });
        }

        document.Status = DocumentModelStatus.Pending;
        document.SubmittedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(document);
    }
    // POST: api/documents/1/approve
    [HttpPost("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài liệu."
            });
        }

        if (document.Status != DocumentModelStatus.Pending)
        {
            return BadRequest(new
            {
                message = "Chỉ tài liệu đang chờ duyệt mới được phê duyệt."
            });
        }

        document.Status = DocumentModelStatus.Approved;
        document.ApprovedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(document);
    }
    // POST: api/documents/1/reject
    [HttpPost("{id:int}/reject")]
    public async Task<IActionResult> Reject(
        int id,
        RejectDocumentDto dto)
    {
        var document = await _context.Documents.FindAsync(id);

        if (document == null)
        {
            return NotFound(new
            {
                message = "Không tìm thấy tài liệu."
            });
        }

        if (document.Status != DocumentModelStatus.Pending)
        {
            return BadRequest(new
            {
                message = "Chỉ tài liệu đang chờ duyệt mới được từ chối."
            });
        }

        if (string.IsNullOrWhiteSpace(dto.Reason))
        {
            return BadRequest(new
            {
                message = "Lý do từ chối không được để trống."
            });
        }

        document.Status = DocumentModelStatus.Rejected;
        document.RejectionReason = dto.Reason;

        await _context.SaveChangesAsync();

        return Ok(document);
    }
}