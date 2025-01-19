using BookLibrarySystem.Data.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using System.Net.Http.Headers;

namespace BookLibrarySystem.IntegrationTests
{
    public class BooksControllerIntegrationTests : BookScenariosBase
    {
        [Fact]
        public async Task Get_all_product_items_and_response_status_code_ok()
        {
            // Arrange
            using BooksTestServer server = CreateServer();
            using HttpClient Client = server.CreateClient();
            Client.BaseAddress = new Uri("https://localhost:7156");
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Act
            HttpResponseMessage response = await Client.GetAsync("https://localhost:7156/Books");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        //[Fact]
        //public async Task Test_Get_All()
        //{

            //using (var client = new TestClientProvider().Client)
            //{
            //    var response = await client.GetAsync("/books");

            //    response.EnsureSuccessStatusCode();

            //    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            //}
        //}
        //[Fact]
        //public async Task Test_Post()
        //{
        //    using (var client = new TestClientProvider().Client)
        //    {
        //        var response = await client.PostAsync("/api/employee"
        //                , new StringContent(
        //                JsonConvert.SerializeObject(new Book()
        //                {
        //                    Address = "Test",
        //                    FirstName = "John",
        //                    LastName = "Mak",
        //                    CellPhone = "111-222-3333",
        //                    HomePhone = "222-333-4444"
        //                }),
        //            Encoding.UTF8,
        //            "application/json"));

        //        response.EnsureSuccessStatusCode();

        //        response.StatusCode.Should().Be(HttpStatusCode.OK);
        //    }
        //}
    }
}
