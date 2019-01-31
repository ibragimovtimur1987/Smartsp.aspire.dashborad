using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //ToDo: render data for main page


            return View();
        }

        public ActionResult IndexCommon()
        {
            return View();
        }

        public ActionResult mainPage()
        {
            return View();
        }

        public ActionResult russianData()
        {
            return View();
        }

        public ActionResult internationalVersion()
        {
            return View();
        }

        public ActionResult russionVersion()
        {
            return View();
        }
    }
}