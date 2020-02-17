using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DashBoard.Data.Migrations
{
    public partial class new_fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Team_Key",
                table: "Users",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "Team_Key",
                table: "Logs",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "Latency_Threshold_Ms",
                table: "Domains",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "Team_Key",
                table: "Domains",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Team_Key",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Team_Key",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Latency_Threshold_Ms",
                table: "Domains");

            migrationBuilder.DropColumn(
                name: "Team_Key",
                table: "Domains");
        }
    }
}
