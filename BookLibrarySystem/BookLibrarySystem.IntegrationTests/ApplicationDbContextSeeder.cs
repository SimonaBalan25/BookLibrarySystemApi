using BookLibrarySystem.Data;
using BookLibrarySystem.IntegrationTests.Helpers;

namespace BookLibrarySystem.IntegrationTests
{
    public class ApplicationDbContextSeeder : IApplicationDbContextSeeder
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContextSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedDatabase()
        {
            // Add entities to the DbContext
            Utilities.InitializeDbForTests(_context);
        }
    }
}
