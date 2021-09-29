using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace ClaimForm_Importer
{
    public class FirebaseClient : IDatabaseClient
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

        public async Task UpdateAsync(Dictionary<string, string> data)
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

                // Check the response
                if (response.StatusCode.ToString() == "OK")
                    Console.WriteLine("Data sent to Firebase successfully!");
                else
                {
                    Console.WriteLine("Data failed to send to Firebase with the following response:");
                    Console.WriteLine(response.ToString());
                }
            }
        }
    }
}
