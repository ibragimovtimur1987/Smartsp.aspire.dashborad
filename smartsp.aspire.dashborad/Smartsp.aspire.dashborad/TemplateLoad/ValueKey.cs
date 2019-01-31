using Smartsp.aspire.TemplateLoad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemplateLoad
{
    public class ValueKey
    {
        public Indicator MCP { get; set; }
        public Region Region { get; set; }
        public string Source { get; set; }
        public int DataYear { get; set; }
        public int SurveyYear { get; set; }
    }
}
