using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookstore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForiegnKeyCategoryForProductInSnapShoot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CategoryId",
                value: 3);
        }
    }
}
