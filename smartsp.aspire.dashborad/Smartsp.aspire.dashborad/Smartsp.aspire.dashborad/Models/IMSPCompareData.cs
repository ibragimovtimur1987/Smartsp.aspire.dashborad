using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartsp.aspire.dashborad.Models
{
    interface IMSPCompareData
    {
        void Load(int region, int[] categories, int startYear, int endYear);
    }
}
