using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class DictionaryModel
    {
        private List<DictionaryRow> rows = new List<DictionaryRow>();

        public IEnumerable<DictionaryRow> Rows
        {
            get
            {
                var result = new List<DictionaryRow>();
                foreach (var itemL0 in rows.Where(x => 0 == x.Level))
                {
                    result.Add(itemL0);
                    foreach (var itemL1 in GetChild(itemL0))
                    {
                        result.Add(itemL1);
                        foreach (var itemL2 in GetChild(itemL1))
                        {
                            result.Add(itemL2);
                            bool empty = true;
                            foreach (var itemL3 in GetChild(itemL2))
                            {
                                result.Add(itemL3);
                                empty = false;
                            }
                            //if (empty)
                            //{
                            //    result.Add(new DictionaryRow() {
                            //        Name = "В выбранном субъекте по подкатегории МСП нет",
                            //        Level = 3, IsMark = true
                            //    });
                            //}
                        }
                    }
                }

                return result;
            }
        }
        public void Load(int region, string search)
        {
            using (DataContext context = new DataContext())
            {
                var subCategories = new List<DictionaryRow>();
                var categories = new List<DictionaryRow>();
                var area = new List<DictionaryRow>();
                var data = context.LoadDictionary(region);
                if (search != "")
                {
                    //var indicators = context.LoadDictionary(region).Where(x => x.Name.Contains("search") && x.Level == 3).ToList();
                   
                    var indicators = data.Where(x => x.Name.Contains(search) && x.Level == 3).ToList();
                    if (indicators.Count == 0) return;
                    FillRows(data, indicators);
                }
                else
                {
                    //var indicators = data.Where(x => x.Level == 3).ToList();
                    //if (indicators.Count == 0) return;
                    //FillRows(data, indicators);
                    rows = context.LoadDictionary(region);
                }
            }
        }

        private void FillRows(List<DictionaryRow> data, List<DictionaryRow> indicators)
        {
            foreach (var ind in indicators)
            {
                if (rows.Where(x => x.Id == ind.AreaId).ToList().Count == 0) rows.Add(data.Where(x => x.Id == ind.AreaId && x.Level == 0).FirstOrDefault());
                if (rows.Where(x => x.Id == ind.CategoryId).ToList().Count == 0) rows.Add(data.Where(x => x.Id == ind.CategoryId && x.Level == 1).FirstOrDefault());
                if (rows.Where(x => x.Id == ind.SubCategoryId).ToList().Count == 0) rows.Add(data.Where(x => x.Id == ind.SubCategoryId && x.Level == 2).FirstOrDefault());
                rows.Add(ind);
            }
        }

        public IEnumerable<DictionaryRow> GetChild(DictionaryRow row)
        {
            switch (row.Level)
            {
                case 0: return rows.Where(x => 1 == x.Level && x.AreaId == row.Id);
                case 1: return rows.Where(x => 2 == x.Level && x.CategoryId == row.Id && x.AreaId == row.AreaId);
                case 2: return rows.Where(x => 3 == x.Level && x.SubCategoryId == row.Id && x.AreaId == row.AreaId
                && x.CategoryId == row.CategoryId);
            }
            return new DictionaryRow[0];
        }
    }
}