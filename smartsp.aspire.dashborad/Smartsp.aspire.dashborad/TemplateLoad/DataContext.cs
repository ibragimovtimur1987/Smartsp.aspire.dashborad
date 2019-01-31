using Npgsql;
using Smartsp.aspire.TemplateLoad;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateLoad
{
    public class DataContext : IDisposable
    {
        private Dictionary<string, int> categoryMap = new Dictionary<string, int>();
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

        public void Dispose()
        {
            if (null != _connection)
            {
                _connection.Close();
                _connection = null;
            }
        }

        private void Op(string sql, Action<NpgsqlDataReader> action)
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

        private int WriteOp(string sql, List<NpgsqlParameter> args)
        {
            //return 1; //Test only
            using (NpgsqlCommand command = new NpgsqlCommand(sql, _connection))
            {
                args.ForEach(x => command.Parameters.Add(x));
                return command.ExecuteNonQuery();
            }
        }
        private Dictionary<int, string> LoadCategories()
        {
            var result = new Dictionary<int, string>();

            Op(@"SELECT ""Id"", ""Name"" FROM public.""Category"";", (dr =>
            result.Add(Convert.ToInt32(dr["Id"]), Convert.ToString(dr["Name"]))));

            return result;

        }
        private List<Indicator> LoadIndicators()
        {
            var result = new List<Indicator>();

            Op(@"SELECT ""Id"", ""Name"", ""Category"", ""Area"", ""SubCategory"", ""Region"" FROM public.""Indicators"";", (dr =>
            result.Add(new Indicator()
            { Id = Convert.ToInt32(dr["Id"]), Name = Convert.ToString(dr["Name"]),
                CategoryId = Convert.ToInt32(dr["Category"]),
                AreaId = dr.IsDBNull(dr.GetOrdinal("Area")) ? 0 : Convert.ToInt32(dr["Area"]),
                SubCategoryId = dr.IsDBNull(dr.GetOrdinal("SubCategory")) ? 0 : Convert.ToInt32(dr["SubCategory"]),
                RegionId = dr.IsDBNull(dr.GetOrdinal("Region")) ? 0 : Convert.ToInt32(dr["Region"]),
            })));

            return result;

        }

        public List<Region> LoadRegions()
        {
            var result = new List<Region>();
            Op("SELECT \"Id\", \"Code\", \"Name\" FROM public.\"Regions\";", dr =>
            {
                var r = new Region();
                r.Id = Convert.ToInt32(dr["Id"]);
                r.Code = Convert.ToString(dr["Code"]);
                r.Name = Convert.ToString(dr["Name"]);
                result.Add(r);
            });

            return result;
        }

        public void EnsureCategories(List<string> categories)
        {
            int a = 0;
            var dbcategories = LoadCategories();
            categories.Where(x => !dbcategories.ContainsValue(x)).ToList().ForEach(z =>
                a += WriteOp(@"INSERT INTO public.""Category""(""Name"") VALUES(:name);",
                    new List<NpgsqlParameter>() { new NpgsqlParameter("name", z) }));

            Console.WriteLine("Added {0} categories", a);
        }

        public void EnsureIndicators(List<Indicator> indicatoros)
        {
            int a = 0;
            var dbcategories = LoadCategories();
            var dbindicators = LoadIndicators();
            var dbregions = LoadRegions();
            indicatoros.ForEach(x => x.AreaId = dbcategories.FirstOrDefault(y => y.Value.Trim().ToUpper() == x.Area.Trim().ToUpper()).Key);
            indicatoros.ForEach(x => x.CategoryId = dbcategories.FirstOrDefault(y => y.Value.Trim().ToUpper() == x.Category.Trim().ToUpper()).Key);
            indicatoros.ForEach(x => x.SubCategoryId = dbcategories.FirstOrDefault(y => y.Value.Trim().ToUpper() == x.SubCategory.Trim().ToUpper()).Key);
            indicatoros.ForEach(x => x.RegionId = dbregions.FirstOrDefault(y => y.Name.Trim().ToUpper() == x.Region.Trim().ToUpper()).Id);
            var list = indicatoros.Where(x => !dbindicators.Any(y => x.CategoryId == y.CategoryId && x.AreaId == y.AreaId && x.SubCategoryId == y.SubCategoryId
                    && x.RegionId == y.RegionId && x.Name.Trim().ToUpper() == y.Name.Trim().ToUpper())).ToList();
            list.ForEach(
                z => a += WriteOp(@"INSERT INTO public.""Indicators""(""Name"", ""Area"", ""Category"", ""SubCategory"", ""Region"",""Source"") VALUES (:name, :area, :category, :subcategory, :region, :source);",
                 new List<NpgsqlParameter>() {
                     new NpgsqlParameter("name", z.Name),
                     new NpgsqlParameter("area", z.AreaId),
                     new NpgsqlParameter("category", z.CategoryId),
                     new NpgsqlParameter("subcategory", z.SubCategoryId),
                     new NpgsqlParameter("region", z.RegionId),
                     new NpgsqlParameter("source", ExcelLoader.SourceValue)}));

            Console.WriteLine("Added {0} indicators", a);

           
        }
        public void UpdateValues(List<KeyValuePair<Key, ValueKey>> values)
        {
            int a = WriteOp(@"delete FROM public.""Values"" where ""Values"".""Source"" = 'TemplateLoad' ", new List<NpgsqlParameter>());
            Console.WriteLine("Deleted {0} values", a);
            var indicators = values.Select(x => x.Value.MCP).ToList();
            var dbcategories = LoadCategories();
            var dbindicators = LoadIndicators();
            indicators.ForEach(x => x.CategoryId = dbcategories.FirstOrDefault(y => y.Value.Trim().ToUpper() == x.Category.Trim().ToUpper()).Key);
            indicators.ForEach(x => x.SubCategoryId = dbcategories.FirstOrDefault(y => y.Value.Trim().ToUpper() == x.SubCategory.Trim().ToUpper()).Key);
            indicators.Where(
                x => dbindicators.Any(y => x.CategoryId == y.CategoryId && x.SubCategoryId == y.SubCategoryId 
                && x.Name.Trim().ToUpper() == y.Name.Trim().ToUpper())).ToList().ForEach(
                x => x.Id = dbindicators.FirstOrDefault(y => x.CategoryId == y.CategoryId 
                && x.Name.Trim().ToUpper() == y.Name.Trim().ToUpper() && x.SubCategoryId == y.SubCategoryId).Id);
            var regions = values.Select(x => x.Value.Region).ToList();
            var dbregions = LoadRegions();
            regions.Where(
                x => dbregions.Any(y => x.Name.Trim() == y.Name.Trim())).ToList().ForEach(
                x => x.Id = dbregions.FirstOrDefault(y => x.Name.Trim() == y.Name.Trim()).Id);

            //foreach (var region in dbregions)
            //{
            //    foreach(var val in values)
            //    {
            //       if(region.Name == val.Value.Region.Name)
            //        {
            //            val.Value.Region.Id = region.Id;
            //        }
            //    }
            //}
            //foreach (var indicator in dbindicators)
            //{
            //    foreach (var val in values)
            //    {
            //        if (indicator.Name == val.Value.MCP.Name)
            //        {
            //            val.Value.MCP.Id = indicator.Id;
            //        }
            //    }
            //}
            a = 0;
            values.Where(x => 0 != x.Value.MCP.Id && 0 != x.Value.Region.Id).ToList().ForEach(z =>
                a += WriteOp(
                    @"INSERT INTO public.""Values""
                    (""Region"", ""Indicator"", ""DataYear"",""FinSource""
                        ,""TargetingCategorical"",""TargetingMeansTested"", ""Values"",""Source"")
                    VALUES
                    (:region, :indicator, :dy, :FinSource, :TargetingCategorical, :TargetingMeansTested, :val,:source)",
                    new List<NpgsqlParameter>() {
                    new NpgsqlParameter("indicator", z.Value.MCP.Id),
                    new NpgsqlParameter("region", z.Value.Region.Id),
                    new NpgsqlParameter("dy", z.Value.DataYear),
                    new NpgsqlParameter("FinSource",z.Value.MCP.SourceFunding),
                    new NpgsqlParameter("TargetingCategorical",z.Value.MCP.TargetingCategorical),
                    new NpgsqlParameter("TargetingMeansTested",z.Value.MCP.TargetingMeansTested),
                    new NpgsqlParameter("val", new float[] { z.Value.MCP.CountRecipients,0,0,0,z.Value.MCP.CostFinancing}),
                    new NpgsqlParameter("source", z.Value.Source),

                }));

            Console.WriteLine("Added {0} values", a);

            var notLoadedValues = values.Where(x => 0 == x.Value.MCP.Id && 0 == x.Value.Region.Id).Select(y=>y.Value.MCP.Name + ";" + y.Value.MCP.Category + ";" + y.Value.Region.Name).ToArray();

            System.IO.File.WriteAllLines("notLoadedValues.txt", notLoadedValues);

        }
        //public float[] GetValue()
        //{
        //    var float[]
        //}
    }
}
