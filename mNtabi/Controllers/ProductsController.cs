using mNtabi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class ProductsController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB NDB = new NtabiDB();

        string corp = "NTB";

        // GET: Products
        public ActionResult Index(string N = "", string A = "")
        {
            var BestLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                    .Where(w => w.PDT_NATION_CODE.Equals(N))
                                    .ToList();

            var chkImg = DB.PDT_5.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                .Select(s => new
                                {
                                    key = (s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO + s.Key.PDT_YY + s.Key.PDT_SEQ).ToString(),
                                });

            List<string> imgLists = new List<string>();

            foreach (var item in chkImg)
            {
                imgLists.Add(item.key);
            }

            BestLists = BestLists.Where(w => imgLists.Contains(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                                 .ToList();

            if (string.IsNullOrEmpty(A))
            {
                BestLists = null;
                ViewBag.PackPdt = null;
                ViewBag.FreePdt = null;
                ViewBag.LocalPdt = null;

                return View();
            }
            else if (A == "ETC")
            {
                List<string> area = new List<string>();

                area.Add("TOK");
                area.Add("TYO");
                area.Add("OSA");
                area.Add("KYU");
                area.Add("FUK");
                area.Add("OKA");
                area.Add("OKI");
                area.Add("HOK");
                area.Add("CTS");
                area.Add("TSM");
                area.Add("TSU");

                BestLists = BestLists.Where(w => !area.Contains(w.PDT_AREA_CODE))
                                     .ToList();
            }
            else
            {
                List<string> area = getAreaCode(A);

                BestLists = BestLists.Where(w => area.Contains(w.PDT_AREA_CODE))
                                     .ToList();
            }

            var rData0 = BestLists.Join(DB.PDT_2,
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
                                               a.PDT_DAYS_CODE,
                                               a.PDT_OPTIONS,
                                               a.PDT_ORDER_NO,
                                               a.PDT_COMBINE_HTL,
                                               b.PDT_IMG
                                           });
            DateTime now = DateTime.Now;

            var chkDateLists = DB.PDT_1.ToList()
                                       .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                       .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.PRC_SEQ, g.TRF_SEQ })
                                       .Select(s => new
                                       {
                                           s.Key.CORP_CODE,
                                           s.Key.PDT_TYPE_CODE,
                                           s.Key.PDT_IST_EMP_NO,
                                           s.Key.PDT_YY,
                                           s.Key.PDT_SEQ,
                                           s.Key.PRC_SEQ,
                                           s.Key.TRF_SEQ,
                                       });

            var minPriceLists = chkDateLists.Join(DB.PRC_0,
                                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      b.PRC_Adult,
                                                      a.TRF_SEQ,
                                                  })
                                                  .GroupBy(g => new
                                                  {
                                                      g.CORP_CODE,
                                                      g.PDT_TYPE_CODE,
                                                      g.PDT_IST_EMP_NO,
                                                      g.PDT_YY,
                                                      g.PDT_SEQ
                                                  })
                                                  .Select(s => new
                                                  {
                                                      s.Key.CORP_CODE,
                                                      s.Key.PDT_TYPE_CODE,
                                                      s.Key.PDT_IST_EMP_NO,
                                                      s.Key.PDT_YY,
                                                      s.Key.PDT_SEQ,
                                                      minPrice = s.Min(min => min.PRC_Adult),
                                                      TRF_SEQ = s.Min(min => min.TRF_SEQ)
                                                  });

            var secondLists = rData0.Join(minPriceLists,
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
                                             a.PDT_IMG,
                                             minPrice = a.PDT_COMBINE_HTL == "Y" ? Convert.ToInt32(getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, "2018-12-31", Convert.ToInt32(a.PDT_DAYS_CODE.ToString().Substring(0, 2)))) : b.minPrice,
                                             a.PDT_COMBINE_HTL,
                                             a.PDT_DAYS_CODE,
                                             a.PDT_OPTIONS,
                                             a.PDT_ORDER_NO,
                                             b.TRF_SEQ,
                                             DATE = "",
                                         }).ToList();

            var dateLists = DB.PDT_1.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                    .Select(s => new
                                    {
                                        s.Key.CORP_CODE,
                                        s.Key.PDT_TYPE_CODE,
                                        s.Key.PDT_IST_EMP_NO,
                                        s.Key.PDT_YY,
                                        s.Key.PDT_SEQ,
                                        Date = s.Min(min => min.PDT_DATE).Substring(0, 7) + " ~ " + s.Max(max => max.PDT_DATE).Substring(0, 7)
                                    });

            secondLists = secondLists.Join(dateLists,
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
                                              a.PDT_IMG,
                                              a.minPrice,
                                              a.PDT_COMBINE_HTL,
                                              a.PDT_DAYS_CODE,
                                              a.PDT_OPTIONS,
                                              a.PDT_ORDER_NO,
                                              a.TRF_SEQ,
                                              DATE = b.Date,
                                          }).ToList();

            var traffData = DB.TRF_0.ToList();

            var rData00 = secondLists.Join(traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")),
                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                      (a, b) => new
                                      {
                                          a.CORP_CODE,
                                          a.PDT_TYPE_CODE,
                                          a.PDT_IST_EMP_NO,
                                          a.PDT_YY,
                                          a.PDT_SEQ,
                                          a.PDT_TITLE,
                                          a.PDT_CONTENT,
                                          a.PDT_DAYS_CODE,
                                          a.PDT_OPTIONS,

                                          TRF_TITLE = b.TRF_TITLE.Trim(),
                                          TRF_SAREA = b.TRF_SAREA.Trim(),
                                          b.TRF_STIME,
                                          b.TRF_EAREA,
                                          b.TRF_ETIME,

                                          a.PDT_IMG,
                                          a.minPrice,
                                          a.DATE,
                                          a.PDT_ORDER_NO,
                                      })
                                      .OrderBy(o => o.PDT_ORDER_NO)
                                      .ThenBy(t => Convert.ToInt32(t.minPrice));

            var PackPdt = rData00.Where(w => w.PDT_OPTIONS.Equals("best")).Where(w => w.PDT_TYPE_CODE.Equals("PT")).Take(1);
            var FreePdt = rData00.Where(w => w.PDT_OPTIONS.Equals("best")).Where(w => w.PDT_TYPE_CODE.Equals("FT")).Take(1);
            var LocalPdt = rData00.Where(w => w.PDT_OPTIONS.Equals("best")).Where(w => w.PDT_TYPE_CODE.Equals("LT")).Take(1);

            if (A == "KMC")
            {
                PackPdt = rData00;
                FreePdt = null;
                LocalPdt = null;
            }

            ViewBag.PackPdt = PackPdt;
            ViewBag.FreePdt = FreePdt;
            ViewBag.LocalPdt = LocalPdt;

            return View(rData00);
        }

        // GET: Products/Lists
        public ActionResult Lists(string N = "", string A = "", string T = "", string S = "")
        {
            string traff = "",
                   tourType = "";

            string nowDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (string.IsNullOrEmpty(N) && string.IsNullOrEmpty(A) & string.IsNullOrEmpty(T) & string.IsNullOrEmpty(S))
            {
                return Redirect("/Community/Chat");
            }

            switch (T)
            {
                case "A":
                    traff = "AIR";
                    tourType = "FT";
                    break;
                case "S":
                    traff = "SHP";
                    tourType = "FT";
                    break;
                case "L":
                    tourType = "LT";
                    break;
                case "P":
                    tourType = "PT";
                    break;
                case "F":
                    tourType = "FT";
                    break;
            }

            if (N == "JP" && (A == "KYU" || A == "FUK") && T == "P")
            {
                traff = "AIR";
            }

            var PdtLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                //.Where(w => area.Contains(w.PDT_AREA_CODE))
                //.Where(w => w.PDT_SEQ.ToString().Equals("9"))
                                   .Select(s => new
                                   {
                                       s.CORP_CODE,
                                       s.PDT_TYPE_CODE,
                                       s.PDT_IST_EMP_NO,
                                       s.PDT_YY,
                                       s.PDT_SEQ,
                                       s.PDT_TITLE,
                                       s.PDT_CONTENT,
                                       s.PDT_DAYS_CODE,
                                       s.PDT_OPTIONS,
                                       s.PDT_ORDER_NO,
                                       s.PDT_COMBINE_HTL,
                                       s.PDT_NATION_CODE,
                                       s.PDT_AREA_CODE,
                                       s.PDT_UDT_DATE,
                                   })
                                   .ToList();

            if (A == "ETC")
            {
                List<string> area = new List<string>();

                area.Add("TOK");
                area.Add("TYO");
                area.Add("OSA");
                area.Add("KYU");
                area.Add("FUK");
                area.Add("OKA");
                area.Add("OKI");
                area.Add("HOK");
                area.Add("CTS");
                area.Add("TSM");
                area.Add("TSU");

                PdtLists = PdtLists.Where(w => w.PDT_NATION_CODE.Equals("JP"))
                                   .Where(w => !area.Contains(w.PDT_AREA_CODE))
                                   .OrderByDescending(o => o.PDT_UDT_DATE)
                                   .ToList();
            }
            else if (!string.IsNullOrEmpty(A))
            {
                List<string> area = getAreaCode(A);

                PdtLists = PdtLists.Where(w => area.Contains(w.PDT_AREA_CODE))
                                   .ToList();
            }

            PdtLists = PdtLists.Where(w => w.PDT_TYPE_CODE.Equals(tourType))
                               .ToList();

            if ((A == "KYU" || A == "FUK") && tourType == "PT")
            {
                List<string> area = getAreaCode(A);

                var vpLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                      .Where(w => w.PDT_TYPE_CODE.Equals("VP"))
                                      .Where(w => area.Contains(w.PDT_AREA_CODE))
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            s.PDT_TITLE,
                                            s.PDT_CONTENT,
                                            s.PDT_DAYS_CODE,
                                            s.PDT_OPTIONS,
                                            s.PDT_ORDER_NO,
                                            s.PDT_COMBINE_HTL,
                                            s.PDT_NATION_CODE,
                                            s.PDT_AREA_CODE,
                                            s.PDT_UDT_DATE,
                                        }).ToList();

                PdtLists = PdtLists.Union(vpLists).ToList();
            }

            if (A == "OSA" && tourType == "FT")
            {
                PdtLists = PdtLists.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ != "NTBFT221812"))
                                   .ToList();
            }

            var chkImg = DB.PDT_5.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                .Select(s => new
                                {
                                    key = (s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO + s.Key.PDT_YY + s.Key.PDT_SEQ).ToString(),
                                });

            List<string> imgLists = new List<string>();

            foreach (var item in chkImg)
            {
                imgLists.Add(item.key);
            }

            PdtLists = PdtLists.Where(w => imgLists.Contains(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                //.Where(w => w.PDT_TYPE_CODE.Equals("FT"))
                //.Where(w => w.PDT_IST_EMP_NO == 20)
                //.Where(w => w.PDT_YY.Equals("18"))
                //.Where(w => w.PDT_SEQ == 3)
                //.Take(5)
                                                   .ToList();

            DateTime now = DateTime.Now;

            var traffData = DB.TRF_0.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).ToList();

            if (traff.Length == 3)
            {
                traffData = traffData.Where(w => w.TRF_TYPE.Equals(traff)).ToList();
            }

            var chkDateLists = DB.PDT_1.ToList()
                                       .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                       .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.PRC_SEQ, g.TRF_SEQ })
                                       .Select(s => new
                                       {
                                           s.Key.CORP_CODE,
                                           s.Key.PDT_TYPE_CODE,
                                           s.Key.PDT_IST_EMP_NO,
                                           s.Key.PDT_YY,
                                           s.Key.PDT_SEQ,
                                           s.Key.PRC_SEQ,
                                           s.Key.TRF_SEQ,
                                       });

            List<string> chkTrf = new List<string>();

            switch (S)
            {
                case "KE":
                    chkTrf.Add("KE");
                    chkTrf.Add("대한항공");
                    chkTrf.Add("대한 항공");
                    break;
                case "BX":
                    chkTrf.Add("BX");
                    chkTrf.Add("에어부산");
                    chkTrf.Add("에어 부산");
                    break;
                case "LJ":
                    chkTrf.Add("LJ");
                    chkTrf.Add("진에어");
                    break;
                case "JL":
                    chkTrf.Add("JL");
                    chkTrf.Add("일본항공");
                    break;
                case "j7C":
                    chkTrf.Add("7C");
                    chkTrf.Add("제주항공");
                    chkTrf.Add("제주 항공");
                    break;
                case "PK":
                    chkTrf.Add("PK");
                    chkTrf.Add("부관훼리");
                    break;
                case "CA":
                    chkTrf.Add("CA");
                    chkTrf.Add("카멜리아");
                    break;
                case "BE":
                    chkTrf.Add("BE");
                    chkTrf.Add("비틀");
                    break;
                case "PA":
                    chkTrf.Add("PA");
                    chkTrf.Add("팬스타");
                    break;
                case "NN":
                    chkTrf.Add("NN");
                    chkTrf.Add("니나");
                    break;
                case "KB":
                    chkTrf.Add("KB");
                    chkTrf.Add("코비");
                    break;
                case "OF":
                    chkTrf.Add("OF");
                    chkTrf.Add("오션플라워");
                    break;
            }

            if (chkTrf.Count != 0)
            {

                traffData = traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
                                     .Where(w => chkTrf.Any(w.TRF_TITLE.Contains))
                                     .ToList();

                if (S == "BE")
                {
                    List<string> area = getAreaCode("KYU");

                    PdtLists = PdtLists.Where(w => area.Contains(w.PDT_AREA_CODE))
                                       .ToList();
                }
            }

            var rData0 = PdtLists.Join(DB.PDT_2,
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
                                               a.PDT_DAYS_CODE,
                                               a.PDT_OPTIONS,
                                               a.PDT_ORDER_NO,
                                               a.PDT_COMBINE_HTL,
                                               b.PDT_IMG
                                           });

            var trafAflLstae = chkDateLists.Join(traffData,
                                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      a.PRC_SEQ,
                                                      a.TRF_SEQ,
                                                  });

            var minPriceLists = trafAflLstae.Join(DB.PRC_0,
                                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      b.PRC_Adult,
                                                      a.TRF_SEQ,
                                                  })
                                                  .GroupBy(g => new
                                                  {
                                                      g.CORP_CODE,
                                                      g.PDT_TYPE_CODE,
                                                      g.PDT_IST_EMP_NO,
                                                      g.PDT_YY,
                                                      g.PDT_SEQ
                                                  })
                                                  .Select(s => new
                                                  {
                                                      s.Key.CORP_CODE,
                                                      s.Key.PDT_TYPE_CODE,
                                                      s.Key.PDT_IST_EMP_NO,
                                                      s.Key.PDT_YY,
                                                      s.Key.PDT_SEQ,
                                                      minPrice = s.Min(min => min.PRC_Adult),
                                                      TRF_SEQ = s.Min(min => min.TRF_SEQ)
                                                  });

            var secondLists = rData0.Join(minPriceLists,
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
                                             a.PDT_IMG,
                                             minPrice = a.PDT_COMBINE_HTL == "Y" ? Convert.ToInt32(getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, "2018-12-31", Convert.ToInt32(a.PDT_DAYS_CODE.ToString().Substring(0, 2)))) : b.minPrice,
                                             a.PDT_COMBINE_HTL,
                                             a.PDT_DAYS_CODE,
                                             a.PDT_OPTIONS,
                                             a.PDT_ORDER_NO,
                                             b.TRF_SEQ,
                                             DATE = "",
                                         }).ToList();

            var dateLists = DB.PDT_1.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                    .Select(s => new
                                    {
                                        s.Key.CORP_CODE,
                                        s.Key.PDT_TYPE_CODE,
                                        s.Key.PDT_IST_EMP_NO,
                                        s.Key.PDT_YY,
                                        s.Key.PDT_SEQ,
                                        Date = s.Min(min => min.PDT_DATE).Substring(0, 7) + " ~ " + s.Max(max => max.PDT_DATE).Substring(0, 7)
                                    });

            secondLists = secondLists.Join(dateLists,
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
                                              a.PDT_IMG,
                                              a.minPrice,
                                              a.PDT_COMBINE_HTL,
                                              a.PDT_DAYS_CODE,
                                              a.PDT_OPTIONS,
                                              a.PDT_ORDER_NO,
                                              a.TRF_SEQ,
                                              DATE = b.Date,
                                          }).ToList();

            var rData00 = secondLists.Join(traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")),
                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                      (a, b) => new
                                      {
                                          a.CORP_CODE,
                                          a.PDT_TYPE_CODE,
                                          a.PDT_IST_EMP_NO,
                                          a.PDT_YY,
                                          a.PDT_SEQ,
                                          a.PDT_TITLE,
                                          a.PDT_CONTENT,
                                          a.PDT_DAYS_CODE,
                                          a.PDT_OPTIONS,

                                          TRF_TITLE = getNNNNNNNAsjdkl(b.TRF_TITLE.Trim()),//b.TRF_TITLE.Trim(),
                                          TRF_SAREA = b.TRF_SAREA.Trim(),
                                          b.TRF_STIME,
                                          b.TRF_EAREA,
                                          b.TRF_ETIME,

                                          a.PDT_IMG,
                                          a.minPrice,
                                          a.DATE,
                                          a.PDT_ORDER_NO,
                                      })
                                      .OrderBy(o => o.PDT_ORDER_NO)
                                      .ThenBy(t => Convert.ToInt32(t.minPrice));

            var trfDatas = rData00.GroupBy(g => new { g.TRF_TITLE })
                                  .Select(s => new
                                  {
                                      s.Key.TRF_TITLE,
                                  });

            var sCityDatas = rData00.GroupBy(g => new { g.TRF_SAREA })
                                    .Select(s => new
                                    {
                                        s.Key.TRF_SAREA,
                                    });

            ViewBag.trfDatas = trfDatas;
            ViewBag.sCityDatas = sCityDatas;

            var user = Session["user"] as User;

            ViewBag.chat = "";
            ViewBag.nickName = "";

            if (user != null)
            {
                string RoomNumber = getChatRoomNum(user.Login).Data.ToString();
                try
                {
                    var chatDB = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(RoomNumber))
                                                .Select(s => new
                                                {
                                                    chkUser = s.CHAT_User_YY,
                                                    messages = s.CHAT_Message1,
                                                    date = s.CHAT_Message_Date + " " + s.CHAT_Message_Time
                                                });

                    string nickName = DB.CHAT_Room.Where(w => w.CHAT_Room_ID.ToString().Equals(RoomNumber))
                                                  .Single().CHAT_Name.ToString();

                    ViewBag.chat = chatDB;
                    ViewBag.nickName = nickName;
                }
                catch
                {
                    ViewBag.chat = "";
                    ViewBag.nickName = "";
                }
            }

            return View(rData00);
        }
        // GET: Products/Views
        public ActionResult Views(string C, string K)
        {
            try
            {
                var priKeys = getCode(C, K);

                string pdttype = priKeys[1].ToString();
                string pdtyy = priKeys[3].ToString();

                int pdtemp = Convert.ToInt32(priKeys[2].ToString());
                short pdtseq = Convert.ToInt16(priKeys[4].ToString());

                var defaultLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                    //.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                           .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                           .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                           .Where(w => w.PDT_YY.Equals(pdtyy))
                                           .Where(w => w.PDT_SEQ == pdtseq)
                                           .Select(s => new
                                           {
                                               //Code = code,//s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                               s.CORP_CODE,
                                               s.PDT_TYPE_CODE,
                                               s.PDT_IST_EMP_NO,
                                               s.PDT_YY,
                                               s.PDT_SEQ,
                                               Title = s.PDT_TITLE,
                                               Content = s.PDT_CONTENT,
                                               Option = s.PDT_OPTIONS,
                                               key = s.PDT_IST_EMP_NO,
                                               emp = s.PDT_EMP_NO,
                                               area = s.PDT_AREA_CODE,
                                               city = s.PDT_TOUR_CITY,
                                               minCnt = s.PDT_MIN_COUNT,
                                               s.PDT_DAYS_CODE,
                                               s.PDT_COMBINE_HTL,
                                           });

                var pdt_ImgLists = defaultLists.ToList()
                                               .Join(DB.PDT_2,
                                                    a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                                    b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                    (a, b) => new
                                                    {
                                                        a.CORP_CODE,
                                                        a.PDT_TYPE_CODE,
                                                        a.PDT_IST_EMP_NO,
                                                        a.PDT_YY,
                                                        a.PDT_SEQ,
                                                        a.Title,
                                                        a.Content,
                                                        a.Option,
                                                        b.PDT_IMG,
                                                        b.PDT_SCH0,
                                                        b.PDT_SCH1,
                                                        b.PDT_SCH2,
                                                        b.PDT_SCH3,
                                                        b.PDT_SCH4,
                                                        b.PDT_SCH5,
                                                        b.PDT_SCH6,
                                                        b.PDT_SCH7,
                                                        a.key,
                                                        a.area,
                                                        a.city,
                                                        a.minCnt,
                                                        a.PDT_DAYS_CODE,
                                                        a.PDT_COMBINE_HTL,
                                                    });

                var pdt_SchLists = pdt_ImgLists.ToList()
                                               .Join(DB.PDT_3,
                                                    a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                                    b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                    (a, b) => new
                                                    {
                                                        a.CORP_CODE,
                                                        a.PDT_TYPE_CODE,
                                                        a.PDT_IST_EMP_NO,
                                                        a.PDT_YY,
                                                        a.PDT_SEQ,
                                                        a.Title,
                                                        a.Content,
                                                        a.Option,
                                                        a.PDT_IMG,
                                                        a.PDT_SCH0,
                                                        a.PDT_SCH1,
                                                        a.PDT_SCH2,
                                                        a.PDT_SCH3,
                                                        a.PDT_SCH4,
                                                        a.PDT_SCH5,
                                                        a.PDT_SCH6,
                                                        a.PDT_SCH7,
                                                        a.key,
                                                        b.SCH_DAY,
                                                        b.SCH_CONT,
                                                        a.area,
                                                        a.city,
                                                        a.minCnt,
                                                        a.PDT_DAYS_CODE,
                                                        a.PDT_COMBINE_HTL,
                                                    });

                var minPriceLists = DB.PRC_0.ToList()
                                            .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                            .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                            .Where(w => w.PDT_YY.Equals(pdtyy))
                                            .Where(w => w.PDT_SEQ == pdtseq)
                    //.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(C))
                                            .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                            .Select(s => new
                                            {
                                                s.Key.CORP_CODE,
                                                s.Key.PDT_TYPE_CODE,
                                                s.Key.PDT_IST_EMP_NO,
                                                s.Key.PDT_YY,
                                                s.Key.PDT_SEQ,
                                                Currency = s.Min(min => min.PRC_Currency_CODE),
                                                Price = s.Min(min => min.PRC_Adult + min.PRC_Profit),
                                            });

                var PdtLists = pdt_SchLists.Join(minPriceLists,
                                                a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                (a, b) => new
                                                {
                                                    Code = C,
                                                    a.Title,
                                                    a.Content,
                                                    a.Option,
                                                    a.PDT_IMG,
                                                    a.PDT_SCH0,
                                                    a.PDT_SCH1,
                                                    a.PDT_SCH2,
                                                    a.PDT_SCH3,
                                                    a.PDT_SCH4,
                                                    a.PDT_SCH5,
                                                    a.PDT_SCH6,
                                                    a.PDT_SCH7,
                                                    b.Currency,
                                                    b.Price,
                                                    a.key,
                                                    a.SCH_DAY,
                                                    a.SCH_CONT,
                                                    a.area,
                                                    a.city,
                                                    a.minCnt,
                                                    a.PDT_DAYS_CODE,
                                                    a.PDT_COMBINE_HTL,
                                                });

                var chkPdtHtl = DB.PDT_4.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq);

                if (chkPdtHtl.Any())
                {
                    ViewBag.isHtl = true;
                }
                else
                {
                    ViewBag.isHtl = false;
                }


                DateTime now = DateTime.Now;

                // 날짜 몇일 뒤부터?
                now = now.AddDays(2);

                var getDateData = DB.PDT_1.ToList()
                                          .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                          .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                          .Where(w => w.PDT_YY.Equals(pdtyy))
                                          .Where(w => w.PDT_SEQ == pdtseq)
                                          .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                          .GroupBy(g => new { g.PDT_DATE })
                                          .Select(s => new
                                          {
                                              date = s.Key.PDT_DATE,
                                          });

                ViewBag.Date = Json(getDateData);

                var empData = DB.EMP_0.Join(defaultLists,
                                           a => a.EMP_NO,
                                           b => b.emp,
                                           (a, b) => new
                                           {
                                               no = a.EMP_NO,
                                               tel = a.EMP_TEL1 + a.EMP_TEL2 + a.EMP_TEL3,
                                               mail = a.EMP_EMAIL,
                                               name = a.EMP_NAME,
                                               txt = a.EMP_INTRO,
                                           });

                ViewBag.emp = empData;

                var imgLists = DB.PDT_5.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                       .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                       .Where(w => w.PDT_YY.Equals(pdtyy))
                                       .Where(w => w.PDT_SEQ == pdtseq)
                                       .Select(s => new
                                       {
                                           s.PDT_IMG_URL
                                       });

                ViewBag.imgLists = imgLists;

                var rvData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp))
                                          .Where(w => w.post_type.Equals("review"))
                                          .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                          .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                          .Where(w => w.PDT_YY.Equals(pdtyy))
                                          .Where(w => w.PDT_SEQ == pdtseq)
                                          .Where(w => w.DEL_FG == "N")
                                          .Select(s => new
                                          {
                                              s.post_content,
                                              s.post_options,
                                              date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                              s.name
                                          })
                                          .ToList();

                ViewBag.rvData = rvData;

                return View(PdtLists);
            }
            catch
            {
                return Content("<script>alert('잘못된 접근 경로 입니다.'); location.href='/';</script>");
            }
        }

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

        public JsonResult getChatRoomNum(string user, string nickName = "", string password = "")
        {
            user = user.Replace("ASPN15TRGT", "");

            string User_YY = "";
            int User_Seq = 0;

            if (user.IndexOf("GUEST") >= 0)
            {
                User_YY = "GUST";
                User_Seq = 0;
            }
            else
            {
                try
                {
                    User_YY = user.Substring(0, 4).ToString();
                    User_Seq = Convert.ToInt32(user.Substring(4).ToString());
                }
                catch
                {
                    User_YY = "ADMN";
                    User_Seq = 0;
                }
            }

            var chkChat = DB.CHAT_User_Room.Where(w => w.CHAT_User_YY.ToString().Equals(User_YY))
                                           .Where(w => w.CHAT_User_Seq == User_Seq);

            int resultNum = 0;

            if (chkChat.Any())
            {
                resultNum = chkChat.Single().CHAT_Room_ID;
            }
            else
            {
                int idx = 1;

                var cntRoomData = DB.CHAT_Room.Select(s => new { cnt = s.CHAT_Room_ID }).Max(m => m.cnt);

                try
                {
                    idx = cntRoomData + 1;
                }
                catch
                {
                    idx = 1;
                }

                if (nickName == "")
                {
                    nickName = NDB.CU001.Where(w => w.CU_YY.ToString().Equals(User_YY))
                                        .Where(w => w.CU_SEQ.ToString().Equals(User_Seq.ToString()))
                                        .Single().CU_NM_KOR.ToString();
                }

                CHAT_Room RoomData = new CHAT_Room();

                RoomData.CHAT_Room_ID = idx;
                RoomData.CHAT_Title = "개설합니다.";
                RoomData.CHAT_Name = nickName;
                RoomData.CHAT_Password = password;

                DB.CHAT_Room.Add(RoomData);

                CHAT_User_Room userRoom = new CHAT_User_Room();

                userRoom.CHAT_Room_ID = idx;
                userRoom.CHAT_User_YY = User_YY;
                userRoom.CHAT_User_Seq = User_Seq;

                DB.CHAT_User_Room.Add(userRoom);

                CHAT_User_Room userRoom2 = new CHAT_User_Room();

                userRoom2.CHAT_Room_ID = idx;
                userRoom2.CHAT_User_YY = "ADMN";
                userRoom2.CHAT_User_Seq = 0;

                DB.CHAT_User_Room.Add(userRoom2);

                DB.SaveChanges();

                resultNum = idx;
            }

            return Json(resultNum);
        }

        public string getNNNNNNNAsjdkl(string trf)
        {
            if (trf.Contains("KE"))
            {
                return "대한항공";
            }
            else if (trf.Contains("BX"))
            {
                return "에어부산";
            }
            else if (trf.Contains("LJ"))
            {
                return "진에어";
            }
            else if (trf.Contains("JL"))
            {
                return "일본항공";
            }
            else if (trf.Contains("7C"))
            {
                return "제주항공";
            }
            else if (trf.Contains("OZ"))
            {
                return "아시아나";
            }
            else if (trf.Contains("SU"))
            {
                return "러시아 항공";
            }
            else if (trf.Contains("TW"))
            {
                return "티웨이";
            }
            else if (trf.Contains("PK"))
            {
                return "부관훼리";
            }
            else if (trf.Contains("CA"))
            {
                return "카멜리아";
            }
            else if (trf.Contains("BE"))
            {
                return "비틀";
            }
            else if (trf.Contains("PA"))
            {
                return "팬스타";
            }

            return trf;
        }

        public List<string> getAreaCode(string A)
        {
            List<string> area = new List<string>();

            switch (A)
            {
                case "FUK":
                    area.Add("KYU");
                    area.Add("FUK");
                    break;
                case "TYO":
                    area.Add("TOK");
                    area.Add("TYO");
                    break;
                case "CTS":
                    area.Add("HOK");
                    area.Add("CTS");
                    break;
                case "TSU":
                case "TSM":
                    area.Add("TSM");
                    area.Add("TSU");
                    break;
                case "OKI":
                case "OKA":
                    area.Add("OKA");
                    area.Add("OKI");
                    break;
                case "TWN":
                case "TPE":
                    area.Add("TWN");
                    area.Add("TPE");
                    area.Add("TPC");
                    area.Add("TPS");
                    break;
                default:
                    area.Add(A);
                    break;
            }

            return area;
        }

        public double exchangedToDouble(string currency, string money)
        {
            double exchanged_USD = 12,
                   exchanged_JPY = 10.5,
                   exchanged_KRW = 1,
                   returnMoney = 0;

            switch (currency)
            {
                case "JPY":
                    returnMoney = Convert.ToInt32(money) * exchanged_JPY;
                    break;
                case "KRW":
                    returnMoney = Convert.ToInt32(money) * exchanged_KRW;
                    break;
                case "USD":
                    returnMoney = Convert.ToInt32(money) * exchanged_USD;
                    break;
            }

            return returnMoney;
        }

        public double getHotelMinPrice(string Corp_Code, string Pdt_Type_Code, int Pdt_ist_emp_No, string Pdt_yy, int Pdt_Seq, string tDate, int inCnt)
        {
            double minPrice = 0;

            string toDay = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                SqlConnection dbconn = new SqlConnection("server=211.253.24.26;uid=NT_Sessions;pwd=sessions1234;database=Ncom");
                dbconn.Open();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = dbconn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_SearchotelPackPrice_Stat";

                cmd.Parameters.Add("@Corp_Code", Corp_Code);
                cmd.Parameters.Add("@Pdt_Type_Code", Pdt_Type_Code);
                cmd.Parameters.Add("@Pdt_ist_emp_No", Pdt_ist_emp_No);
                cmd.Parameters.Add("@Pdt_yy", Pdt_yy);
                cmd.Parameters.Add("@Pdt_Seq", Pdt_Seq);
                cmd.Parameters.Add("@ToDate", tDate);
                cmd.Parameters.Add("@Term", inCnt);
                cmd.Parameters.Add("@ExJPY", 10.5);
                cmd.Parameters.Add("@ExUSD", 12);

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var tempData = rd.GetString(0);
                        var tempPrice = exchangedToDouble(rd.GetString(2).ToString(), rd.GetInt32(1).ToString());

                        if (minPrice == 0 || minPrice > tempPrice)
                        {
                            minPrice = tempPrice;
                        }
                    }
                }
            }
            catch (Exception e)
            {

            }

            return minPrice;
        }

        public int getMinPrice(int p1 = 0, int p2 = 0, int p3 = 0, int p4 = 0)
        {

            //[1]Init
            int min = Int32.MaxValue;

            //[2] Input
            int[] data = { p1, p2, p3, p4 };

            //[3] Process : MAX
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                    continue;

                if (min > data[i])
                {
                    min = data[i];
                }
            }

            //[4] Output        
            return min;
        }

        public int getInCnt(string code, string key, int hotelSeq)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var hotelList = DB.PDT_4.ToList()
                                    .Where(w => w.CORP_CODE.Equals(corp))
                                    .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                    .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                    .Where(w => w.PDT_YY.Equals(pdtyy))
                                    .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                    .Where(w => w.PDT_HTL_SEQ == hotelSeq)
                                    .Where(w => w.PDT_CANRES != null)
                                    .Where(w => w.PDT_CANRES.Equals("Y"))
                                    .Count();





            return hotelList + 1;
        }

        public JsonResult JSON_getChkin(string code, string key, string trfCD)
        {
            var priKeys = getCode(code, key);

            string pdttype = priKeys[1].ToString();
            string pdtyy = priKeys[3].ToString();

            int pdtemp = Convert.ToInt32(priKeys[2].ToString());
            short pdtseq = Convert.ToInt16(priKeys[4].ToString());

            var trfData = DB.TRF_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                  .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdtseq)
                                  .Where(w => w.TRF_SEQ.ToString().Equals(trfCD))
                                  .Where(w => w.TRF_SUB_SEQ == 1)
                                  .SingleOrDefault()
                                  .TRF_TITLE
                                  .ToString();

            int returnValue = 0;

            List<string> chkTrf = new List<string>();

            chkTrf.Add("PK");
            chkTrf.Add("CM");
            chkTrf.Add("카멜리아");
            chkTrf.Add("부관훼리");

            if (!string.IsNullOrEmpty(trfData) && chkTrf.Any(trfData.Contains))
            {
                returnValue = 1;
            }

            return Json(returnValue);
        }

        public JsonResult getPriceData(string code, string key, string prcCode, string trfCode, string date, string inHotels = "")
        {
            var priKeys = getCode(code, key);

            string pdttype = priKeys[1].ToString();
            string pdtyy = priKeys[3].ToString();

            int pdtemp = Convert.ToInt32(priKeys[2].ToString());
            short pdtseq = Convert.ToInt16(priKeys[4].ToString());

            var Data2 = DB.PRC_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                .Where(w => w.PDT_YY.Equals(pdtyy))
                                .Where(w => w.PDT_SEQ == pdtseq)
                                .Where(w => w.PRC_SEQ.ToString().Equals(prcCode));

            var returnData = Data2.ToList()
                                  .Select(s => new
                                  {
                                      DATE = date,
                                      APrice = s.PRC_Adult == 0 ? 0 : exchangedToDouble(s.PRC_Currency_CODE, (s.PRC_Adult + s.PRC_Profit).ToString()),
                                      CPrice = s.PRC_Child == 0 ? 0 : exchangedToDouble(s.PRC_Currency_CODE, (s.PRC_Child + s.PRC_Profit).ToString()),
                                      BPrice = s.PRC_Baby == 0 ? 0 : exchangedToDouble(s.PRC_Currency_CODE, (s.PRC_Baby + s.PRC_Profit).ToString()),
                                  })
                                  .ToList();

            if (!string.IsNullOrEmpty(inHotels))
            {
                string[] hotels = inHotels.Split(',');
                int price = 0;

                int chkIn = Convert.ToInt32(JSON_getChkin(code, key, trfCode).Data.ToString());

                for (int i = 0; i < hotels.Length; i++)
                {
                    if (string.IsNullOrEmpty(hotels[i].Trim()))
                        continue;

                    var tempData = DB.HTL_2.ToList()
                                           .Where(w => w.HTL_DATE.Equals(DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((chkIn) + i).ToString("yyyy-MM-dd")))
                                           .Where(w => w.HTL_SEQ.ToString().Equals(hotels[i].Trim().ToString()));

                    string tempPrice = tempData.Join(DB.HTL_1,
                                                     a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                     b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                                    (a, b) => new
                                                    {
                                                        b.HTL_SEQ,
                                                        b.HTL_PRC_SEQ,
                                                        b.HTL_Currency_CODE,
                                                        b.HTL_PRICE,
                                                        b.HTL_PRICE2,
                                                        b.HTL_PRICE3,
                                                        b.HTL_PRICE4,
                                                        b.HTL_PROFIT,
                                                        price = exchangedToDouble(b.HTL_Currency_CODE, (b.HTL_PRICE2 + b.HTL_PROFIT).ToString()),
                                                    })
                                                    .OrderBy(o => o.price)
                                                    .Take(1)
                                                    .SingleOrDefault()
                                                    .price
                                                    .ToString();

                    price += Convert.ToInt32(tempPrice);
                }

                returnData = returnData.Select(s => new
                {
                    s.DATE,
                    APrice = Math.Ceiling(Convert.ToDouble(s.APrice + price) * 0.01) * 100,
                    CPrice = Math.Ceiling(Convert.ToDouble(s.CPrice) * 0.01) * 100,// + price,
                    BPrice = Math.Ceiling(Convert.ToDouble(s.BPrice) * 0.01) * 100,// + price,
                })
                .ToList();
            }

            return Json(returnData);
        }

        public JsonResult JSON_getMinPrice(string code, string key, string date)//, string combineHotel = "Y")
        {
            var priKeys = getCode(code, key);

            string pdttype = priKeys[1].ToString();
            string pdtyy = priKeys[3].ToString();

            int pdtemp = Convert.ToInt32(priKeys[2].ToString());
            short pdtseq = Convert.ToInt16(priKeys[4].ToString());

            var defaultLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                       .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                       .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                       .Where(w => w.PDT_YY.Equals(pdtyy))
                                       .Where(w => w.PDT_SEQ == pdtseq)
                                       .Select(s => new
                                       {
                                           s.PDT_COMBINE_HTL,
                                       });

            string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

            var thisMonthPriceData5 = DB.PDT_1.ToList()
                                              .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                              .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                              .Where(w => w.PDT_YY.Equals(pdtyy))
                                              .Where(w => w.PDT_SEQ == pdtseq)
                                              .Where(w => w.PDT_DATE.Equals(date));

            var thisMonthPriceData = thisMonthPriceData5.Join(DB.PRC_0,
                                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      month = a.PDT_DATE.Substring(5, 2),
                                                      days = a.PDT_DATE.Substring(8),
                                                      price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      a.PDT_DATE,
                                                      traff = a.TRF_SEQ.ToString(),
                                                  });

            if (!string.IsNullOrEmpty(chkCombinHotels) && chkCombinHotels.Equals("Y"))
            {
                var hotelData = DB.PDT_4.ToList()
                                        .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq)
                                        .OrderBy(o => o.PDT_IN);

                var trfData = DB.TRF_0.ToList()
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                      .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == pdtseq)
                                      .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"));

                thisMonthPriceData = thisMonthPriceData.Join(trfData,
                                                             a => a.traff.ToString(),
                                                             b => b.TRF_SEQ.ToString(),
                                                            (a, b) => new
                                                            {
                                                                a.month,
                                                                a.days,
                                                                a.price,

                                                                a.PDT_TYPE_CODE,
                                                                a.PDT_IST_EMP_NO,
                                                                a.PDT_YY,
                                                                a.PDT_SEQ,
                                                                PDT_DATE = b.TRF_TYPE == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                                                                traff = b.TRF_TYPE,
                                                            })
                                                            .ToList();

                foreach (object hotel in hotelData)
                {
                    string inDays = hotel.GetType().GetProperties()[5].GetValue(hotel, null).ToString();
                    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                    var hotelPriceData = DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
                                                 .Select(s => new
                                                 {
                                                     s.HTL_SEQ,
                                                     s.HTL_PRC_SEQ,
                                                     s.HTL_Currency_CODE,
                                                     HTL_PRICE = string.IsNullOrEmpty(s.HTL_PRICE.ToString()) ? "0" : s.HTL_PRICE.ToString(),
                                                     HTL_PRICE2 = string.IsNullOrEmpty(s.HTL_PRICE2.ToString()) ? "0" : s.HTL_PRICE2.ToString(),
                                                     HTL_PRICE3 = string.IsNullOrEmpty(s.HTL_PRICE3.ToString()) ? "0" : s.HTL_PRICE3.ToString(),
                                                     HTL_PRICE4 = string.IsNullOrEmpty(s.HTL_PRICE4.ToString()) ? "0" : s.HTL_PRICE4.ToString(),
                                                     s.HTL_PROFIT,
                                                 });

                    var hotelPrice = hotelDateData.ToList()
                                                  .Join(hotelPriceData,
                                                        a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                        b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                                        (a, b) => new
                                                        {
                                                            a.HTL_DATE,
                                                            b.HTL_Currency_CODE,
                                                            //minPrice = getMinPrice(Convert.ToInt32(b.HTL_PRICE), Convert.ToInt32(b.HTL_PRICE2), Convert.ToInt32(b.HTL_PRICE3), Convert.ToInt32(b.HTL_PRICE4)) + b.HTL_PROFIT,

                                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                                                        });

                    thisMonthPriceData = thisMonthPriceData.Join(hotelPrice,
                                                                 a => a.PDT_DATE,
                                                                 b => b.HTL_DATE,
                                                                (a, b) => new
                                                                {
                                                                    a.month,
                                                                    a.days,
                                                                    price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                                                                    a.PDT_TYPE_CODE,
                                                                    a.PDT_IST_EMP_NO,
                                                                    a.PDT_YY,
                                                                    a.PDT_SEQ,
                                                                    PDT_DATE = DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                                                                    a.traff,
                                                                });
                }
            }

            var TMPD_MinData = thisMonthPriceData.GroupBy(g => new { g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                                 .Select(s => new
                                                 {
                                                     minPrice = s.Min(min => min.price)
                                                 })
                                                 .ToList();

            thisMonthPriceData = thisMonthPriceData.Join(TMPD_MinData,
                                                         a => a.price,
                                                         b => b.minPrice,
                                                        (a, b) => new
                                                        {
                                                            a.month,
                                                            a.days,
                                                            a.price,

                                                            a.PDT_TYPE_CODE,
                                                            a.PDT_IST_EMP_NO,
                                                            a.PDT_YY,
                                                            a.PDT_SEQ,
                                                            a.PDT_DATE,
                                                            a.traff,
                                                        })
                                                        .OrderBy(o => o.days)
                                                        .ToList();

            var returnData = thisMonthPriceData.Select(s => new
            {
                s.month,
                s.days,
                s.price,
            });

            return Json(returnData);
        }

        public JsonResult JSON_getPrc(string code, string key, string date, int trfCD)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var daysData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                   .Where(w => w.TRF_SEQ == trfCD)
                                   .Where(w => w.PDT_STATE_CODE != "03")
                                   .Where(w => w.PDT_DATE.Equals(date))
                                   .GroupBy(g => new { g.PRC_SEQ })
                                   .Select(s => new
                                   {
                                       code,
                                       PRC_SEQ = s.Key.PRC_SEQ
                                   });

            var prcData = DB.PRC_0.ToList()
                                  .Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                  .Join(daysData,
                                 a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + a.PRC_SEQ,
                                 b => b.code + b.PRC_SEQ,
                                 (a, b) => new
                                 {
                                     a.PRC_SEQ,
                                     a.PRC_EXP,
                                 });

            return Json(prcData);
        }

        public JsonResult JSON_getTrf(string code, string key, string date)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var daysData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                   .Where(w => w.PDT_STATE_CODE != "03")
                                   .Where(w => w.PDT_DATE.Equals(date))
                                   .GroupBy(g => new { g.TRF_SEQ })
                                   .Select(s => new
                                   {
                                       code,
                                       TRF_SEQ = s.Key.TRF_SEQ
                                   });

            var trfData = DB.TRF_0.ToList()
                                  .Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                  .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
                                  .Join(daysData,
                                 a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + a.TRF_SEQ,
                                 b => b.code + b.TRF_SEQ,
                                 (a, b) => new
                                 {
                                     a.TRF_SEQ,
                                     TRF_TITLE = (a.TRF_TITLE),
                                     a.TRF_STIME,
                                     a.TRF_ETIME,
                                 });

            return Json(trfData);
        }

        public JsonResult JSON_getTraffData(string code, string key)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var traffData = DB.TRF_0.ToList()
                                    .Where(w => w.CORP_CODE.Equals(corp))
                                    .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                    .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                    .Where(w => w.PDT_YY.Equals(pdtyy))
                                    .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                    .Select(s => new
                                    {
                                        s.TRF_SEQ,
                                        s.TRF_SUB_SEQ,
                                        s.TRF_TYPE,
                                        TRF_TITLE = (s.TRF_TITLE),
                                        s.TRF_SAREA,
                                        s.TRF_STIME,
                                        s.TRF_EAREA,
                                        s.TRF_ETIME,
                                        title = getNNNNNNNAsjdkl(s.TRF_TITLE)
                                    }); 

            return Json(traffData);
        }

        public JsonResult JSON_getHotels(string code, string key, string date)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var htlList = DB.PDT_4.ToList()
                                  .Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                  .Select(s => new
                                  {
                                      s.PDT_HTL_SEQ,
                                      s.PDT_IN,
                                      s.PDT_CANRES,
                                      s.CORP_CODE,
                                      s.PDT_TYPE_CODE,
                                      s.PDT_IST_EMP_NO,
                                      s.PDT_YY,
                                      s.PDT_SEQ,
                                      inCnt = getInCnt(code, key, s.PDT_HTL_SEQ),
                                  });

            var hotelState = DB.HTL_2.Where(w => w.HTL_DATE.Equals(date))
                                     .Where(w => w.HTL_STATE_CODE.Equals("01"))
                                     .GroupBy(g => new { g.CORP_CODE, g.HTL_SEQ });

            htlList = htlList.Join(hotelState,
                                     a => a.PDT_HTL_SEQ,
                                     b => b.Key.HTL_SEQ,
                                    (a, b) => new
                                    {
                                        a.PDT_HTL_SEQ,
                                        a.PDT_IN,
                                        a.PDT_CANRES,
                                        a.CORP_CODE,
                                        a.PDT_TYPE_CODE,
                                        a.PDT_IST_EMP_NO,
                                        a.PDT_YY,
                                        a.PDT_SEQ,
                                        a.inCnt,
                                    });

            var HotelLists = htlList.Join(DB.HTL_0.Where(w => w.HTL_PROC_CODE.Equals("02")),
                                            a => a.PDT_HTL_SEQ,
                                            b => b.HTL_SEQ,
                                           (a, b) => new
                                           {
                                               a.PDT_IN,
                                               a.PDT_HTL_SEQ,
                                               b.HTL_NAME,
                                               a.inCnt,
                                           });

            return Json(HotelLists);
        }

        public JsonResult JSON_getDays(string code, string key, string trfCode = "", string prcCode = "")
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var dateData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq));
            //.Where(w => w.PDT_STATE_CODE != "03");

            if (!string.IsNullOrEmpty(trfCode))
            {
                dateData = dateData.Where(w => w.TRF_SEQ == Convert.ToInt32(trfCode));
            }

            if (!string.IsNullOrEmpty(prcCode))
            {
                dateData = dateData.Where(w => w.PRC_SEQ == Convert.ToInt32(prcCode));
            }

            var dates = dateData.Select(s => new
            {
                s.TRF_SEQ,
                s.PRC_SEQ,
                day = s.PDT_DATE,
                state = s.PDT_STATE_CODE,
                s.PDT_TYPE_CODE,
                s.PDT_IST_EMP_NO,
                s.PDT_YY,
                s.PDT_SEQ,
            });

            DateTime now = DateTime.Now;

            var results = dates.ToList().Where(w => DateTime.ParseExact(w.day, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

            if (string.IsNullOrEmpty(trfCode) || string.IsNullOrEmpty(prcCode))
            {
                var prcdatas = DB.PRC_0.ToList()
                                       .Where(w => w.CORP_CODE.Equals(corp))
                                       .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                       .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                       .Where(w => w.PDT_YY.Equals(pdtyy))
                                       .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq));

                var trfdatas = DB.TRF_0.ToList()
                                       .Where(w => w.CORP_CODE.Equals(corp))
                                       .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                       .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                       .Where(w => w.PDT_YY.Equals(pdtyy))
                                       .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq));

                var testDays = results.Join(prcdatas,
                                           a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + a.PRC_SEQ,
                                           b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + b.PRC_SEQ,
                                           (a, b) => new
                                           {
                                               a.TRF_SEQ,
                                               a.PRC_SEQ,
                                               a.day,
                                               a.state,
                                               a.PDT_TYPE_CODE,
                                               a.PDT_IST_EMP_NO,
                                               a.PDT_YY,
                                               a.PDT_SEQ
                                           });

                testDays = testDays.Join(trfdatas,
                                        a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + a.TRF_SEQ,
                                        b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + b.TRF_SEQ,
                                        (a, b) => new
                                        {
                                            a.TRF_SEQ,
                                            a.PRC_SEQ,
                                            a.day,
                                            a.state,
                                            a.PDT_TYPE_CODE,
                                            a.PDT_IST_EMP_NO,
                                            a.PDT_YY,
                                            a.PDT_SEQ
                                        })
                                        .OrderBy(o => o.day);

                var resultDays = testDays.ToList()
                                         .GroupBy(g => new { g.day })
                                         .Select(s => new
                                         {
                                             day = s.Key.day,
                                             state = s.Max(max => max.state),
                                         });

                return Json(resultDays);
            }

            return Json(results);
        }

        public JsonResult JSON_udtReview(string pCode, string pKey, string cont, string type, string title, string star = "")
        {
            var priKey = getCode(pCode, pKey);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string pdtyy = priKey[3];

            int emp = Convert.ToInt32(priKey[2].ToString());

            short pdtseq = Convert.ToInt16(priKey[4].ToString());

            //string type = "review";
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            var user = Session["user"] as User;

            var memData = from mf in NDB.CU001
                          where mf.ASHOP_SITE_CD + mf.CU_YY + mf.CU_SEQ == user.Login
                          select new
                          {
                              CU_YY = mf.CU_YY,
                              CU_SEQ = mf.CU_SEQ,
                              phone = mf.HANDPHONE1 + mf.HANDPHONE2 + mf.HANDPHONE3,
                              id = mf.CU_ID,
                              name = mf.CU_NM_KOR

                          };

            var pdtData = DB.PDT_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdtseq) // == pdtseq)
                                  .Select(s => new
                                  {
                                      s.PDT_TITLE
                                  });

            string pdtTitle = pdtData.Single().PDT_TITLE.ToString();

            if (!string.IsNullOrEmpty(title))
            {
                pdtTitle = title;
            }

            int postID = 1;

            var boardData = from bf in DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp))
                            where bf.post_type == type
                            orderby bf.post_ID descending
                            select new
                            {
                                postid = bf.post_ID
                            };

            try { postID = int.Parse(boardData.Take(1).Single().postid.ToString()) + 1; }
            catch { }

            NT_Board_2 board = new NT_Board_2();

            board.CORP_CODE = corp;
            board.post_ID = postID;
            board.CU_YY = memData.Single().CU_YY;
            board.CU_SEQ = memData.Single().CU_SEQ;
            board.PDT_TYPE_CODE = pdt_type;
            board.PDT_IST_EMP_NO = emp;
            board.PDT_YY = pdtyy;
            board.PDT_SEQ = pdtseq;
            board.post_title = pdtTitle;
            board.post_content = cont;
            board.post_type = type;
            board.post_status = "N";
            board.DEL_FG = "N";
            board.post_options = star;

            board.ist_date = date;
            board.ist_time = time;

            board.udt_date = date;
            board.udt_time = time;

            board.name = memData.Single().name;
            board.password = null;

            board.post_category = null;

            switch (type)
            {
                case "qna":
                    break;
            }

            DB.NT_Board_2.Add(board);
            DB.SaveChanges();

            string rtnTxt = "";

            switch (type)
            {
                case "review":
                    rtnTxt = "후기 등록을 완료했습니다.";
                    break;
                case "qna":
                    rtnTxt = "상품 문의를 등록 했습니다.";
                    break;
            }

            return Json(rtnTxt);
        }

        public JsonResult JSON_getPdtLists(string N, string A, string T, string trf)
        {
            string traff = "",
                   tourType = "";

            string nowDate = DateTime.Now.ToString("yyyy-MM-dd");

            switch (T)
            {
                case "A":
                    traff = "AIR";
                    tourType = "FT";
                    break;
                case "S":
                    traff = "SHP";
                    tourType = "FT";
                    break;
                case "L":
                    tourType = "LT";
                    break;
                case "P":
                    tourType = "PT";
                    break;
                case "F":
                    tourType = "FT";
                    break;
            }

            var PdtLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                   .Select(s => new
                                   {
                                       s.CORP_CODE,
                                       s.PDT_TYPE_CODE,
                                       s.PDT_IST_EMP_NO,
                                       s.PDT_YY,
                                       s.PDT_SEQ,
                                       s.PDT_TITLE,
                                       s.PDT_CONTENT,
                                       s.PDT_DAYS_CODE,
                                       s.PDT_OPTIONS,
                                       s.PDT_ORDER_NO,
                                       s.PDT_COMBINE_HTL,
                                       s.PDT_NATION_CODE,
                                       s.PDT_AREA_CODE,
                                       s.PDT_UDT_DATE,
                                   })
                                   .ToList();

            if (A == "ETC")
            {
                List<string> area = new List<string>();

                area.Add("TOK");
                area.Add("TYO");
                area.Add("OSA");
                area.Add("KYU");
                area.Add("FUK");
                area.Add("OKA");
                area.Add("OKI");
                area.Add("HOK");
                area.Add("CTS");
                area.Add("TSM");
                area.Add("TSU");

                PdtLists = PdtLists.Where(w => w.PDT_NATION_CODE.Equals("JP"))
                                   .Where(w => !area.Contains(w.PDT_AREA_CODE))
                                   .OrderByDescending(o => o.PDT_UDT_DATE)
                                   .ToList();
            }
            else
            {
                List<string> area = getAreaCode(A);

                PdtLists = PdtLists.Where(w => area.Contains(w.PDT_AREA_CODE))
                                   .ToList();
            }

            PdtLists = PdtLists.Where(w => w.PDT_TYPE_CODE.Equals(tourType))
                               .ToList();

            if (A == "KYU" || A == "FUK")
            {
                List<string> area = getAreaCode(A);

                var vpLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                      .Where(w => w.PDT_TYPE_CODE.Equals("VP"))
                                      .Where(w => area.Contains(w.PDT_AREA_CODE))
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            s.PDT_TITLE,
                                            s.PDT_CONTENT,
                                            s.PDT_DAYS_CODE,
                                            s.PDT_OPTIONS,
                                            s.PDT_ORDER_NO,
                                            s.PDT_COMBINE_HTL,
                                            s.PDT_NATION_CODE,
                                            s.PDT_AREA_CODE,
                                            s.PDT_UDT_DATE,
                                        }).ToList();

                PdtLists = PdtLists.Union(vpLists).ToList();
            }

            var chkImg = DB.PDT_5.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                .Select(s => new
                                {
                                    key = (s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO + s.Key.PDT_YY + s.Key.PDT_SEQ).ToString(),
                                });

            List<string> imgLists = new List<string>();

            foreach (var item in chkImg)
            {
                imgLists.Add(item.key);
            }

            PdtLists = PdtLists.Where(w => imgLists.Contains(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                                                   .ToList();

            var rData0 = PdtLists.Join(DB.PDT_2,
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
                                               a.PDT_DAYS_CODE,
                                               a.PDT_OPTIONS,
                                               a.PDT_ORDER_NO,
                                               a.PDT_COMBINE_HTL,
                                               b.PDT_IMG
                                           });
            DateTime now = DateTime.Now;

            var chkDateLists = DB.PDT_1.ToList()
                                       .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                       .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.PRC_SEQ, g.TRF_SEQ })
                                       .Select(s => new
                                       {
                                           s.Key.CORP_CODE,
                                           s.Key.PDT_TYPE_CODE,
                                           s.Key.PDT_IST_EMP_NO,
                                           s.Key.PDT_YY,
                                           s.Key.PDT_SEQ,
                                           s.Key.PRC_SEQ,
                                           s.Key.TRF_SEQ,
                                       });

            var minPriceLists = chkDateLists.Join(DB.PRC_0,
                                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      b.PRC_Adult,
                                                      a.TRF_SEQ,
                                                  })
                                                  .GroupBy(g => new
                                                  {
                                                      g.CORP_CODE,
                                                      g.PDT_TYPE_CODE,
                                                      g.PDT_IST_EMP_NO,
                                                      g.PDT_YY,
                                                      g.PDT_SEQ
                                                  })
                                                  .Select(s => new
                                                  {
                                                      s.Key.CORP_CODE,
                                                      s.Key.PDT_TYPE_CODE,
                                                      s.Key.PDT_IST_EMP_NO,
                                                      s.Key.PDT_YY,
                                                      s.Key.PDT_SEQ,
                                                      minPrice = s.Min(min => min.PRC_Adult),
                                                      TRF_SEQ = s.Min(min => min.TRF_SEQ)
                                                  });

            var secondLists = rData0.Join(minPriceLists,
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
                                             a.PDT_IMG,
                                             minPrice = a.PDT_COMBINE_HTL == "Y" ? Convert.ToInt32(getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, "2018-12-31", Convert.ToInt32(a.PDT_DAYS_CODE.ToString().Substring(0, 2)))) : b.minPrice,
                                             a.PDT_COMBINE_HTL,
                                             a.PDT_DAYS_CODE,
                                             a.PDT_OPTIONS,
                                             a.PDT_ORDER_NO,
                                             b.TRF_SEQ,
                                             DATE = "",
                                         }).ToList();

            var dateLists = DB.PDT_1.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                    .Select(s => new
                                    {
                                        s.Key.CORP_CODE,
                                        s.Key.PDT_TYPE_CODE,
                                        s.Key.PDT_IST_EMP_NO,
                                        s.Key.PDT_YY,
                                        s.Key.PDT_SEQ,
                                        Date = s.Min(min => min.PDT_DATE).Substring(0, 7) + " ~ " + s.Max(max => max.PDT_DATE).Substring(0, 7)
                                    });

            secondLists = secondLists.Join(dateLists,
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
                                              a.PDT_IMG,
                                              a.minPrice,
                                              a.PDT_COMBINE_HTL,
                                              a.PDT_DAYS_CODE,
                                              a.PDT_OPTIONS,
                                              a.PDT_ORDER_NO,
                                              a.TRF_SEQ,
                                              DATE = b.Date,
                                          }).ToList();

            var traffData = DB.TRF_0.ToList();

            if (traff.Length == 3)
            {
                traffData = DB.TRF_0.Where(w => w.TRF_TYPE.Equals(traff)).ToList();
            }

            List<string> chkTrf = new List<string>();

            switch (trf)
            {
                case "KE":
                    chkTrf.Add("KE");
                    chkTrf.Add("대한항공");
                    chkTrf.Add("대한 항공");
                    break;
                case "BX":
                    chkTrf.Add("BX");
                    chkTrf.Add("에어부산");
                    chkTrf.Add("에어 부산");
                    break;
                case "LJ":
                    chkTrf.Add("LJ");
                    chkTrf.Add("진에어");
                    break;
                case "JL":
                    chkTrf.Add("JL");
                    chkTrf.Add("일본항공");
                    break;
                case "j7C":
                    chkTrf.Add("7C");
                    chkTrf.Add("제주항공");
                    chkTrf.Add("제주 항공");
                    break;
                case "PK":
                    chkTrf.Add("PK");
                    chkTrf.Add("부관훼리");
                    break;
                case "CA":
                    chkTrf.Add("CA");
                    chkTrf.Add("카멜리아");
                    break;
                case "BE":
                    chkTrf.Add("BE");
                    chkTrf.Add("비틀");
                    break;
                case "PA":
                    chkTrf.Add("PA");
                    chkTrf.Add("팬스타");
                    break;
                case "NN":
                    chkTrf.Add("NN");
                    chkTrf.Add("니나");
                    break;
                case "KB":
                    chkTrf.Add("KB");
                    chkTrf.Add("코비");
                    break;
                case "OF":
                    chkTrf.Add("OF");
                    chkTrf.Add("오션플라워");
                    break;
                case "air":
                    chkTrf.Add("KE");
                    chkTrf.Add("대한항공");
                    chkTrf.Add("대한 항공");
                    chkTrf.Add("BX");
                    chkTrf.Add("에어부산");
                    chkTrf.Add("에어 부산");
                    chkTrf.Add("LJ");
                    chkTrf.Add("진에어");
                    chkTrf.Add("JL");
                    chkTrf.Add("일본항공");
                    chkTrf.Add("7C");
                    chkTrf.Add("제주항공");
                    chkTrf.Add("제주 항공");
                    break;
                case "shp":
                    chkTrf.Add("PK");
                    chkTrf.Add("부관훼리");
                    chkTrf.Add("CA");
                    chkTrf.Add("카멜리아");
                    chkTrf.Add("BE");
                    chkTrf.Add("비틀");
                    chkTrf.Add("PA");
                    chkTrf.Add("팬스타");
                    chkTrf.Add("NN");
                    chkTrf.Add("니나");
                    chkTrf.Add("KB");
                    chkTrf.Add("코비");
                    chkTrf.Add("OF");
                    chkTrf.Add("오션플라워");
                    break;
            }

            if (chkTrf.Count != 0)
            {

                traffData = traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
                                     .Where(w => chkTrf.Any(w.TRF_TITLE.Contains))
                                     .ToList();
            }

            var rData00 = secondLists.Join(traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")),
                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                      (a, b) => new
                                      {
                                          a.CORP_CODE,
                                          a.PDT_TYPE_CODE,
                                          a.PDT_IST_EMP_NO,
                                          a.PDT_YY,
                                          a.PDT_SEQ,
                                          a.PDT_TITLE,
                                          a.PDT_CONTENT,
                                          a.PDT_DAYS_CODE,
                                          a.PDT_OPTIONS,

                                          TRF_TITLE = b.TRF_TITLE.Trim(),
                                          TRF_SAREA = b.TRF_SAREA.Trim(),
                                          b.TRF_STIME,
                                          b.TRF_EAREA,
                                          b.TRF_ETIME,

                                          a.PDT_IMG,
                                          a.minPrice,
                                          a.DATE,
                                          a.PDT_ORDER_NO,
                                      })
                                      .OrderBy(o => o.PDT_ORDER_NO)
                                      .ThenBy(t => Convert.ToInt32(t.minPrice));

            return Json(rData00);
        }
    }
}