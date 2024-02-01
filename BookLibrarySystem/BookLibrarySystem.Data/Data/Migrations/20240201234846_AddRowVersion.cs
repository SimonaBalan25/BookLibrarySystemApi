using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "07ff36e0-5f94-485a-90e2-53d65ea85b9b");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "3dc08091-41f3-4107-ad4f-2abbf0e6b3d8");

            migrationBuilder.DropColumn(
                name: "Renewed",
                table: "Loans");

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Books",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "5e216669-f56b-41b1-bfab-09633cf4af67", null, "NormalUser", "USER" },
            //        { "b94fb39d-f167-40aa-bc42-8de657acb088", null, "Administrator", "ADMINISTRATOR" }
            //    });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "5e216669-f56b-41b1-bfab-09633cf4af67");

            //migrationBuilder.DeleteData(
            //    table: "AspNetRoles",
            //    keyColumn: "Id",
            //    keyValue: "b94fb39d-f167-40aa-bc42-8de657acb088");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Books");

            migrationBuilder.AddColumn<bool>(
                name: "Renewed",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.InsertData(
            //    table: "AspNetRoles",
            //    columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            //    values: new object[,]
            //    {
            //        { "07ff36e0-5f94-485a-90e2-53d65ea85b9b", null, "NormalUser", "USER" },
            //        { "3dc08091-41f3-4107-ad4f-2abbf0e6b3d8", null, "Administrator", "ADMINISTRATOR" }
            //    });

            migrationBuilder.UpdateData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 1,
                column: "Renewed",
                value: false);

            migrationBuilder.UpdateData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 2,
                column: "Renewed",
                value: false);
        }
    }
}
