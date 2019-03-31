using EnglishWordsPrintUtility.Models;
using System.Collections.Generic;
using System.IO;

namespace EnglishWordsPrintUtility
{
    public class WordsRepository
    {      
        public List<EngRusNoteModel> NotesEngRus { get; private set; }

       
        public static WordsRepository LoadFromFile(string filePath)
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
    }
}
