using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using EnglishWordsToPrint.Annotations;
using Microsoft.Win32;

namespace EnglishWordsToPrint.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private const string _outputPath = "toPrint.xlsx";

        private const string _exelTemplateFilePath = "Templates/template.xlsx";

        private string _viberCsvFilePath;


        private bool _isFileChoosed;

        public bool IsFileChoosed
        {
            get { return _isFileChoosed; }
            set
            {
                _isFileChoosed = value;
                OnPropertyChanged(nameof(IsFileChoosed));
            }
        }


        public void OpenViberCsvFile()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "csv|*.csv";
            if (ofd.ShowDialog() == true)
            {
                _viberCsvFilePath = ofd.FileName;
                IsFileChoosed = true;
            }
        }

        public void CreateExcelFile()
        {
            var dic = GetDictionaryFromViberCsvFile(_viberCsvFilePath);
            var excelDocument = new ExcelDocument(dic, _exelTemplateFilePath);
            excelDocument.Save(_outputPath);

            Process.Start(_outputPath);
        }

        private static Dictionary<string, string> GetDictionaryFromViberCsvFile(string filePath)
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

            var words = new Dictionary<string, string>();
            list.ForEach(x =>
            {
                var arr = x.Split(' ');
                if (arr.Length > 1)
                {
                    words[arr[0]] = arr[1];
                }
            });

            return words;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
