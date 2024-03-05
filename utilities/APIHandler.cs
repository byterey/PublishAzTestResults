using AzDoTestPlanSDK.Model;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace PublishAzDoTestResults.utilities
{
    internal class APIHandler
    {
        static HttpClient client = new HttpClient();
        string UrlEndpoint = "";
        public void SetRequestHeaders(string Token)
        {

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Token);
        }

        internal StringContent SetRequestBody(string Payload)
        {

            var content = new StringContent(Payload, Encoding.UTF8, "application/json");
            return content;
        }

        public async Task<Value> PostAPIRequest(string UrlPath, StringContent content)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string responseContent = "";

            response = await client.PostAsync($"{UrlPath}", content);
            responseContent = await response.Content.ReadAsStringAsync();
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseContent));

            return await JsonSerializer.DeserializeAsync<Value>(stream);

        }

        public async Task<Value> PatchAPIRequest(string UrlPath, StringContent content)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            string responseBody = "";

            var request = new HttpRequestMessage(new HttpMethod("PATCH"), $"{UrlPath}")
            {
                Content = content
            };

            response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                responseBody = await response.Content.ReadAsStringAsync();
            }
            else
            {
                Console.WriteLine("Error: " + response.StatusCode);
            }

            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(responseBody));

            return await JsonSerializer.DeserializeAsync<Value>(stream);
        }

        public async Task<GetAzDoTestRun> GetAPIRequest()
        {
            var streamTask = client.GetStreamAsync(UrlEndpoint);

            return await JsonSerializer.DeserializeAsync<GetAzDoTestRun>(await streamTask);
        }

        public void SetURLEndPoint(string UrlEndpoint)
        {
            this.UrlEndpoint = UrlEndpoint;
        }
    }
}

