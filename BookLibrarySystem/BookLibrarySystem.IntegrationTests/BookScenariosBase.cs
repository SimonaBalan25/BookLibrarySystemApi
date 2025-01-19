using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using BookLibrarySystem.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BookLibrarySystem.Data;

namespace BookLibrarySystem.IntegrationTests
{
    public class BookScenariosBase
    {
        public static BooksTestServer CreateServer()
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Test.json");

            IWebHostBuilder hostBuilder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    configurationBuilder                        
                        .AddJsonFile("appsettings.Test.json", optional: false)
                        .AddEnvironmentVariables();
                })
                .UseConfiguration(configurationBuilder.Build())
                .UseEnvironment("Test")
                .UseStartup<TestStartup>();

            BooksTestServer testServer = new(hostBuilder);

            testServer.Host.MigrateDbContext<ApplicationDbContext>((_, __) => { });

            return testServer;
        }

        public static async Task<T?> GetRequestContent<T>(
            HttpResponseMessage httpResponseMessage)
        {
            JsonSerializerOptions jsonSettings = new()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
            };

            return JsonSerializer.Deserialize<T>(
                await httpResponseMessage.Content.ReadAsStringAsync(),
                jsonSettings);
        }

        public static StringContent BuildRequestContent<T>(T content)
        {
            string serialized = JsonSerializer.Serialize(content);

            return new StringContent(serialized, Encoding.UTF8, "application/json");
        }
    }
}