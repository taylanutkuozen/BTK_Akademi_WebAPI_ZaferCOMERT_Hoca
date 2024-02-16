using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateManyto1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "849cc29e-a05b-4632-8f8f-4404b26e319a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "df53a3c7-01a1-4a5e-8c91-9da2abf3aab0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8a58d10-b246-4e85-8225-0199c62a68a0");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 3);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6b666d9d-7968-4ca9-ac73-04e13ef7db47", null, "Admin", "ADMIN" },
                    { "9ad32d80-0895-4349-a247-3792cc35665c", null, "User", "USER" },
                    { "9f9fa4a5-9109-45be-9d47-ec5c05a015e1", null, "Editor", "EDITOR" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookID", "CategoryID", "Price", "Title" },
                values: new object[] { 4, 1, 125m, "Incognito" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b666d9d-7968-4ca9-ac73-04e13ef7db47");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9ad32d80-0895-4349-a247-3792cc35665c");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9f9fa4a5-9109-45be-9d47-ec5c05a015e1");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "BookID",
                keyValue: 4);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "849cc29e-a05b-4632-8f8f-4404b26e319a", null, "Editor", "EDITOR" },
                    { "df53a3c7-01a1-4a5e-8c91-9da2abf3aab0", null, "Admin", "ADMIN" },
                    { "f8a58d10-b246-4e85-8225-0199c62a68a0", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "BookID", "CategoryID", "Price", "Title" },
                values: new object[] { 3, 1, 125m, "Incognito" });
        }
    }
}
