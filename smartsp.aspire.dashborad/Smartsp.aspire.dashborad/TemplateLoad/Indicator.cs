using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartsp.aspire.TemplateLoad
{
    public class Indicator
    {
        public int Id { get; set; }
        public int AreaId { get; set; }
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public int RegionId { get; set; }

        public string Area { get; set; }
        public string Category { get; set; }
        public string SubCategory { get; set; }
        public string Name { get; set; }
        public string Region { get; set; }
        public string SourceFunding { get; set; } //Excel cell M
        public bool TargetingCategorical { get; set; } //Values[6], Excel cell S
        public bool TargetingMeansTested { get; set; } //Values[7], Excel cell T
        public float CountRecipients{ get; set; }
        public float CostFinancing { get; set; }
 
    }
}
