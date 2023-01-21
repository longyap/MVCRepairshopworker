using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AutomotiveBookingRepair.Migrations
{
    public partial class adduserrole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userrole",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserData",
                columns: table => new
                {
                    CustomerID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerName = table.Column<string>(nullable: false),
                    CustomerEmail = table.Column<string>(nullable: false),
                    CustomerAddress = table.Column<string>(nullable: false),
                    CustomerDOB = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserData", x => x.CustomerID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserData");

            migrationBuilder.DropColumn(
                name: "userrole",
                table: "AspNetUsers");
        }
    }
}
