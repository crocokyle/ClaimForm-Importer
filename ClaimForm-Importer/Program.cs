using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ClaimForm_Importer
{
    class Program
    {
        public static double fieldConfidenceThreshold = 0.9;
        public static double formConfidenceThreshold = 0.7;
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
                string filepath;
                filepath = args[0];
                DirectoryInfo directory = new DirectoryInfo(filepath);
                if (CheckDir(directory.FullName))
                {

                    // Process the forms
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

                            // Send the returned data to firebase and check the response
                            var response = await Firebase.PostData(form);
                            if (response.StatusCode.ToString() == "OK")
                                Console.WriteLine("Data sent to Firebase successfully!");
                            else
                            {
                                Console.WriteLine("Data failed to send to Firebase with the following response:");
                                Console.WriteLine(response.ToString());
                            }
                        }
                    }
                    else
                    {
                        // Exit, we didn't process anything.
                        Console.WriteLine($"No forms were found in {directory.FullName}. Press <ENTER> to exit.");
                        Console.ReadLine();
                        Environment.Exit(66);
                    }
                }
                else
                {
                    // Exit, directory does not exist
                    Environment.Exit(66);
                }
            }
            else
            {
                // Exit, argments not met
                Environment.Exit(78);
            }
        }

        static async Task<List<Dictionary<string, string>>> ProcessPdfFilesAsync(DirectoryInfo directory)
        {
            List<Dictionary<string, string>> formsData = new List<Dictionary<string, string>>();

            Console.WriteLine($"Importing forms (CMS1500) from \"{directory.FullName}\"");

            // Iterate through each pdf that isn't "empty-form.pdf"
            foreach (var pdfFile in directory.GetFiles("*.pdf"))
            {

                if (pdfFile.Name != "empty-form.pdf")
                {
                    // Upload the pdf and wait for a response.
                    Console.WriteLine($"Processing form {pdfFile.Name}...");
                    Dictionary<string, string> thisFormData = await FormHandler.SendFormAsync(pdfFile.FullName, formConfidenceThreshold, fieldConfidenceThreshold);

                    // Add this form data to our list of forms data
                    formsData.Add(thisFormData);
                }
            }
            return formsData;
        }

        static bool CheckArgs(string[] args)
        {
            // Check for inappropriate argument length
            if (args?.Length != 1)
            {
                Console.WriteLine("Please specify only a source folder argument.");
                Console.WriteLine("Usage: ClaimForm.exe <source-folder>");
                return false;
            }
            else
            {
                return true;
            }
        }

        static bool CheckDir(string dir)
        {
            // Make sure the path exists
            if (!(Directory.Exists(dir)))
            {
                Console.WriteLine($"File path: \"{dir}\" does not exist");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

