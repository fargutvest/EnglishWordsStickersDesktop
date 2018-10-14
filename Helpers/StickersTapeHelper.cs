using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnglishWordsPrintUtility.Models;
using FlexCel.Report;
using FlexCel.XlsAdapter;


namespace EnglishWordsPrintUtility.Helpers
{
    public static class StickersTapeHelper
    {
        public static void SaveDictionaryToTapeFile(Dictionary<string, string> dic, string templateFilePath, string path)
        {
            var template = File.ReadAllBytes(templateFilePath);
            var report = new FlexCelReport();

            report.AddTable("Pages", new List<PageModel>
            {
                new PageModel(dic.Select(x => new StickerModel { English = x.Key, Russian = x.Value }).ToList())
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
