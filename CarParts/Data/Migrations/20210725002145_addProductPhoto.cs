using Microsoft.EntityFrameworkCore.Migrations;

namespace CarParts.Data.Migrations
{
    public partial class addProductPhoto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductPhotoPath",
                table: "Products",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductPhotoPath",
                table: "Products");
        }
    }
}
