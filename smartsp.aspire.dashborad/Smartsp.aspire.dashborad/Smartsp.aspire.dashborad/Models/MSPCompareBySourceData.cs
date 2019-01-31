using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MSPCompareBySourceData : IMSPCompareData
    {
        public string ViewTitle { get; set; }
        public string ViewTableFirstColTitle { get; set; }
        public int Year { get; set; }
        public Region Region { get; set; }
        public MspCategory MspArea { get; set; }
        public List<KeyValuePair<int, string>> RowTitlePairList { get; private set; }
        public List<MSPCompareBySourceDataItem> ValuesByYear { get; set; }

        //1-я таблица
        public void Load(int regions, int[] areas, int year)
        {
            Year = year;
            using (var context = new MSPCompareBySourceDataContext())
            {
                Region = context.GetRegion(regions);
                RowTitlePairList = context.GetRowTitlePairListAreas(areas);
                ValuesByYear = context.GetMSPCompareBySourceData(regions, areas, year);
            }
            ViewTitle = "Сравнение по источникам финансирования МСП";
            ViewTableFirstColTitle = "Тип/категория/подкатегория МСП";
        }

        //2-я таблица
        public void Load(int regions, int[] areas, int startYear, int endYear)
        {
            using (var context = new MSPCompareBySourceDataContext())
            {
                Region = context.GetRegion(regions);
                RowTitlePairList = context.GetRowTitlePairListSources();
                ValuesByYear = context.GetMSPCompareBySourceData(regions, areas, startYear, endYear);
            }
            ViewTitle = "Сравнение по источникам финансирования МСП";
            ViewTableFirstColTitle = "Источник финансирования";
        }

        //3-я таблица
        internal void Load(int[] regions, int[] areas, int year)
        {
            using (var context = new MSPCompareBySourceDataContext())
            {
                RowTitlePairList = context.GetRowTitlePairListRegions(regions);
                ValuesByYear = context.GetMSPCompareBySourceData(regions, areas, year);
            }
            ViewTitle = "Сравнение по источникам финансирования МСП";
            Year = year;
            ViewTableFirstColTitle = "Субъекты";
        }


    }
    public class MSPCompareBySourceDataItem
    {
        //Год
        public int Year { get; set; }

        public string Title { get; set; }
        //Регион
        public Region Region { get; set; }

        public MspCategory MspArea { get; set; }

        //Федеральное
        public List<KeyValuePair<string, float>> Federal { get; set; }
        public string FederalColTitle { get; set; }

        //Региональное
        public List<KeyValuePair<string, float>> Regional { get; set; }
        public string RegionalColTitle { get; set; }

        //Софинансирование
        public List<KeyValuePair<string, float>> Common { get; set; }
        public string CommonColTitle { get; set; }

        //Софинансирование
        public List<KeyValuePair<string, float>> Summ { get; set; }
        public string SummColTitle { get; set; }

        //соцподдержка
        public List<KeyValuePair<string, float>> SocialSupport { get; set; }

        public string SocialSupportColTitle { get; set; }

        //рынок труда
        public List<KeyValuePair<string, float>> LaborMarket { get; set; }

        public string LaborMarketColTitle { get; set; }


        public List<Column> Columns { get; set; } 
        public class Column
        {
            public int AreaId { get; set; }
            public string Title { get; set; }
            public List<KeyValuePair<string, float>> Values { get; set; }
        }

    }
}