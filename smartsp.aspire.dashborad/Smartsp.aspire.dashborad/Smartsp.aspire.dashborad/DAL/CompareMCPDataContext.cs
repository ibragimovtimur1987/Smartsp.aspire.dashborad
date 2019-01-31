using Smartsp.aspire.dashborad.Models;
using Smartsp.aspire.dashborad.Models.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.DAL
{
    public class CompareMCPDataContext : DataContext
    {
        //1
        public List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int region, int[] categories, int startYear, int endYear)
        {
            var result = new List<MSPCompareByTypeDataItem>();


            for (var year = startYear; year <= endYear; year++)
            {
                var item = new MSPCompareByTypeDataItem()
                {
                    Year = year,
                    Title = year.ToString(),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>()
                };

                Op(QueryMspCompareByType(region, categories, year),
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
        //1
        private string QueryMspCompareByType(int region, int[] categories, int year)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT c.""Id"", c.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Category"" c
                     join public.""Indicators"" i on i.""Area"" = c.""Id""
                     join public.""Values"" v on i.""Id"" = v.""Indicator""
                     join public.""Regions"" r on r.""Id"" = v.""Region""
                where r.""Id"" = {0}
                      and i.""Area"" in ({1})
                      and v.""DataYear"" = {2}
                group by c.""Id"", c.""Name""
                ), pres(Id, Name, Beneficiaries, Expenditures, SortOrder)
                as
                (  
                select c.""Id"", c.""Name"", cte.""Beneficiaries"", cte.""Expenditures"", c.""SortOrder"" from public.""Category"" c
                left outer join cte on cte.""Id"" = c.""Id""
                where c.""Id"" in ({1})
                ) select Id, Name, Beneficiaries, Expenditures, SortOrder from pres
                union all
                select 999999, 'Итого' as Name, sum(Beneficiaries) as Beneficiaries, sum(Expenditures) as Expenditures, 999999 from pres
                order by SortOrder, Id 
                ", region, string.Join(",", categories), year);
            return res;
        }

        internal List<KeyValuePair<int, string>> GetRowTitlePairListCategories(int[] categories)
        {
            var result = new List<KeyValuePair<int, string>>();
            Op(QueryRowTitlePairListCategories(categories),
                    dr =>
                    {
                        var regId = Convert.ToInt32(dr["Id"]);
                        var name = Convert.ToString(dr["Name"]);
                        result.Add(new KeyValuePair<int, string>(regId, name));
                    });
            result.Add(new KeyValuePair<int, string>(999999, "Итого"));
            return result;
        }

        private string QueryRowTitlePairListCategories(int[] categories)
        {
            var res = string.Format(@"
                SELECT *
                FROM public.""Category"" c
                where c.""Id"" in ({0}) 
                order by c.""SortOrder"" 
                ", string.Join(",", categories));
            return res;
        }

        //5
        internal List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int region, int type, int category, int[] subcategories, int startYear, int endYear)
        {
            var result = new List<MSPCompareByTypeDataItem>();

            for (int year = startYear; year <= endYear; year++)
            {
                var item = new MSPCompareByTypeDataItem()
                {
                    Year = year,
                    Title = year.ToString(),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>()

                };
                Op(QueryMspCompareByType(region, type, category, subcategories, year),
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

        //5 and 6
        private string QueryMspCompareByType(int region, int type, int category, int[] subcategories, int year)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT c.""Id"", c.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Category"" c
                     join public.""Indicators"" i on i.""SubCategory"" = c.""Id""
                     join public.""Values"" v on i.""Id"" = v.""Indicator""
                     join public.""Regions"" r on r.""Id"" = v.""Region""
                where r.""Id"" = {0}
                      and i.""Area"" = {1}
                      and i.""Category"" = {2}
                      and i.""SubCategory"" in ({3})
                      and v.""DataYear"" = {4}
                group by c.""Id"", c.""Name""
                ), pres(Id, Name, Beneficiaries, Expenditures, SortOrder)
                as
                (  
                select c.""Id"", c.""Name"", cte.""Beneficiaries"", cte.""Expenditures"", c.""SortOrder"" from public.""Category"" c
                left outer join cte on cte.""Id"" = c.""Id""
                where c.""Id"" in ({3})
                ) select Id, Name, Beneficiaries, Expenditures, SortOrder from pres
                union all
                select 999999, 'Итого' as Name, sum(Beneficiaries) as Beneficiaries, sum(Expenditures) as Expenditures, 999999 from pres
                order by SortOrder, Id
                ", region, type, category, string.Join(",", subcategories), year);
            return res;
        }

        //6
        internal List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int[] regions, int type, int category, int[] subcategories, int year)
        {
            var result = new List<MSPCompareByTypeDataItem>();

            for (int i = 0; i < regions.Length; i++)
            {
                var item = new MSPCompareByTypeDataItem()
                {
                    Region = GetRegion(regions[i]),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>()

                };
                item.Title = item.Region.RegionName;
                Op(QueryMspCompareByType(regions[i], type, category, subcategories, year),
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

        internal List<KeyValuePair<int, string>> GetRowTitlePairListRegions(int[] regions)
        {
            var result = new List<KeyValuePair<int, string>>();
            Op(QueryRowTitlePairListRegions(regions),
                    dr =>
                    {
                        var regId = Convert.ToInt32(dr["Id"]);
                        var name = Convert.ToString(dr["Name"]);
                        result.Add(new KeyValuePair<int, string>(regId, name));
                    });
            return result;
        }

        private string QueryRowTitlePairListRegions(int[] regions)
        {
            var res = string.Format(@"
                SELECT *
                FROM public.""Regions"" r
                where r.""Id"" in ({0})
                order by r.""Id""
                ", string.Join(",", regions));
            return res;
        }

        internal List<MspCategory> GetMspAreas()
        {
            var result = new List<MspCategory>();
            Op("SELECT \"Id\", \"Name\" FROM public.\"Category\" where \"Level\"=0 order by \"SortOrder\";", dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        internal List<MspCategory> GetMspCategories()
        {
            var result = new List<MspCategory>();
            Op("SELECT \"Id\", \"Name\" FROM public.\"Category\" where \"Level\"=1 order by \"SortOrder\";", dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        internal List<MspCategory> GetMspSubCategories()
        {
            var result = new List<MspCategory>();
            Op("SELECT \"Id\", \"Name\" FROM public.\"Category\" where \"Level\"=2 order by \"SortOrder\";", dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        internal MspCategory GetMspArea(int type)
        {
       
            var result = new MspCategory() { Id = type };

            Op("SELECT \"Name\" FROM public.\"Category\" where \"Id\" = " + type + " order by \"SortOrder\";", dr => result.Title = Convert.ToString(dr["Name"]));

            return result;
        }
        internal MspCategory GetCategoryById(int categoryId)
        {
            var result = new MspCategory() { Id = categoryId };
            Op(MSPCompareQuery.GetCategoryById(categoryId),
                    dr =>
                    {
                        result.Title = Convert.ToString(dr["Name"]);
                    });
            return result;
        }
        internal List<MspCategory> GetMspCategoriesByArea(int type)
        {
            var result = new List<MspCategory>();
            var query = string.Format("SELECT \"Id\", \"Name\" FROM public.\"Category\" where \"Level\"=1 and \"Parent\"={0} order by \"SortOrder\";", type);
            Op(query, dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        internal List<MspCategory> GetMspSubCategoriesByArea(int type)
        {
            var result = new List<MspCategory>();
            var query = string.Format(@"
                SELECT c.""Id"", c.""Name"" 
                FROM public.""Category"" c
                     inner join public.""Category"" pc on c.""Parent"" = pc.""Id""
                where c.""Level""=2 
                      and pc.""Parent"" = {0}
                order by c.""SortOrder""
                ", type);
            Op(query, dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        internal List<MspCategory> GetMspSubCategoriesByCategory(int category)
        {
            var result = new List<MspCategory>();
            var query = string.Format("SELECT \"Id\", \"Name\" FROM public.\"Category\" where \"Level\"=2 and \"Parent\"={0} order by \"SortOrder\";", category);
            Op(query, dr =>
            {
                var r = new MspCategory();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Title = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }
        //3
        internal List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int region, int type, int[] category, int startYear, int endYear)
        {
            var result = new List<MSPCompareByTypeDataItem>();
            for (var year = startYear; year <= endYear; year++)
            {
                var item = new MSPCompareByTypeDataItem()
                {
                    Year = year,
                    Title = year.ToString(),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>()
                };

                Op(QueryMspCompareByType(region, type, category, year),
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
        //4
        internal List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int year, int type, int[] category, int[] regions)
        {
            var result = new List<MSPCompareByTypeDataItem>();
            var dic = new Dictionary<int, MSPCompareByTypeDataItem>();
            for (int i = 0; i < category.Length; i++)
            {
                var item = new MSPCompareByTypeDataItem() { MspCategory = GetMspCategory(category[i])};
                item.Title = item.MspCategory.Title;
                item.Beneficiaries = new List<KeyValuePair<string, float>>();
                item.Expenditures = new List<KeyValuePair<string, float>>();
                result.Add(item);
                dic.Add(category[i], item);
            }

            for (int i = 0; i < category.Length; i++)
            {
                Op(QueryMspCompareByType(year, type, category[i], regions),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        dic[category[i]].Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                        dic[category[i]].Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    });
            }

            var finItem = new MSPCompareByTypeDataItem()
            {
                Title = "Итого",
                Beneficiaries = new List<KeyValuePair<string, float>>(),
                Expenditures = new List<KeyValuePair<string, float>>()
            };
            Op(QueryMspCompareByType(year, type, category, regions),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        finItem.Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                        finItem.Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    });
            result.Add(finItem);
            return result;
        }

        private string QueryMspCompareByType(int year, int type, int category, int[] regions)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT r.""Id"", r.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Regions"" r
                     join public.""Values"" v on r.""Id"" = v.""Region""
                     join public.""Indicators"" i on v.""Indicator"" = i.""Id""
                where r.""Id"" in ({3})
                      and i.""Area"" = {1}
                      and i.""Category"" = {2}
                      and v.""DataYear"" = {0}
                group by r.""Id"", r.""Name""
                )
                select r.""Id"", r.""Name"", cte.""Beneficiaries"", cte.""Expenditures"" from public.""Regions"" r
                left outer join cte on cte.""Id"" = r.""Id""
                where r.""Id"" in ({3})
                order by r.""Id""
                ", year, type, category, string.Join(",", regions));
            return res;
        }
        private string QueryMspCompareByType(int year, int type, int[] categories, int[] regions)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT r.""Id"", r.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Regions"" r
                     join public.""Values"" v on r.""Id"" = v.""Region""
                     join public.""Indicators"" i on v.""Indicator"" = i.""Id""
                where r.""Id"" in ({3})
                      and i.""Area"" = {1}
                      and i.""Category"" in ({2})
                      and v.""DataYear"" = {0}
                group by r.""Id"", r.""Name""
                )
                select r.""Id"", r.""Name"", cte.""Beneficiaries"", cte.""Expenditures"" from public.""Regions"" r
                left outer join cte on cte.""Id"" = r.""Id""
                where r.""Id"" in ({3})
                order by r.""Id""
                ", year, type, string.Join(",", categories), string.Join(",", regions));
            return res;
        }
        private MspCategory GetMspCategory(int v)
        {
            var result = new MspCategory() { Id = v };

            Op("SELECT \"Name\" FROM public.\"Category\" where \"Id\" = " + v + ";", dr => result.Title = Convert.ToString(dr["Name"]));

            return result;
        }

        //2
        public List<MSPCompareByTypeDataItem> GetMSPCompareByTypeData(int[] regions, int[] categories, int year)
        {
            var result = new List<MSPCompareByTypeDataItem>();


            for (int i = 0; i < categories.Length; i++)
            {
                var item = new MSPCompareByTypeDataItem()
                {
                    MspCategory = GetMspArea(categories[i]),
                    Beneficiaries = new List<KeyValuePair<string, float>>(),
                    Expenditures = new List<KeyValuePair<string, float>>()
                };
                item.Title = item.MspCategory.Title;
                Op(QueryMspCompareByType(regions, categories[i], year),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        item.Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                        item.Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    });
                result.Add(item);
            }
            var finItem = new MSPCompareByTypeDataItem()
            {
                Title = "Итого",
                Beneficiaries = new List<KeyValuePair<string, float>>(),
                Expenditures = new List<KeyValuePair<string, float>>()
            };
            Op(QueryMspCompareByType(regions, categories, year),
                    dr =>
                    {
                        var name = Convert.ToString(dr["Name"]);
                        finItem.Beneficiaries.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Beneficiaries"])));
                        finItem.Expenditures.Add(new KeyValuePair<string, float>(name, Utils.TryParseFloat(dr["Expenditures"])));
                    });
            result.Add(finItem);
            return result;
        }

        //2
        private string QueryMspCompareByType(int[] regions, int category, int year)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT r.""Id"", r.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Regions"" r
                     join public.""Values"" v on r.""Id"" = v.""Region""
                     join public.""Indicators"" i on v.""Indicator"" = i.""Id""
                where r.""Id"" in ({0})
                      and i.""Area"" = {1}
                      and v.""DataYear"" = {2}
                group by r.""Id"", r.""Name""
                )
                select r.""Id"", r.""Name"", cte.""Beneficiaries"", cte.""Expenditures"" from public.""Regions"" r
                left outer join cte on cte.""Id"" = r.""Id""
                where r.""Id"" in ({0})
                ", string.Join(",", regions), category, year);
            return res;
        }

        //2 fin
        private string QueryMspCompareByType(int[] regions, int[] categories, int year)
        {
            var res = string.Format(@"
                ;with cte
                as
                (
                SELECT r.""Id"", r.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures""
                FROM public.""Regions"" r
                     join public.""Values"" v on r.""Id"" = v.""Region""
                     join public.""Indicators"" i on v.""Indicator"" = i.""Id""
                where r.""Id"" in ({0})
                      and i.""Area"" in ({1})
                      and v.""DataYear"" = {2}
                group by r.""Id"", r.""Name""
                )
                select r.""Id"", r.""Name"", cte.""Beneficiaries"", cte.""Expenditures"" from public.""Regions"" r
                left outer join cte on cte.""Id"" = r.""Id""
                where r.""Id"" in ({0})
                order by r.""Id""
                ", string.Join(",", regions), string.Join(",", categories), year);
            return res;
        }
        
        
        //3
        private string QueryMspCompareByType(int region, int type, int[] category, int year)
        {
            var res = string.Format(@"
                ;with cte(Id, Name, Beneficiaries, Expenditures, SortOrder) as
                (
                    SELECT c.""Id"", c.""Name"", sum(v.""Values""[1]) as ""Beneficiaries"" ,  sum(v.""Values""[5]) as ""Expenditures"", c.""SortOrder""
                    FROM public.""Indicators"" i
                        right join public.""Category"" as c on c.""Id"" = i.""Category""
                        left join public.""Values"" as v on i.""Id"" = v.""Indicator""
                    where i.""Area"" = {1} and i.""Region"" = {0} and v.""DataYear"" = {3} and c.""Id"" in ({2})
                    group by c.""Id"", c.""Name"", c.""SortOrder""
                ) select {3} as DataYear, cte.Id, cte.Name, cte.Beneficiaries, cte.Expenditures, SortOrder from cte
                union all
                select {3} as DataYear, 999999, N'Итого', sum(cte.Beneficiaries), sum(cte.Expenditures), 999999 from cte
                order by SortOrder, Id
                ", region, type, string.Join(",", category), year);
            return res;
        }
        
    }
}