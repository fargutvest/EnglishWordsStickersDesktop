using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnglishWordsPrintUtility.Models
{
    public class RowModel
    {
        public StickerModel Column1 { get; }
        public StickerModel Column2 { get; }
        public StickerModel Column3 { get; }
        public StickerModel Column4 { get; }

        public RowModel( StickerModel column1, StickerModel column2, StickerModel column3, StickerModel column4)
        {
            Column1 = column1;
            Column2 = column2;
            Column3 = column3;
            Column4 = column4;
        }
    }
}
