using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorsToBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthor");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b3daaee2-566e-4301-9613-0b07ed9388f1");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d73f9e01-3cb4-4eb2-ad64-1bc1b81f9a74");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6f90352e-31aa-4b4d-ba25-42e0d168eda0", null, "Administrator", "ADMINISTRATOR" },
                    { "f1307eb9-8c96-48b8-bec8-7aaac8aa8fa8", null, "NormalUser", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6f90352e-31aa-4b4d-ba25-42e0d168eda0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f1307eb9-8c96-48b8-bec8-7aaac8aa8fa8");

            migrationBuilder.CreateTable(
                name: "BookAuthor",
                columns: table => new
                {
                    AuthorId = table.Column<int>(type: "int", nullable: false),
                    BookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthor", x => new { x.AuthorId, x.BookId });
                    table.ForeignKey(
                        name: "FK_BookAuthor_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookAuthor_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "b3daaee2-566e-4301-9613-0b07ed9388f1", null, "NormalUser", "USER" },
                    { "d73f9e01-3cb4-4eb2-ad64-1bc1b81f9a74", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_BookId",
                table: "BookAuthor",
                column: "BookId");
        }
    }
}
