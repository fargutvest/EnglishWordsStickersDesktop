using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using EnglishWordsPrintUtility.Annotations;
using Microsoft.Win32;

namespace EnglishWordsPrintUtility.ViewModels
{
    public class MainWindowVM : INotifyPropertyChanged
    {
        private const string _outputPath = "toPrint.xlsx";

        private const string _exelTemplateFilePath = "Templates/template.xlsx";

        #region Properties

        private bool _isFileChoosed;

        public bool IsFileChoosed
        {
            get => _isFileChoosed;
            set
            {
                _isFileChoosed = value;
                OnPropertyChanged(nameof(IsFileChoosed));
            }
        }

        public DateTime dateTimeValue = DateTime.Now;

        public DateTime DateTimeValue
        {
            get { return dateTimeValue; }
            set
            {
                dateTimeValue = value;
                OnPropertyChanged(nameof(DateTimeValue));
            }
        }

        #endregion

        private WordsRepository repository;

        public void OpenViberCsvFile()
        {
            var ofd = new OpenFileDialog();
            ofd.Filter = "csv|*.csv";
            if (ofd.ShowDialog() == true)
            {
                IsFileChoosed = true;
                repository = WordsRepository.LoadFromFile(ofd.FileName);
                GetEarlierDateTime();
            }
        }

        private void GetEarlierDateTime()
        {
            DateTimeValue = repository.NotesEngRus.OrderBy(x => x.DateTime).First().DateTime;
        }

        public void CreateExcelFile()
        {
            var dic = new Dictionary<string, string>();

            repository.NotesEngRus.ForEach(note =>
            {
                if (note.DateTime < DateTimeValue)
                {
                    return;
                }
                if (dic.ContainsKey(note.English))
                {
                    dic[note.English] += $"/{note.Russian}";
                }
                else
                {
                    dic[note.English] = note.Russian;
                }
            });

            try
            {
                StickersDocumentGenerator.Generate(dic, _exelTemplateFilePath, _outputPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                MessageBox.Show("To work with trial flexcel need to open source code in VS  and run program under debug.");
                return;
            }

            Process.Start(_outputPath);
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
