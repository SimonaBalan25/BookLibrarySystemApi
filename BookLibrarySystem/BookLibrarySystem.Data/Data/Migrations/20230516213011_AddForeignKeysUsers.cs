using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddForeignKeysUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Authors_AuthorId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Authors_AuthorId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitingList_Authors_AuthorId",
                table: "WaitingList");

            migrationBuilder.DropIndex(
                name: "IX_WaitingList_AuthorId",
                table: "WaitingList");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_AuthorId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Loans_AuthorId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "WaitingList");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Loans");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "WaitingList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Reservations",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Loans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingList_ApplicationUserId",
                table: "WaitingList",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_ApplicationUserId",
                table: "Reservations",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_AspNetUsers_ApplicationUserId",
                table: "Reservations",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingList_AspNetUsers_ApplicationUserId",
                table: "WaitingList",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_AspNetUsers_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_AspNetUsers_ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_WaitingList_AspNetUsers_ApplicationUserId",
                table: "WaitingList");

            migrationBuilder.DropIndex(
                name: "IX_WaitingList_ApplicationUserId",
                table: "WaitingList");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropIndex(
                name: "IX_Loans_ApplicationUserId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WaitingList");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Loans");

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "WaitingList",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Reservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Loans",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WaitingList_AuthorId",
                table: "WaitingList",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_AuthorId",
                table: "Reservations",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_AuthorId",
                table: "Loans",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Authors_AuthorId",
                table: "Loans",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Authors_AuthorId",
                table: "Reservations",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WaitingList_Authors_AuthorId",
                table: "WaitingList",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");
        }
    }
}
