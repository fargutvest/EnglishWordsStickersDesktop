namespace EnglishWordsPrintUtility.Models
{
    public class StickerModel
    {
        public string English { get; }

        public string Spell { get; set; }

        public string Russian { get; }

        public StickerModel(string english, string spell, string russian)
        {
            English = english;
            Spell = spell;
            Russian = russian;
        }
    }
}
