using Smartsp.aspire.dashborad.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class Utils
    {
        public static float TryParseFloat(object value)
        {
            float res;
            if (!float.TryParse(value.ToString(), out res))
            {
                return FormatExtensions.FakeFloatasDBNull;
               // default(float);
            }
            return res;
        }
    }
}