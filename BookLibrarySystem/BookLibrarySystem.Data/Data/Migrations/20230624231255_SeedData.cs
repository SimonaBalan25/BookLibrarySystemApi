using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLibrarySystem.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "Country", "Name" },
                values: new object[,]
                {
                    { 1, "Japan", "Haruki Murakami" },
                    { 2, "Denmark", "Helle Helle" },
                    { 3, "Belgium", "Georges Simenon" },
                    { 4, "Denmark", "Martin Simon" },
                    { 5, "USA", "Avi Silberchatz" },
                    { 6, "USA", "Paul Auster" }
                });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Genre", "ISBN", "LoanedQuantity", "NumberOfCopies", "NumberOfPages", "Publisher", "ReleaseYear", "Title" },
                values: new object[,]
                {
                    { 1, "Fiction-SF", "978-606-123-1", 0, 3, 505, "Klim", 2007, "Kafka on the shore" },
                    { 2, "Fiction-Romance", "093-184-732-2", 0, 4, 808, "Klim", 2011, "1Q84" },
                    { 3, "Fiction-Thriller", "731-847-427-0", 0, 3, 0, "Samleren", 2011, "Rodby-Puttgarden" },
                    { 4, "Fiction-Crime", "743-263-482-8", 0, 5, 144, "Lindhart op Linghorf", 2011, "Maigret" },
                    { 5, "NonFiction-Textbook", "943-921-813-0", 0, 10, 505, "McGraw-Hill", 2010, "Database System Concenpts 6th Edition" },
                    { 7, "NonFiction-Guide", "453-263-283-4", 0, 5, 255, "Textmaster", 2014, "Windows 8.1-Effectiv udden touch" },
                    { 8, "Fiction-Crime", "253-273-284-9", 0, 3, 458, "Faber and Faber", 1985, "The New York Triogy" }
                });

            migrationBuilder.InsertData(
                table: "BookAuthors",
                columns: new[] { "AuthorId", "BookId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 2, 3 },
                    { 2, 4 },
                    { 3, 5 },
                    { 5, 7 },
                    { 6, 8 }
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "ApplicationUserId", "BookId", "BorrowedDate", "DueDate", "ReturnedDate" },
                values: new object[,]
                {
                    { 1, "c72ac90e-ca57-4c2f-b04b-17409032aea8", 1, new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, "c72ac90e-ca57-4c2f-b04b-17409032aea8", 2, new DateTime(2023, 5, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 5, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), null }
                });

            migrationBuilder.InsertData(
                table: "Reservations",
                columns: new[] { "Id", "ApplicationUserId", "BookId", "ReservedDate", "Status" },
                values: new object[,]
                {
                    { 1, "c72ac90e-ca57-4c2f-b04b-17409032aea8", 3, new DateTime(2023, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 2, "c72ac90e-ca57-4c2f-b04b-17409032aea8", 4, new DateTime(2023, 5, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 1, 2 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 3, 5 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 5, 7 });

            migrationBuilder.DeleteData(
                table: "BookAuthors",
                keyColumns: new[] { "AuthorId", "BookId" },
                keyValues: new object[] { 6, 8 });

            migrationBuilder.DeleteData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Reservations",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 8);
        }
    }
}
