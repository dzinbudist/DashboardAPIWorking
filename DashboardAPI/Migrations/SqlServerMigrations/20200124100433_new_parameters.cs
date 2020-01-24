using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations.SqlServerMigrations
{
    public partial class new_parameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Error_Text",
                table: "Logs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Error_Text",
                table: "Logs");
        }
    }
}
