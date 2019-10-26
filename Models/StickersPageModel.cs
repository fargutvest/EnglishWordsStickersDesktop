using System.Collections.Generic;

namespace EnglishWordsStickers.Models
{
    internal sealed class StickersPageModel
    {
        public List<StickersRowModel> Rows { get; }

        public StickersPageModel(List<StickersRowModel> rows)
        {
            Rows = rows;
        }
    }
}