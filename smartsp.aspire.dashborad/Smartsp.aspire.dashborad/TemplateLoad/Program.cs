using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            bool a = Convert.ToBoolean(Convert.ToInt16("1"));
            bool b = Convert.ToBoolean(Convert.ToInt16("0"));
           // var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\transponate_data_29.05.2017_Error7.xlsx";
          //  var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\transponate_data_09.10.2018.xlsx";
            var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\transponate_data_20.10.2018.xlsx";
            var loader = new ExcelLoader();
            loader.Load(path);
           // var categories = loader.Categories;
           // var inicators = loader.Indicators;

            using (var context = new DataContext())
            {
               // context.EnsureCategories(loader.Categories);
                context.EnsureIndicators(loader.Indicators);
                context.UpdateValues(loader.Values);
            }
            Console.ReadKey();
        }
    }
}
