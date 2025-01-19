using BookLibrarySystem.Data;
using Microsoft.EntityFrameworkCore;


namespace BookLibrarySystem.IntegrationTests
{
    public class ApiIntegrationTestFixture : IClassFixture<CustomWebApplicationFactory>, IDisposable
    {
        private ApplicationDbContext _todoDbContext = null!;
        protected CustomWebApplicationFactory _factory;

        protected HttpClient HttpClient = null!;

        public ApiIntegrationTestFixture(CustomWebApplicationFactory factory)
        {
            _factory = factory; 
            _todoDbContext = _factory.GetContext();
            HttpClient = _factory.CreateClient();
            //HttpClient = HttpClientFactory.Create(_factory);
        }

        protected async Task<ApplicationDbContext> GetDbContextAsync()
        {
            //await _todoDbContext.Database.EnsureCreatedAsync();
            return _factory.GetContext();
        }

        protected async Task AuthenticateAsync()
        {
            HttpClient.DefaultRequestHeaders.Add("X-Api-Key", "12345678-1234-1234-1234-1234567890ab");
        }

        public void Dispose()
        {
            //_todoDbContext.Database.EnsureDeleted();
            //_todoDbContext.Dispose();
            _factory.Dispose();
            HttpClient.Dispose();
        }
    }
}
