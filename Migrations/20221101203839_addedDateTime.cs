using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoCom_API.Migrations
{
    public partial class addedDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "event_time",
                schema: "NoCom",
                table: "event_logs");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "NoCom",
                table: "profile",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "NoCom",
                table: "profile",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "NoCom",
                table: "event_logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "NoCom",
                table: "event_logs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                schema: "NoCom",
                table: "comments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                schema: "NoCom",
                table: "comments",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "NoCom",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "NoCom",
                table: "profile");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "NoCom",
                table: "event_logs");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "NoCom",
                table: "event_logs");

            migrationBuilder.DropColumn(
                name: "created_at",
                schema: "NoCom",
                table: "comments");

            migrationBuilder.DropColumn(
                name: "updated_at",
                schema: "NoCom",
                table: "comments");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "event_time",
                schema: "NoCom",
                table: "event_logs",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));
        }
    }
}
