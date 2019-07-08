using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MicrosoftGraphApiBeta
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start calling the Graph API beta");
            string json = CallGraphApiBetaChannel().GetAwaiter().GetResult();
            Console.WriteLine(json);
        }

        private static async Task<string> CallGraphApiBetaChannel()
        {
            // Retrieve the access token
            var accessToken = await GetAccessToken(
                "<Your tenant id here>",
                "<Your client id here>",
                "<Your client secret here>");

            // Set the version to beta
            var graphApiVersion = "beta"; // 'beta' or 'v1.0'

            // Set the endpoint and the action we want to execute
            var endpoint = $"https://graph.microsoft.com/{graphApiVersion}";
            var action = "/applications";

            // Create the http client
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Get, endpoint + action))
            {
                // Set the headers including the authorization bearer header
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Sending the request which actually is a http get
                using (var response = await client.SendAsync(request))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the result as string, since the response will be json
                        var result = await response.Content.ReadAsStringAsync();
                        // Do something with the result
                        return result;
                    }

                    // Do something when it is not successful
                }
            }

            return string.Empty;
        }

        private static async Task<string> GetAccessToken(
            string tenantId,
            string clientId,
            string clientSecret)
        {
            var builder = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithTenantId(tenantId)
                .WithRedirectUri("http://localhost/")
                .Build();

            var acquiredTokenResult = builder.AcquireTokenForClient(
                // Here we set the scope to https://graph.microsoft.com/.default
                new List<string> { "https://graph.microsoft.com/.default" });
            var tokenResult = await acquiredTokenResult.ExecuteAsync();
            return tokenResult.AccessToken;
        }
    }
}
