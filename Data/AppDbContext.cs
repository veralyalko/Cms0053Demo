using Cms0053Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Cms0053Demo.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DemoScenario> DemoScenarios => Set<DemoScenario>();
    public DbSet<AttachmentTransaction> AttachmentTransactions => Set<AttachmentTransaction>();
    public DbSet<ValidationStageResult> ValidationStageResults => Set<ValidationStageResult>();
    public DbSet<AuditEvent> AuditEvents => Set<AuditEvent>();
    public DbSet<PresenterSession> PresenterSessions => Set<PresenterSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttachmentTransaction>()
            .HasOne(t => t.Scenario)
            .WithMany(s => s.Transactions)
            .HasForeignKey(t => t.ScenarioId);

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
