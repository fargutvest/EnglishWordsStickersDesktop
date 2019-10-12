using System;
using EnglishWordsPrintUtility.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace EnglishWordsPrintUtility
{
    public class GSheetsRepository
    {
        public static List<string> Get(string filePath)
        {
            var spreadsheetId = string.Empty;
            using (var reader = new StreamReader(filePath))
            {
                var lineString = reader.ReadLine();
                spreadsheetId = Regex.Match(lineString ?? throw new InvalidOperationException(), "(?<=doc_id....).*?(?=\")").Value;
            }

            SheetsService service;
            var credentialdPath = "credentials.json";
            using (var stream = new FileStream(credentialdPath, FileMode.Open, FileAccess.Read))
            {
                var credPath = "token.json";
                var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { SheetsService.Scope.SpreadsheetsReadonly },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);

                service = new SheetsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Google Sheets API .NET Quickstart"
                });
            }
            
            var words = GetNotHighlightedWords(service, spreadsheetId);
            return words;
        }

        private static List<string> GetNotHighlightedWords(SheetsService service, string spreadsheetId)
        {
            var result = new List<string>();
            var request = service.Spreadsheets.Get(spreadsheetId);
            request.IncludeGridData = true;
            var response = request.Execute();
            var rowData = response.Sheets[0].Data[0].RowData;
            foreach (var row in rowData)
            {
                foreach (var cell in row.Values)
                {
                    if (cell.UserEnteredValue == null)
                    {
                        continue;
                    }
                    var word = cell.UserEnteredValue.StringValue;
                    var isHighlighted = cell.UserEnteredFormat.BackgroundColor != null;
                    if (isHighlighted == false)
                    {
                        result.Add(word);
                    }
                }
            }

            return result;
        }


        #region  Unused
        
        public static List<string> LoadFromCsvFile(string filePath)
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

            return notes.Select(_ => _.Eng).ToList();
        }

        private static IList<IList<object>> ExcludeCachedNotes(IList<IList<object>> notes, string cacheFilePath)
        {
            try
            {
                if (File.Exists(cacheFilePath))
                {
                    var json = File.ReadAllText(cacheFilePath);
                    var fromCache = (List<EngRusNoteModel>)json.DeserialiseToObject(typeof(List<EngRusNoteModel>));
                    return notes.Where(n => fromCache.All(c => c.Eng != n[0].ToString())).ToList();
                }
                return notes;
            }
            catch (Exception)
            {
                return notes;
            }
        }

        private static void UpdateCache(List<EngRusNoteModel> notes, string cacheFilePath)
        {
            var toUpdate = new List<EngRusNoteModel>();
            if (File.Exists(cacheFilePath))
            {
                var jsonRead = File.ReadAllText(cacheFilePath);
                toUpdate = (List<EngRusNoteModel>)jsonRead.DeserialiseToObject(typeof(List<EngRusNoteModel>));
                File.Delete(cacheFilePath);
            }

            toUpdate.AddRange(notes);
            var jsonToSave = toUpdate.SerialiseToJson();
            File.WriteAllText(cacheFilePath, jsonToSave);
        }


        #endregion
    }
}
