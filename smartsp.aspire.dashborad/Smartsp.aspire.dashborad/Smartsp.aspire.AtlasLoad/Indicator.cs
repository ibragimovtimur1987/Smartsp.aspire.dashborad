using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartsp.aspire.AtlasLoad
{
    public class Indicator
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }

        public string Category { get; set; }
        public string Name { get; set; }

        public int Column { get; set; }
    }
}
