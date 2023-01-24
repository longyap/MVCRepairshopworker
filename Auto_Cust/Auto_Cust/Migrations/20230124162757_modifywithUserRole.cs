using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auto_Cust.Migrations
{
    public partial class modifywithUserRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "userrole",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomerAutomotive",
                columns: table => new
                {
                    CustomerAutoID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerAutoBrand = table.Column<string>(nullable: false),
                    CustomerAutoColor = table.Column<string>(nullable: false),
                    CustomerAutoRegisterDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAutomotive", x => x.CustomerAutoID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomerAutomotive");

            migrationBuilder.DropColumn(
                name: "userrole",
                table: "AspNetUsers");
        }
    }
}
