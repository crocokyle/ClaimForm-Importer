using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;

namespace ClaimForm_Importer
{
    static class Firebase
    {
        public static async Task<HttpResponseMessage> PostData(Dictionary<string, string> data)
        {
            string firebaseUrl = Environment.GetEnvironmentVariable("FIREBASE_URL") + "CMS1500.json";
            Console.WriteLine($"Sending data to {firebaseUrl}...");

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync(
                    firebaseUrl,
                     new StringContent(json, Encoding.UTF8, "application/json"));

                return response;
            }
        }
    }
}
