using Smartsp.aspire.dashborad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Smartsp.aspire.dashborad.Controllers
{
    public class DictionaryController : Controller
    {
        // GET: Dictionary
        public ActionResult Index()
        {
            var model = new MainModel();      
            model.InitForDictonary();
            return View(model);
        }

        public ActionResult aboutData()
        {
            return View();
        }
        
    }
}