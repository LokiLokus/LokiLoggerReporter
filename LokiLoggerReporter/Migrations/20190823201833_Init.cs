using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace lokiloggerreporter.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sources",
                columns: table => new
                {
                    SourceId = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Version = table.Column<string>(nullable: true),
                    Tag = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Secret = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sources", x => x.SourceId);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ThreadId = table.Column<int>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    LogLevel = table.Column<int>(nullable: false),
                    Message = table.Column<string>(nullable: true),
                    Class = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    Line = table.Column<int>(nullable: false),
                    LogTyp = table.Column<int>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true),
                    SourceId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Logs_Sources_SourceId",
                        column: x => x.SourceId,
                        principalTable: "Sources",
                        principalColumn: "SourceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_SourceId",
                table: "Logs",
                column: "SourceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Sources");
        }
    }
}
