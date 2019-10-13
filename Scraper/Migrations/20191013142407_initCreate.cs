using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Scraper.Migrations
{
    public partial class initCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sessions",
                columns: table => new
                {
                    requestTime = table.Column<DateTime>(nullable: false),
                    keyWords = table.Column<string>(nullable: true),
                    requestedUrl = table.Column<string>(nullable: true),
                    query = table.Column<string>(nullable: true),
                    appearedList = table.Column<string>(nullable: true),
                    numberOfResults = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessions", x => x.requestTime);
                });

            migrationBuilder.CreateTable(
                name: "singleResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    url = table.Column<string>(nullable: true),
                    sessionrequestTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_singleResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_singleResults_sessions_sessionrequestTime",
                        column: x => x.sessionrequestTime,
                        principalTable: "sessions",
                        principalColumn: "requestTime",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_singleResults_sessionrequestTime",
                table: "singleResults",
                column: "sessionrequestTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "singleResults");

            migrationBuilder.DropTable(
                name: "sessions");
        }
    }
}
