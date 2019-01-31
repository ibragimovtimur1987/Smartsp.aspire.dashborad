using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class CategoryChartController : Controller
    {
        // GET: CategoryChart
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Draw(string title, string maxValue, string category, int[] indicators, int[] region, int startYear, int endYear, int width, int itemWidth)
        {
            var model = new CategotyChartData() { Title = title ?? category, ClientId = Guid.NewGuid().ToString("N") };

            if (null != region && 1 == region.Length)
            {
                model.Load(category ?? title, indicators ?? new int[0], region[0], startYear, endYear, width, itemWidth);

                JavaScriptSerializer js = new JavaScriptSerializer();
                ViewBag.categories = js.Serialize(model.Categories);
                ViewBag.series = js.Serialize(model.Series);
                model.maxValue = maxValue;
                return PartialView("CategoryChartByYear", model);
            }


            return Content(string.Empty);
        }


    }
}