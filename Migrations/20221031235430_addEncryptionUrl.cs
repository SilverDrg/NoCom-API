using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoCom_API.Migrations
{
    public partial class addEncryptionUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "encrypted_url",
                schema: "NoCom",
                table: "comments",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "encrypted_url",
                schema: "NoCom",
                table: "comments");
        }
    }
}
