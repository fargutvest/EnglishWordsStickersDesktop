using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace EnglishWordsStickers.Models
{
    public class EngRusNoteModel
    {
        private static readonly char _splitter = ',';
        private static readonly string _dateTimeFormat = "dd/MM/yyyy hh:mm:ss tt";
        private const string _engRusPattern = @"^[a-z]+\s[а-я]+";

        public DateTime DateTime { get; set; }
        public string Eng { get; set; }
        public string Spell { get; set; }
        public string Rus { get; set; }


        public static bool TryParse(string text, out EngRusNoteModel model)
        {
            model = null;
            try
            {
                var splitted = text.Split(_splitter);

                if (splitted.Length > 4)
                {
                    var dateTime = DateTime.ParseExact($"{splitted[0]} {splitted[1]}", _dateTimeFormat, CultureInfo.CurrentCulture);
                    var sender = splitted[2];
                    var phone = splitted[3];
                    var message = splitted[4];

                    var regex = new Regex(_engRusPattern);

                    if (regex.Matches(message).Count != 1)
                    {
                        return false;
                    }

                    var firstSpaceIndex = message.IndexOf(' ');
                    var eng = message.Substring(0, firstSpaceIndex);
                    var rus = message.Substring(firstSpaceIndex + 1);
                    model = new EngRusNoteModel
                    {
                        Eng = eng,
                        Rus = rus,
                        DateTime = dateTime
                    };

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
