using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models.Queries
{
    public class MSPCompareQuery
    {
        public static string GetCategoryById(int categoryId)
        {
            var res = string.Format(@"
                SELECT c.""Id"", c.""Name""
                FROM public.""Category"" c
                where c.""Id"" = {0} 
                ", categoryId);
            return res;
        }
    }
}