using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
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