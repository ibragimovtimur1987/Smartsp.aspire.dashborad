using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartsp.aspire.AtlasLoad
{
    class Program
    {
        static void Main(string[] args)
        {
            var loader = new ExcelLoader();
            var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\Для графиков_Россия.xlsx";
            // var path = @"C:\timur\dashborad\Smartsp.aspire.dashborad\Documents\Ключевые индикаторы Атласа_Aspire_06.03.2017.xlsx";
            // var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\Для графиков_1906.xlsx";
            // var path = @"C:\Users\ibragimovrt\Source\Repos\smartsp.aspire.dashborad\Smartsp.aspire.dashborad\Documents\Для графиков_Камчатка.xlsx";
            loader.Load(path);
            using (var context = new DataContext())
            {
               //context.DumpCategories();
               // context.EnsureCategories(loader.Categories);
               // context.EnsureIndicators(loader.Indicators);
             //   context.EnsureRegions(loader.Regions);
                context.UpdateValues(loader.Values);
            }
            Console.ReadKey();
        }
    }
}

