using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NCompany.Models;
using System.Security.Cryptography;
using System.Text;

namespace NCompany.Controllers
{
    public class ManageController : Controller
    {
        DataBase DB = new DataBase();

        public string getPer(string code)
        {
            string txt = "";

            switch(code)
            {
                case "PA":
                    txt = "시스템 관리자";
                    break;
                case "AC":
                    txt = "회계관리자";
                    break;
                case "OP":
                    txt = "오퍼레이터";
                    break;
                case "GD":
                    txt = "가이드";
                    break;
            }

            return txt;
        }

        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var Data = DB.EMP_0.ToList()
                               .Join(DB.POS_0.ToList(),
                                    a => a.POS_CODE,
                                    b => b.POS_CODE,
                                    (a, b) => new
                                    {
                                        a.EMP_NO,
                                        per = getPer(a.PER_CODE.ToString()),
                                        a.EMP_NAME,
                                        b.POS_NAME,
                                        b.POS_NO
                                    }
                               ).OrderBy(o => o.POS_NO);

            var GroData = DB.GRO_0.Select(s => new
                                  {
                                      s.GRO_CODE,
                                      s.GRO_NAME,
                                  });

            var PosData = DB.POS_0.Select(s => new
                                  {
                                      s.POS_CODE,
                                      s.POS_NAME,
                                  });

            var PerData = DB.PER_0.Select(s => new
                                  {
                                      s.PER_CODE
                                  });

            ViewBag.GroData = GroData;
            ViewBag.PosData = PosData;
            ViewBag.PerData = PerData;

            return View(Data);
        }

        public ActionResult Organization()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var DefaultData1 = DB.GRO_0.Where(w => string.IsNullOrEmpty(w.GRO_PARENT))
                                       .Select(s => new
                                       {
                                           num = s.GRO_NO,
                                           code = s.GRO_CODE,
                                           name = s.GRO_NAME,
                                           parent = s.GRO_PARENT,
                                       });

            var DefaultData2 = DefaultData1.Join(DB.GRO_0,
                                                a => a.code,
                                                b => b.GRO_PARENT,
                                                (a, b) => new
                                                {
                                                    num = b.GRO_NO,
                                                    code = b.GRO_CODE,
                                                    name = b.GRO_NAME,
                                                    parent = b.GRO_PARENT,
                                                });

            var DefaultData = DefaultData1.Union(DefaultData2);

            DefaultData = DB.GRO_0.Select(s => new
                                   {
                                       num = s.GRO_NO,
                                       code = s.GRO_CODE,
                                       name = s.GRO_NAME,
                                       parent = s.GRO_PARENT,
                                   });
            return View(DefaultData);
        }

        public ActionResult Position()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult UserUpdate(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string name     = f["name"],
                   id       = f["id"],
                   mail     = f["mail"],
                   pwd      = f["pwd"],
                   group    = f["group"],
                   birth    = f["birth"],
                   position = f["pos"],
                   tel      = f["tel"],
                   per      = f["per"],
                   phone    = f["phone"],
                   feature  = f["feature"],
                   tel1     = tel.Substring(0, 3),
                   tel2     = "",
                   tel3     = "",
                   phone1   = phone.Substring(0, 3),
                   phone2   = "",
                   phone3   = "";

            int seq = 0;

            try
            {
                seq = Convert.ToInt32(f["seq"].ToString());
            }
            catch(Exception e)
            {

            }

            tel1 = tel.Substring(0, 3);
            tel3 = tel.Substring(tel.Length - 4);

            int lastIndex = tel.IndexOf(tel3);

            tel2 = tel.Substring(3, lastIndex - 3);

            phone1 = phone.Substring(0, 3);
            phone3 = phone.Substring(phone.Length - 4);

            lastIndex = phone.IndexOf(phone3);

            phone2 = phone.Substring(3, lastIndex - 3);

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, pwd);

            var chkData = DB.EMP_0.Where(w => w.EMP_NO == seq);

            if (chkData.Any())
            {
                chkData.Single().EMP_NAME   = name;
                chkData.Single().EMP_ID     = id;
                chkData.Single().EMP_EMAIL  = mail;
                chkData.Single().EMP_PWD    = hash.Substring(0, 20);
                chkData.Single().GRO_CODE   = group;
                chkData.Single().EMP_BIRTH  = birth;
                chkData.Single().POS_CODE   = position;
                chkData.Single().PER_CODE   = per;
                chkData.Single().EMP_TEL1   = tel1;
                chkData.Single().EMP_TEL2   = tel2;
                chkData.Single().EMP_TEL3   = tel3;
                chkData.Single().EMP_PHONE1 = phone1;
                chkData.Single().EMP_PHONE2 = phone2;
                chkData.Single().EMP_PHONE3 = phone3;
            }
            else
            {
                DateTime now = DateTime.Now;

                EMP_0 Data = new EMP_0();

                Data.EMP_NO     = seq;
                Data.EMP_NAME   = name;
                Data.EMP_ID     = id;
                Data.EMP_EMAIL  = mail;
                Data.EMP_PWD    = pwd;
                Data.GRO_CODE   = group;
                Data.EMP_BIRTH  = birth;
                Data.POS_CODE   = position;
                Data.EMP_TEL1   = tel1;
                Data.EMP_TEL2   = tel2;
                Data.EMP_TEL3   = tel3;
                Data.PER_CODE   = per;
                Data.EMP_PHONE1 = phone1;
                Data.EMP_PHONE2 = phone2;
                Data.EMP_PHONE3 = phone3;
                Data.EMP_JOIN   = now;

                DB.EMP_0.Add(Data);
            }

            DB.SaveChanges();

            return Redirect("/Manage");
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

        public JsonResult JSON_getUser(string seq)
        {
            int emp_no = Convert.ToInt32(seq.ToString());

            var Data = DB.EMP_0.Where(w => w.EMP_NO == emp_no)
                               .Select(s => new
                               {
                                   no = s.EMP_NO,
                                   name = s.EMP_NAME,
                                   id = s.EMP_ID,
                                   mail = s.EMP_EMAIL,
                                   gro = s.GRO_CODE,
                                   bir = s.EMP_BIRTH,
                                   pos = s.POS_CODE,
                                   tel = s.EMP_TEL1 + s.EMP_TEL2 + s.EMP_TEL3,
                                   per = s.PER_CODE,
                                   phone = s.EMP_PHONE1 + s.EMP_PHONE2 + s.EMP_PHONE3,
                               });

            return Json(Data);
        }
    }
}