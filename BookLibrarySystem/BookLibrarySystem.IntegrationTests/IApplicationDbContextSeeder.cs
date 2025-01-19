using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrarySystem.IntegrationTests
{
    public interface IApplicationDbContextSeeder
    {
        void SeedDatabase();
    }
}
