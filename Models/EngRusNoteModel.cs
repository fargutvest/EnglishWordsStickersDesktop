using System;
using System.Globalization;
using System.Text.RegularExpressions;


namespace EnglishWordsPrintUtility.Models
{
    public class EngRusNoteModel
    {
        private static readonly char splitter = ',';
        private static readonly string dateTimeFormat = "dd/MM/yyyy hh:mm:ss tt";
        private const string engRusPattern = @"^[a-z]+\s[а-я]+";

        public DateTime DateTime { get; private set; }
        public string English { get; private set; }
        public string Russian { get; private set; }


        public static bool TryParse(string text, out EngRusNoteModel model)
        {
            model = null;
            try
            {
                var splitted = text.Split(splitter);

                if (splitted.Length > 4)
                {
                    var dateTime = DateTime.ParseExact($"{splitted[0]} {splitted[1]}", dateTimeFormat, CultureInfo.CurrentCulture);
                    var sender = splitted[2];
                    var phone = splitted[3];
                    var message = splitted[4];

                    var regex = new Regex(engRusPattern);

                    if (regex.Matches(message).Count != 1)
                    {
                        return false;
                    }

                    var firstSpaceIndex = message.IndexOf(' ');
                    var eng = message.Substring(0, firstSpaceIndex);
                    var rus = message.Substring(firstSpaceIndex + 1);
                    model = new EngRusNoteModel
                    {
                        English = eng,
                        Russian = rus,
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
