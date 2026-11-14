using System;

using DocumentModel = global::Document.API.Models.Document;
using Microsoft.EntityFrameworkCore;

namespace Document.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DocumentModel> Documents { get; set; }
}