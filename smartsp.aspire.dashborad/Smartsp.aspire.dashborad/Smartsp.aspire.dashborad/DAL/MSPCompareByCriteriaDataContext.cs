using Smartsp.aspire.dashborad.Models;
using Smartsp.aspire.dashborad.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.DAL
{
    public class MSPCompareByCriteriaDataContext : CompareMCPDataContext
    {
        //1-я таблица боковик
        internal List<KeyValuePair<int, string>> GetRowTitlePairListCriteria()
        {
            var result = new List<KeyValuePair<int, string>>();
            result.Add(new KeyValuePair<int, string>(1, "Адресный критерий"));
            result.Add(new KeyValuePair<int, string>(2, "Категориальный критерий"));
            //result.Add(new KeyValuePair<int, string>(3, "Смешанный критерий"));
            result.Add(new KeyValuePair<int, string>(999999, "Итого"));

            return result;
        }

        internal List<MSPCompareByCriteriaDataItem> GetMSPCompareByCriteriaData(int region, int area, int category, int startYear, int endYear)
        {
            var result = new List<MSPCompareByCriteriaDataItem>();
            for (var year = startYear; year <= endYear; year++)
            {
                var item = new MSPCompareByCriteriaDataItem()
                {
                    Year = year,
                    Title = year.ToString(),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>(),
                    
                };

                item.BeneficiariesColTitle = "Количество получателей";
                item.ExpendituresColTitle = "Расходы на финансирование выплаты (тыс. руб.)";

                Op(MSPCompareByCriteriaQuery.QueryMspCompareByType(region, area, category, year),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        item.Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                        item.Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    });
                result.Add(item);
            }
            return result;
        }

        internal List<MSPCompareByCriteriaDataItem> GetMSPCompareByCriteriaData(int[] regions, int area, int category, int year)
        {
            var result = new List<MSPCompareByCriteriaDataItem>();
            var item = new MSPCompareByCriteriaDataItem()
            {
                Year = year,
                Title = year.ToString(),
                Beneficiaries = new List<KeyValuePair<string, float>>(),
                Expenditures = new List<KeyValuePair<string, float>>(),
                Beneficiaries1 = new List<KeyValuePair<string, float>>(),
                Expenditures1 = new List<KeyValuePair<string, float>>(),
                //Beneficiaries2 = new List<KeyValuePair<string, float>>(),
                //Expenditures2 = new List<KeyValuePair<string, float>>(),
                Beneficiaries3 = new List<KeyValuePair<string, float>>(),
                Expenditures3 = new List<KeyValuePair<string, float>>()
            };

            item.BeneficiariesColTitle = "Количество получателей";
            item.ExpendituresColTitle = "Расходы на финансирование выплаты (тыс. руб.)";

            Op(MSPCompareByCriteriaQuery.QueryMspCompareByType(regions, area, category, year),
                dr =>
                {
                    var name = Convert.ToString(dr["Name"]);
                    item.Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                    item.Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    item.Beneficiaries1.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries1"])));
                    item.Expenditures1.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures1"])));
                    //item.Beneficiaries2.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries2"])));
                    //item.Expenditures2.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures2"])));
                    item.Beneficiaries3.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries3"])));
                    item.Expenditures3.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures3"])));
                });
            result.Add(item);
            return result;
        }
    }
}