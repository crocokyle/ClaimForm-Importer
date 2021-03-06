using Azure.AI.FormRecognizer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ClaimForm_Importer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load Environment Variables
            string root = Directory.GetCurrentDirectory();
            string dotenv = Path.Combine(root, ".env");
            DotEnv.Load(dotenv);

            // Verify args
            if (args?.Length > 0)
            {
                // Verify directory
                string filepath = args?.FirstOrDefault();
                var directory = new DirectoryInfo(filepath);
                if (Directory.Exists(directory?.FullName))
                {
                    // Send and correct the forms
                    List<Dictionary<string, string>> formsData = await ProcessPdfFilesAsync(directory);

                    // Check to see if we processed anything
                    if (formsData.Count > 0)
                    {
                        foreach (Dictionary<string, string> form in formsData)
                        {
                            // Display the output
                            Console.WriteLine("\n");
                            Console.WriteLine("Analysis Complete! - Form Data:");
                            Console.WriteLine("-----------------------------------------------------");
                            foreach (KeyValuePair<string, string> kvp in form)
                            {
                                Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                            }
                            Console.WriteLine("-----------------------------------------------------");
                            Console.WriteLine("\n");

                            // Send the returned data to firebase
                            string firebaseUrl = Environment.GetEnvironmentVariable("FIREBASE_URL");
                            string database = "CMS1500";
                            var firebaseClient = new FirebaseClient(firebaseUrl, database);
                            await firebaseClient.UpdateAsync(form);
                        }
                    }
                    else
                    {
                        // Exit, we didn't process anything.
                        Console.WriteLine($"No forms were found in \"{filepath}\"");
                    }
                }
                else
                {
                    // Exit, directory does not exist
                    Console.WriteLine($"File path: \"{filepath}\" does not exist");
                }
            }
            else
            {
                // Exit, argments not met
                Console.WriteLine("Please specify a source folder argument.");
                Console.WriteLine("Usage: ClaimForm.exe <source-folder>");
            }
        }

        static async Task<List<Dictionary<string, string>>> ProcessPdfFilesAsync(DirectoryInfo directory)
        {
            var formsData = new List<Dictionary<string, string>>();

            Console.WriteLine($"Importing forms (CMS1500) from \"{directory.FullName}\"");

            // Iterate through each pdf that isn't "empty-form.pdf"
            foreach (FileInfo pdfFile in directory.GetFiles("*.pdf"))
            {
                if (pdfFile.Name != "empty-form.pdf")
                {
                    // Send the pdf and perform corrections.
                    Console.WriteLine($"Processing form {pdfFile.Name}...");
                    RecognizedFormCollection recognizedForm = await FormHandler.SendFormAsync(pdfFile.FullName);
                    Dictionary<string, string> thisFormData = FormHandler.CorrectForm(recognizedForm);

                    // Add this form data to our list of forms data
                    formsData.Add(thisFormData);
                }
            }
            return formsData;
        }
    }
}