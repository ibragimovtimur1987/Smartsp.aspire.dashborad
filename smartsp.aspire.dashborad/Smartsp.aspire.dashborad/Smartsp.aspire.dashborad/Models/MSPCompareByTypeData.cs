using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MSPCompareByTypeData : IMSPCompareData
    {
        public int Year { get; set; }
        public Region Region { get; set; }
        public MspCategory MspArea { get; set; }
        public MspCategory MspCategory { get; set; }
        public MspCategory MspSubCategory { get; set; }

        public List<MSPCompareByTypeDataItem> ValuesByYear { get; set; }


        public List<KeyValuePair<int, string>> RowTitlePairList { get; set; }

        //1
        //Если выбран 1 субъект и 2 типа МСП одновременно, то выбор категорий и подкатегорий не доступен, период может быть выбран любой
        public void Load(int region, int[] categories, int startYear, int endYear)
        {
            using (var context = new CompareMCPDataContext())
            {
                Region = context.GetRegion(region);
                RowTitlePairList = context.GetRowTitlePairListCategories(categories);
                ValuesByYear = context.GetMSPCompareByTypeData(region, categories, startYear, endYear);
            }
        }

        //2
        //если выбрано 2 и более субъекта и 2 типа МСП одновременно, то выбор категорий и подкатегорий не доступен, а период может быть равен только одному году
        public void Load(int[] regions, int[] categories, int year)
        {
            using (var context = new CompareMCPDataContext())
            {
                Year = year;
                RowTitlePairList = context.GetRowTitlePairListRegions(regions);
                ValuesByYear = context.GetMSPCompareByTypeData(regions, categories, year);
            }
        }

        //3
        //если выбран 1 субъект, 1 Направления социальной поддержки , 2 и более категории внутри типа МСП, то выбор подкатегорий не доступен, период может быть любой
        internal void Load(int region, int type, int[] category, int startYear, int endYear)
        {
            using (var context = new CompareMCPDataContext())
            {
                MspArea = context.GetMspArea(type);
                Region = context.GetRegion(region);
                RowTitlePairList = context.GetRowTitlePairListCategories(category);
                ValuesByYear = context.GetMSPCompareByTypeData(region, type, category, startYear, endYear);
            }
        }

        //4
        //если выбрано 2 и более субъекта, 1 Направления социальной поддержки , 2 и более категории внутри типа МСП, то выбор подкатегорий не доступен, а период может быть равен только одному году
        internal void Load(int year, int type, int[] category, int[] regions)
        {
            using (var context = new CompareMCPDataContext())
            {
                Year = year;
                MspArea = context.GetMspArea(type);
                RowTitlePairList = context.GetRowTitlePairListRegions(regions);
                ValuesByYear = context.GetMSPCompareByTypeData(year, type, category, regions);
            }
        }

        //5
        //если выбран 1 субъект, 1 Направления социальной поддержки , 1 категория внутри типа МСП, то выбор подкатегорий доступен, период может быть любой
        internal void Load(int region, int type, int category, int[] subcategories, int startYear, int endYear)
        {
            using (var context = new CompareMCPDataContext())
            {
                Region = context.GetRegion(region);
                MspArea = context.GetMspArea(type);
                MspCategory = context.GetMspArea(category);
                RowTitlePairList = context.GetRowTitlePairListCategories(subcategories);
                ValuesByYear = context.GetMSPCompareByTypeData(region, type, category, subcategories, startYear, endYear);
            }
        }

        //6
        //если выбрано 2 и более субъекта, 1 Направления социальной поддержки , 1 категория внутри типа МСП, то выбор категорий доступен, а период может быть равен только одному году
        internal void Load(int[] regions, int type, int category, int[] subcategories, int year)
        {
            using (var context = new CompareMCPDataContext())
            {
                Year = year;
                MspArea = context.GetMspArea(type);
                MspCategory = context.GetMspArea(category);
                RowTitlePairList = context.GetRowTitlePairListCategories(subcategories);
                ValuesByYear = context.GetMSPCompareByTypeData(regions, type, category, subcategories, year);
            }
        }

        internal List<MspCategory> GetMspCategoriesByArea(int type)
        {
            using (var context = new CompareMCPDataContext())
            {
                var res = context.GetMspCategoriesByArea(type);
                return res;
            }
        }
        internal List<MspCategory> GetMspSubCategoriesByArea(int type)
        {
            using (var context = new CompareMCPDataContext())
            {
                var res = context.GetMspSubCategoriesByArea(type);
                return res;
            }
        }
        internal List<MspCategory> GetMspSubCategoriesByCategory(int category)
        {
            using (var context = new CompareMCPDataContext())
            {
                var res = context.GetMspSubCategoriesByCategory(category);
                return res;
            }
        }
    }
    public class MSPCompareByTypeDataItem
    {
        //Год
        public int Year { get; set; }

        public string Title { get; set; }
        //Регион
        public Region Region { get; set; }

        public MspCategory MspCategory { get; set; }

        //Количество получателей
        //1. Социальная поддержка
        //2. Рынок труда
        public List<KeyValuePair<string, float>> Beneficiaries { get; set; }

        //Расходы на финансирование выплаты (тыс. руб.)
        //1. Социальная поддержка
        //2. Рынок труда
        public List<KeyValuePair<string, float>> Expenditures { get; set; }

    }
}