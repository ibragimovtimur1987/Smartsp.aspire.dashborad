using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class RegionChartController : Controller
    {
        // GET: CategoryChart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Draw(string title, string maxValue, string valueName, string category, int[] indicators, int[] region, int startYear, int endYear, string enabledlegend, int width, int itemWidth)
        {
            var model = new RegionChartData() { Title = title ?? category, ClientId = Guid.NewGuid().ToString("N"), Year = startYear.ToString() };

            if (region.Length > 1)
            {
                model.Load(indicators ?? new int[0], region, startYear, endYear, enabledlegend, width, itemWidth);
                JavaScriptSerializer js = new JavaScriptSerializer();
                ViewBag.categories = js.Serialize(model.CategoriesRegions);
                ViewBag.series = js.Serialize(model.Series);
                ViewBag.CountRegion = region.Length;
                model.maxValue = maxValue;
                model.valueName = valueName;
                return PartialView("CategoryChartByRegion", model);
            }


            return Content(string.Empty);
        }


    }
}