using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Smartsp.aspire.AtlasLoad
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
            //var a = args[0].Value;
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

            Op(@"SELECT ""Id"", ""Name"", ""Category"" FROM public.""Indicators"";", (dr =>
            result.Add(new Indicator()
                { Id = Convert.ToInt32(dr["Id"]), Name = Convert.ToString(dr["Name"]), CategoryId = Convert.ToInt32(dr["Category"]) })));
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
            indicatoros.ForEach(x => x.CategoryId = dbcategories.FirstOrDefault(y => y.Value == x.Category).Key);
            indicatoros.Where(x => !dbindicators.Any(y => x.CategoryId == y.CategoryId && x.Name == y.Name)).ToList().ForEach(
                z => a += WriteOp(@"INSERT INTO public.""Indicators""(""Name"", ""Category"") VALUES (:name, :category);",
                 new List<NpgsqlParameter>() {
                     new NpgsqlParameter("name", z.Name),
                     new NpgsqlParameter("category", z.CategoryId)}));

            Console.WriteLine("Added {0} indicators", a);
        }

        public void EnsureRegions(List<Region> regions)
        {
            int a = 0;
            var dbregions = LoadRegions();
            regions.Where(x => dbregions.All(y => y.Name != x.Name)).ToList().ForEach(z =>
                a += WriteOp(@"INSERT INTO public.""Regions""(""Name"",""Code"") VALUES(:name,:Code);", new List<NpgsqlParameter>() { new NpgsqlParameter("name", z.Name), new NpgsqlParameter("Code", z.Code) }));

            Console.WriteLine("Added {0} regions", a);
        }

        public void UpdateValues(List<KeyValuePair<ValueKey, float>> values)
        {
            var countDel = WriteOp(@"delete FROM public.""Values"" where  ""Values"".""Source"" = 'AtlasLoad' ", new List<NpgsqlParameter>());
            //var countDel = WriteOp(@"delete FROM public.""Values"" where ""Values"".""Region"" = 92 AND ""Values"".""Source"" = 'AtlasLoad' ", new List<NpgsqlParameter>());
            var indicators = values.Select(x => x.Key.MCP).ToList();
            var dbcategories = LoadCategories();
            var dbindicators = LoadIndicators();

            indicators.ForEach(x => x.CategoryId = dbcategories.FirstOrDefault(y => y.Value.Trim() == x.Category.Trim()).Key);
            // по имени показатель ..члена ДХ, получающего меру.. почему то не мапится, делаем жестко
            var l = values.Where(
                 x => x.Key.MCP.Name.Contains("(члена ДХ, получающего меру)")).ToList();
            l.ForEach(x => x.Key.MCP.CategoryId = 5);
            l.ForEach(x => x.Key.MCP.Id = 13);

            indicators.Where(
                x => dbindicators.Any(y => x.CategoryId == y.CategoryId && x.Name.Trim() == y.Name.Trim())).ToList().ForEach(
                x => x.Id = dbindicators.FirstOrDefault(y => x.CategoryId == y.CategoryId && x.Name.Trim() == y.Name.Trim()).Id);
            var regions = values.Select(x => x.Key.Region).ToList();
            var dbregions = LoadRegions();
            regions.Where(
                x => dbregions.Any(y => x.Code.Trim() == y.Code.Trim())).ToList().ForEach(
                x => x.Id = dbregions.FirstOrDefault(y => x.Code.Trim() == y.Code.Trim()).Id);

            int a = 0;
            var testErrorIndicators = values.Where(x => x.Key.MCP.Id==0).ToList();
            var testErrorRegions = values.Where(x => x.Key.Region.Id == 0).ToList();
            values.Where(x => 0 != x.Key.MCP.Id && 0 != x.Key.Region.Id).ToList().ForEach(z =>
                a += WriteOp(@"INSERT INTO public.""Values""
                    (""Region"", ""Indicator"", ""DataYear"", ""SurveyYear"", ""Values"",""Source"")
                    VALUES
                    (:region, :indicator, :dy, :sy, :val,:source)", 
                    new List<NpgsqlParameter>() {
                    new NpgsqlParameter("indicator", z.Key.MCP.Id),
                    new NpgsqlParameter("region", z.Key.Region.Id),
                    new NpgsqlParameter("dy", z.Key.DataYear),
                    new NpgsqlParameter("sy", z.Key.SurveyYear),
                    new NpgsqlParameter("val", new float[] { z.Value }),
                    new NpgsqlParameter("source", z.Key.Source),
                }));

            Console.WriteLine("Added {0} values", a);
            Console.WriteLine("Not found indicators in {0} values", testErrorIndicators.Count);
            Console.WriteLine("Not found regions in {0} values", testErrorRegions.Count);
            // Define the root element 
            //XmlDocument xmlDocument = new XmlDocument();
            //var serializer = new XmlSerializer(typeof(List<KeyValuePair < ValueKey, float >>),
            //                                   new XmlRootAttribute("testError"));
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    serializer.Serialize(stream, testError);
            //    stream.Position = 0;
            //    xmlDocument.Load(stream);
            //    xmlDocument.Save(@"C: \Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\testError.xml");
            //    stream.Close();
            //}
        }

        
        public void ClearAllValues()
        {
        }

        public void DumpCategories()
        {
            var dbcategories = LoadCategories();
            File.WriteAllLines("cat.txt", dbcategories.Select(x => x.Value));
        }
    }
}
