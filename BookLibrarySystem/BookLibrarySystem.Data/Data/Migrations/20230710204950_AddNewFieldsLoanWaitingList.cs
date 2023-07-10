using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewFieldsLoanWaitingList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "39df0779-d853-402b-aeab-145a138daa1a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6b9f5145-ea00-456c-be27-9870dc438a26");

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "WaitingList",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Renewed",
                table: "Loans",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "15f321af-4b79-4802-bedb-57f8e6f4ae09", null, "NormalUser", "USER" },
                    { "85288b9d-acda-4cdb-9a88-dd04ffa629be", null, "Administrator", "ADMINISTRATOR" }
                });

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

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WaitingList",
                keyColumn: "Id",
                keyValue: 1,
                column: "Position",
                value: 0);

            migrationBuilder.UpdateData(
                table: "WaitingList",
                keyColumn: "Id",
                keyValue: 2,
                column: "Position",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "15f321af-4b79-4802-bedb-57f8e6f4ae09");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "85288b9d-acda-4cdb-9a88-dd04ffa629be");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "WaitingList");

            migrationBuilder.DropColumn(
                name: "Renewed",
                table: "Loans");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "39df0779-d853-402b-aeab-145a138daa1a", null, "NormalUser", "USER" },
                    { "6b9f5145-ea00-456c-be27-9870dc438a26", null, "Administrator", "ADMINISTRATOR" }
                });

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: 2);
        }
    }
}
