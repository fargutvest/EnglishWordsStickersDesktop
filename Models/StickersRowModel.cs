namespace EnglishWordsStickers.Models
{
    public class StickersRowModel
    {
        public StickerModel Column1 { get; }
        public StickerModel Column2 { get; }
        public StickerModel Column3 { get; }
        public StickerModel Column4 { get; }

        public StickersRowModel(StickerModel column1, StickerModel column2, StickerModel column3, StickerModel column4)
        {
            Column1 = column1;
            Column2 = column2;
            Column3 = column3;
            Column4 = column4;
        }
    }
}
