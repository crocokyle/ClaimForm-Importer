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
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            Console.WriteLine(dotenv);
            DotEnv.Load(dotenv);

            // Verify args and directory
            string filepath;
            if (CheckArgs(args))
            { 
                filepath = args[0];
                DirectoryInfo d = new DirectoryInfo(filepath);
                if (CheckDir(d.FullName))
                {
                    await FindPDFs(d);
                }
                else
                    Environment.Exit(66);
            }
            else
                Environment.Exit(78);
        }

        static async Task FindPDFs(DirectoryInfo d)
        {
            Console.WriteLine($"Importing forms (CMS1500) from \"{d.FullName}\"");

            // Iterate through each pdf that isn't "empty-form.pdf"
            foreach (var file in d.GetFiles("*.pdf"))
            {
                if (file.Name != "empty-form.pdf")
                {
                    // Upload the pdf and wait for a response.
                    Console.WriteLine($"Processing form {file.Name}...");
                    Dictionary<string, string> formData = await FormHandler.SendForm(file.FullName, formConfidenceThreshold, fieldConfidenceThreshold);

                    Console.WriteLine("\n");
                    Console.WriteLine("Analysis Complete! - Form Data:");
                    Console.WriteLine("-----------------------------------------------------");
                    foreach (KeyValuePair<string, string> kvp in formData)
                    {
                        Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                    }
                    Console.WriteLine("-----------------------------------------------------");
                    Console.WriteLine("\n");

                    // Send the returned data to firebase and check the response
                    var response = await Firebase.PostData(formData);
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

        static bool CheckArgs(string[] args)
        {
            // Check for inappropriate argument length
            if (args == null || args.Length != 1)
            {
                Console.WriteLine("Please specify only a source folder argument.");
                Console.WriteLine("Usage: ClaimForm.exe <source-folder>");
                return false;
            }
            else
                return true;
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
                return true;
        }
    }
}

