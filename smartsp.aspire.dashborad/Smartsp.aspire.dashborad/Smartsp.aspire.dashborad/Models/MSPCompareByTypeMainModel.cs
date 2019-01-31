using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MSPCompareByTypeMainModel : MainModel
    {
        public List<MspCategory> MspAreas { get; set; }
        public List<MspCategory> MspCategories { get; set; }
        public List<MspCategory> MspSubCategories { get; set; }
        public List<MspCategory> SelectedMspAreas { get; set; }
        public List<MspCategory> SelectedMspCategories { get; set; }
        public List<MspCategory> SelectedMspSubCategories { get; set; }

        public override void Init()
        {
            using (var context = new CompareMCPDataContext())
            {
                MspSubCategories = context.GetMspSubCategories();
                MspCategories = context.GetMspCategories();
                RegionList = context.GetRegions(false);
                MspAreas = context.GetMspAreas();
            }
        }
    }
}