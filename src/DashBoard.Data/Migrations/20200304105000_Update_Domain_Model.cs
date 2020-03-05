using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DashBoard.Data.Migrations
{
    public partial class Update_Domain_Model : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Last_Notified",
                table: "Domains",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Last_Notified",
                table: "Domains");
        }
    }
}
