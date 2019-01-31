using Smartsp.aspire.dashborad.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Smartsp.aspire.dashborad.Models
{
    public class MainModel
    {
        public int StartYear { get; set; }
        public int EndYear { get; set; }

        public List<KeyValuePair<int, string>> Years
        {
            get
            {
                var result = new List<KeyValuePair<int, string>>();
                result.Add(new KeyValuePair<int, string>(0, "Не выбрано"));
                for (int i = 2013; i <= DateTime.Today.Year; i++)
                {
                    result.Add(new KeyValuePair<int, string>(i, i.ToString()));
                }
             
                return result;
            }
        }


        public List<KeyValuePair<int, string>> YearsKpi
        {
            get
            {
                var result = new List<KeyValuePair<int, string>>();
                result.Add(new KeyValuePair<int, string>(0, "Не выбрано"));
                for (int i = 2014; i <= 2018; i++)
                {
                    result.Add(new KeyValuePair<int, string>(i, i.ToString()));
                }

                return result;
            }
        }

        public List<int> RegionIds { get; set; }
        public List<Region> RegionList { get; set; }

        public MSPEfficiencyData Data { get; set; }

        public virtual void Init()
        {
            using (var context = new DataContext())
            {
                RegionList = context.GetRegions(true);
            }
        }
        public virtual void InitForDictonary()
        {
            using (var context = new DataContext())
            {
                RegionList = new List<Region>();
                var Region = new Region();
                Region.RegionId = 0;
                Region.RegionName = "Не выбрано";
                Region.Code = "0";
                RegionList.Add(Region);
                RegionList.AddRange(context.GetRegions(false));
            }
        }
    }
}