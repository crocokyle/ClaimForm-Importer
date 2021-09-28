using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace ClaimForm_Importer
{
    public class FirebaseClient
    {
        public string Url;
        public string Database;
        private string RequestUrl;

        public FirebaseClient(string url, string database)
        {
            this.Url = url;
            this.Database = database;
            this.RequestUrl = $"{this.Url}{this.Database}.json";
        }
        public async Task<HttpResponseMessage> PostData(Dictionary<string, string> data)
        {
            Console.WriteLine($"Sending data to {RequestUrl}...");

            // Serialize Dictionary to JSON
            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            // Create a HttpClient and send the POST request
            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    RequestUrl,
                    new StringContent(json, Encoding.UTF8, "application/json"));

                return response;
            }
        }
    }
}
