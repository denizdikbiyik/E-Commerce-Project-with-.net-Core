using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace denizdikbiyik_CET322_FinalProject.Data.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserContent",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UserCreatedDate",
                table: "AspNetUsers",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "UserFirstName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserImageUrl",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserLastName",
                table: "AspNetUsers",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserTelNo",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserContent",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserCreatedDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserFirstName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserImageUrl",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserLastName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "UserTelNo",
                table: "AspNetUsers");
        }
    }
}
