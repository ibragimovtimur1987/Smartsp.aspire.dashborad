using Npgsql;
using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.DAL
{
    public class DataContext : IDisposable
    {
        private const string connKey = "AspireDBConnection";
        private NpgsqlConnection _connection;

        public DataContext()
        {
            _connection = new NpgsqlConnection(ConfigurationManager.ConnectionStrings[connKey].ConnectionString);
            _connection.Open();
        }

        public DataContext(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
        }

        protected void Op(string sql, Action<NpgsqlDataReader> action)
        {
            using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
            using (NpgsqlDataReader dr = command.ExecuteReader())
            {
                while (dr.Read())
                {
                    action(dr);
                }
            }
        }

        protected Dictionary<int, string> LoadIndicators()
        {
            var result = new Dictionary<int, string>();

            Op(@"SELECT ""Id"", ""Name"" FROM public.""Indicators"";", (dr =>
            result.Add(Convert.ToInt32(dr["Id"]), Convert.ToString(dr["Name"]))));

            return result;

        }

        protected string QueryOneRegionValues(int region, int start, int end, int[] indicators)
        {
            return
            @"SELECT ""Id"", ""Values"".""Region"", ""Indicator"", ""DataYear"", ""SurveyYear"", ""Values""
                FROM public.""Values""
                where ""Region"" = " + region + @" and ""DataYear"" between " + start + " and " + end + @"
                and ""Indicator"" in (" + indicators.Aggregate(string.Empty, (s,x) =>  
                    s + (!string.IsNullOrEmpty(s) ?  "," : string.Empty) + x.ToString()) + ");";
        }
        private string QueryOneRegionValues(int region, int year, int[] indicators)
        {
            return
            @"SELECT ""Id"", ""Values"".""Region"", ""Indicator"", ""DataYear"", ""SurveyYear"", ""Values""
                FROM public.""Values""
                where ""Region"" = " + region + @" and ""DataYear"" = " + year + @"
                and ""Indicator"" in (" + indicators.Aggregate(string.Empty, (s, x) =>
                    s + (!string.IsNullOrEmpty(s) ? "," : string.Empty) + x.ToString()) + ");";
        }
        public List<Region> GetRegions(bool forChart)
        {
            var result = new List<Region>();
            Op(string.Format("SELECT \"Id\", \"Code\", \"Name\" FROM public.\"Regions\" where \"Id\"!={0} order by \"Id\";",forChart?"5":"92"), dr =>
            {

                var r = new Region();
                r.RegionId = Convert.ToInt32(dr["Id"]);
                r.Code = Convert.ToString(dr["Code"]);
                r.RegionName = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }

        public Region GetRegion(int id)
        {
            var result = new Region() { RegionId = id } ;

            Op("SELECT \"Name\" FROM public.\"Regions\" where \"Id\" = " + id + ";", dr => result.RegionName = Convert.ToString(dr["Name"]));

            return result;
        }

        public List<MSPEfficiencyDataItem> GetMSPEfficiencyData(int region, int startYear, int endYear)
        {
            var result = new List<MSPEfficiencyDataItem>();
            Dictionary<int, MSPEfficiencyDataItem> dic = new Dictionary<int, MSPEfficiencyDataItem>();
            for (int i = startYear; i <= endYear; i++)
            {
                var item = new MSPEfficiencyDataItem() { Year = i };
                item.Total = new List<KeyValuePair<string, double>>();
                item.Verified = new List<KeyValuePair<string, double>>();
                result.Add(item);
                dic.Add(i, item);
            }

            var indicators = LoadIndicators();
            Op(QueryOneRegionValues(region, startYear, endYear, new int[] { 13, 21, 22, 23, 25, 26, 27 , 436 ,20, 24,} ), 
               dr =>
               {
                   int year = Convert.ToInt32(dr["DataYear"]);
                   int indicator = Convert.ToInt32(dr["Indicator"]);
                   var floatValue = dr["Values"] as float[];
                   double[] value = new double[floatValue.Length];
                   for (int i = 0; i < floatValue.Length; i++)
                      value[i] = Convert.ToDouble(floatValue[i].ToString("0.0"));
                   if (null != value)
                   {
                       switch (indicator)
                       {
                           case 13:
                               dic[year].Transfert = new KeyValuePair<string, double>(indicators[indicator], value[0]); break;
                           case 21:
                               dic[year].Total.Add(new KeyValuePair<string, double>("Сокращение численности бедных", value[0])); break;
                           case 22:
                               dic[year].Total.Add(new KeyValuePair<string, double>("Сокращение дефицита дохода у населения", value[0])); break;
                           case 23:
                               dic[year].Total.Add(new KeyValuePair<string, double>("Доля выплат, идущая на сокращение дефицита дохода у населения", value[0])); break;
                           case 25:
                               dic[year].Verified.Add(new KeyValuePair<string, double>("Сокращение численности бедных", value[0])); break;
                           case 26:
                               dic[year].Verified.Add(new KeyValuePair<string, double>("Сокращение дефицита дохода у населения", value[0])); break;
                           case 27:
                               dic[year].Verified.Add(new KeyValuePair<string, double>("Доля выплат, идущая на сокращение дефицита дохода у населения", value[0])); break;

                           case 436:
                               dic[year].Total.Add(new KeyValuePair<string, double>("Индекс Джини в результате предоставления МСП", value[0])); break;
                           case 20:
                               dic[year].Total.Add(new KeyValuePair<string, double>("Сокращение неравенства (индекса Джини) в результате предоставления МСП, %", value[0])); break;
                           case 24:
                               dic[year].Verified.Add(new KeyValuePair<string, double>("Сокращение неравенства (индекса Джини) в результате предоставления МСП, %", value[0])); break;

                       }
                   }
               });

            return result;
        }
        public List<MSPEfficiencyDataItem> GetMSPEfficiencyData(int startYear, int[] regions)
        {
            var result = new List<MSPEfficiencyDataItem>();
            Dictionary<int, MSPEfficiencyDataItem> dic = new Dictionary<int, MSPEfficiencyDataItem>();
            var indicators = LoadIndicators();
            for (int i = 0; i < regions.Length; i++)
            {
                var item = new MSPEfficiencyDataItem() { Year = startYear, Region = i };
                item.Total = new List<KeyValuePair<string, double>>();
                item.Verified = new List<KeyValuePair<string, double>>();
                result.Add(item);
                dic.Add(i, item);



                Op(QueryOneRegionValues(i, startYear, new int[] { 13, 21, 22, 23, 25, 26, 27 }),
                   dr =>
                   {
                       //int year = Convert.ToInt32(dr["DataYear"]);
                       int indicator = Convert.ToInt32(dr["Indicator"]);
                       float[] value = dr["Values"] as float[];
                       if (null != value)
                       {
                           switch (indicator)
                           {
                               case 13: dic[i].Transfert = new KeyValuePair<string, double>(indicators[indicator], value[0]); break;
                               case 21:
                               case 22:
                               case 23:
                                   dic[i].Total.Add(new KeyValuePair<string, double>(indicators[indicator], value[0])); break;
                               case 25:
                               case 26:
                               case 27:
                                   dic[i].Verified.Add(new KeyValuePair<string, double>(indicators[indicator], value[0])); break;
                           }
                       }
                   });
            }

            return result;
        }

        public List<CategotyChartDataItem > GetIndicatorsByCategory( string indicatorName, List<KeyValuePair<int, string>> displayIndicatorNames, List<int> indicatorsIds, int region, int startYear, int endYear)
        {
           
            var result = new List<CategotyChartDataItem>();
            Dictionary<int, CategotyChartDataItem> dic = new Dictionary<int, CategotyChartDataItem>();

            for (int i = startYear; i <= endYear; i++)
            {
                var item = new CategotyChartDataItem() { Year = i , DisplayIndicatorNames = displayIndicatorNames , };
                item.Values = new List<KeyValuePair<string, float>>();
                item.DictionaryValues = new Dictionary<string, decimal>();
                foreach (var indicatorsId in indicatorsIds)
                {
                    item.DictionaryValues.Add(indicatorsId.ToString(), 0);
                }
                result.Add(item);
                dic.Add(i, item);
            }
            var strindicatorsIds = "";
            strindicatorsIds = GetStringFromListInt(indicatorsIds, strindicatorsIds);

            Op(@"SELECT ""Values"".""Region"", ""Values"".""Indicator"", ""DataYear"", ""SurveyYear"", 
            round(cast(""Values""[1] as numeric),2) as Val, ""Indicators"".""Name"" as ""IndicatorName"", ""Indicators"".""Id"" as ""IndicatorId""
                FROM public.""Values""
                inner join public.""Indicators"" on ""Values"".""Indicator"" = ""Indicators"".""Id""
                inner join public.""Category"" on ""Category"".""Id"" = ""Indicators"".""Category""
                where  ""Indicators"".""Id"" In(" + strindicatorsIds + ")"  + @"
                and ""Values"".""Region"" = " + region +
                @" and ""DataYear"" between " + startYear + " and " + endYear + ";",
                (dr) => {
                    var indicatorId = dr["IndicatorId"].ToString();
                    int year = Convert.ToInt32(dr["DataYear"]);
                    string indicator = Convert.ToString(dr["IndicatorName"]);
                    if (null != dr["Val"] )
                    {
                        var val = Convert.ToDecimal(dr["Val"]);
                        var convertVal = val.ToString("0.0");                     
                        dic[year].DictionaryValues[indicatorId]= Convert.ToDecimal(convertVal);
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
        public List<KeyValuePair<int, string>> GetRegionDisplayNames(int regionIds)
        {
            var result = new List<KeyValuePair<int, string>>();
            Op(@"SELECT ""Regions"".""Name"", ""Regions"".""Id""
                            FROM public.""Regions"" where ""Regions"".""Id"" In(" + regionIds.ToString() + "); ",
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
        public List<DictionaryRow> LoadDictionary(int region)
        {
            var result = new List<DictionaryRow>();
            var map = new Dictionary<int, DictionaryRow>();
            Op(@"SELECT ""Id"", ""Name"", ""Level"", ""Parent"", ""SortOrder""
                FROM public.""Category"" where ""Level"" between 0 and 2", dr => {
                var item = new DictionaryRow()
                {
                    Id = Convert.ToInt32(dr["Id"]),
                    Name = Convert.ToString(dr["Name"]),
                    Level = Convert.ToInt32(dr["Level"]),
                    Sort = Convert.ToInt32(dr["SortOrder"])
                };
                switch (item.Level)
                {
                    case 1: item.AreaId = Convert.ToInt32(dr["Parent"]); break;
                    case 2: item.CategoryId = Convert.ToInt32(dr["Parent"]); break;
                }
                map.Add(item.Id, item);
            });
            result.AddRange(map.Values);
            result.Where(x => 2 == x.Level && map.ContainsKey(x.CategoryId)).ToList().ForEach(z => z.AreaId = map[z.CategoryId].AreaId);
//DISTINCT ON(""Name"") 
            Op(@"SELECT  ""Id"", ""Name"", ""Category"", ""Area"", ""SubCategory"", ""SortOrder""
                FROM public.""Indicators"" where ""Region"" = " + region.ToString() , dr =>
               {
               //if (result.Where(x => x.Name == (string)dr["Name"]).ToList().Count == 0)
               //{
                   result.Add(new DictionaryRow()
                       {
                           Id = Convert.ToInt32(dr["Id"]),
                           Name = Convert.ToString(dr["Name"]),
                           Level = 3,
                           AreaId = Convert.ToInt32(dr["Area"]),
                           CategoryId = Convert.ToInt32(dr["Category"]),
                           SubCategoryId = Convert.ToInt32(dr["SubCategory"]),
                           Sort = Convert.ToInt32(dr["SortOrder"])
                       });
                   //}
               });


            result.Sort((a, b) => a.Sort.CompareTo(b.Sort));
            //var debug = result.Where(x => x.Id == 1966).First();
            return result;
        }

        public void Dispose()
        {
            if (null != _connection)
            {
                _connection.Close();
                _connection = null;
            }
        }
    }
}