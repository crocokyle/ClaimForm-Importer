using System;
using System.IO;
using ClaimForm_Importer;
using CSHttpClientSample;

namespace ClaimForm_Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load Environment Variables
            var root = Directory.GetCurrentDirectory();
            var dotenv = Path.Combine(root, ".env");
            Console.WriteLine(dotenv);
            DotEnv.Load(dotenv);

            string filepath = args[0];
            DirectoryInfo d = new DirectoryInfo(filepath);

            // Verify args and directory
            if (CheckArgs(args) & CheckDir(d.FullName))
            {
                FindPDFs(d);
            }
        }

        static void FindPDFs(DirectoryInfo d)
        {
            Console.WriteLine($"Importing forms (CMS1500) from \"{d.FullName}\"");

            // Iterate through each pdf that isn't "empty-form.pdf"
            foreach (var file in d.GetFiles("*.pdf"))
            {
                if (file.Name != "empty-form.pdf")
                {
                    Console.WriteLine($"Importing \"{file.Name}\"");
                    FormRecognizer.MakeRequest(file.FullName);
                    Console.WriteLine("Hit ENTER to exit...");
                    Console.ReadLine();
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

