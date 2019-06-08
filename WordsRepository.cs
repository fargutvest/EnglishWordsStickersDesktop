using System;
using EnglishWordsPrintUtility.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;
using HtmlAgilityPack;

namespace EnglishWordsPrintUtility
{
    public class WordsRepository
    {
        public List<EngRusNoteModel> NotesEngRus { get; private set; }


        public static WordsRepository LoadFromCsvFile(string filePath)
        {
            var notes = new List<EngRusNoteModel>();

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var lineString = reader.ReadLine()?.ToLower();
                    var parseResult = EngRusNoteModel.TryParse(lineString, out var noteModel);

                    if (parseResult)
                    {
                        notes.Add(noteModel);
                    }
                }
            }

            var model = new WordsRepository
            {
                NotesEngRus = notes
            };

            return model;
        }

        public static WordsRepository LoadFromGSheetFile(string filePath)
        {
            string spreadsheetId = string.Empty;
            using (var reader = new StreamReader(filePath))
            {
                var lineString = reader.ReadLine();
                spreadsheetId = Regex.Match(lineString ?? throw new InvalidOperationException(), "(?<=doc_id....).*?(?=\")").Value;
            }

            UserCredential credential;
            var credentialdPath = "credentials.json";
            using (var stream = new FileStream(credentialdPath, FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { SheetsService.Scope.SpreadsheetsReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }


            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Sheets API .NET Quickstart"
            });


            var request = service.Spreadsheets.Values.Get(spreadsheetId, "A:A");

            var response = request.Execute();
            var values = response.Values;
            int counter = 0;
            var notes = new List<EngRusNoteModel>();
            if (values != null)
            {
                foreach (var value in values)
                {
                    counter++;
                    var percent = ((double)counter / values.Count) * 100;

                    Console.Write($"\r{(int)percent}%");
                    var en = value[0].ToString();
                    VisitWooordhunt(en, out var spell, out var russian);

                    if (russian == "")
                    {
                        VisitContextreverso(en, out russian);
                    }

                    notes.Add(new EngRusNoteModel
                    {
                        English = en,
                        Spell = spell,
                        Russian = russian
                    });
                }
            }


            var model = new WordsRepository
            {
                NotesEngRus = notes
            };

            return model;
        }

        private static void VisitWooordhunt(string en, out string spell, out string russian)
        {
            spell = "";
            russian = "";
            try
            {
                var url = $"http://wooordhunt.ru/word/{en}";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                spell = doc.DocumentNode.SelectSingleNode("//*[@id=\"uk_tr_sound\"]/span[1]")?.InnerText ?? "";
                russian = doc.DocumentNode.SelectSingleNode("//*[@id=\"wd_content\"]/span/text()")?.InnerText ?? "";
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private static void VisitContextreverso(string en, out string russian)
        {
            russian = "";
            try
            {
                var url = $"https://context.reverso.net/translation/english-russian/{en}";
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var text = doc.DocumentNode.SelectSingleNode("//*[@id=\"translations-content\"]/a[1]")?.InnerText ??
                           doc.DocumentNode.SelectSingleNode("//*[@id=\"translations-content\"]/div[1]")?.InnerText;
                russian = text?.Replace(Regex.Match(text, "^( |\n)*").Value, "") ?? "";
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
