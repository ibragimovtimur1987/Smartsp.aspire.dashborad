using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Extensions
{
    public static class FormatExtensions
    {
        public const float FakeFloatasDBNull = -1;
        public static string ToFormattedString(this float input, int n = 2)
        {
            if (input == FakeFloatasDBNull) return "-"; //если значение = -1, то считаем что данных в базе нет и отображаем как -
            string nformat = string.Format("n{0}", n);
            var res = input.ToString(nformat, CultureInfo.CreateSpecificCulture("ru-RU"));
            return res;
        }
    }
}