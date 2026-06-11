using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cms0053Demo.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DiagnosisCode = table.Column<string>(type: "TEXT", nullable: false),
                    DiagnosisDescription = table.Column<string>(type: "TEXT", nullable: false),
                    AmountBilled = table.Column<decimal>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClearinghouseRecords",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackingNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    LoincCode = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    PulledAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClearinghouseRecords", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EmrDocuments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DocumentName = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    LoincCode = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmrDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackingNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentTypeRequested = table.Column<string>(type: "TEXT", nullable: false),
                    RequestReason = table.Column<string>(type: "TEXT", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProviderEmail = table.Column<string>(type: "TEXT", nullable: false),
                    SecureUploadToken = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentRequests_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TrackingNumber = table.Column<string>(type: "TEXT", nullable: false),
                    SourceType = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ServiceDate = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    LoincCode = table.Column<string>(type: "TEXT", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", nullable: false),
                    StoredFileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", nullable: false),
                    FileHash = table.Column<string>(type: "TEXT", nullable: false),
                    X12Envelope = table.Column<string>(type: "TEXT", nullable: false),
                    CdaDocument = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimId = table.Column<int>(type: "INTEGER", nullable: true),
                    AttachmentRequestId = table.Column<int>(type: "INTEGER", nullable: true),
                    ClearinghouseRecordId = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentTransactions_AttachmentRequests_AttachmentRequestId",
                        column: x => x.AttachmentRequestId,
                        principalTable: "AttachmentRequests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachmentTransactions_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttachmentTransactions_ClearinghouseRecords_ClearinghouseRecordId",
                        column: x => x.ClearinghouseRecordId,
                        principalTable: "ClearinghouseRecords",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuditEvents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttachmentTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    EventType = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    EventHash = table.Column<string>(type: "TEXT", nullable: false),
                    PreviousHash = table.Column<string>(type: "TEXT", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditEvents_AttachmentTransactions_AttachmentTransactionId",
                        column: x => x.AttachmentTransactionId,
                        principalTable: "AttachmentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ValidationStageResults",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttachmentTransactionId = table.Column<int>(type: "INTEGER", nullable: false),
                    StageOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    StageName = table.Column<string>(type: "TEXT", nullable: false),
                    Passed = table.Column<bool>(type: "INTEGER", nullable: false),
                    DurationMs = table.Column<int>(type: "INTEGER", nullable: false),
                    Detail = table.Column<string>(type: "TEXT", nullable: false),
                    ExecutedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidationStageResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidationStageResults_AttachmentTransactions_AttachmentTransactionId",
                        column: x => x.AttachmentTransactionId,
                        principalTable: "AttachmentTransactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentRequests_ClaimId",
                table: "AttachmentRequests",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTransactions_AttachmentRequestId",
                table: "AttachmentTransactions",
                column: "AttachmentRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTransactions_ClaimId",
                table: "AttachmentTransactions",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_AttachmentTransactions_ClearinghouseRecordId",
                table: "AttachmentTransactions",
                column: "ClearinghouseRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_AttachmentTransactionId",
                table: "AuditEvents",
                column: "AttachmentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidationStageResults_AttachmentTransactionId",
                table: "ValidationStageResults",
                column: "AttachmentTransactionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditEvents");

            migrationBuilder.DropTable(
                name: "EmrDocuments");

            migrationBuilder.DropTable(
                name: "ValidationStageResults");

            migrationBuilder.DropTable(
                name: "AttachmentTransactions");

            migrationBuilder.DropTable(
                name: "AttachmentRequests");

            migrationBuilder.DropTable(
                name: "ClearinghouseRecords");

            migrationBuilder.DropTable(
                name: "Claims");
        }
    }
}
