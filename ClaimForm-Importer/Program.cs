using System;
using System.IO;

namespace ClaimForm_Importer
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length != 1)
            {
                Console.WriteLine("Please specify only a source folder argument.");
                Console.WriteLine("Usage: ClaimForm.exe <source-folder>");
            }
            else
            {
                string filepath = args[0];
                Console.WriteLine($"Importing CMS1500 data from \"{filepath}\"");
                
                DirectoryInfo d = new DirectoryInfo(filepath);

                if (!(Directory.Exists(d.FullName)))
                {
                    Console.WriteLine($"File path: \"{d.FullName}\" does not exist");
                    Environment.Exit(0);
                }
                else
                {
                    foreach (var file in d.GetFiles("*.pdf"))
                    {
                        if (file.Name != "empty-form.pdf")
                        {
                            Console.WriteLine($"Importing \"{file.Name}\"");
                        }
                    }
                }
            }
            
        }
    }
}
