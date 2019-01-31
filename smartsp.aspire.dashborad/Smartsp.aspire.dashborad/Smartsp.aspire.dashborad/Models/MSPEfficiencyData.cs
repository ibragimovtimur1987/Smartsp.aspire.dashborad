using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MSPEfficiencyDataItem
    {
        //Год
        public int Year { get; set; }

        //Subject
        public int Region { get; set; }

        //Средний размер трансферта (руб./мес) на бенефициара (члена ДХ, получающего меру)
        public KeyValuePair<string, double> Transfert { get; set; }

        //Эффективность всех МСП (%):
        //Сокращение дефицита дохода у населения 
        //Доля выплат, идущая  на сокращение дефицита дохода у населения 
        //Сокращение численности бедных
        public List<KeyValuePair<string, double>> Total { get; set; }

        //Эффективность МСП с проверкой дохода(%)
        //Сокращение дефицита дохода у населения 
        //Доля выплат, идущая  на сокращение дефицита дохода у населения 
        //Сокращение численности бедных
        public List<KeyValuePair<string, double>> Verified { get; set; }
    }


    public class MSPEfficiencyData
    {
        public Year Year { get; set; }
        public Region Region { get; set; }
        public List<MSPEfficiencyDataItem> ValuesByYear { get; set; }

        public List<string> RowTitles
        {
            get
            {
                return GetRowNames();
            }
        }


        public void Load(int region, int startYear, int endYear)
        {
            using (var context = new DataContext())
            {
                Region = context.GetRegion(region);
                ValuesByYear = context.GetMSPEfficiencyData(region, startYear, endYear);
            }
        }
        public void Load(int[] regions, int startYear)
        {
            using (var context = new DataContext())
            {
                ValuesByYear = context.GetMSPEfficiencyData(startYear, regions);
            }
        }
        private List<string> GetRowNames()
        {
            var result = new List<string>();
            if (null == ValuesByYear && 0 == ValuesByYear.Count)
            {
                return new List<string> {
                        "Средний размер трансферта (руб./мес) на бенефициара (члена ДХ, получающего меру)",
                        "Сокращение численности бедных",
                        "Сокращение дефицита дохода у населения",
                        "Доля выплат, идущая на сокращение дефицита дохода у населения" ,
                        "Индекс Джини в результате предоставления МСП",
                        "Сокращение неравенства (индекса Джини) в результате предоставления МСП, %"
                };

            }

            result.Add(!string.IsNullOrEmpty(ValuesByYear.FirstOrDefault().Transfert.Key) 
                ? ValuesByYear.FirstOrDefault().Transfert.Key
                : "Средний размер трансферта (руб./мес) на бенефициара (члена ДХ, получающего меру)");


            //foreach (var item in ValuesByYear.FirstOrDefault().Total)
            //{
            //    if (!result.Contains(item.Key))
            //    {
            //        result.Add(item.Key);
            //    }
            //}

            //foreach (var item in ValuesByYear.FirstOrDefault().Verified)
            //{
            //    if (!result.Contains(item.Key))
            //    {
            //        result.Add(item.Key);
            //    }                
            //}

            foreach (var item in new List<string> {
                        "Сокращение численности бедных",
                        "Сокращение дефицита дохода у населения",
                        "Доля выплат, идущая на сокращение дефицита дохода у населения",
                        "Индекс Джини в результате предоставления МСП",
                        "Сокращение неравенства (индекса Джини) в результате предоставления МСП, %"})
            {
                if (!result.Contains(item))
                {
                    result.Add(item);
                }
            }


            return result;
        }
    }
}