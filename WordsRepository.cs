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
using System.Net;

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


            var request = service.Spreadsheets.Values.Get(spreadsheetId, "A:Z");

            var response = request.Execute();
            var values = response.Values;

            var notes = new List<EngRusNoteModel>();
            if (values != null)
            {
                foreach (var value in values)
                {
                    var res = TryGetSpell(value[0].ToString(), out var resultSpell);

                    notes.Add(new EngRusNoteModel()
                    {
                        English = value[0].ToString(),
                        Spell = resultSpell,
                        Russian = value[2].ToString()
                    });
                }
            }


            var model = new WordsRepository
            {
                NotesEngRus = notes
            };

            return model;
        }

       private static bool TryGetSpell(string word, out string spell)
        {
            spell = "";
            var req = WebRequest.Create($"http://wooordhunt.ru/word/{word}");
            var resp = (HttpWebResponse)req.GetResponse();

            using (StreamReader stream = new StreamReader(
                resp.GetResponseStream()))
            {
                var htmlResponce = stream.ReadToEnd();
                var stage1 = Regex.Match(htmlResponce, "uk_tr_sound.*class=\"transcription\"> \\|?.*\\|").Value;
                var stage2 = Regex.Match(stage1, "(\\|.*\\|)").Value;
                spell = stage2.Trim('|');
            }

            return spell != "";
        }

       
    }
}
