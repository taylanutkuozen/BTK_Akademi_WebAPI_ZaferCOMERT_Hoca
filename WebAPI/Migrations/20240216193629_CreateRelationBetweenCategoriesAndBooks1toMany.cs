using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateRelationBetweenCategoriesAndBooks1toMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "06bd3eed-1bad-490a-b640-dee6fc7829d3");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5af238de-40e1-4ce3-b3a5-ee33062bc43f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bf28488c-d213-45bb-a814-d04a0f3dc094");

            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4098e4d6-f229-42f2-aff0-0e9c9ba1cca8", null, "Admin", "ADMIN" },
                    { "50d3226f-9d47-4494-88a2-ab53190cf4c8", null, "User", "USER" },
                    { "e680d962-9c81-4270-a192-a2e13f3e8b19", null, "Editor", "EDITOR" }
                });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 1,
                column: "CategoryID",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 2,
                column: "CategoryID",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 3,
                column: "CategoryID",
                value: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Books_CategoryID",
                table: "Books",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "CategoryID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Categories_CategoryID",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_CategoryID",
                table: "Books");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4098e4d6-f229-42f2-aff0-0e9c9ba1cca8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "50d3226f-9d47-4494-88a2-ab53190cf4c8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "e680d962-9c81-4270-a192-a2e13f3e8b19");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Books");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "06bd3eed-1bad-490a-b640-dee6fc7829d3", null, "Editor", "EDITOR" },
                    { "5af238de-40e1-4ce3-b3a5-ee33062bc43f", null, "User", "USER" },
                    { "bf28488c-d213-45bb-a814-d04a0f3dc094", null, "Admin", "ADMIN" }
                });
        }
    }
}
