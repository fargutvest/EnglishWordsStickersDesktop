using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using EnglishWordsToPrint.Models;
using FlexCel.Report;
using FlexCel.XlsAdapter;


namespace EnglishWordsToPrint
{
    public class ExcelDocument
    {
        private string _templateFilePath;
        private Dictionary<string, string> _dic;


        public ExcelDocument(Dictionary<string, string> dic, string templateFilePath)
        {
            _dic = dic;
            _templateFilePath = templateFilePath;
        }

        public void Save(string path)
        {
            var template = File.ReadAllBytes(_templateFilePath);
            var report = new FlexCelReport();

            report.AddTable("Pages", new List<PageModel> { new PageModel
            {
                Stickers = _dic.Select(x => new StickerModel { English = x.Key, Russian = x.Value }).ToList()
            } });

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
