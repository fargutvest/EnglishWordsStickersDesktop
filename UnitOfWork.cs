using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Xml;
using EnglishWordsStickers.Models;
using HtmlAgilityPack;

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

                    VisitWooordhunt(eng, out var spell, out var rus);

                    if (string.IsNullOrEmpty(rus))
                    {
                        VisitContextreverso(eng, out rus);
                    }

                    Console.Write($"\r{(int)percent}% {eng}({spell}):{rus} {new string(' ', 20)}");

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
                Process.Start("cmd.exe", $" /C \"{_outputPath}\"");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("To work with trial flexcel need to open source code in VS  and run program under debug.");
                Console.ReadKey();
            }
        }

        private static void VisitWooordhunt(string en, out string spell, out string rus)
        {
            spell = "";
            rus = "";
            try
            {
                var url = $"http://wooordhunt.ru/word/{en}";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                spell = doc.DocumentNode.SelectNodes("//*[@id=\"uk_tr_sound\"]/span[1]")?.First()?.InnerHtml;
                rus = doc.DocumentNode.SelectNodes("//*[@id=\"wd_content\"]/span/text()")?.First()?.InnerHtml;
                

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
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var text = doc.DocumentNode.SelectNodes("//*[@id=\"translations-content\"]/a[1]")?.First()?.InnerHtml ??
                           doc.DocumentNode.SelectNodes("//*[@id=\"translations-content\"]/div[1]")?.First()?.InnerHtml;

                rus = text?.Replace(Regex.Match(text, "^( |\n)*").Value, "") ?? "";
            }
            catch (Exception)
            {
                // ignored
            }
        }

    }
}
