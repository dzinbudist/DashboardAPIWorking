using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Data.Migrations.SqlServerMigrations
{
    public partial class MergeToOneTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Portals");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Service_Type = table.Column<string>(nullable: true),
                    Method = table.Column<string>(nullable: true),
                    Basic_Auth = table.Column<bool>(nullable: false),
                    Auth_User = table.Column<string>(nullable: true),
                    Auth_Password = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    Notification_Email = table.Column<string>(nullable: true),
                    Interval_Ms = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Deleted = table.Column<bool>(nullable: false),
                    Created_By = table.Column<int>(nullable: false),
                    Modified_By = table.Column<int>(nullable: false),
                    Date_Created = table.Column<DateTime>(nullable: false),
                    Date_Modified = table.Column<DateTime>(nullable: false),
                    Last_Fail = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Domains", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.CreateTable(
                name: "Portals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Admin_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_By = table.Column<int>(type: "int", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Interval_Ms = table.Column<int>(type: "int", nullable: false),
                    Last_Fail = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified_By = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Admin_Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created_By = table.Column<int>(type: "int", nullable: false),
                    Date_Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date_Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Interval_Ms = table.Column<int>(type: "int", nullable: false),
                    Last_Fail = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Modified_By = table.Column<int>(type: "int", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });
        }
    }
}
