using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DashBoard.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Domains",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Service_Name = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    Service_Type = table.Column<int>(nullable: false),
                    Method = table.Column<int>(nullable: false),
                    Basic_Auth = table.Column<bool>(nullable: false),
                    Auth_User = table.Column<string>(nullable: true),
                    Auth_Password = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    Notification_Email = table.Column<string>(nullable: false),
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

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Domain_Id = table.Column<int>(nullable: false),
                    Log_Date = table.Column<DateTime>(nullable: false),
                    Error_Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true),
                    Role = table.Column<string>(nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Created_By = table.Column<int>(nullable: false),
                    Modified_By = table.Column<int>(nullable: false),
                    Date_Created = table.Column<DateTime>(nullable: false),
                    Date_Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Domains");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
