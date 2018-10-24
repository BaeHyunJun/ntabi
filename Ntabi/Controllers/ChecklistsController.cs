using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
{
    public class ChecklistsController : Controller
    {
        // GET: Checklists
        public ActionResult Index()
        {
            return View();
        }
    }
}