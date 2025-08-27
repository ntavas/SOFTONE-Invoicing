using System.Net.Http.Headers;

namespace Invoicing.Tests.Shared;

public static class ApiClientExtensions
{
    public static HttpClient WithBearer(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
    
}