using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoCom_API.Migrations
{
    public partial class CommentIsDeleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_deleted",
                schema: "NoCom",
                table: "comments",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_deleted",
                schema: "NoCom",
                table: "comments");
        }
    }
}
