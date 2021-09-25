using System;

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
                Console.WriteLine($"Importing CMS1500 data from {args[0]}...");
            }
            
        }
    }
}
