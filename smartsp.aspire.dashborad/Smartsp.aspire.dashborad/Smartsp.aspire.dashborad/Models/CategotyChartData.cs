using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class CategotyChartDataItem
    {
        public int Year { get; set; }
        public Dictionary<string, decimal> DictionaryValues { get; set; }
        public List<KeyValuePair<string, float>> Values { get; set; }
        public List<KeyValuePair<int, string>> DisplayIndicatorNames { get; set; }
    }

    public class CategotyChartData
    {
        public string maxValue { get; set; }
        public string ClientId { get; set; }
        List<CategotyChartDataItem> ValuesByYear { get; set; }
        public string RegionDisplayName;
        public string Title { get; set; }
        public int Width { get; set; }
        public int ItemWidth { get; set; }
        public List<string> Categories
        {
            get
            {
                var result = new List<string>();
                ValuesByYear.ForEach(x => result.Add(x.Year.ToString()));
                return result;
            }
        }
        public List<ChartSerie> Series
        {
            get
            {
                var result = new Dictionary<string, ChartSerie>();

                ValuesByYear.ForEach(x =>
                {
                    foreach (var dv in x.DictionaryValues)
                    //  x.Values.ForEach(y =>
                    {
                        if (!result.ContainsKey(dv.Key))
                        {
                            result.Add(dv.Key, new ChartSerie() { name = x.DisplayIndicatorNames.FirstOrDefault(kvp => kvp.Key == Convert.ToInt32(dv.Key)).Value, data = new List<decimal>() });
                        }
                        result[dv.Key].data.Add(dv.Value);
                    };

                    //x.Values.ForEach(y =>
                    //{
                    //    if (!result.ContainsKey(y.Key))
                    //    {
                    //        result.Add(y.Key, new ChartSerie() { name = y.Key, data = new List<float>() });
                    //    }
                    //    result[y.Key].data.Add(y.Value);
                    //});
                });

                return result.Values.ToList();
            }
        }

        public void Load(string indicatorName, int[] indicatorIds, int region, int start, int end, int width, int itemWidth)
        {
            using (var context = new DataContext())
            {
                Width = width;
                ItemWidth = itemWidth;
                var DisplayIndicatorNames = context.GetDisplayIndicatorNames(indicatorIds.ToList());
                var RegionDisplayNames = context.GetRegionDisplayNames(region);
                 RegionDisplayName = RegionDisplayNames[0].Value;
                ValuesByYear = context.GetIndicatorsByCategory( indicatorName, DisplayIndicatorNames, indicatorIds.ToList(), region, start, end);
            }
        }
    }
}