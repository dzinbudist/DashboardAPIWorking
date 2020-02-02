using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations.SqlServerMigrations
{
    public partial class new_parameters2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "Created_By",
                table: "Services",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "Services",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Deleted",
                table: "Services",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Modified_By",
                table: "Services",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Created_By",
                table: "Portals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "Portals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Deleted",
                table: "Portals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Modified_By",
                table: "Portals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Log_Date",
                table: "Logs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Date_Deleted",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Modified_By",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "Date_Deleted",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "Modified_By",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "Log_Date",
                table: "Logs");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Logs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
