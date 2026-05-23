using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace deneme2._0.Migrations
{
    
    public partial class AddMultiImages : Migration
    {
        
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "JournalEntries");

            migrationBuilder.CreateTable(
                name: "JournalImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    ImageData = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    ImageContentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalImages_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JournalImages_JournalEntryId",
                table: "JournalImages",
                column: "JournalEntryId");
        }

        
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JournalImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "JournalEntries",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}
