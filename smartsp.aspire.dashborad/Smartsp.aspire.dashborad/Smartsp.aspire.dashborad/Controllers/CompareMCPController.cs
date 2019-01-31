using Smartsp.aspire.dashborad.Models;
using Smartsp.aspire.dashborad.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class CompareMCPController : Controller
    {
        // GET: CompareMCP
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompareMain()
        {
            return View();
        }

        public ActionResult RussianDataMain()
        {
            return View();
        }

        public ActionResult CompareBySource()
        {
            return View();
        }

        public ActionResult CompareByCriteria()
        {
            return View();
        }
        #region MSPCompareByType
        [HttpPost]
        public ActionResult MSPCompareByType(int region, int[] categories, int startYear, int endYear)
        {
            var model = new MSPCompareByTypeData();

            model.Load(region, categories, startYear, endYear);
            return PartialView("MSPCompareByType", model);

        }

        [HttpPost]
        public ActionResult MSPCompareByType2(int[] regions, int[] categories, int year)
        {
            var model = new MSPCompareByTypeData();

            model.Load(regions, categories, year);
            return PartialView("MSPCompareByType2", model);

        }
        [HttpPost]
        public ActionResult MSPCompareByType3(int region, int type, int[] category, int startYear, int endYear)
        {
            var model = new MSPCompareByTypeData();

            model.Load(region, type, category, startYear, endYear);
            return PartialView("MSPCompareByType3", model);

        }

        [HttpPost]
        public ActionResult MSPCompareByType4(int year, int type, int[] category, int[] regions)
        {
            var model = new MSPCompareByTypeData();

            model.Load(year, type, category, regions);
            return PartialView("MSPCompareByType4", model);

        }

        [HttpPost]
        public ActionResult MSPCompareByType5(int region, int type, int category, int[] subcategories, int startYear, int endYear)
        {
            var model = new MSPCompareByTypeData();

            model.Load(region, type, category, subcategories, startYear, endYear);
            return PartialView("MSPCompareByType5", model);

        }

        [HttpPost]
        public ActionResult MSPCompareByType6(int[] regions, int type, int category, int[] subcategories, int year)
        {
            var model = new MSPCompareByTypeData();

            model.Load(regions, type, category, subcategories, year);
            return PartialView("MSPCompareByType6", model);

        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType(int region, int[] categories, int startYear, int endYear)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(region, categories, startYear, endYear);
            var res = GetPartialViewContent(model, "MSPCompareByType");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle,  FileName = ExportFile.GetNameFileExport("Report1") }
            };
        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType2(int[] regions, int[] categories, int year)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(regions, categories, year);
            var res = GetPartialViewContent(model, "MSPCompareByType2");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report2") }
            };
        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType3(int region, int type, int[] category, int startYear, int endYear)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(region, type, category, startYear, endYear);
            var res = GetPartialViewContent(model, "MSPCompareByType3");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report3")  }
            };
        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType4(int year, int type, int[] category, int[] regions)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(year, type, category, regions);
            var res = GetPartialViewContent(model, "MSPCompareByType4");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report4")  }
            };
        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType5(int region, int type, int category, int[] subcategories, int startYear, int endYear)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(region, type, category, subcategories, startYear, endYear);
            var res = GetPartialViewContent(model, "MSPCompareByType5");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report5") }
            };
        }

        [HttpPost]
        public ActionResult PrintMSPCompareByType6(int[] regions, int type, int category, int[] subcategories, int year)
        {
            string handle = Guid.NewGuid().ToString();
            var model = new MSPCompareByTypeData();
            model.Load(regions, type, category, subcategories, year);
            var res = GetPartialViewContent(model, "MSPCompareByType6");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report6") }
            };
        }

        [HttpGet]
        public virtual ActionResult Download(string fileGuid, string fileName)
        {
            if (TempData[fileGuid] != null)
            {
                //var enc = new UTF8Encoding(false);
                byte[] utf8bytes = Encoding.UTF8.GetBytes(TempData[fileGuid].ToString());
                //byte[] Indianbytes = enc.GetBytes(TempData[fileGuid].ToString());
              //  var win1252Bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(1252), utf8bytes);
                return File(utf8bytes, "application/vnd.ms-excel;charset=UTF-8", fileName);
            }
            else
            {
                return new EmptyResult();
            }
        }

        private string GetPartialViewContent(IMSPCompareData model, string partialViewName)
        {
            var viewData = new ViewDataDictionary(model);
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(this.ControllerContext, partialViewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, viewData, new TempDataDictionary(), sw);
                viewResult.View.Render(viewContext, sw);
                //var e = sw.Encoding;
                return sw.GetStringBuilder().ToString()
                    .Replace("Скачать в Excel", "")
                    .Insert(0, "<meta http-equiv=\"content-type\" content=\"application/vnd.ms-excel; charset=UTF-8\" />");
            }
        }

        public JsonResult GetMspCategoriesByArea(int type)
        {
            var model = new MSPCompareByTypeData();
            var res = model.GetMspCategoriesByArea(type);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMspSubCategoriesByArea(int type)
        {
            var model = new MSPCompareByTypeData();
            var res = model.GetMspSubCategoriesByArea(type);
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMspSubCategoriesByCategory(int category)
        {
            var model = new MSPCompareByTypeData();
            var res = model.GetMspSubCategoriesByCategory(category);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Compare By Source
        [HttpPost]
        public ActionResult MSPCompareBySource(int regions, int[] areas, int year)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, year);
            return PartialView("MSPCompareBySource", model);

        }

        [HttpPost]
        public ActionResult MSPCompareBySource2(int regions, int[] areas, int startYear, int endYear)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, startYear, endYear);
            return PartialView("MSPCompareBySource2", model);

        }

        [HttpPost]
        public ActionResult MSPCompareBySource3(int[] regions, int[] areas, int year)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, year);
            return PartialView("MSPCompareBySource3", model);

        }
        [HttpPost]
        public ActionResult PrintMSPCompareBySource(int regions, int[] areas, int year)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, year);

            string handle = Guid.NewGuid().ToString();
            var res = GetPartialViewContent(model, "MSPCompareBySource");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report")  }
            };

        }

        [HttpPost]
        public ActionResult PrintMSPCompareBySource2(int regions, int[] areas, int startYear, int endYear)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, startYear, endYear);
            string handle = Guid.NewGuid().ToString();
            var res = GetPartialViewContent(model, "MSPCompareBySource2");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report2") }
            };

        }

        [HttpPost]
        public ActionResult PrintMSPCompareBySource3(int[] regions, int[] areas, int year)
        {
            var model = new MSPCompareBySourceData();

            model.Load(regions, areas, year);
            string handle = Guid.NewGuid().ToString();
            var res = GetPartialViewContent(model, "MSPCompareBySource3");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report3")  }
            };

        }
        #endregion

        #region Compare By Criteria
        [HttpPost]
        public ActionResult MSPCompareByCriteria(int region, int area, int category, int startYear, int endYear)
        {
            var model = new MSPCompareByCriteriaData();

            model.Load(region, area, category, startYear, endYear);
            return PartialView("MSPCompareByCriteria", model);

        }

        [HttpPost]
        public ActionResult MSPCompareByCriteria2(int[] regions, int area, int category, int year)
        {
            var model = new MSPCompareByCriteriaData();

            model.Load(regions, area, category, year);
            return PartialView("MSPCompareByCriteria2", model);

        }

        [HttpPost]
        public ActionResult PrintMSPCompareByCriteria(int region, int area, int category, int startYear, int endYear)
        {
            var model = new MSPCompareByCriteriaData();

            model.Load(region, area, category, startYear, endYear);
            string handle = Guid.NewGuid().ToString();
            var res = GetPartialViewContent(model, "MSPCompareByCriteria");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report1") }
            };

        }

        [HttpPost]
        public ActionResult PrintMSPCompareByCriteria2(int[] regions, int area, int category, int year)
        {
            var model = new MSPCompareByCriteriaData();

            model.Load(regions, area, category, year);
            string handle = Guid.NewGuid().ToString();
            var res = GetPartialViewContent(model, "MSPCompareByCriteria2");
            TempData[handle] = res;
            return new JsonResult()
            {
                Data = new { FileGuid = handle, FileName = ExportFile.GetNameFileExport("Report2")  }
            };

        }
        #endregion
    }
}