using mNtabi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class PassController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB NDB = new NtabiDB();

        public string[] getCode(string code, string key)
        {
            var priKey = code.Split(key.ToCharArray());

            string corp = priKey[0].Substring(0, 3);
            string type = priKey[0].Substring(3, 2);

            string t = priKey[0] + key;

            var priKey3 = code.Substring(t.Length);

            string pdtyy = priKey3.Substring(0, 2);
            string pdtseq = priKey3.Substring(2);

            string[] arrCode = { corp, type, key, pdtyy, pdtseq };

            return arrCode;
        }

        // GET: Pass
        public ActionResult Index(string t = "")
        {
            var passLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                    .Where(w => w.PDT_TYPE_CODE.Equals("PS"))
                                    .Select(s => new
                                    {
                                        s.CORP_CODE,
                                        s.PDT_TYPE_CODE,
                                        s.PDT_IST_EMP_NO,
                                        s.PDT_YY,
                                        s.PDT_SEQ,
                                        s.PDT_TITLE,
                                        s.PDT_CONTENT,
                                        s.PDT_ORDER_NO,
                                        s.PDT_AREA_CODE,
                                    });

            if (!string.IsNullOrEmpty(t))
            {
                passLists = passLists.Where(w => w.PDT_AREA_CODE.Equals(t));
            }

            var rData0 = passLists.Join(DB.PDT_2,
                                           a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
                                           b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
                                           (a, b) => new
                                           {
                                               a.CORP_CODE,
                                               a.PDT_TYPE_CODE,
                                               a.PDT_IST_EMP_NO,
                                               a.PDT_YY,
                                               a.PDT_SEQ,
                                               a.PDT_TITLE,
                                               a.PDT_CONTENT,
                                               a.PDT_ORDER_NO,
                                               b.PDT_IMG
                                           });

            var rData1 = rData0.Join(DB.PRC_0,
                                     a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
                                     b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
                                    (a, b) => new
                                    {
                                        a.CORP_CODE,
                                        a.PDT_TYPE_CODE,
                                        a.PDT_IST_EMP_NO,
                                        a.PDT_YY,
                                        a.PDT_SEQ,
                                        a.PDT_TITLE,
                                        a.PDT_CONTENT,
                                        a.PDT_ORDER_NO,
                                        a.PDT_IMG,
                                        b.PRC_Adult,
                                        b.PRC_Currency_CODE,
                                    });

            var rData2 = rData1.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.PRC_Currency_CODE })
                               .Select(s => new
                               {
                                   s.Key.CORP_CODE,
                                   s.Key.PDT_TYPE_CODE,
                                   s.Key.PDT_IST_EMP_NO,
                                   s.Key.PDT_YY,
                                   s.Key.PDT_SEQ,
                                   s.FirstOrDefault().PDT_TITLE,
                                   s.FirstOrDefault().PDT_CONTENT,
                                   s.FirstOrDefault().PDT_ORDER_NO,
                                   s.FirstOrDefault().PDT_IMG,
                                   s.Key.PRC_Currency_CODE,
                                   minPrice = s.Min(min => min.PRC_Adult),

                               });

            return View(rData2);
        }

        // GET: Pass/Views
        public ActionResult Views(string C, string K)
        {
            var priKey = getCode(C, K);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string pdtyy = priKey[3];

            int emp = Convert.ToInt32(priKey[2]);
            int pdtseq = Convert.ToInt32(priKey[4]);

            var passLists = DB.PDT_0.ToList()
                              .Where(w => w.CORP_CODE.Equals(corp))
                              .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                              .Where(w => w.PDT_IST_EMP_NO == (emp))
                              .Where(w => w.PDT_YY.Equals(pdtyy))
                              .Where(w => w.PDT_SEQ == (pdtseq))
                              .Where(w => w.PDT_PROC_CODE.Equals("02"))
                              .Select(s => new
                              {
                                  s.CORP_CODE,
                                  s.PDT_TYPE_CODE,
                                  s.PDT_IST_EMP_NO,
                                  s.PDT_YY,
                                  s.PDT_SEQ,
                                  s.PDT_TITLE,
                                  s.PDT_CONTENT,
                                  s.PDT_ORDER_NO,
                              });

            if (!passLists.Any())
            {
                return Content("<script>alert('마감된 상품입니다.'); location.href='/';</script>");
            }

            var rData0 = passLists.Join(DB.PDT_2,
                                           a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
                                           b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
                                           (a, b) => new
                                           {
                                               a.CORP_CODE,
                                               a.PDT_TYPE_CODE,
                                               a.PDT_IST_EMP_NO,
                                               a.PDT_YY,
                                               a.PDT_SEQ,
                                               a.PDT_TITLE,
                                               a.PDT_CONTENT,
                                               a.PDT_ORDER_NO,
                                               b.PDT_IMG,
                                               b.PDT_SCH0,
                                               b.PDT_SCH1,
                                               b.PDT_SCH2,
                                               b.PDT_SCH3,
                                               b.PDT_SCH4,
                                               b.PDT_SCH5,
                                               b.PDT_SCH6,
                                               b.PDT_SCH7,
                                           });

            var prcData = DB.PRC_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == (emp))
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == (pdtseq))
                                  .Select(s => new
                                  {
                                      s.PRC_SEQ,
                                      s.PRC_Currency_CODE,
                                      s.PRC_Adult,
                                      s.PRC_EXP,
                                      s.PRC_Child,
                                      s.PRC_Baby,
                                  });

            ViewBag.prcData = prcData;

            return View(rData0);
        }
    }
}