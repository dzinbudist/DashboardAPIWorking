using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations.SqlServerMigrations
{
    public partial class UserModelUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Created_By",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Created",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Date_Modified",
                table: "Users",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Modified_By",
                table: "Users",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "Users",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Created_By",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Date_Created",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Date_Modified",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Modified_By",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "Users");
        }
    }
}
