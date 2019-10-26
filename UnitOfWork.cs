using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Xml;
using EnglishWordsStickers.Models;

namespace EnglishWordsStickers
{
    public class UnitOfWork
    {  
        private string _outputPath;
        private string _pathSourceFile;
        private const string _exelTemplateFilePath = "Templates/template.xlsx";

        public UnitOfWork(string pathSourceFile, string outputPath)
        {
            _outputPath = outputPath;
            _pathSourceFile = pathSourceFile;
        }


        public void Run()
        {
            var engWords = GSheetsRepository.Get(_pathSourceFile);
            var notes = new List<EngRusNoteModel>();
            var counter = 0;
            if (engWords != null)
            {
                foreach (var eng in engWords)
                {
                    counter++;
                    var percent = (double)counter / engWords.Count * 100;

                    Console.Write($"\r{(int)percent}%");
                    VisitWooordhunt(eng, out var spell, out var rus);

                    if (string.IsNullOrEmpty(rus))
                    {
                        VisitContextreverso(eng, out rus);
                    }

                    if (string.IsNullOrEmpty(rus))
                    {
                        continue;
                    }

                    var synonyms = rus.Split(',');

                    if (synonyms.Length > 1)
                    {
                        rus = $"{synonyms[0]}, {synonyms[1]}";
                    }

                    notes.Add(new EngRusNoteModel
                    {
                        Eng = eng,
                        Spell = spell,
                        Rus = rus
                    });
                }
            }

            var dic = new Dictionary<string, (string Spell, string Russian)>();

            notes.ForEach(note =>
            {
                var key = note.Eng;

                if (dic.ContainsKey(note.Eng))
                {
                    var value = dic[key];
                    dic[key] = (value.Spell, value.Russian += $"/{note.Rus}");
                }
                else
                {
                    dic[key] = (note.Spell, note.Rus);
                }
            });


            try
            {
                StickersDocumentGenerator.Generate(dic, _exelTemplateFilePath, _outputPath);
                Process.Start(_outputPath);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("To work with trial flexcel need to open source code in VS  and run program under debug.");
            }
        }

        private static void VisitWooordhunt(string en, out string spell, out string rus)
        {
            spell = "";
            rus = "";
            try
            {
                var url = $"http://wooordhunt.ru/word/{en}";
                var web = new XmlDocument();
                web.Load(url);
                spell = web.DocumentElement.SelectSingleNode("//*[@id=\"uk_tr_sound\"]/span[1]")?.InnerText ?? "";
                rus = web.DocumentElement.SelectSingleNode("//*[@id=\"wd_content\"]/span/text()")?.InnerText ?? "";
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void VisitContextreverso(string en, out string rus)
        {
            rus = "";
            try
            {
                var url = $"https://context.reverso.net/translation/english-russian/{en}";
                var web = new XmlDocument();
                web.Load(url);
                var text = web.DocumentElement.SelectSingleNode("//*[@id=\"translations-content\"]/a[1]")?.InnerText ??
                           web.DocumentElement.SelectSingleNode("//*[@id=\"translations-content\"]/div[1]")?.InnerText;
                rus = text?.Replace(Regex.Match(text, "^( |\n)*").Value, "") ?? "";
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
