using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations.SqlServerMigrations
{
    public partial class log_changed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Request_Ref",
                table: "Logs");

            migrationBuilder.DropColumn(
                name: "Request_Type",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "Domain_Id",
                table: "Logs",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain_Id",
                table: "Logs");

            migrationBuilder.AddColumn<int>(
                name: "Request_Ref",
                table: "Logs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Request_Type",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
