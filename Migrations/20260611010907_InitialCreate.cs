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
                name: "DemoScenarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimAmount = table.Column<decimal>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    LoincCode = table.Column<string>(type: "TEXT", nullable: false),
                    ScenarioType = table.Column<string>(type: "TEXT", nullable: false),
                    ClinicianName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemoScenarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AttachmentTransactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ScenarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    TrackingNumber = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderNPI = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientName = table.Column<string>(type: "TEXT", nullable: false),
                    PatientDOB = table.Column<DateOnly>(type: "TEXT", nullable: false),
                    ClaimNumber = table.Column<string>(type: "TEXT", nullable: false),
                    DocumentType = table.Column<string>(type: "TEXT", nullable: false),
                    LoincCode = table.Column<string>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    X12Envelope = table.Column<string>(type: "TEXT", nullable: false),
                    CdaDocument = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    IsTampered = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttachmentTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AttachmentTransactions_DemoScenarios_ScenarioId",
                        column: x => x.ScenarioId,
                        principalTable: "DemoScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PresenterSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActiveScenarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    CurrentScreen = table.Column<int>(type: "INTEGER", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PresenterSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PresenterSessions_DemoScenarios_ActiveScenarioId",
                        column: x => x.ActiveScenarioId,
                        principalTable: "DemoScenarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                name: "IX_AttachmentTransactions_ScenarioId",
                table: "AttachmentTransactions",
                column: "ScenarioId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditEvents_AttachmentTransactionId",
                table: "AuditEvents",
                column: "AttachmentTransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_PresenterSessions_ActiveScenarioId",
                table: "PresenterSessions",
                column: "ActiveScenarioId");

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
                name: "PresenterSessions");

            migrationBuilder.DropTable(
                name: "ValidationStageResults");

            migrationBuilder.DropTable(
                name: "AttachmentTransactions");

            migrationBuilder.DropTable(
                name: "DemoScenarios");
        }
    }
}
