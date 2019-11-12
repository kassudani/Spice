using Microsoft.EntityFrameworkCore.Migrations;

namespace Spice.Data.Migrations
{
    public partial class updatingMenuItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem");

            migrationBuilder.DropIndex(
                name: "IX_MenuItem_CategoryId",
                table: "MenuItem");

            migrationBuilder.AddColumn<int>(
                name: "Category Id",
                table: "MenuItem",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_Category Id",
                table: "MenuItem",
                column: "Category Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_Category_Category Id",
                table: "MenuItem",
                column: "Category Id",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MenuItem_Category_Category Id",
                table: "MenuItem");

            migrationBuilder.DropIndex(
                name: "IX_MenuItem_Category Id",
                table: "MenuItem");

            migrationBuilder.DropColumn(
                name: "Category Id",
                table: "MenuItem");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItem_CategoryId",
                table: "MenuItem",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_MenuItem_Category_CategoryId",
                table: "MenuItem",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
