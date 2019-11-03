using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnglishWordsStickers.Models;
using FlexCel.Report;
using FlexCel.XlsAdapter;


namespace EnglishWordsStickers
{
    public static class StickersDocumentGenerator
    {
        public static void Generate(Dictionary<string, (string Spell, string Russian)> dic, string templateFilePath, string path)
        {
            var report = new FlexCelReport();

            var stickers = dic.OrderBy(_=>_.Key).Select(x => new StickerModel(x.Key, x.Value.Spell, x.Value.Russian)).ToList();
            var rows = new List<StickersRowModel>();
            
            for (var i = 0; i < stickers.Count; i += 4)
            {
                rows.Add(GetRowOfStickers(i, stickers));
            }
            
            report.AddTable("Pages", new List<StickersPageModel>
            {
                new StickersPageModel(rows)
            });

            WriteInFile(report, path, templateFilePath);
        }

        private static StickersRowModel GetRowOfStickers(int i, List<StickerModel> stickers)
        {
            StickerModel Predicate(int column)
            {
                return stickers.Count - 1 > i + column ? stickers[i + column] : null;
            }

            return new StickersRowModel(stickers[i], Predicate(1), Predicate(2), Predicate(3));
        }

        private static void WriteInFile(FlexCelReport report, string path, string templatePath)
        {
            using (var templateStream = new MemoryStream(File.ReadAllBytes(templatePath)))
            {
                using (var outStream = new MemoryStream())
                {
                    report.Run(templateStream, outStream);

                    using (var excelStream = new MemoryStream(outStream.ToArray()))
                    {
                        var excel = new XlsFile();
                        excel.Open(excelStream);

                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        excel.Save(path);
                    }
                }
            }
        }

    }
}
