using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class MagazineController : Controller
    {
        // GET: Magazine
        public ActionResult Index()
        {
            return View();
        }

        // GET: Magazine/Views
        public ActionResult Views()
        {
            return View();
        }
    }
}