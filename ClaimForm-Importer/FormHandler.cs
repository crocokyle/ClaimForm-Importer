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
        public static double fieldConfidenceThreshold = 0.9;
        public static double formConfidenceThreshold = 0.7;
        public static async Task<RecognizedFormCollection> SendFormAsync(string pdfPath)
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

            return forms;
        }

        public static Dictionary<string, string> CorrectForm(RecognizedFormCollection forms)
        { 
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
                        Console.WriteLine($"Form confidence is below the threshold ({formConfidenceThreshold}).");
                        continue;
                    }
                }

                // Iterate through the fields in the response
                Console.WriteLine($"Form was analyzed with model with ID: {form.ModelId}");
                foreach (FormField field in form.Fields.Values)
                {
                    string fieldValue = "";

                    // Is the field a SelectionMark?
                    // NOTE: field confidence is very high on SelectionMarks, but a check could be added
                    if (field.Value.ValueType.ToString() == "SelectionMark")
                    {
                        fieldValue = field.Value.AsSelectionMarkState().ToString();
                    }
                    else
                    {
                        // Check for empty field values
                        if (field.ValueData == null)
                        {
                            Console.WriteLine($"{field.Name} is empty. Please manually enter the value:");
                            fieldValue = Console.ReadLine();
                        }

                        // Check for low field confidence
                        else if (field.Confidence < fieldConfidenceThreshold)
                        {
                            Console.WriteLine($"{field.Name} confidence({field.Confidence}) is below the threshold ({fieldConfidenceThreshold}).");
                            Console.WriteLine($"Got \"{field.ValueData.Text}\", please manually enter the correct value.");

                            fieldValue = Console.ReadLine();
                        }

                        // Assign the value for the field to what was guessed by ACS
                        else
                            fieldValue = field.ValueData.Text;
                    }
                    // Add the determined value to the dictionary
                    formData.Add(field.Name, fieldValue);
                }
            }
            return formData;
        }
    }
}