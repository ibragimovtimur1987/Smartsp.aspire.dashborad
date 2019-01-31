using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class DictionaryRow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }

        public int AreaId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }

        public int Sort { get; set; }
        public bool IsMark { get; set; }

        public string Label
        {
            get
            {
                switch (Level)
                {
                    case 0: return "Тип: ";
                    case 1: return "Категория: ";
                    case 2: return "Подкатегория: ";
                }
                return string.Empty;
            }
        }
    }
}