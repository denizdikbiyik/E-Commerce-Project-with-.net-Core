using Microsoft.EntityFrameworkCore.Migrations;

namespace denizdikbiyik_CET322_FinalProject.Data.Migrations
{
    public partial class _3005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductCountSales",
                table: "Sales",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductCountSales",
                table: "Sales");
        }
    }
}
