using BookLibrarySystem.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibrarySystem.IntegrationTests
{
    public class BooksTestServer : TestServer
    {
        public BooksTestServer(IWebHostBuilder builder) : base(builder)
        {
            DbContext = Host.Services.GetRequiredService<ApplicationDbContext>();
        }

        public ApplicationDbContext DbContext { get; set; }
    }
}
