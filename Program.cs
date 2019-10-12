using System;
using System.IO;

namespace EnglishWordsPrintUtility
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                NotifyUser("Wrong arguments");
                return;
            }

            var pathSourceFile = args[0];
            var outputPath = args[1];


            if (!File.Exists(pathSourceFile))
            {
                NotifyUser($"Not found source file with specified path {pathSourceFile}.");
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                NotifyUser($"Specified output directory {outputPath} doesn`t exist.");
                return;
            }

            try
            {
                new UnitOfWork(pathSourceFile, outputPath).Run();
            }
            catch (Exception e)
            {
                NotifyUser(e.Message);
            }
        }

        private static void NotifyUser(string message)
        {
            Console.WriteLine(message);
            Console.ReadLine();
        }
    }
}
