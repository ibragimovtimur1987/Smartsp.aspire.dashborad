using Smartsp.aspire.dashborad.Models;
using Smartsp.aspire.dashborad.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class TablesController : Controller
    {
        // GET: Tables
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MCPEfficiency(int[] region, int startYear, int endYear)
        {
            var model = new MSPEfficiencyData();
            if (null != region && 1 == region.Length)
            {
                model.Load(region[0], startYear, endYear);
                return PartialView("MCPEfficiency", model);
            }
            //if (null != region && 1 < region.Length && startYear == endYear)
            //{
            //    model.Load(region, startYear);
            //    return PartialView("MCPEfficiencyForYear", model);
            //}
            return Content(string.Empty);
        }
        [HttpPost]
        public ActionResult PrintMCPEfficiency(int[] region, int startYear, int endYear)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPEfficiencyData();
            model.Load(region[0], startYear, endYear);
            var res = GetPartialViewContent(model, "MCPEfficiency");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle,
                FileName = ExportFile.GetNameFileExport("Report1") }
            };
        }
        private string GetPartialViewContent(MSPEfficiencyData model, string partialViewName)
        {
            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialViewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString().Replace("Скачать в Excel", "");
            }
        }
    }
}