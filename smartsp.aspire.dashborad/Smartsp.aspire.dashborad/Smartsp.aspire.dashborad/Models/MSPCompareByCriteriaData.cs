using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MSPCompareByCriteriaData : IMSPCompareData
    {
        public string ViewTitle { get; set; }
        public string ViewTableFirstColTitle { get; set; }
        public int Year { get; set; }
        public Region Region { get; set; }
        public MspCategory Area { get; set; }
        public MspCategory Category { get; set; }
        public List<KeyValuePair<int, string>> RowTitlePairList { get; private set; }
        public List<MSPCompareByCriteriaDataItem> ValuesByYear { get; set; }

        //1-я таблица

        public void Load(int region, int area, int category, int startYear, int endYear)
        {
            using (var context = new MSPCompareByCriteriaDataContext())
            {

                Region = context.GetRegion(region);
                Area = context.GetCategoryById(area);
                Category = context.GetCategoryById(category);
                RowTitlePairList = context.GetRowTitlePairListCriteria();
                ValuesByYear = context.GetMSPCompareByCriteriaData(region, area, category, startYear, endYear);
            }
            ViewTitle = string.Format("Сравнение по критерию определения нуждаемости внутри категории «{0}» по типу МСП «{1}»", Category.Title, Area.Title);
            ViewTableFirstColTitle = "Критерий определения нуждаемости";
        }
        //2-я таблица
        internal void Load(int[] regions, int area, int category, int year)
        {
            using (var context = new MSPCompareByCriteriaDataContext())
            {
                Area = context.GetCategoryById(area);
                Category = context.GetCategoryById(category);
                RowTitlePairList = context.GetRowTitlePairListRegions(regions);
                ValuesByYear = context.GetMSPCompareByCriteriaData(regions, area, category, year);
            }
            ViewTitle = string.Format("Сравнение по критерию определения нуждаемости внутри категории «{0}» по типу МСП «{1}»", Category.Title, Area.Title);
            ViewTableFirstColTitle = "Субъект";
        }

        public void Load(int region, int[] categories, int startYear, int endYear)
        {
            throw new NotImplementedException();
        }
    }
    public class MSPCompareByCriteriaDataItem
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
        public List<KeyValuePair<string, float>> Beneficiaries { get; set; }
        public string BeneficiariesColTitle { get; set; }

        //рынок труда
        public List<KeyValuePair<string, float>> Expenditures { get; set; }
        public string ExpendituresColTitle { get; set; }

        public List<KeyValuePair<string, float>> Beneficiaries1 { get; set; }
        public List<KeyValuePair<string, float>> Expenditures1 { get; set; }

        //public List<KeyValuePair<string, float>> Beneficiaries2 { get; set; }
        //public List<KeyValuePair<string, float>> Expenditures2 { get; set; }

        public List<KeyValuePair<string, float>> Beneficiaries3 { get; set; }
        public List<KeyValuePair<string, float>> Expenditures3 { get; set; }
    }
}