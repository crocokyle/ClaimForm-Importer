using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.IO;
using Azure;
using Azure.AI.FormRecognizer;

namespace CSHttpClientSample
{
    static class FormRecognizer
    {
        public static async void MakeRequest(string pdf)
        {
            // Grab environment variables
            string endpoint = Environment.GetEnvironmentVariable("ACS_ENDPOINT");
            string acsKey = Environment.GetEnvironmentVariable("ACS_KEY");
            string modelId = Environment.GetEnvironmentVariable("MODEL_ID");
            string firebaseKey = Environment.GetEnvironmentVariable("FIREBASE_KEY");

            // Create a FormRecognizerClient
            var credential = new AzureKeyCredential(acsKey);
            var client = new FormRecognizerClient(new Uri(endpoint), credential);

        }
    }
}