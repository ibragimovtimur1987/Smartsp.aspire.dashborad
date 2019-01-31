using Smartsp.aspire.dashborad.Models;
using Smartsp.aspire.dashborad.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static Smartsp.aspire.dashborad.Models.MSPCompareBySourceDataItem;

namespace Smartsp.aspire.dashborad.DAL
{
    public class MSPCompareBySourceDataContext : CompareMCPDataContext
    {
        //первая таблица - значения
        public List<MSPCompareBySourceDataItem> GetMSPCompareBySourceData(int regions, int[] areas, int year)
        {
            var result = new List<MSPCompareBySourceDataItem>();

            var item = new MSPCompareBySourceDataItem()
            {
                Federal = new List<KeyValuePair<string, float>>(),
                Regional = new List<KeyValuePair<string, float>>(),
                Common = new List<KeyValuePair<string, float>>()
            };

            Op(MSPCompareBySourceQuery.QueryMspCompareBySource(regions, areas, year),
                dr =>
                {
                    var name = Convert.ToString(dr["Name"]);
                    item.Federal.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Federal"])));
                    item.Regional.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Regional"])));
                    item.Common.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Common"])));
                });
            result.Add(item);

            return result;
        }

        //первая таблица - боковик
        internal List<KeyValuePair<int, string>> GetRowTitlePairListAreas(int[] areas)
        {
            var result = new List<KeyValuePair<int, string>>();
            Op(MSPCompareBySourceQuery.GetCategoriesTree(areas),
                    dr =>
                    {
                        var regId = Convert.ToInt32(dr["Id"]);
                        var name = Convert.ToString(dr["Name"]);
                        result.Add(new KeyValuePair<int, string>(regId, name));
                    });
            result.Add(new KeyValuePair<int, string>(999999, "Итого"));
            return result;
        }
        //вторая таблица - боковик
        internal List<KeyValuePair<int, string>> GetRowTitlePairListSources()
        {
            var result = new List<KeyValuePair<int, string>>();
            result.Add(new KeyValuePair<int, string>(1, "Федеральный бюджет (тыс.руб.)"));
            result.Add(new KeyValuePair<int, string>(2, "Региональный бюджет (тыс.руб.)"));
            result.Add(new KeyValuePair<int, string>(3, "Софинансирование (тыс.руб.)"));
            result.Add(new KeyValuePair<int, string>(999999, "Итого (тыс. руб.)"));

            return result;
        }

        //вторая таблица - значения
        internal List<MSPCompareBySourceDataItem> GetMSPCompareBySourceData(int regions, int[] areas, int startYear, int endYear)
        {
            var result = new List<MSPCompareBySourceDataItem>();
            for (var year = startYear; year <= endYear; year++)
            {
                var item = new MSPCompareBySourceDataItem()
                {
                    Year = year,
                    Title = year.ToString(),
                    Columns = new List<Column>()
                };
                foreach(int area in areas)
                {
                    item.Columns.Add(new Column()
                    {
                        AreaId = area,
                        Title = GetCategoryById(area).Title,
                        Values = new List<KeyValuePair<string, float>>()
                    });
                }
                
                
                Op(MSPCompareBySourceQuery.QueryMspCompareByType(regions, areas, year),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        foreach (int area in areas)
                        {
                            var col = item.Columns.Where(t => t.AreaId == area).FirstOrDefault();
                            if (col.Title == "1. Социальная поддержка")
                            {
                                col.Values.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["SocialSupport"])));
                            }
                            if (col.Title == "2. Рынок труда")
                            {
                                col.Values.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["LaborMarket"])));
                            }
                        }
                    });
                result.Add(item);
            }
            return result;
        }

        //третья таблица - значения
        public List<MSPCompareBySourceDataItem> GetMSPCompareBySourceData(int[] regions, int[] areas, int year)
        {
            var result = new List<MSPCompareBySourceDataItem>();
            for(var i = 0; i < areas.Length; i++)
            {
                var item = new MSPCompareBySourceDataItem()
                {

                    Federal = new List<KeyValuePair<string, float>>(),
                    Regional = new List<KeyValuePair<string, float>>(),
                    Common = new List<KeyValuePair<string, float>>(),
                    Summ = new List<KeyValuePair<string, float>>(),
                    FederalColTitle = "Расходы из федерального бюджета (тыс. руб.)",
                    RegionalColTitle = "Расходы из регионального бюджета (тыс. руб.)",
                    CommonColTitle = "Софинансирование (тыс.руб.)",
                    SummColTitle = "Итого (тыс.руб.)"
                };

                item.Title = GetCategoryById(areas[i]).Title;

                Op(MSPCompareBySourceQuery.QueryMspCompareByType(regions, areas[i], year),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        item.Federal.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Federal"])));
                        item.Regional.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Regional"])));
                        item.Common.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Common"])));
                        item.Summ.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Summ"])));
                    });

                result.Add(item);
            }


            return result;
        }
    }
}