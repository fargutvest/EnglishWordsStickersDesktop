using System.Collections.Generic;

namespace EnglishWordsToPrint.Models
{
    internal sealed class PageModel
    {
        public List<StickerModel> Stickers { get; set; }

        public PageModel()
        {
            Stickers = new List<StickerModel>();
        }
    }
}