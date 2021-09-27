using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer;
using Azure.AI.FormRecognizer.Models;

namespace ClaimForm_Importer
{
    static class FormHandler
    {
        public static async Task<Dictionary<string, string>> SendFormAsync(string pdfPath, double formConfidenceThreshold, double fieldConfidenceThreshold)
        {
            // Create pdf FileStream
            using var stream = new FileStream(pdfPath, FileMode.Open);

            // Get environment variables
            string endpoint = Environment.GetEnvironmentVariable("ACS_ENDPOINT");
            string acsKey = Environment.GetEnvironmentVariable("ACS_KEY");
            string modelId = Environment.GetEnvironmentVariable("MODEL_ID");

            // Create a FormRecognizerClient
            var credential = new AzureKeyCredential(acsKey);
            var client = new FormRecognizerClient(new Uri(endpoint), credential);

            // Send the filestream to ACS and wait for response
            RecognizeCustomFormsOperation operation = await client.StartRecognizeCustomFormsAsync(modelId, stream);
            Response<RecognizedFormCollection> operationResponse = await operation.WaitForCompletionAsync();
            RecognizedFormCollection forms = operationResponse.Value;
            Dictionary<string, string> formData = new Dictionary<string, string> { };

            // Iterate through the forms in the response
            foreach (RecognizedForm form in forms)
            {
                Console.WriteLine($"Form of type: {form.FormType}");

                if (form.FormTypeConfidence.HasValue)
                {
                    Console.WriteLine($"Form type confidence: {form.FormTypeConfidence.Value}");

                    // Check for low form confidence
                    if (form.FormTypeConfidence.Value < formConfidenceThreshold)
                    {
                        Console.WriteLine($"{pdfPath} confidence is below the threshold ({formConfidenceThreshold}).");
                        continue;
                    }
                }

                // Iterate through the fields in the response
                Console.WriteLine($"Form was analyzed with model with ID: {form.ModelId}");
                foreach (FormField field in form.Fields.Values)
                {
                    string value = "";

                    // Is the field a SelectionMark?
                    // NOTE: field confidence is very high on SelectionMarks, but a check could be added
                    if (field.Value.ValueType.ToString() == "SelectionMark")
                    {
                        value = field.Value.AsSelectionMarkState().ToString();
                    }
                    else
                    {
                        // Check for empty field values
                        if (field.ValueData == null)
                        {
                            Console.WriteLine($"{field.Name} is empty. Please manually enter the value:");
                            value = Console.ReadLine();
                        }

                        // Check for low field confidence
                        else if (field.Confidence < fieldConfidenceThreshold)
                        {
                            Console.WriteLine($"{field.Name} confidence({field.Confidence}) is below the threshold ({fieldConfidenceThreshold}).");
                            Console.WriteLine($"Got \"{field.ValueData.Text}\", please manually enter the correct value.");

                            value = Console.ReadLine();
                        }

                        // Assign the value for the field to what was guessed by ACS
                        else
                            value = field.ValueData.Text;
                    }
                    // Add the determined value to the dictionary
                    formData.Add(field.Name, value);
                }
            }
            return formData;
        }
    }
}