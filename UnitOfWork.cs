using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnglishWordsPrintUtility.Models;
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

            if (pathSourceFile.EndsWith(".csv"))
                repository = WordsRepository.LoadFromCsvFile(pathSourceFile);
            if (pathSourceFile.EndsWith(".gsheet"))
                repository = WordsRepository.LoadFromGSheetFile(pathSourceFile);

            earlierDate = repository.NotesEngRus.Count > 0 ? repository.NotesEngRus.OrderBy(x => x.DateTime).First().DateTime : DateTime.Now;
        }

        private DateTime earlierDate;

        public void Start()
        {
            CreateExcelFile();
        }

        private void CreateExcelFile()
        {
            var dic = new Dictionary<string, (string Spell, string Russian)>();

            repository.NotesEngRus.ForEach(note =>
            {
                var key = note.English;

                if (note.DateTime < earlierDate)
                {
                    return;
                }
                if (dic.ContainsKey(note.English))
                {
                    var value = dic[key];
                    dic[key] = (value.Spell, value.Russian += $"/{note.Russian}");
                }
                else
                {
                    dic[key] = (note.Spell, note.Russian);
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
