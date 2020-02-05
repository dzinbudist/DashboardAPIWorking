using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations.SqlServerMigrations
{
    public partial class RemoveErrorCodeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error_Code",
                table: "Logs");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error_Code",
                table: "Logs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
