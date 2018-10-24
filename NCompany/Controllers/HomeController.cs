using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Security.Cryptography;
using System.Text;

namespace NCompany.Controllers
{
    public class HomeController : Controller
    {
        DataBase DB = new DataBase();

        public string convertDate8To10(string date)
        {
            if (date.Length != 8)
                return date;

            string result = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);

            return result;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection f)
        {
            string id = f["loginID"];

            var data = DB.EMP_0.Where(w => w.PER_CODE != "GD")
                               .Where(w => w.EMP_ID.Equals(id))
                               .Select(s => new
                               {
                                   s.EMP_NO,
                                   s.EMP_NAME,
                                   s.EMP_ID,
                                   s.EMP_PWD,
                                   s.PER_CODE,
                                   s.POS_CODE,
                               });

                       //from user in DB.EMP_0
                       //where user.EMP_ID == id &&
                       //select new
                       //{
                       //    PK = user.ASHOP_SITE_CD + user.CU_YY + user.CU_SEQ,
                       //    Name = user.CU_NM_KOR,
                       //    ID = user.CU_ID,
                       //    PW = user.CU_PASS
                       //};

            int erLogin = 0;

            if (data.Count() == 0)
            {
                erLogin = 1;
            }
            else
            {
                string pwd = f["loginPWD"];
                MD5 md5Hash = MD5.Create();
                string hash = GetMd5Hash(md5Hash, pwd);
                data = data.Where(p => p.EMP_PWD.Equals(hash.Substring(0, 20)));

                if (data.Count() == 0)
                {
                    erLogin = 2;
                }
                else
                {
                    Session["user"] = new User() { Login = data.Single().EMP_NO.ToString(), ID = data.Single().EMP_ID, Name = data.Single().EMP_NAME, PER_CODE = data.Single().PER_CODE, POS_CODE = data.Single().POS_CODE };
                }
            }

            string erMsg = "";

            switch (erLogin)
            {
                case 1:
                    erMsg = "존재하지 않는 아이디 입니다.";
                    break;
                case 2:
                    erMsg = "비밀번호가 틀립니다.";
                    break;
            }

            string Url = "/";

            if (!string.IsNullOrEmpty(erMsg))
            {

                if (!string.IsNullOrEmpty(f["redirect"]))
                {
                    Url = f["redirect"];
                }

                return Content("<script>alert('" + erMsg + "');location.href='" + Url + "';</script>");
            }

            Url = "/Home/DashBoard";

            if (!string.IsNullOrEmpty(f["redirect"]))
            {
                Url = f["redirect"];
            }

            return Redirect(Url);
        }

        public ActionResult DashBoard()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string emp = user.Login.ToString();

            var pdt = DB.PDT_0.ToList()
                              .Where(w => w.PDT_EMP_NO == Convert.ToInt32(emp))
                              .Select(s => new
                              {
                                  s.PDT_PROC_CODE,
                                  s.PDT_TITLE,
                                  s.PDT_OPTIONS,
                                  s.PDT_SEQ,
                              })
                              .OrderByDescending(o => o.PDT_SEQ);

            var rev = DB.REV_0.ToList()
                              .Where(w => w.CHG_EMP_NO == Convert.ToInt32(emp))
                              .Select(s => new
                              {
                                  s.REV_STATE,
                                  s.REV_STARTDAY,
                                  s.PDT_TITLE,
                                  s.CU_NAME,
                                  s.REV_SEQ,
                              })
                              .OrderByDescending(o => o.REV_STARTDAY);

            var qna = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals("NDT"))
                                   .Where(w => w.DEL_FG.Equals("N"))
                                   .Where(w => w.post_type.Equals("qna"))
                                   .Where(w => w.post_parent == null)
                                   .OrderByDescending(o => o.post_ID)
                                   .Select(s => new
                                   {
                                       s.post_title,
                                       s.post_status
                                   });

            var notice = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals("NDT"))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("notice"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)    
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.ist_date
                                      });

            var review = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals("NDT"))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("review"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.post_options
                                      });

            var evData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals("NDT"))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("event"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.ist_date,
                                          s.post_thumb
                                      });

            ViewBag.pdt = pdt;
            ViewBag.rev = rev;
            ViewBag.qna = qna;
            ViewBag.notice = notice;
            ViewBag.review = review;
            ViewBag.evData = evData;

            return View();
        }


        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
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
    }
}