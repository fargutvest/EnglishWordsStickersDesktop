using System.Collections.Generic;

namespace EnglishWordsPrintUtility.Models
{
    internal sealed class PageModel
    {
        public List<RowModel> Rows { get; }

        public PageModel(List<RowModel> rows)
        {
            Rows = rows;
        }
    }
}