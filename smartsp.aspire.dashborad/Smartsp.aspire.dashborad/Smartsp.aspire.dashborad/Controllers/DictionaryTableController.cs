using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class DictionaryTableController : Controller
    {
        // GET: DictionaryTable
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Filter(int region, string searchVal)
        {
            var model = new DictionaryModel();
            model.Load(region, searchVal);         
            return PartialView("View", model);
        }
       
    }
}