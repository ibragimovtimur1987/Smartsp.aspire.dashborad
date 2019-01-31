using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class ViewController : Controller
    {
        // GET: View
        public ActionResult Index()
        {
            var model = new MainModel();
            model.Init();

            return PartialView(model);
        }

  
        public ActionResult MSPCompareByType()
        {
            var model = new MSPCompareByTypeMainModel();
            model.Init();

            return PartialView(model);
        }


        public ActionResult MSPCompareBySource()
        {
            var model = new MSPCompareByTypeMainModel();
            model.Init();

            return PartialView(model);
        }

        public ActionResult MSPCompareByCriteria()
        {
            var model = new MSPCompareByTypeMainModel();
            model.Init();

            return PartialView(model);
        }
    }
}