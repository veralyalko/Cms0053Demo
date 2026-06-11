using Cms0053Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<AttachmentRequest> AttachmentRequests => Set<AttachmentRequest>();
    public DbSet<ClearinghouseRecord> ClearinghouseRecords => Set<ClearinghouseRecord>();
    public DbSet<EmrDocument> EmrDocuments => Set<EmrDocument>();
    public DbSet<AttachmentTransaction> AttachmentTransactions => Set<AttachmentTransaction>();
    public DbSet<ValidationStageResult> ValidationStageResults => Set<ValidationStageResult>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttachmentRequest>()
            .HasOne(r => r.Claim)
            .WithMany(c => c.AttachmentRequests)
            .HasForeignKey(r => r.ClaimId);

        modelBuilder.Entity<AttachmentTransaction>()
            .HasOne(t => t.Claim)
            .WithMany(c => c.Attachments)
            .HasForeignKey(t => t.ClaimId)
            .IsRequired(false);

        modelBuilder.Entity<AttachmentTransaction>()
            .HasOne(t => t.AttachmentRequest)
            .WithMany(r => r.Transactions)
            .HasForeignKey(t => t.AttachmentRequestId)
            .IsRequired(false);

        modelBuilder.Entity<AttachmentTransaction>()
            .HasOne(t => t.ClearinghouseRecord)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.ClearinghouseRecordId)
            .IsRequired(false);

        modelBuilder.Entity<ValidationStageResult>()
            .HasOne(r => r.Transaction)
            .WithMany(t => t.StageResults)
            .HasForeignKey(r => r.AttachmentTransactionId);

        modelBuilder.Entity<AuditEvent>()
            .HasOne(e => e.Transaction)
            .WithMany(t => t.AuditEvents)
            .HasForeignKey(e => e.AttachmentTransactionId);
    }
}
