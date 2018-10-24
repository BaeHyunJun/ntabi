using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class GuideController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        public bool chkUser()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return false;
            }

            if ((user.PER_CODE != "GD" || user.POS_CODE != "PG") && user.PER_CODE != "PA")
            {
                Session.Abandon();
                Session["user"] = null;

                return false; //Redirect("/Guide");
            }

            return true;
        }

        // GET: Guide
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (chkUser())
            {
                return Redirect("/Guide/Schedule");
            }

            return View();
        }

        //public ActionResult Schedule()
        //{
        //    if (!chkUser())
        //    {
        //        string msg = "가이드 계정으로 로그인 해주세요.";

        //        return Content("<script>alert('" + msg + "');location.href='/Guide';</script>");
        //    }

        //    var schData = DB.COU_0//.Where(w => w.COU_EMP_NO.ToString().Equals(user.Login))//.Where(w => w.COU_AREA_CODE.Equals(area))
        //                          .ToList()
        //                          .Select(s => new
        //                          {
        //                              key = s.COU_SEQ,
        //                              date = s.COU_DATE.Substring(4, 2) + "/" + s.COU_DATE.Substring(6, 2),
        //                              title = s.COU_TITLE,
        //                              cnt = s.COU_CNT,
        //                              car = s.COU_CAR_SEQ.ToString(),
        //                              emp = s.COU_EMP_NO.ToString(),
        //                              s.COU_DATE
        //                          });

        //    var user = Session["user"] as User;

        //    if (false)
        //    {
        //        schData = schData.Where(w => w.emp.ToString().Equals(user.Login));
        //    }

        //    DateTime now = DateTime.Now;

        //    schData = schData.Where(w => Convert.ToInt32(w.COU_DATE) > Convert.ToInt32(now.ToString("yyyyMMdd")));

        //    //var getDateData = DB.PDT_1.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
        //    //                          .GroupBy(g => new { g.PDT_DATE })
        //    //                          .Select(s => new
        //    //                          {
        //    //                              date = s.Key.PDT_DATE,
        //    //                          });

        //    //var daaaa = Json(schData);

        //    //ViewBag.Date = daaaa.Data;

            
        //    return View(schData);
        //}

        //// GET: Guide/Views
        //public ActionResult Views(string date, string key)
        //{
        //    if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(key))
        //    {
        //        return Redirect("/Guide");
        //    }

        //    var schData = DB.COU_0.Where(w => w.COU_DATE.Equals(date))
        //                          .Where(w => w.COU_SEQ.ToString().Equals(key))
        //                          .ToList()
        //                          .Select(s => new
        //                          {
        //                              s.COU_TITLE,
        //                              date = s.COU_DATE.Substring(0, 4) + "." + s.COU_DATE.Substring(4, 2) + "." + s.COU_DATE.Substring(6, 2),
        //                              s.COU_DRIVER,
        //                              s.COU_CAR_SEQ,
        //                              s.COU_CNT,
        //                              s.COU_NOTE
        //                          });

        //    return View(schData);
        //}

        //public JsonResult getDays(string area = "", string emp = "")
        //{
        //    var schData = DB.COU_0.Select(s => new
        //                          {
        //                              day = s.COU_DATE,
        //                              s.COU_AREA_CODE,
        //                              s.COU_EMP_NO,
        //                          });

        //    if (!string.IsNullOrEmpty(area))
        //    {
        //        schData = schData.Where(w => w.COU_AREA_CODE.Equals(area));
        //    }

        //    if (!string.IsNullOrEmpty(emp))
        //    {
        //        schData = schData.Where(w => w.COU_EMP_NO.ToString().Equals(emp));
        //    }

        //    return Json(schData);
        //}
    }
}