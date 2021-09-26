﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ClaimForm_Importer
{
    class Program
    {
        public static double fieldConfidenceThreshold = 0.7;
        public static double formConfidenceThreshold = 0.7;
        static async Task Main(string[] args)
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
                await FindPDFs(d);
            }
        }

        static async Task FindPDFs(DirectoryInfo d)
        {
            Console.WriteLine($"Importing forms (CMS1500) from \"{d.FullName}\"");

            // Iterate through each pdf that isn't "empty-form.pdf"
            foreach (var file in d.GetFiles("*.pdf"))
            {
                if (file.Name != "empty-form.pdf" && file.Name != "test2.pdf") // TODO Remove second conditional
                {
                    // Upload the pdf and wait for a response.
                    Console.WriteLine($"Uploading form {file.Name}...");
                    Task formData = FormHandler.SendForm(file.FullName, formConfidenceThreshold, fieldConfidenceThreshold);
                    // Anything we can do while waiting?
                    Console.WriteLine("Processing...");
                    await formData;
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

