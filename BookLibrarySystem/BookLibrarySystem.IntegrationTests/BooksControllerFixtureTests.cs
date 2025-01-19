using BookLibrarySystem.Logic.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.Common;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text;
using System.Net;
using BookLibrarySystem.Data;

namespace BookLibrarySystem.IntegrationTests
{
    public class BooksControllerFixtureTests : ApiIntegrationTestFixture
    {       
        protected readonly HttpClient _client;

        public BooksControllerFixtureTests(CustomWebApplicationFactory factory) : base(factory) { }       

        //[TEST GET ALL BOOKS]
        [Fact]
        public async Task CanGetBooks()
        {
            // The endpoint that gets all books22
            await AuthenticateAsync();
            using (var scope = _factory.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IApplicationDbContextSeeder>();
                seeder.SeedDatabase();
            }
            var response = await HttpClient.GetAsync("/Books");
            
            response.EnsureSuccessStatusCode();
           
            var stringResponse = await response.Content.ReadAsStringAsync();
            var books = JsonConvert.DeserializeObject<IList<BookDto>>(stringResponse);

            // Assert based on expected value.
            Assert.Contains("Expected Book Title 1", books?[0].Title);
        }

        //[TEST ADD BOOK]
        [Fact]
        public async Task PostAddAnotherNewBook()
        {
            // The endpoint that gets all books22
            await AuthenticateAsync();
            using (var scope = _factory.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IApplicationDbContextSeeder>();
                seeder.SeedDatabase();
            }

            var bookData = new BookDto { Id = 28, Title = "New Book to Add", Genre="Fiction", LoanedQuantity=3, NumberOfCopies=2,Publisher="Bentley", NumberOfPages=241, ReleaseYear=2022, Status=Data.Models.BookStatus.Available,Version=new byte[] { 0x11, 0x22 }, ISBN="220-888-111" };
            var json =
                    new StringContent(System.Text.Json.JsonSerializer.Serialize(bookData,
                                                               new JsonSerializerOptions
                                                               {
                                                                   DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                                                               }), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync("/Books", json );

            response.EnsureSuccessStatusCode();

            var stringResponse = await response.Content.ReadAsStringAsync();
            var bookToAdd = JsonConvert.DeserializeObject<BookDto>(stringResponse);

            // Assert based on expected value.
            Assert.Equal("New Book to Add", bookToAdd.Title);
        }

        //delete 1 book
        [Fact]
        public async Task DeleteBook_ShouldBeMinusOne()
        {
            await AuthenticateAsync();
            using (var scope = _factory.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IApplicationDbContextSeeder>();
                seeder.SeedDatabase();
            }

            var bookData = new BookDto { Id = 28, Title = "New Book to Add", Genre = "Fiction", LoanedQuantity = 3, NumberOfCopies = 2, Publisher = "Bentley", NumberOfPages = 241, ReleaseYear = 2022, Status = Data.Models.BookStatus.Available, Version = new byte[] { 0x11, 0x22 }, ISBN = "220-888-111" };
            var json =
                    new StringContent(System.Text.Json.JsonSerializer.Serialize(bookData,
                                                               new JsonSerializerOptions
                                                               {
                                                                   DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                                                               }), Encoding.UTF8, "application/json");
            var httpResponse = await HttpClient.DeleteAsync("/Books/2");

            httpResponse.EnsureSuccessStatusCode();

            //var stringResponse = await response.Content.ReadAsStringAsync();
            
            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        }

        //[TEST RESERVE BOOK]
        [Fact]
        public async Task ReserveBook_SetsBookInReservations()
        {
            await AuthenticateAsync();
            using (var scope = _factory.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<IApplicationDbContextSeeder>();
                seeder.SeedDatabase();
            }

            var appUserId = Guid.NewGuid().ToString();
            var httpResponse = await HttpClient.PostAsync($"/Books/ReserveAsync?bookId=1&&appUserId={appUserId}", null);

            Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
            
            var dbContext = _factory.Services.GetRequiredService<ApplicationDbContext>();
            Assert.True(dbContext.Reservations.Any(r => r.BookId == 1));
        }
    }
}
