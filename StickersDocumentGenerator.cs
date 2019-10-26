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
            var template = File.ReadAllBytes(templateFilePath);
            var report = new FlexCelReport();

            var stickers = dic.Select(x => new StickerModel(x.Key, x.Value.Spell, x.Value.Russian)).ToList();
            var rows = new List<StickersRowModel>();

            StickerModel predicate(int i, int column)
            {
                return stickers.Count - 1 > i + column ? stickers[i + column] : null;
            }

            for (var i = 0; i < stickers.Count; i += 4)
            {
                rows.Add(new StickersRowModel(stickers[i], predicate(i, 1), predicate(i, 2), predicate(i, 3)));
            }


            report.AddTable("Pages", new List<StickersPageModel>
            {
                new StickersPageModel(rows)
            });

            using (var templateStream = new MemoryStream(template))
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
