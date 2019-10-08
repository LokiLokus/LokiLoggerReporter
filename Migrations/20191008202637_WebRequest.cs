using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace lokiloggerreporter.Migrations
{
    public partial class WebRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WebRequestId",
                table: "Logs",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WebRequest",
                columns: table => new
                {
                    WebRequestId = table.Column<string>(nullable: false),
                    HttpMethod = table.Column<string>(nullable: true),
                    Scheme = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    QueryString = table.Column<string>(nullable: true),
                    ClientIp = table.Column<string>(nullable: true),
                    TraceId = table.Column<string>(nullable: true),
                    RequestBody = table.Column<string>(nullable: true),
                    ResponseBody = table.Column<string>(nullable: true),
                    StatusCode = table.Column<int>(nullable: false),
                    Exception = table.Column<string>(nullable: true),
                    Start = table.Column<DateTime>(nullable: false),
                    End = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebRequest", x => x.WebRequestId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_WebRequestId",
                table: "Logs",
                column: "WebRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Logs_WebRequest_WebRequestId",
                table: "Logs",
                column: "WebRequestId",
                principalTable: "WebRequest",
                principalColumn: "WebRequestId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logs_WebRequest_WebRequestId",
                table: "Logs");

            migrationBuilder.DropTable(
                name: "WebRequest");

            migrationBuilder.DropIndex(
                name: "IX_Logs_WebRequestId",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "WebRequestId",
                table: "Logs");
        }
    }
}
