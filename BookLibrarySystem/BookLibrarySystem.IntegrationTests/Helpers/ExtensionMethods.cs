using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BookLibrarySystem.IntegrationTests.Helpers
{
    public static class ExtensionMethods
    {
        public static Task<T> GetAndDeserialize<T>(this HttpClient client, string requestUri)
        {
            return client.GetFromJsonAsync<T>(requestUri);
        }
    }
}
