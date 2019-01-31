using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.DAL
{
    public class DataContextRegionChart : DataContext
    {
        public List<RegionChartDataItem> GetIndicatorsByCategoryForRegions(List<int> indicatorsIds, List<KeyValuePair<int, string>> displayIndicatorNames, List<KeyValuePair<int, string>> regionDisplayNames, int[] regions, int startYear, int endYear)
        {
            var result = new List<RegionChartDataItem>();
            Dictionary<int, RegionChartDataItem> dic = new Dictionary<int, RegionChartDataItem>();
            
            foreach (var region in regions)
            {
                var item = new RegionChartDataItem() { Year = startYear, Region = region, DisplayIndicatorNames = displayIndicatorNames,DisplayRegionName = regionDisplayNames.FirstOrDefault(kvp => kvp.Key == region).Value };
                item.DictionaryValues = new Dictionary<string, decimal>();
                foreach (var indicatorsId in indicatorsIds)
                {
                    item.DictionaryValues.Add(indicatorsId.ToString(), 0);
                }
                result.Add(item);
                dic.Add(region, item);
            }
            var strindicatorsIds = "";
            var strregionIds = "";
            strindicatorsIds = GetStringFromListInt(indicatorsIds, strindicatorsIds);
            strregionIds = GetStringFromListInt(regions.ToList(), strregionIds);
            Op(@"SELECT ""Regions"".""Name"",""Values"".""Region"", ""Values"".""Indicator"", ""DataYear"", ""SurveyYear"", 
                round(cast(""Values""[1] as numeric),2) as Val, ""Indicators"".""Name"" as ""IndicatorName"", ""Indicators"".""Id"" as ""IndicatorId""
                FROM public.""Values""
                inner join public.""Indicators"" on ""Values"".""Indicator"" = ""Indicators"".""Id""
                inner join public.""Category"" on ""Category"".""Id"" = ""Indicators"".""Category""
                inner join public.""Regions"" on ""Regions"".""Id"" = ""Values"".""Region""
              where  @ ""DataYear"" between " + startYear + " and " + endYear +
                  @" and ""Indicators"".""Id"" In(" + strindicatorsIds + ")"   +
                   @" and ""Values"".""Region"" In(" + strregionIds + ")" +
                 ";",
                 (dr) =>
                 {
                     string indicatorId = Convert.ToString(dr["Indicator"]);
                     int region = Convert.ToInt32(dr["Region"]);
                     //float[] value = dr["Val"] as float[];
                     if (null != dr["Val"])
                     {
                         var val = Convert.ToDecimal(dr["Val"]);
                         var convertVal = val.ToString("0.0");
                         dic[region].DictionaryValues[indicatorId] = Convert.ToDecimal(convertVal);
                     }                 
                 });
                return result;
        }
      
        public List<KeyValuePair<int, string>> GetDisplayIndicatorNames(List<int> indicatorsIds)
        {
            var strindicatorsIds = "";
            strindicatorsIds = GetStringFromListInt(indicatorsIds, strindicatorsIds);
            var result = new List<KeyValuePair<int, string>>();
            Op(@"SELECT ""Indicators"".""Name"", ""Indicators"".""Id""
                            FROM public.""Indicators"" where ""Indicators"".""Id"" In(" + strindicatorsIds + "); ",
           (dr) =>
           {
               int id = Convert.ToInt32(dr["Id"]);
               //if (indicatorsIds.Count > 0 && !indicatorsIds.Contains(id))
               //{
               //    return;
               //}
               string indicatorName = Convert.ToString(dr["Name"]);
               result.Add(new KeyValuePair<int, string>(id, indicatorName));
           });
            return result;
        }
        public List<KeyValuePair<int, string>> GetRegionDisplayNames(List<int> regionIds)
        {
            var strregionsIds = "";
            strregionsIds = GetStringFromListInt(regionIds, strregionsIds);
            var result = new List<KeyValuePair<int, string>>();
            Op(@"SELECT ""Regions"".""Name"", ""Regions"".""Id""
                            FROM public.""Regions"" where ""Regions"".""Id"" In(" + strregionsIds + "); ",
           (dr) =>
           {
               int id = Convert.ToInt32(dr["Id"]);
               string indicatorName = Convert.ToString(dr["Name"]);
               result.Add(new KeyValuePair<int, string>(id, indicatorName));
           });
            return result;
        }

        private static string GetStringFromListInt(List<int> indicatorsIds, string strindicatorsIds)
        {
            if (indicatorsIds.Count > 1)
            {
                foreach (var indicatorsId in indicatorsIds)
                {
                    if (strindicatorsIds != "") strindicatorsIds = strindicatorsIds + "," + indicatorsId.ToString();
                    else strindicatorsIds = indicatorsId.ToString();
                }
            }
            else
            {
                strindicatorsIds = indicatorsIds[0].ToString();
            }

            return strindicatorsIds;
        }
    }
}