using System.Collections.Generic;

namespace EnglishWordsPrintUtility.Models
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