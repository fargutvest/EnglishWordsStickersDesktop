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
                Console.WriteLine(Resources.WrongArguments);
                return;
            }

            var pathSourceFile = args[0];
            var outputPath = args[1];


            if (!File.Exists(pathSourceFile))
            {
                Console.WriteLine(Resources.NotFoundSourceFile);
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(outputPath)))
            {
                Console.WriteLine(Resources.OutputDirectoryDoesntExist);
                return;
            }

            new UnitOfWork(pathSourceFile, outputPath).Start();
        }
    }
}
