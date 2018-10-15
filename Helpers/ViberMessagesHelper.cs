using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;


namespace EnglishWordsPrintUtility.Helpers
{
    public static class ViberMessagesHelper
    {
        public static Dictionary<string, string> ExctractEngRusDictionaryFromCsvFile(string filePath)
        {
            var list = new List<string>();
            const string pattern = @"^[a-z]+\s[а-я]+";
            var regex = new Regex(pattern);

            using (var reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine()?.ToLower();
                    line = line?.Substring(line.LastIndexOf(',') + 1);
                    if (line != null && regex.Matches(line).Count == 1)
                    {
                        list.Add(line);
                    }
                }
            }

            const string rusPattern = @"[а-я]";
            var rusRegex = new Regex(rusPattern);
            var words = new Dictionary<string, string>();
            list.ForEach(x =>
            {
                var rusIndex = rusRegex.Match(x).Index;
                var eng = x.Substring(0, x.IndexOf(' '));
                words[eng] = x.Substring(rusIndex);
            });

            return words;
        }
    }
}
