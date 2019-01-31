using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class RegionChartDataItem
    {
        public int Year { get; set; }
        public List<KeyValuePair<string, decimal>> Values { get; set; }
        public Dictionary<string, decimal> DictionaryValues { get; set; }
        public int Region { get; set; }
        public string DisplayRegionName { get; set; }
        public List<KeyValuePair<int, string>> DisplayIndicatorNames { get; set; }
    }

    public class RegionChartData
    {
        public string maxValue { get; set; }
        public string valueName { get; set; }
        public string ClientId { get; set; }
        List<RegionChartDataItem> ValuesByRegion { get; set; }
        public string Year { get; set; }
        public string Title { get; set; }
        public string enabledlegend { get; set; }
        public int Width { get; set; }
        public int ItemWidth { get; set; }
        //public List<string> Categories
        //{
        //    get
        //    {
        //        var result = new List<string>();
        //        ValuesByYear.ForEach(x => result.Add(x.Year.ToString()));
        //        return result;
        //    }
        //}
        public List<string> CategoriesRegions
        {
            get
            {
                var result = new List<string>();
                ValuesByRegion.ForEach(x => result.Add(x.DisplayRegionName));
                return result;
            }
        }
        public List<ChartSerie> Series
        {
            get
            {
                var result = new Dictionary<string, ChartSerie>();

                ValuesByRegion.ForEach(x =>
                {
                    foreach(var dv in x.DictionaryValues)
                  //  x.Values.ForEach(y =>
                    {
                        if (!result.ContainsKey(dv.Key))
                        {
                           result.Add(dv.Key, new ChartSerie()  { name = x.DisplayIndicatorNames.FirstOrDefault(kvp => kvp.Key == Convert.ToInt32(dv.Key)).Value.Replace("\r\n",""), data = new List<decimal>() });
                           // result.Add(dv.Key, new ChartSerie() { name = "tttt", data = new List<decimal>() });
                        }
                        result[dv.Key].data.Add(dv.Value);
                    };
                });

                return result.Values.ToList();
            }
        }
        public void Load(int[] indicatorIds, int[] regionIds, int start, int end,string enabledlegend, int width, int itemWidth)
        {
            using (var context = new DataContextRegionChart())
            {
                Width = width;
                ItemWidth = itemWidth;
                this.enabledlegend = enabledlegend.ToString();
                var DisplayIndicatorNames = context.GetDisplayIndicatorNames(indicatorIds.ToList());
                var RegionDisplayNames = context.GetRegionDisplayNames(regionIds.ToList());
                ValuesByRegion = context.GetIndicatorsByCategoryForRegions(indicatorIds.ToList(), DisplayIndicatorNames, RegionDisplayNames, regionIds, start, end);
            }
           
        }
    }
}