namespace EnglishWordsPrintUtility.Models
{
    public class StickerModel
    {
        public string English { get; }

        public string Russian { get; }

        public StickerModel(string english, string russian)
        {
            English = english;
            Russian = russian;
        }
    }
}
