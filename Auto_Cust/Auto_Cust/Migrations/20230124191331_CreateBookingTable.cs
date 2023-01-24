using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Auto_Cust.Migrations
{
    public partial class CreateBookingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingTable",
                columns: table => new
                {
                    BookingID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Booking_CustomerAutoBrand = table.Column<string>(nullable: true),
                    Booking_CustomerAutoColor = table.Column<string>(nullable: true),
                    BookingDate = table.Column<DateTime>(nullable: false),
                    Services = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingTable", x => x.BookingID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingTable");
        }
    }
}
