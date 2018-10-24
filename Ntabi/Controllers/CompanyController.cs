using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
{
    public class CompanyController : Controller
    {
        // GET: Company
        public ActionResult Index()
        {
            return View();
        }

        // GET: Company/Map
        public ActionResult Map()
        {
            return View();
        }

        // GET: Company/Advertisement
        public ActionResult Advertisement()
        {
            return View();
        }

        // GET: Company/Map
        public ActionResult privacy()
        {
            return View();
        }

        // GET: Company/Map
        public ActionResult service()
        {
            return View();
        }
    }
}