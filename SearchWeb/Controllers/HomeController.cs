using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SearchService;

namespace SearchWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetRifList(string cedula)
        {  
            var service = new SearchService.Service();
            var list = service.GetRifList(cedula);

            return this.Json(list,JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCompanyName(string rif)
        {
            var service = new SearchService.Service();
            var company = service.GetCompanyName(rif);

            return this.Json(company, JsonRequestBehavior.AllowGet);
        }
    }
}