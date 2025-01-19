

using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;

namespace BookLibrarySystem.IntegrationTests
{
    public static class HttpClientFactory
    {
        public static HttpClient Create(CustomWebApplicationFactory factory)
        {
            var httpClient = factory.CreateClient();
            httpClient.BaseAddress = new Uri("https://localhost:7156/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return httpClient;
        }
    }
}
