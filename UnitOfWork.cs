using System;
using System.Collections.Generic;
using System.Linq;
using EnglishWordsPrintUtility.Properties;

namespace EnglishWordsPrintUtility
{
    public class UnitOfWork
    {
        private string outputPath;

        private const string exelTemplateFilePath = "Templates/template.xlsx";

        private WordsRepository repository;
        

        public UnitOfWork(string pathSourceFile, string outputPath)
        {
            this.outputPath = outputPath;
            repository = WordsRepository.LoadFromFile(pathSourceFile);
            DateTimeValue = repository.NotesEngRus.OrderBy(x => x.DateTime).First().DateTime;
        }

        public DateTime DateTimeValue { get; set; }

        public void Start()
        {
            CreateExcelFile();
        }
        
        private void CreateExcelFile()
        {
            var dic = new Dictionary<string, string>();

            repository.NotesEngRus.ForEach(note =>
            {
                if (note.DateTime < DateTimeValue)
                {
                    return;
                }
                if (dic.ContainsKey(note.English))
                {
                    dic[note.English] += $"/{note.Russian}";
                }
                else
                {
                    dic[note.English] = note.Russian;
                }
            });

            try
            {
                StickersDocumentGenerator.Generate(dic, exelTemplateFilePath, outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(Resources.WorkWithTrial);
            }
        }
    }
}
