using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using EnglishWordsPrintUtility.Annotations;
using EnglishWordsPrintUtility.Helpers;
using Microsoft.Win32;

namespace EnglishWordsPrintUtility.ViewModels
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
            var dic = ViberMessagesHelper.ExctractEngRusDictionaryFromCsvFile(_viberCsvFilePath);
            try
            {
                StickersTapeHelper.SaveDictionaryToTapeFile(dic, _exelTemplateFilePath, _outputPath);
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
