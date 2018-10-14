using System.Collections.Generic;

namespace EnglishWordsPrintUtility.Models
{
    internal sealed class PageModel
    {
        public List<StickerModel> Stickers { get; }

        public PageModel( List<StickerModel> stickers)
        {
            Stickers = stickers;
        }
    }
}