using System;
using System.IO;
using EnglishWordsPrintUtility;
using EnglishWordsPrintUtility.Properties;

namespace CSharpToUmlConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 2)
            {
                NotifyUser(Resources.WrongArguments);
                return;
            }

            var pathSourceFile = args[0];
            var outputPath = args[1];


            if (!File.Exists(pathSourceFile))
            {
                NotifyUser(string.Format(Resources.NotFoundSourceFile, pathSourceFile));
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                NotifyUser(string.Format(Resources.OutputDirectoryDoesntExist, outputPath));
                return;
            }

            try
            {
                new UnitOfWork(pathSourceFile, outputPath).Start();
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
