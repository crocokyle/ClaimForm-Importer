using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;

namespace CSHttpClientSample
{
    static class FormRecognizer
    {
        public static async void MakeRequest(string pdf)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            bool includeDetails = false;

            var acsKey = Environment.GetEnvironmentVariable("ACS_KEY");
            var modelID = Environment.GetEnvironmentVariable("MODEL_ID");
            var firebaseKey = Environment.GetEnvironmentVariable("FIREBASE_KEY");

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", acsKey);

            // Request parameters
            var uri = "https://eastus.api.cognitive.microsoft.com/formrecognizer/v2.1-preview.3/custom/models/" +
                modelID + "/analyze?includeTextDetails=" + includeDetails;
            Console.WriteLine(uri);

            HttpResponseMessage response;

            // Request body
            var rawPDF = File.ReadAllText(pdf);

            byte[] byteData = Encoding.UTF8.GetBytes(rawPDF);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                response = await client.PostAsync(uri, content);
                Console.WriteLine(response);
            }

        }
    }
}