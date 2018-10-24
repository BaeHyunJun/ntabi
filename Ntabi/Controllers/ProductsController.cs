using Ntabi.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
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

                                          TRF_TITLE = getNNNNNNNAsjdkl(b.TRF_TITLE.Trim()),
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



            //var firstLists = BestLists.Join(DB.PDT_2,
            //                                a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                                (a, b) => new
            //                                {
            //                                    a.CORP_CODE,
            //                                    a.PDT_TYPE_CODE,
            //                                    a.PDT_IST_EMP_NO,
            //                                    a.PDT_YY,
            //                                    a.PDT_SEQ,
            //                                    a.PDT_TITLE,
            //                                    a.PDT_CONTENT,
            //                                    a.PDT_COMBINE_HTL,
            //                                    inCnt = a.PDT_DAYS_CODE.Substring(0, 2),
            //                                    b.PDT_IMG,
            //                                }).ToList();

            //DateTime now = DateTime.Now;

            //var chkDateLists = DB.PDT_1.ToList().Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

            //var minPriceLists = chkDateLists.Join(DB.PRC_0,
            //                                      a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
            //                                      b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
            //                                      (a, b) => new
            //                                      {
            //                                          a.CORP_CODE,
            //                                          a.PDT_TYPE_CODE,
            //                                          a.PDT_IST_EMP_NO,
            //                                          a.PDT_YY,
            //                                          a.PDT_SEQ,
            //                                          b.PRC_Adult,
            //                                      })
            //                                      .GroupBy(g => new
            //                                      {
            //                                          g.CORP_CODE,
            //                                          g.PDT_TYPE_CODE,
            //                                          g.PDT_IST_EMP_NO,
            //                                          g.PDT_YY,
            //                                          g.PDT_SEQ
            //                                      })
            //                                      .Select(s => new
            //                                      {
            //                                          s.Key.CORP_CODE,
            //                                          s.Key.PDT_TYPE_CODE,
            //                                          s.Key.PDT_IST_EMP_NO,
            //                                          s.Key.PDT_YY,
            //                                          s.Key.PDT_SEQ,
            //                                          minPrice = s.Min(min => min.PRC_Adult)
            //                                      });

            //var secondLists = firstLists.Join(minPriceLists,
            //                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                                  (a, b) => new
            //                                  {
            //                                      a.CORP_CODE,
            //                                      a.PDT_TYPE_CODE,
            //                                      a.PDT_IST_EMP_NO,
            //                                      a.PDT_YY,
            //                                      a.PDT_SEQ,
            //                                      a.PDT_TITLE,
            //                                      a.PDT_CONTENT,
            //                                      a.PDT_IMG,
            //                                      minPrice = b.minPrice.ToString(),
            //                                      a.PDT_COMBINE_HTL,
            //                                      a.inCnt,
            //                                      //minPrice = a.PDT_COMBINE_HTL == "N" ? b.minPrice : b.minPrice + getHotelMinPrice("1, 2, 3, 4", "", "", Convert.ToInt32(a.inCnt)),
            //                                  });

            //string nowDate = DateTime.Now.ToString("yyyy-MM-dd");

            //var getHotelDatas = secondLists.Join(DB.PDT_4,
            //                                     a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
            //                                     b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                                    (a, b) => new
            //                                    {
            //                                        a.CORP_CODE,
            //                                        a.PDT_TYPE_CODE,
            //                                        a.PDT_IST_EMP_NO,
            //                                        a.PDT_YY,
            //                                        a.PDT_SEQ,
            //                                        minPrice = a.PDT_COMBINE_HTL == "N" ? a.minPrice : a.minPrice,//(Convert.ToInt32(a.minPrice) + getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, nowDate, "", Convert.ToInt32(a.inCnt))).ToString(),
            //                                    })
            //                                    .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
            //                                    .Select(s => new
            //                                    {
            //                                        s.Key.CORP_CODE,
            //                                        s.Key.PDT_TYPE_CODE,
            //                                        s.Key.PDT_IST_EMP_NO,
            //                                        s.Key.PDT_YY,
            //                                        s.Key.PDT_SEQ,
            //                                        minPrice = s.Min(min => min.minPrice),
            //                                    });

            //secondLists = from f in secondLists
            //              join j in getHotelDatas on f.CORP_CODE + f.PDT_TYPE_CODE + f.PDT_IST_EMP_NO + f.PDT_YY + f.PDT_SEQ + "_" + f.PDT_IST_EMP_NO equals j.CORP_CODE + j.PDT_TYPE_CODE + j.PDT_IST_EMP_NO + j.PDT_YY + j.PDT_SEQ + "_" + j.PDT_IST_EMP_NO into jj
            //              from jf in jj.DefaultIfEmpty()
            //              select new
            //              {
            //                  f.CORP_CODE,
            //                  f.PDT_TYPE_CODE,
            //                  f.PDT_IST_EMP_NO,
            //                  f.PDT_YY,
            //                  f.PDT_SEQ,
            //                  f.PDT_TITLE,
            //                  f.PDT_CONTENT,
            //                  f.PDT_IMG,
            //                  minPrice = jf != null ? jf.minPrice : f.minPrice,
            //                  f.PDT_COMBINE_HTL,
            //                  f.inCnt,
            //              };

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

            switch(T)
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

            foreach(var item in chkImg)
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





















            //var firstLists = rData0.Join(DB.PDT_2,
            //                             a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                             b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                            (a, b) => new
            //                            {
            //                                a.CORP_CODE,
            //                                a.PDT_TYPE_CODE,
            //                                a.PDT_IST_EMP_NO,
            //                                a.PDT_YY,
            //                                a.PDT_SEQ,
            //                                a.PDT_TITLE,
            //                                a.PDT_CONTENT,
            //                                a.PDT_COMBINE_HTL,
            //                                inCnt = a.PDT_DAYS_CODE.Substring(0, 2),
            //                                b.PDT_IMG,
            //                                a.PDT_DAYS_CODE,
            //                                a.PDT_OPTIONS,
            //                                chkIn = Convert.ToInt32(a.PDT_DAYS_CODE.Substring(2)) - Convert.ToInt32(a.PDT_DAYS_CODE.Substring(0, 2)),
            //                                a.PDT_ORDER_NO,
            //                            }).ToList();

            //DateTime now = DateTime.Now;

            //var dateLists = DB.PDT_1.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
            //                        .Select(s => new
            //                        {
            //                            s.Key.CORP_CODE,
            //                            s.Key.PDT_TYPE_CODE,
            //                            s.Key.PDT_IST_EMP_NO,
            //                            s.Key.PDT_YY,
            //                            s.Key.PDT_SEQ,
            //                            Date = s.Min(min => min.PDT_DATE).Substring(0, 7) + " ~ " + s.Max(max => max.PDT_DATE).Substring(0, 7)
            //                        });

            //var chkDateLists = DB.PDT_1.ToList()
            //                           .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
            //                           .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.PRC_SEQ, g.TRF_SEQ })
            //                           .Select(s => new
            //                           {
            //                               s.Key.CORP_CODE,
            //                               s.Key.PDT_TYPE_CODE,
            //                               s.Key.PDT_IST_EMP_NO,
            //                               s.Key.PDT_YY,
            //                               s.Key.PDT_SEQ,
            //                               s.Key.PRC_SEQ,
            //                               s.Key.TRF_SEQ,
            //                           });

            //var minPriceLists = chkDateLists.Join(DB.PRC_0,
            //                                      a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
            //                                      b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
            //                                      (a, b) => new
            //                                      {
            //                                          a.CORP_CODE,
            //                                          a.PDT_TYPE_CODE,
            //                                          a.PDT_IST_EMP_NO,
            //                                          a.PDT_YY,
            //                                          a.PDT_SEQ,
            //                                          b.PRC_Adult,
            //                                          a.TRF_SEQ,
            //                                      })
            //                                      .GroupBy(g => new
            //                                      {
            //                                          g.CORP_CODE,
            //                                          g.PDT_TYPE_CODE,
            //                                          g.PDT_IST_EMP_NO,
            //                                          g.PDT_YY,
            //                                          g.PDT_SEQ
            //                                      })
            //                                      .Select(s => new
            //                                      {
            //                                          s.Key.CORP_CODE,
            //                                          s.Key.PDT_TYPE_CODE,
            //                                          s.Key.PDT_IST_EMP_NO,
            //                                          s.Key.PDT_YY,
            //                                          s.Key.PDT_SEQ,
            //                                          minPrice = s.Min(min => min.PRC_Adult),
            //                                          TRF_SEQ = s.Min(min => min.TRF_SEQ)
            //                                      });

            //var secondLists = firstLists.Join(minPriceLists,
            //                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                                  (a, b) => new
            //                                  {
            //                                      a.CORP_CODE,
            //                                      a.PDT_TYPE_CODE,
            //                                      a.PDT_IST_EMP_NO,
            //                                      a.PDT_YY,
            //                                      a.PDT_SEQ,
            //                                      a.PDT_TITLE,
            //                                      a.PDT_CONTENT,
            //                                      a.PDT_IMG,
            //                                      minPrice = b.minPrice.ToString(),
            //                                      a.PDT_COMBINE_HTL,
            //                                      a.inCnt,
            //                                      a.PDT_DAYS_CODE,
            //                                      a.PDT_OPTIONS,
            //                                      b.TRF_SEQ,
            //                                      DATE = "",
            //                                      a.chkIn,
            //                                      a.PDT_ORDER_NO,
            //                                  });

            //var getHotelDatas = secondLists.Join(DB.PDT_4,
            //                                     a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
            //                                     b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                                    (a, b) => new
            //                                    {
            //                                        a.CORP_CODE,
            //                                        a.PDT_TYPE_CODE,
            //                                        a.PDT_IST_EMP_NO,
            //                                        a.PDT_YY,
            //                                        a.PDT_SEQ,
            //                                        minPrice = a.PDT_COMBINE_HTL == "N" ? a.minPrice : a.minPrice//(Convert.ToInt32(a.minPrice) + getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, /*nowDate*/ "2018-07-24", "2018-08-24", Convert.ToInt32(a.inCnt), a.chkIn > 1 ? "1" : "2")).ToString(),
            //                                    })
            //                                    .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
            //                                    .Select(s => new
            //                                    {
            //                                        s.Key.CORP_CODE,
            //                                        s.Key.PDT_TYPE_CODE,
            //                                        s.Key.PDT_IST_EMP_NO,
            //                                        s.Key.PDT_YY,
            //                                        s.Key.PDT_SEQ,
            //                                        minPrice = s.Min(min => min.minPrice),
            //                                    });

            //secondLists = from f in secondLists
            //              join j in getHotelDatas on f.CORP_CODE + f.PDT_TYPE_CODE + f.PDT_IST_EMP_NO + f.PDT_YY + f.PDT_SEQ + "_" + f.PDT_IST_EMP_NO equals j.CORP_CODE + j.PDT_TYPE_CODE + j.PDT_IST_EMP_NO + j.PDT_YY + j.PDT_SEQ + "_" + j.PDT_IST_EMP_NO into jj
            //              from jf in jj.DefaultIfEmpty()
            //              select new
            //              {
            //                  f.CORP_CODE,
            //                  f.PDT_TYPE_CODE,
            //                  f.PDT_IST_EMP_NO,
            //                  f.PDT_YY,
            //                  f.PDT_SEQ,
            //                  f.PDT_TITLE,
            //                  f.PDT_CONTENT,
            //                  f.PDT_IMG,
            //                  minPrice = jf != null ? jf.minPrice : f.minPrice,
            //                  f.PDT_COMBINE_HTL,
            //                  f.inCnt,
            //                  f.PDT_DAYS_CODE,
            //                  f.PDT_OPTIONS,
            //                  f.TRF_SEQ,
            //                  DATE = "",
            //                  f.chkIn,
            //                  f.PDT_ORDER_NO,
            //              };

            //secondLists = secondLists.Join(dateLists,
            //                               a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
            //                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                              (a, b) => new
            //                              {
            //                                  a.CORP_CODE,
            //                                  a.PDT_TYPE_CODE,
            //                                  a.PDT_IST_EMP_NO,
            //                                  a.PDT_YY,
            //                                  a.PDT_SEQ,
            //                                  a.PDT_TITLE,
            //                                  a.PDT_CONTENT,
            //                                  a.PDT_IMG,
            //                                  a.minPrice,
            //                                  a.PDT_COMBINE_HTL,
            //                                  a.inCnt,
            //                                  a.PDT_DAYS_CODE,
            //                                  a.PDT_OPTIONS,
            //                                  a.TRF_SEQ,
            //                                  DATE = b.Date,
            //                                  a.chkIn,
            //                                  a.PDT_ORDER_NO,
            //                              });







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
                                                      minPrice = b.PRC_Adult + b.PRC_Profit,
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
                                                      minPrice = s.Min(min => min.minPrice),
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

            //foreach (var item in secondLists)
            //{
            //    item.minPrice = Convert.ToInt32(getHotelMinPrice(item.CORP_CODE, item.PDT_TYPE_CODE, item.PDT_IST_EMP_NO, item.PDT_YY, item.PDT_SEQ, "2018-12-31", Convert.ToInt32(item.PDT_DAYS_CODE.ToString().Substring(0, 2))));
            //}
                
                //.Select(s => new
                //                            {
                //                                s.CORP_CODE,
                //                                s.PDT_TYPE_CODE,
                //                                s.PDT_IST_EMP_NO,
                //                                s.PDT_YY,
                //                                s.PDT_SEQ,
                //                                s.PDT_TITLE,
                //                                s.PDT_CONTENT,
                //                                s.PDT_IMG,
                //                                minPrice = 0,
                //                                s.PDT_COMBINE_HTL,
                //                                s.PDT_DAYS_CODE,
                //                                s.PDT_OPTIONS,
                //                                s.PDT_ORDER_NO,
                //                            });





















            ////DateTime now = DateTime.Now;

            //// 날짜 몇일 뒤부터?
            //now = now.AddDays(2);

            //var rData1 = rData0.Join(DB.PDT_1,
            //                         a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.PDT_TITLE,
            //                            a.PDT_CONTENT,
            //                            a.PDT_DAYS_CODE,
            //                            a.PDT_OPTIONS,
            //                            a.PDT_ORDER_NO,
            //                            a.PDT_COMBINE_HTL,
            //                            a.PDT_IMG,
            //                            b.PDT_DATE,
            //                            b.PRC_SEQ,
            //                            b.TRF_SEQ,
            //                        })
            //                        .ToList()
            //                        .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

            //var rData2 = rData1.Join(DB.PRC_0,
            //                         a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
            //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.PDT_TITLE,
            //                            a.PDT_CONTENT,
            //                            a.PDT_DAYS_CODE,
            //                            a.PDT_OPTIONS,
            //                            a.PDT_ORDER_NO,
            //                            a.PDT_COMBINE_HTL,
            //                            a.PDT_IMG,
            //                            a.PDT_DATE,
            //                            a.PRC_SEQ,
            //                            a.TRF_SEQ,

            //                            price = exchangedToDouble(b.PRC_Currency_CODE, (b.PRC_Adult + b.PRC_Profit).ToString()),
            //                            //price = a.PDT_COMBINE_HTL == "Y" ? 0 : exchangedToDouble(b.PRC_Currency_CODE, (b.PRC_Adult + b.PRC_Profit).ToString()),
            //                        });

            //var rData3 = rData2.Join(DB.PDT_4,
            //                         a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.price,

            //                            a.TRF_SEQ,
            //                            a.PRC_SEQ,

            //                            a.PDT_DATE,
            //                            SIN_DATE = traff == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,

            //                            b.PDT_IN,
            //                            b.PDT_HTL_SEQ,
            //                        })
            //                        .ToList();

            //var rData4 = rData3.Join(DB.HTL_2.Where(w => w.HTL_STATE_CODE.Equals("01")),
            //                         a => a.PDT_HTL_SEQ + a.SIN_DATE,
            //                         b => b.HTL_SEQ + b.HTL_DATE,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.price,

            //                            a.PDT_HTL_SEQ,

            //                            a.TRF_SEQ,
            //                            a.PRC_SEQ,

            //                            a.PDT_DATE,
            //                            a.SIN_DATE,

            //                            DATE = DateTime.ParseExact(b.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((a.PDT_IN) - 1).ToString("yyyy-MM-dd"),
            //                        })
            //                        .ToList()
            //                        .Where(w => DateTime.ParseExact(w.SIN_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

            //var rData5 = rData4.Join(DB.HTL_2.Where(w => w.HTL_STATE_CODE.Equals("01")),
            //                         a => a.PDT_HTL_SEQ + "_" + a.DATE,
            //                         b => b.HTL_SEQ + "_" + b.HTL_DATE,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.price,

            //                            a.PDT_HTL_SEQ,

            //                            a.TRF_SEQ,
            //                            a.PRC_SEQ,

            //                            a.PDT_DATE,
            //                            a.SIN_DATE,
            //                            a.DATE,

            //                            b.HTL_PRC_SEQ,
            //                        });

            //var rData6 = rData5.Join(DB.HTL_1,
            //                         a => a.PDT_HTL_SEQ + "_" + a.HTL_PRC_SEQ,
            //                         b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
            //                        (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.price,

            //                            a.TRF_SEQ,
            //                            a.PRC_SEQ,

            //                            a.PDT_DATE,
            //                            a.SIN_DATE,
            //                            a.DATE,

            //                            b.HTL_Currency_CODE,

            //                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
            //                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
            //                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
            //                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
            //                        });

            //var rData7 = rData6.Select(s => new
            //                    {
            //                        s.CORP_CODE,
            //                        s.PDT_TYPE_CODE,
            //                        s.PDT_IST_EMP_NO,
            //                        s.PDT_YY,
            //                        s.PDT_SEQ,
            //                        s.price,

            //                        s.PDT_DATE,
            //                        s.SIN_DATE,
            //                        s.DATE,

            //                        s.TRF_SEQ,
            //                        s.PRC_SEQ,

            //                        hotelPrice = exchangedToDouble(s.HTL_Currency_CODE, s.minPrice.ToString()),
            //                    })
            //                    .OrderBy(o => o.SIN_DATE);

            //var rData8 = rData7.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, g.price, g.PDT_DATE, g.SIN_DATE })
            //                   .Select(s => new
            //                   {
            //                       s.Key.CORP_CODE,
            //                       s.Key.PDT_TYPE_CODE,
            //                       s.Key.PDT_IST_EMP_NO,
            //                       s.Key.PDT_YY,
            //                       s.Key.PDT_SEQ,
            //                       s.Key.price,

            //                       s.Key.PDT_DATE,
            //                       s.Key.SIN_DATE,

            //                       totalPrice = s.Sum(sum => sum.hotelPrice),
            //                   });

            //var rData9 = rData8//.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
            //                   .Select(s => new
            //                   {
            //                       s.CORP_CODE,
            //                       s.PDT_TYPE_CODE,
            //                       s.PDT_IST_EMP_NO,
            //                       s.PDT_YY,
            //                       s.PDT_SEQ,

            //                       s.PDT_DATE,

            //                       price = s.price + s.totalPrice,
            //                   });

            //var rData10 = rData9.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
            //                    .Select(s => new
            //                    {
            //                        s.Key.CORP_CODE,
            //                        s.Key.PDT_TYPE_CODE,
            //                        s.Key.PDT_IST_EMP_NO,
            //                        s.Key.PDT_YY,
            //                        s.Key.PDT_SEQ,

            //                        price = s.Min(min => min.price),
            //                    });

            //var rData11 = rData2.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ, })
            //                    .Select(s => new
            //                    {
            //                        s.Key.CORP_CODE,
            //                        s.Key.PDT_TYPE_CODE,
            //                        s.Key.PDT_IST_EMP_NO,
            //                        s.Key.PDT_YY,
            //                        s.Key.PDT_SEQ,
            //                        s.FirstOrDefault().PDT_TITLE,
            //                        s.FirstOrDefault().PDT_CONTENT,
            //                        s.FirstOrDefault().PDT_DAYS_CODE,
            //                        s.FirstOrDefault().PDT_OPTIONS,

            //                        s.FirstOrDefault().PDT_IMG,
                                    
            //                        price = s.Min(min => min.price),
                                    
            //                        s.FirstOrDefault().TRF_SEQ,
            //                        s.FirstOrDefault().PDT_ORDER_NO,
            //                    })
            //                    .OrderBy(o => o.PDT_ORDER_NO);

            //var rData99 = from f in rData11
            //              join j in rData10 on f.CORP_CODE + f.PDT_TYPE_CODE + f.PDT_IST_EMP_NO + f.PDT_YY + f.PDT_SEQ + "_" + f.PDT_IST_EMP_NO equals j.CORP_CODE + j.PDT_TYPE_CODE + j.PDT_IST_EMP_NO + j.PDT_YY + j.PDT_SEQ + "_" + j.PDT_IST_EMP_NO into jj
            //              from jf in jj.DefaultIfEmpty()
            //              select new
            //              {
            //                  f.CORP_CODE,
            //                  f.PDT_TYPE_CODE,
            //                  f.PDT_IST_EMP_NO,
            //                  f.PDT_YY,
            //                  f.PDT_SEQ,
            //                  f.PDT_TITLE,
            //                  f.PDT_CONTENT,
            //                  f.PDT_DAYS_CODE,
            //                  f.PDT_OPTIONS,

            //                  f.TRF_SEQ,

            //                  f.PDT_IMG,
            //                  price = jf == null ? f.price : jf.price
            //              };

            var rData00 = secondLists.Join(traffData,//.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")),
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

                                          TRF_TITLE = getNNNNNNNAsjdkl(b.TRF_TITLE.Trim()),
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
                    var readChat = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(RoomNumber))
                                                  .Where(w => w.CHAT_User_YY.Equals("ADMN"))
                                                  .Where(w => w.CHAT_isNew.Equals("Y"));

                    if (readChat.Any())
                    {
                        foreach (var item in readChat)
                        {
                            item.CHAT_isNew = "N";
                        }

                        DB.SaveChanges();
                    }

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

            //var rData1 = rData0.Join(traffData,
            //                         a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
            //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
            //                         (a, b) => new
            //                        {
            //                            a.CORP_CODE,
            //                            a.PDT_TYPE_CODE,
            //                            a.PDT_IST_EMP_NO,
            //                            a.PDT_YY,
            //                            a.PDT_SEQ,
            //                            a.PDT_TITLE,
            //                            a.PDT_CONTENT,
            //                            a.PDT_DAYS_CODE,
            //                            a.PDT_OPTIONS,
            //                            a.PDT_ORDER_NO,
            //                            a.PDT_COMBINE_HTL,
            //                            a.PDT_DATE,
            //                            a.PRC_SEQ,
            //                            a.PDT_IMG,
            //                            b.TRF_SEQ,
            //                            b.TRF_TITLE,
            //                            b.TRF_SAREA,
            //                            b.TRF_STIME,
            //                            b.TRF_EAREA,
            //                            b.TRF_ETIME,
            //                        });








            //var aaaaa = PdtLists.Join(priceData,
            //                                    a => a.PRC_SEQ,
            //                                    b => b.PRC_SEQ,
            //                                    (a, b) => new
            //                                    {
            //                                        a.TRF_SEQ,
            //                                        a.PDT_DATE,

            //                                        price = exchangedToDouble(b.PRC_Currency_CODE, (b.PRC_Adult + b.PRC_Profit).ToString()), //b.PRC_Adult + b.PRC_Profit,
            //                                    })
            //                                    .ToList();

            //var trfData = DB.TRF_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
            //                      .Where(w => w.PDT_IST_EMP_NO == pdtemp)
            //                      .Where(w => w.PDT_YY.Equals(pdtyy))
            //                      .Where(w => w.PDT_SEQ == pdtseq)
            //                      .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"));

            //aaaaa = aaaaa.Join(trfData,
            //                   a => a.TRF_SEQ,
            //                   b => b.TRF_SEQ,
            //                  (a, b) => new
            //                  {
            //                      a.TRF_SEQ,
            //                      PDT_DATE = b.TRF_TYPE == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,

            //                      a.price,
            //                  })
            //                  .ToList();

            //var hotelData = DB.PDT_4.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
            //                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
            //                        .Where(w => w.PDT_YY.Equals(pdtyy))
            //                        .Where(w => w.PDT_SEQ == pdtseq);

            //foreach (object hotel in hotelData)
            //{
            //    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();
            //    string hotelDate = aaaaa.FirstOrDefault().PDT_DATE;

            //    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
            //                                .Where(w => w.HTL_DATE.Equals(hotelDate))
            //                                .Select(s => new
            //                                {
            //                                    s.HTL_DATE,
            //                                    s.HTL_PRC_SEQ,
            //                                })
            //                                .ToList();

            //    var hotelPriceData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
            //                                 .Select(s => new
            //                                 {
            //                                     s.HTL_PRC_SEQ,
            //                                     s.HTL_Currency_CODE,
            //                                     HTL_PRICE = string.IsNullOrEmpty(s.HTL_PRICE.ToString()) ? "0" : s.HTL_PRICE.ToString(),
            //                                     HTL_PRICE2 = string.IsNullOrEmpty(s.HTL_PRICE2.ToString()) ? "0" : s.HTL_PRICE2.ToString(),
            //                                     HTL_PRICE3 = string.IsNullOrEmpty(s.HTL_PRICE3.ToString()) ? "0" : s.HTL_PRICE3.ToString(),
            //                                     HTL_PRICE4 = string.IsNullOrEmpty(s.HTL_PRICE4.ToString()) ? "0" : s.HTL_PRICE4.ToString(),
            //                                     s.HTL_PROFIT,
            //                                 });

            //    var hotelPrice = hotelDateData.Join(hotelPriceData,
            //                                        a => a.HTL_PRC_SEQ,
            //                                        b => b.HTL_PRC_SEQ,
            //                                        (a, b) => new
            //                                        {
            //                                            a.HTL_DATE,
            //                                            b.HTL_Currency_CODE,

            //                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
            //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
            //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
            //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
            //                                        }).ToList();

            //    if (!hotelPrice.Any())
            //        continue;

            //    aaaaa = aaaaa.Join(hotelPrice,
            //                       a => a.PDT_DATE,
            //                       b => b.HTL_DATE,
            //                      (a, b) => new
            //                      {
            //                          a.TRF_SEQ,
            //                          PDT_DATE = DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),

            //                          price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),
            //                      })
            //                      .ToList();
            //}

            ////var pdtDateLists = PdtLists.Join(DB.PDT_1,
            ////                                a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            ////                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            ////                                (a, b) => new
            ////                                {
            ////                                    a.CORP_CODE,
            ////                                    a.PDT_TYPE_CODE,
            ////                                    a.PDT_IST_EMP_NO,
            ////                                    a.PDT_YY,
            ////                                    a.PDT_SEQ,
            ////                                    b.PDT_DATE,

            ////                                    price = getMinPrice((a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ), a.PDT_IST_EMP_NO.ToString(), b.PDT_DATE),
            ////                                })
            ////                                .ToList()
            ////                                .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

            ////var minPriceLists1 = pdtDateLists.Join(DB.PRC_0,
            ////                                      a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
            ////                                      b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
            ////                                      (a, b) => new
            ////                                      {
            ////                                          a.CORP_CODE,
            ////                                          a.PDT_TYPE_CODE,
            ////                                          a.PDT_IST_EMP_NO,
            ////                                          a.PDT_YY,
            ////                                          a.PDT_SEQ,
            ////                                          a.PDT_DATE,
            ////                                          a.TRF_SEQ,
            ////                                          a.PRC_SEQ,
            ////                                          price = getMinPrice((a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ), a.PDT_IST_EMP_NO.ToString(), a.PDT_DATE),
            ////                                      });

            //var minPriceLists2 = pdtDateLists.GroupBy(g => new
            //                                      {
            //                                          g.CORP_CODE,
            //                                          g.PDT_TYPE_CODE,
            //                                          g.PDT_IST_EMP_NO,
            //                                          g.PDT_YY,
            //                                          g.PDT_SEQ
            //                                      });

            //var minPriceLists = minPriceLists2.Select(s => new
            //                                      {
            //                                          s.Key.CORP_CODE,
            //                                          s.Key.PDT_TYPE_CODE,
            //                                          s.Key.PDT_IST_EMP_NO,
            //                                          s.Key.PDT_YY,
            //                                          s.Key.PDT_SEQ,
            //                                          minPrice = s.Min(min => min.price)
            //                                      });

            //var TempLists = PdtLists.Join(DB.PDT_2,
            //                              a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                              b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                              (a, b) => new
            //                              {
            //                                  a.CORP_CODE,
            //                                  a.PDT_TYPE_CODE,
            //                                  a.PDT_IST_EMP_NO,
            //                                  a.PDT_YY,
            //                                  a.PDT_SEQ,
            //                                  a.PDT_TITLE,
            //                                  a.PDT_CONTENT,
            //                                  a.PDT_DAYS_CODE,
            //                                  a.PDT_OPTIONS,
            //                                  a.PDT_ORDER_NO,
            //                                  a.traf,
            //                                  a.scity,
            //                                  a.stime,
            //                                  a.ecity,
            //                                  a.etime,
            //                                  b.PDT_IMG,
            //                                  //minPrice = "0",
            //                              }).ToList();

            ////var aaa = TempLists.Count();
            ////var bbb = minPriceLists.Count();

            //var resultLists = TempLists.Join(minPriceLists,
            //                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                                  (a, b) => new
            //                                  {
            //                                      a.CORP_CODE,
            //                                      a.PDT_TYPE_CODE,
            //                                      a.PDT_IST_EMP_NO,
            //                                      a.PDT_YY,
            //                                      a.PDT_SEQ,
            //                                      a.PDT_TITLE,
            //                                      a.PDT_CONTENT,
            //                                      a.PDT_DAYS_CODE,
            //                                      a.PDT_OPTIONS,
            //                                      //traf = "",
            //                                      //scity = "",
            //                                      //stime = "",
            //                                      //ecity = "",
            //                                      //etime = "",
            //                                      //PDT_IMG = "",
            //                                      a.traf,
            //                                      a.scity,
            //                                      a.stime,
            //                                      a.ecity,
            //                                      a.etime,
            //                                      a.PDT_IMG,
            //                                      b.minPrice,

            //                                      a.PDT_ORDER_NO,
            //                                  })
            //                                  .OrderBy(o => o.PDT_ORDER_NO);

            //return View(resultLists);
        }

        // GET: Products/hViews
        public ActionResult hViews(string C, string K)
        {
            try
            {
                var priKeys = getCode(C, K);

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
                                               a.POS_CODE,
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

                //string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

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


                //var inHtlTxt = "";

                //foreach(var item in chkPdtHtl)
                //{
                //    inHtlTxt =  item.PDT_IN + ":" + item.PDT_HTL_SEQ + ":" + item.
                //}

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

                //DateTime nextMonth = now.AddMonths(1);

                //nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                //var thisMonthPriceData = DB.PDT_1.ToList()
                //                                 .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                                 .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                                 .Where(w => w.PDT_YY.Equals(pdtyy))
                //                                 .Where(w => w.PDT_SEQ == pdtseq)
                //                                 .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                //                                 .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) < nextMonth)
                //                                 .Join(DB.PRC_0,
                //                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                //                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                //                                      (a, b) => new
                //                                      {
                //                                          days = a.PDT_DATE.Substring(8),
                //                                          price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                //                                          a.PDT_TYPE_CODE,
                //                                          a.PDT_IST_EMP_NO,
                //                                          a.PDT_YY,
                //                                          a.PDT_SEQ,
                //                                          a.PDT_DATE,
                //                                          traff = a.TRF_SEQ.ToString(),
                //                                      });

                //var chkPriceLists = DB.PDT_1.ToList()
                //                            .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                            .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                            .Where(w => w.PDT_YY.Equals(pdtyy))
                //                            .Where(w => w.PDT_SEQ == pdtseq)
                //                            .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                //                            .OrderBy(o => o.PDT_DATE)
                //                            .Take(4)
                //                            .Select(s => new
                //                            {
                //                                s.PDT_DATE,
                //                                s.PDT_STATE_CODE,
                                                
                //                                s.CORP_CODE,
                //                                s.PDT_TYPE_CODE,
                //                                s.PDT_IST_EMP_NO,
                //                                s.PDT_YY,
                //                                s.PDT_SEQ,
                //                                s.TRF_SEQ,
                //                                s.PRC_SEQ
                //                            });

                //var priceDefTable = chkPriceLists.Join(DB.PRC_0,
                //                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                //                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                //                                      (a, b) => new
                //                                      {
                //                                          a.PDT_DATE,
                //                                          a.PDT_STATE_CODE,

                //                                          price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                //                                          a.CORP_CODE,
                //                                          a.PDT_TYPE_CODE,
                //                                          a.PDT_IST_EMP_NO,
                //                                          a.PDT_YY,
                //                                          a.PDT_SEQ,
                //                                          a.TRF_SEQ,
                //                                          a.PRC_SEQ
                //                                      });

                //var trafDatas = priceDefTable.Join(DB.TRF_0,
                //                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                //                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                //                                   (a, b) => new
                //                                   {
                //                                       a.PDT_DATE,
                //                                       a.PDT_STATE_CODE,
                //                                       a.price,
                //                                       b.TRF_TITLE,
                //                                       b.TRF_TYPE,
                //                                       b.TRF_SUB_SEQ
                //                                   });


                //var trafTitle = trafDatas.GroupBy(g => new { g.PDT_DATE })
                //                            .Select(s => new
                //                            {
                //                                s.Key.PDT_DATE,
                //                                title = "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "] - [" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).FirstOrDefault().TRF_TITLE + "]",
                //                                type = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TYPE,
                //                            });

                //var priceTable = priceDefTable.Join(trafTitle,
                //                                a => a.PDT_DATE,
                //                                b => b.PDT_DATE,
                //                                (a, b) => new
                //                                {
                //                                    a.PDT_DATE,
                //                                    b.title,
                //                                    a.PDT_STATE_CODE,
                //                                    a.price,

                //                                    inDate = b.type == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                //                                })
                //                                .ToList();

                //if (chkCombinHotels.Equals("Y"))
                //{
                //    var hotelData = DB.PDT_4.ToList()
                //                            .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                            .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                            .Where(w => w.PDT_YY.Equals(pdtyy))
                //                            .Where(w => w.PDT_SEQ == pdtseq)
                //                            .OrderBy(o => o.PDT_IN);

                //    foreach(object hotel in hotelData)
                //    {
                //        var trfData = DB.TRF_0.ToList()
                //                              .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                              .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                              .Where(w => w.PDT_YY.Equals(pdtyy))
                //                              .Where(w => w.PDT_SEQ == pdtseq)
                //                              .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"));

                //        thisMonthPriceData = thisMonthPriceData.Join(trfData,
                //                                                     a => a.traff.ToString(),
                //                                                     b => b.TRF_SEQ.ToString(),
                //                                                    (a, b) => new
                //                                                    {
                //                                                        a.days,
                //                                                        a.price,

                //                                                        a.PDT_TYPE_CODE,
                //                                                        a.PDT_IST_EMP_NO,
                //                                                        a.PDT_YY,
                //                                                        a.PDT_SEQ,
                //                                                        PDT_DATE = b.TRF_TYPE == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                //                                                        traff = b.TRF_TYPE,
                //                                                    })
                //                                                    .ToList();

                //        string inDays = hotel.GetType().GetProperties()[5].GetValue(hotel, null).ToString();
                //        string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                //        var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                //        var hotelPrice = hotelDateData.ToList()
                //                                      .Join(DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq)),
                //                                            a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                //                                            b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                //                                            (a, b) => new
                //                                            {
                //                                                a.HTL_DATE,
                //                                                b.HTL_Currency_CODE,
                //                                                minPrice = getMinPrice(Convert.ToInt32(b.HTL_PRICE.ToString()), Convert.ToInt32(b.HTL_PRICE2.ToString()), Convert.ToInt32(b.HTL_PRICE3.ToString()), Convert.ToInt32(b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                //                                            });

                //        thisMonthPriceData = thisMonthPriceData.Join(hotelPrice,
                //                                                     a => a.PDT_DATE,
                //                                                     b => b.HTL_DATE,
                //                                                    (a, b) => new
                //                                                    {
                //                                                        a.days,
                //                                                        price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                                        a.PDT_TYPE_CODE,
                //                                                        a.PDT_IST_EMP_NO,
                //                                                        a.PDT_YY,
                //                                                        a.PDT_SEQ,
                //                                                        a.PDT_DATE,
                //                                                        a.traff,
                //                                                    });

                //        priceTable = priceTable.Join(hotelPrice,
                //                                    a => a.inDate,
                //                                    b => b.HTL_DATE,
                //                                    (a, b) => new
                //                                    {
                //                                        a.PDT_DATE,
                //                                        a.title,
                //                                        a.PDT_STATE_CODE,
                //                                        price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                        a.inDate
                //                                    })
                //                                    .ToList();
                //    }
                //}

                //var TMPD_MinData = thisMonthPriceData.GroupBy(g => new { g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                //                                     .Select(s => new
                //                                     {
                //                                         minPrice = s.Min(min => min.price)
                //                                     })
                //                                     .ToList();

                //thisMonthPriceData = thisMonthPriceData.Join(TMPD_MinData,
                //                                             a => a.price,
                //                                             b => b.minPrice,
                //                                            (a, b) => new
                //                                            {
                //                                                a.days,
                //                                                a.price,

                //                                                a.PDT_TYPE_CODE,
                //                                                a.PDT_IST_EMP_NO,
                //                                                a.PDT_YY,
                //                                                a.PDT_SEQ,
                //                                                a.PDT_DATE,
                //                                                a.traff,
                //                                            })
                //                                            .OrderBy(o => o.days)
                //                                            .ToList();

                //ViewBag.TMPD = thisMonthPriceData;
                //ViewBag.priceTable = priceTable;

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
                                               a.POS_CODE,
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
        // GET: Products/Views
        public ActionResult dViews(string C, string K)
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

                //string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

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


                //var inHtlTxt = "";

                //foreach(var item in chkPdtHtl)
                //{
                //    inHtlTxt =  item.PDT_IN + ":" + item.PDT_HTL_SEQ + ":" + item.
                //}

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

                //DateTime nextMonth = now.AddMonths(1);

                //nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                //var thisMonthPriceData = DB.PDT_1.ToList()
                //                                 .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                                 .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                                 .Where(w => w.PDT_YY.Equals(pdtyy))
                //                                 .Where(w => w.PDT_SEQ == pdtseq)
                //                                 .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                //                                 .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) < nextMonth)
                //                                 .Join(DB.PRC_0,
                //                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                //                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                //                                      (a, b) => new
                //                                      {
                //                                          days = a.PDT_DATE.Substring(8),
                //                                          price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                //                                          a.PDT_TYPE_CODE,
                //                                          a.PDT_IST_EMP_NO,
                //                                          a.PDT_YY,
                //                                          a.PDT_SEQ,
                //                                          a.PDT_DATE,
                //                                          traff = a.TRF_SEQ.ToString(),
                //                                      });

                //var chkPriceLists = DB.PDT_1.ToList()
                //                            .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                            .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                            .Where(w => w.PDT_YY.Equals(pdtyy))
                //                            .Where(w => w.PDT_SEQ == pdtseq)
                //                            .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                //                            .OrderBy(o => o.PDT_DATE)
                //                            .Take(4)
                //                            .Select(s => new
                //                            {
                //                                s.PDT_DATE,
                //                                s.PDT_STATE_CODE,

                //                                s.CORP_CODE,
                //                                s.PDT_TYPE_CODE,
                //                                s.PDT_IST_EMP_NO,
                //                                s.PDT_YY,
                //                                s.PDT_SEQ,
                //                                s.TRF_SEQ,
                //                                s.PRC_SEQ
                //                            });

                //var priceDefTable = chkPriceLists.Join(DB.PRC_0,
                //                                       a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                //                                       b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                //                                      (a, b) => new
                //                                      {
                //                                          a.PDT_DATE,
                //                                          a.PDT_STATE_CODE,

                //                                          price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                //                                          a.CORP_CODE,
                //                                          a.PDT_TYPE_CODE,
                //                                          a.PDT_IST_EMP_NO,
                //                                          a.PDT_YY,
                //                                          a.PDT_SEQ,
                //                                          a.TRF_SEQ,
                //                                          a.PRC_SEQ
                //                                      });

                //var trafDatas = priceDefTable.Join(DB.TRF_0,
                //                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                //                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                //                                   (a, b) => new
                //                                   {
                //                                       a.PDT_DATE,
                //                                       a.PDT_STATE_CODE,
                //                                       a.price,
                //                                       b.TRF_TITLE,
                //                                       b.TRF_TYPE,
                //                                       b.TRF_SUB_SEQ
                //                                   });


                //var trafTitle = trafDatas.GroupBy(g => new { g.PDT_DATE })
                //                            .Select(s => new
                //                            {
                //                                s.Key.PDT_DATE,
                //                                title = "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "] - [" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).FirstOrDefault().TRF_TITLE + "]",
                //                                type = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TYPE,
                //                            });

                //var priceTable = priceDefTable.Join(trafTitle,
                //                                a => a.PDT_DATE,
                //                                b => b.PDT_DATE,
                //                                (a, b) => new
                //                                {
                //                                    a.PDT_DATE,
                //                                    b.title,
                //                                    a.PDT_STATE_CODE,
                //                                    a.price,

                //                                    inDate = b.type == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                //                                })
                //                                .ToList();

                //if (chkCombinHotels.Equals("Y"))
                //{
                //    var hotelData = DB.PDT_4.ToList()
                //                            .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                            .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                            .Where(w => w.PDT_YY.Equals(pdtyy))
                //                            .Where(w => w.PDT_SEQ == pdtseq)
                //                            .OrderBy(o => o.PDT_IN);

                //    foreach(object hotel in hotelData)
                //    {
                //        var trfData = DB.TRF_0.ToList()
                //                              .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                //                              .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                //                              .Where(w => w.PDT_YY.Equals(pdtyy))
                //                              .Where(w => w.PDT_SEQ == pdtseq)
                //                              .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"));

                //        thisMonthPriceData = thisMonthPriceData.Join(trfData,
                //                                                     a => a.traff.ToString(),
                //                                                     b => b.TRF_SEQ.ToString(),
                //                                                    (a, b) => new
                //                                                    {
                //                                                        a.days,
                //                                                        a.price,

                //                                                        a.PDT_TYPE_CODE,
                //                                                        a.PDT_IST_EMP_NO,
                //                                                        a.PDT_YY,
                //                                                        a.PDT_SEQ,
                //                                                        PDT_DATE = b.TRF_TYPE == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                //                                                        traff = b.TRF_TYPE,
                //                                                    })
                //                                                    .ToList();

                //        string inDays = hotel.GetType().GetProperties()[5].GetValue(hotel, null).ToString();
                //        string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                //        var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                //        var hotelPrice = hotelDateData.ToList()
                //                                      .Join(DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq)),
                //                                            a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                //                                            b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                //                                            (a, b) => new
                //                                            {
                //                                                a.HTL_DATE,
                //                                                b.HTL_Currency_CODE,
                //                                                minPrice = getMinPrice(Convert.ToInt32(b.HTL_PRICE.ToString()), Convert.ToInt32(b.HTL_PRICE2.ToString()), Convert.ToInt32(b.HTL_PRICE3.ToString()), Convert.ToInt32(b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                //                                            });

                //        thisMonthPriceData = thisMonthPriceData.Join(hotelPrice,
                //                                                     a => a.PDT_DATE,
                //                                                     b => b.HTL_DATE,
                //                                                    (a, b) => new
                //                                                    {
                //                                                        a.days,
                //                                                        price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                                        a.PDT_TYPE_CODE,
                //                                                        a.PDT_IST_EMP_NO,
                //                                                        a.PDT_YY,
                //                                                        a.PDT_SEQ,
                //                                                        a.PDT_DATE,
                //                                                        a.traff,
                //                                                    });

                //        priceTable = priceTable.Join(hotelPrice,
                //                                    a => a.inDate,
                //                                    b => b.HTL_DATE,
                //                                    (a, b) => new
                //                                    {
                //                                        a.PDT_DATE,
                //                                        a.title,
                //                                        a.PDT_STATE_CODE,
                //                                        price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                        a.inDate
                //                                    })
                //                                    .ToList();
                //    }
                //}

                //var TMPD_MinData = thisMonthPriceData.GroupBy(g => new { g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                //                                     .Select(s => new
                //                                     {
                //                                         minPrice = s.Min(min => min.price)
                //                                     })
                //                                     .ToList();

                //thisMonthPriceData = thisMonthPriceData.Join(TMPD_MinData,
                //                                             a => a.price,
                //                                             b => b.minPrice,
                //                                            (a, b) => new
                //                                            {
                //                                                a.days,
                //                                                a.price,

                //                                                a.PDT_TYPE_CODE,
                //                                                a.PDT_IST_EMP_NO,
                //                                                a.PDT_YY,
                //                                                a.PDT_SEQ,
                //                                                a.PDT_DATE,
                //                                                a.traff,
                //                                            })
                //                                            .OrderBy(o => o.days)
                //                                            .ToList();

                //ViewBag.TMPD = thisMonthPriceData;
                //ViewBag.priceTable = priceTable;

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
                                               a.POS_CODE,
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

        // GET: Products/BestLists
        public ActionResult BestLists()
        {
            return View();
        }

        // GET: Products/Reserve
        public ActionResult Reserve(FormCollection f)
        {
            try
            {
                string code     = f["pdtCode"].ToString(),
                       key      = f["pdtKey"].ToString(),
                       trfCode  = f["trfCD"].ToString(),
                       prcCode  = f["prcCD"].ToString(),
                       empNo    = f["empNo"].ToString(),
                       revDate  = f["revDate"].ToString(),
                       ACnt     = f["adultCnt"].ToString(),
                       CCnt     = f["childCnt"].ToString(),
                       BCnt     = f["babyCnt"].ToString(),
                       //hotels   = f["hotel[]"].ToString(),
                       //rooms    = f["room[]"].ToString(),
                       inCnt    = "",//f["inCnt"].ToString(),
                       APrice   = f["APrice"].ToString(),
                       CPrice   = f["CPrice"].ToString(),
                       BPrice   = f["BPrice"].ToString(),
                       inHotels = f["inHotels"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

                string sDate = "",
                       eDate = "";

                int tCnt = 0;

                try
                {
                    sDate = revDate.Split(',')[0].Trim();
                    eDate = revDate.Split(',')[1].Trim();

                    DateTime sDateTime = DateTime.ParseExact(sDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime eDateTime = DateTime.ParseExact(eDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    TimeSpan TS = eDateTime - sDateTime;

                    tCnt = TS.Days;

                    string temp1 = tCnt.ToString();
                    string temp2 = (tCnt + 1).ToString();

                    switch (temp1.Length)
                    {
                        case 1:
                            inCnt = "0" + temp1;
                            break;
                        case 2:
                            inCnt = temp1;
                            break;
                    }

                    switch (temp2.Length)
                    {
                        case 1:
                            inCnt += "0" + temp2;
                            break;
                        case 2:
                            inCnt += temp2;
                            break;
                    }
                }
                catch
                {

                }

                var pdtData = DB.PDT_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Select(s => new
                                      {
                                          code      = code,
                                          key       = key,
                                          title     = s.PDT_TITLE,
                                          nation    = s.PDT_NATION_CODE,
                                          area      = s.PDT_AREA_CODE,
                                          inCnt     = string.IsNullOrEmpty(inCnt) ? s.PDT_DAYS_CODE : inCnt,
                                          stater    = s.PDT_SCITY_CODE,
                                          ACnt      = ACnt,
                                          APrice    = APrice,
                                          CCnt      = CCnt,
                                          CPrice    = CPrice,
                                          BCnt      = BCnt,
                                          BPrice    = BPrice,
                                          revDate   = string.IsNullOrEmpty(sDate) ? revDate : sDate,
                                          empNo     = s.PDT_EMP_NO,
                                      });

                var trfData = DB.TRF_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Where(w => w.TRF_SEQ.ToString().Equals(trfCode))
                                      .Select(s => new
                                      {
                                          code      = code,
                                          key       = key,
                                          trfCode   = trfCode,
                                          TRF_TITLE = getNNNNNNNAsjdkl(s.TRF_TITLE),
                                          s.TRF_STIME,
                                          s.TRF_SAREA,
                                          s.TRF_ETIME,
                                          s.TRF_EAREA,
                                          s.TRF_TYPE,
                                          s.TRF_SUB_SEQ,
                                      });

                var prcData = DB.PRC_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Where(w => w.PRC_SEQ.ToString().Equals(prcCode))
                                      .Select(s => new
                                      {
                                          code      = code,
                                          key       = key,
                                          prcCode   = prcCode,
                                          s.PRC_SEQ,
                                          s.PRC_Currency_CODE,
                                          APrice = s.PRC_Adult > 0 ? s.PRC_Adult + s.PRC_Profit : 0,
                                          CPrice = s.PRC_Child > 0 ? s.PRC_Child + s.PRC_Profit : 0,
                                          BPrice = s.PRC_Baby > 0 ? s.PRC_Baby : 0,
                                          //BPrice = s.PRC_Baby > 0 ? s.PRC_Baby + s.PRC_Profit : 0,
                                          s.PRC_EXP,
                                      });

                var empData = DB.EMP_0.ToList()
                                      .Where(w => w.EMP_NO.ToString().Equals(empNo))
                                      .Select(s => new
                                      {
                                          NAME  = s.EMP_NAME,
                                          TEL   = s.EMP_TEL1 + "-" + s.EMP_TEL2 + "-" + s.EMP_TEL3,
                                          MAIL  = s.EMP_EMAIL,
                                               s.POS_CODE,
                                      });

                var user = Session["user"] as User;

                var userData = NDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login))
                                        .Select(s => new
                                        {
                                            s.CU_NM_KOR,
                                            s.BIRTHDAY,
                                            s.EMAIL,
                                            PHONE = s.HANDPHONE1 + s.HANDPHONE2 + s.HANDPHONE3,
                                            s.CU_NM_ENG_LAST,
                                            s.CU_NM_ENG_FIRST,
                                            s.SEX,
                                            s.CU_YY,
                                            s.CU_SEQ
                                        });

                if (!userData.Any())
                {
                    return Content("<script>alert('잘못된 접근 경로 입니다.'); location.href='/';</script>");
                }

                Dictionary<int, List<string>> HLists = new Dictionary<int, List<string>>();

                if (!string.IsNullOrEmpty(eDate))
                {
                    string hSeq = inHotels;

                    List<string> HData = new List<string>();

                    DateTime sDateTime = DateTime.ParseExact(sDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                    DateTime eDateTime = DateTime.ParseExact(eDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                    var tempHData0 = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(hSeq));
                    var tempHData1 = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(hSeq));
                    var tempHData2 = DB.HTL_2.ToList()
                                             .Where(w => w.HTL_SEQ.ToString().Equals(hSeq))
                                             .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= sDateTime)
                                             .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) < eDateTime);

                    var tempHData3 = tempHData2.Join(tempHData1,
                                                     a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                     b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                                    (a, b) => new
                                                    {
                                                        a.HTL_DATE,
                                                        a.HTL_SEQ,
                                                        b.HTL_PRICE2
                                                    })
                                                    .GroupBy(g => new { g.HTL_DATE, g.HTL_SEQ })
                                                    .Select(s => new { s.Key.HTL_DATE, s.Key.HTL_SEQ }).ToList();

                    var tempHData4 = tempHData3.Join(tempHData0,
                                             a => a.HTL_SEQ,
                                             b => b.HTL_SEQ,
                                            (a, b) => new
                                            {
                                                a.HTL_DATE,
                                                b.HTL_NAME,
                                                b.HTL_SEQ,
                                            }).ToList();

                    int i = 0;

                    foreach (object item in tempHData4)
                    {
                        HData.Add(item.GetType().GetProperties()[0].GetValue(item, null).ToString());
                        HData.Add(item.GetType().GetProperties()[1].GetValue(item, null).ToString());
                        HData.Add(item.GetType().GetProperties()[2].GetValue(item, null).ToString());

                        HLists.Add(i, HData);
                        i++;
                    }

                }
                else
                {
                    string[] hotels = inHotels.Split(',');

                    for (int i = 0; i < hotels.Length; i++)
                    {
                        string hSeq = hotels[i].Trim();

                        if (string.IsNullOrEmpty(hSeq))
                            continue;

                        List<string> HData = new List<string>();
                        int chkIn = Convert.ToInt32(JSON_getChkin(code, key, trfCode).Data.ToString());

                        var tempHData0 = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(hSeq));
                        var tempHData1 = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(hSeq));
                        var tempHData2 = DB.HTL_2.ToList()
                                                 .Where(w => w.HTL_SEQ.ToString().Equals(hSeq))
                                                 .Where(w => w.HTL_DATE.Equals(DateTime.ParseExact(revDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((chkIn) + i).ToString("yyyy-MM-dd")));

                        var tempHData3 = tempHData2.Join(tempHData1,
                                                         a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                         b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                                        (a, b) => new
                                                        {
                                                            a.HTL_DATE,
                                                            a.HTL_SEQ,
                                                            b.HTL_PRICE2
                                                        })
                                                        .GroupBy(g => new { g.HTL_DATE, g.HTL_SEQ })
                                                        .Select(s => new { s.Key.HTL_DATE, s.Key.HTL_SEQ }).ToList();

                        var tempHData4 = tempHData3.Join(tempHData0,
                                                 a => a.HTL_SEQ,
                                                 b => b.HTL_SEQ,
                                                (a, b) => new
                                                {
                                                    a.HTL_DATE,
                                                    b.HTL_NAME,
                                                    b.HTL_SEQ,
                                                }).ToList();

                        HData.Add(tempHData4.SingleOrDefault().HTL_DATE);
                        HData.Add(tempHData4.SingleOrDefault().HTL_NAME);
                        HData.Add(tempHData4.SingleOrDefault().HTL_SEQ.ToString());//, tempHData4.SingleOrDefault().HTL_NAME, tempHData4.SingleOrDefault().HTL_SEQ);

                        HLists.Add(i, HData);
                    }
                }

                ViewBag.rHData = HLists;
                ViewBag.empData = empData;
                ViewBag.trfData = trfData;
                ViewBag.userData = userData;
                ViewBag.prcData = prcData;

                return View(pdtData);
            }
            catch
            {
                return Content("<script>alert('잘못된 접근 경로 입니다.'); location.href='/';</script>");
            }
        }

        // GET: Products/Confirm
        public ActionResult Confirm(FormCollection f)
        {
            string date = f["date"],
                   code = f["code"],
                   key = f["key"],
                   aCnt = f["aCnt"],
                   cCnt = f["cCnt"],
                   bCnt = f["bCnt"],
                   user_YY = f["user_YY"],
                   reqTxt = f["reqTxt"],
                   inHotels = f["inHotels[]"];

            int prcCD = Convert.ToInt32(f["prcCD"]),
                trfCD = Convert.ToInt32(f["trfCD"]),
                user_SEQ = Convert.ToInt32(f["user_SEQ"]);

            string name = f["rName"],
                   email = f["rEmail"],
                   tel = f["rPhone"];

            var user = Session["user"] as User;

            if (string.IsNullOrEmpty(code) || user == null)
            {
                return Redirect("/");
            }

            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            DateTime now = DateTime.Now;

            var strDate = now.ToString("yyyy-MM-dd");
            var revDate = now.ToString("yyyyMMdd");

            var chkRev = DB.REV_0.Where(w => w.REV_DAY.Equals(revDate))
                                  .GroupBy(g => new
                                  {
                                      g.REV_DAY
                                  })
                                  .Select(s => new
                                  {
                                      seq = s.Max(max => max.REV_SEQ),
                                  });

            int lastRev = 1;

            try { lastRev += chkRev.Single().seq; }
            catch { }

            int emp_no = Convert.ToInt32(emp),
                pdt_seq = Convert.ToInt32(pdtseq);

            var pdtData = DB.PDT_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp_no)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdt_seq)
                                  .Select(s => new
                                  {
                                      s.PDT_TITLE,
                                      s.PDT_EMP_NO,
                                      s.PDT_DAYS_CODE,
                                      s.PDT_AREA_CODE,
                                  });

            int chg_emp_no = pdtData.Single().PDT_EMP_NO;
            string PDT_TITLE = pdtData.Single().PDT_TITLE;
            string PDT_DAYS_CODE = pdtData.Single().PDT_DAYS_CODE;
            string PDT_AREA_CODE = pdtData.Single().PDT_AREA_CODE;

            var prcData = DB.PRC_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp_no)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdt_seq)
                                  .Where(w => w.PRC_SEQ == prcCD)
                                  .Select(s => new
                                  {
                                      s.PRC_Adult,
                                      s.PRC_Child,
                                      s.PRC_Baby,
                                      s.PRC_Profit,
                                      s.PRC_Currency_CODE,
                                      s.PRC_EXP,
                                  });


            var CuData = NDB.CU001.Where(w => w.CU_YY.Equals(user_YY))
                                  .Where(w => w.CU_SEQ == user_SEQ)
                                  .Select(s => new
                                  {
                                      s.CU_NM_KOR,
                                      tel = s.CO_TEL1 + s.CO_TEL2 + s.CO_TEL3,
                                      s.CU_NM_ENG_FIRST,
                                      s.CU_NM_ENG_LAST,
                                      s.EMAIL,
                                  });

            int totalCnt = Convert.ToInt32(aCnt);

            int PRC_Adult = (Convert.ToInt32(prcData.Single().PRC_Adult.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(aCnt),
                PRC_Child = (Convert.ToInt32(prcData.Single().PRC_Child.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(cCnt),
                PRC_Baby = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString())),// * Convert.ToInt32(bCnt),
                //PRC_Baby = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(bCnt),
                PRC_ATotal = PRC_Adult * Convert.ToInt32(aCnt),
                PRC_CTotal = PRC_Child * Convert.ToInt32(cCnt),
                PRC_BTotal = PRC_Baby * Convert.ToInt32(bCnt);

            int PRC_Total = PRC_ATotal + PRC_CTotal + PRC_BTotal;

            string PRC_CURRENCY_CODE = prcData.Single().PRC_Currency_CODE.ToString(),
                   PRC_CONTENT = prcData.Single().PRC_EXP.ToString();

            if (cCnt != "0")
            {
                totalCnt += Convert.ToInt32(cCnt);
            }

            int rpSeq = 1;

            var spData = DB.REV_1.Where(w => w.REV_DAY.Equals(revDate))
                                 .Where(w => w.REV_SEQ == lastRev)
                                 .GroupBy(g => new
                                 {
                                     g.CORP_CODE,
                                     g.REV_DAY,
                                     g.REV_SEQ
                                 })
                                 .Select(s => new
                                 {
                                     seq = s.Max(max => max.REV_PRC_SEQ),
                                 });

            try { rpSeq += spData.Single().seq; }
            catch { }

            string HTL_Name = "";
            int HTL_Total = 0;

            /*
             * 호텔 선택시 등록
             * 
             * corp_code            - 회사코드
             * rev_day              - 예약일
             * rev_seq              - 예약 번호
             * htl_seq              - 호텔 번호
             * htl_nation_code      - 호텔 국가
             * htl_area_code        - 호텔 지역
             * htl_type             - 호텔 타입
             * htl_chkin            - 호텔 체크인 날짜 (yyyy-MM-dd)
             * stay_seq             - 숙박 순서
             * htl_staydays         - 총 숙박일
             * cu_yy                - 회원 번호 (yyyy)
             * cu_seq               - 회원 번호
             * cu_name              - 회원 이름
             * cu_name_first        - 회원 영문 이름
             * cu_name_last         - 회원 영문 성
             * cu_hp                - 회원 연락처
             * cu_email             - 회원 이메일
             * adult_cnt            - 성인 수
             * child_cnt            - 소아 수
             * baby_cnt             - 유아 수
             * htl_roomtype         - 룸타입
             * 
             */
            if (!string.IsNullOrEmpty(inHotels))
            {
                string[] hotels = inHotels.Split(',');

                int chkIn = Convert.ToInt32(JSON_getChkin(code, key, trfCD.ToString()).Data.ToString());

                for (int i = 0; i < hotels.Length; i++)
                {
                    if (string.IsNullOrEmpty(hotels[i].Trim()))
                        continue;

                    var tempData0 = DB.HTL_0.ToList()
                                            .Where(w => w.HTL_SEQ.ToString().Equals(hotels[i].Trim().ToString()));

                    var inDays = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((chkIn) + i).ToString("yyyy-MM-dd");

                    var tempDate1 = DB.HTL_2.ToList()
                                            .Where(w => w.HTL_SEQ.ToString().Equals(hotels[i].Trim().ToString()))
                                            .Where(w => w.HTL_DATE.Equals(inDays));

                    string tempPrice = tempDate1.Join(DB.HTL_1,
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

                    HTL_Name = tempData0.SingleOrDefault().HTL_NAME.ToString();
                    HTL_Total += Convert.ToInt32(tempPrice);

                    setRevHotel(corp,
                                revDate,
                                lastRev,
                                hotels[i].Trim(),
                                tempData0.SingleOrDefault().HTL_NATION_CODE.ToString(),
                                tempData0.SingleOrDefault().HTL_AREA_CODE.ToString(),
                                tempData0.SingleOrDefault().HTL_TYPE.ToString(),
                                inDays,
                                (i + 1),
                                hotels.Length - 1,
                                user_YY,
                                user_SEQ,
                                name,
                                CuData.Single().CU_NM_ENG_FIRST,
                                CuData.Single().CU_NM_ENG_LAST,
                                tel,
                                CuData.Single().EMAIL,
                                Convert.ToInt32(aCnt),
                                Convert.ToInt32(cCnt),
                                Convert.ToInt32(bCnt));
                }

                REV_1 HTL_PRICE = new REV_1();

                HTL_PRICE.CORP_CODE = corp;
                HTL_PRICE.REV_DAY = revDate;
                HTL_PRICE.REV_SEQ = lastRev;

//                HTL_PRICE.PDT_TYPE_CODE = pdt_type;
//                HTL_PRICE.PDT_IST_EMP_NO = Convert.ToInt32(emp);
//                HTL_PRICE.PDT_YY = pdtyy;
//                HTL_PRICE.PDT_SEQ = Convert.ToInt32(pdtseq);
//                HTL_PRICE.PRC_SEQ = prcCD;
                HTL_PRICE.PRC_CURRENCY_CODE = PRC_CURRENCY_CODE;
                HTL_PRICE.PRC_CONTENT = "호텔 요금";

                HTL_PRICE.REV_PRC_SEQ = rpSeq;
                HTL_PRICE.REV_PRC_GB = "A";
                HTL_PRICE.REV_PRC_PRICE = HTL_Total;
                HTL_PRICE.REV_PRC_CNT = Convert.ToInt32(totalCnt);
                HTL_PRICE.REV_PRC_TOTAL_PRICE = (HTL_Total * Convert.ToInt32(totalCnt));
                HTL_PRICE.REV_PRC_DISCOUNT = 0;
                HTL_PRICE.REV_PRC_ADDITIONAL_PRICE = 0;
                HTL_PRICE.REV_PRC_CONTENT = HTL_Name;

                DB.REV_1.Add(HTL_PRICE);
                DB.SaveChanges();

                rpSeq++;
            }
            /*
             * 호텔 선택시 등록
             */


            PRC_Total += (HTL_Total * totalCnt);

            if (bCnt != "0")
            {
                totalCnt += Convert.ToInt32(bCnt);
            }

            REV_0 newReserv = new REV_0();

            newReserv.CORP_CODE = corp;
            newReserv.PDT_TYPE_CODE = pdt_type;
            newReserv.PDT_IST_EMP_NO = Convert.ToInt32(emp);
            newReserv.PDT_YY = pdtyy;
            newReserv.PDT_SEQ = Convert.ToInt32(pdtseq);
            newReserv.PDT_TITLE = PDT_TITLE;
            newReserv.PDT_DAYS_CODE = PDT_DAYS_CODE;
            newReserv.CU_YY = user_YY;
            newReserv.CU_SEQ = Convert.ToInt32(user_SEQ);
            newReserv.CU_NAME = CuData.Single().CU_NM_KOR;

            newReserv.REV_DAY = revDate;
            newReserv.REV_SEQ = lastRev;
            newReserv.REV_CNT = totalCnt;
            newReserv.REV_STATE = "20";
            newReserv.REV_STARTDAY = date;
            newReserv.ADULT_CNT = Convert.ToInt32(aCnt);
            newReserv.CHILD_CNT = Convert.ToInt32(cCnt);
            newReserv.BABY_CNT = Convert.ToInt32(bCnt);
            newReserv.REV_PRICE = PRC_Total;
            newReserv.REV_CHK_PRICE = "N";
            newReserv.REV_CONTENT = reqTxt;

            newReserv.IST_EMP_NO = chg_emp_no;                   // 등록사원
            newReserv.CHG_EMP_NO = chg_emp_no;                   // 담당사원
            newReserv.SEL_EMP_NO = chg_emp_no;                   // 판매사원
            newReserv.UDT_EMP_NO = chg_emp_no;                   // 수정사원

            newReserv.IST_DATE = strDate;
            newReserv.UDT_DATE = strDate;
            newReserv.SEL_DATE = strDate;

            DB.REV_0.Add(newReserv);

            DB.SaveChanges();

            List<string> dls = new List<string>();

            dls.Add(CuData.Single().CU_NM_KOR.ToString());
            dls.Add(PDT_TITLE.ToString());
            dls.Add(date.ToString());
            dls.Add(PDT_DAYS_CODE.ToString());
            dls.Add(revDate.ToString());
            dls.Add(lastRev.ToString());

            /* 판매 금액 입력 */

            REV_1 SELL_PRICE = new REV_1();

            SELL_PRICE.CORP_CODE = corp;
            SELL_PRICE.REV_DAY = revDate;
            SELL_PRICE.REV_SEQ = lastRev;

            SELL_PRICE.PDT_TYPE_CODE = pdt_type;
            SELL_PRICE.PDT_IST_EMP_NO = Convert.ToInt32(emp);
            SELL_PRICE.PDT_YY = pdtyy;
            SELL_PRICE.PDT_SEQ = Convert.ToInt32(pdtseq);
            SELL_PRICE.PRC_SEQ = prcCD;
            SELL_PRICE.PRC_CURRENCY_CODE = PRC_CURRENCY_CODE;
            SELL_PRICE.PRC_CONTENT = PRC_CONTENT;

            SELL_PRICE.REV_PRC_SEQ = rpSeq;
            SELL_PRICE.REV_PRC_GB = "A";
            SELL_PRICE.REV_PRC_PRICE = PRC_Adult;
            SELL_PRICE.REV_PRC_CNT = Convert.ToInt32(aCnt);
            SELL_PRICE.REV_PRC_TOTAL_PRICE = PRC_ATotal;
            SELL_PRICE.REV_PRC_DISCOUNT = 0;
            SELL_PRICE.REV_PRC_ADDITIONAL_PRICE = 0;
            SELL_PRICE.REV_PRC_CONTENT = "성인 요금";

            DB.REV_1.Add(SELL_PRICE);

            if (Convert.ToInt32(cCnt) > 0)
            {
                rpSeq++;

                REV_1 SELL_PRICE_child = new REV_1();

                SELL_PRICE_child.CORP_CODE = corp;
                SELL_PRICE_child.REV_DAY = revDate;
                SELL_PRICE_child.REV_SEQ = lastRev;

                SELL_PRICE_child.PDT_TYPE_CODE = pdt_type;
                SELL_PRICE_child.PDT_IST_EMP_NO = Convert.ToInt32(emp);
                SELL_PRICE_child.PDT_YY = pdtyy;
                SELL_PRICE_child.PDT_SEQ = Convert.ToInt32(pdtseq);
                SELL_PRICE_child.PRC_SEQ = prcCD;
                SELL_PRICE_child.PRC_CURRENCY_CODE = PRC_CURRENCY_CODE;
                SELL_PRICE_child.PRC_CONTENT = PRC_CONTENT;

                SELL_PRICE_child.REV_PRC_SEQ = rpSeq;
                SELL_PRICE_child.REV_PRC_GB = "C";
                SELL_PRICE_child.REV_PRC_PRICE = PRC_Child;
                SELL_PRICE_child.REV_PRC_CNT = Convert.ToInt32(cCnt);
                SELL_PRICE_child.REV_PRC_TOTAL_PRICE = PRC_CTotal;
                SELL_PRICE_child.REV_PRC_DISCOUNT = 0;
                SELL_PRICE_child.REV_PRC_ADDITIONAL_PRICE = 0;
                SELL_PRICE_child.REV_PRC_CONTENT = "소아 요금";

                DB.REV_1.Add(SELL_PRICE_child);
            }

            if (Convert.ToInt32(bCnt) > 0)
            {
                rpSeq++;

                REV_1 SELL_PRICE_Baby = new REV_1();

                SELL_PRICE_Baby.CORP_CODE = corp;
                SELL_PRICE_Baby.REV_DAY = revDate;
                SELL_PRICE_Baby.REV_SEQ = lastRev;

                SELL_PRICE_Baby.PDT_TYPE_CODE = pdt_type;
                SELL_PRICE_Baby.PDT_IST_EMP_NO = Convert.ToInt32(emp);
                SELL_PRICE_Baby.PDT_YY = pdtyy;
                SELL_PRICE_Baby.PDT_SEQ = Convert.ToInt32(pdtseq);
                SELL_PRICE_Baby.PRC_SEQ = prcCD;
                SELL_PRICE_Baby.PRC_CURRENCY_CODE = PRC_CURRENCY_CODE;
                SELL_PRICE_Baby.PRC_CONTENT = PRC_CONTENT;

                SELL_PRICE_Baby.REV_PRC_SEQ = rpSeq;
                SELL_PRICE_Baby.REV_PRC_GB = "B";
                SELL_PRICE_Baby.REV_PRC_PRICE = PRC_Baby;
                SELL_PRICE_Baby.REV_PRC_CNT = Convert.ToInt32(bCnt);
                SELL_PRICE_Baby.REV_PRC_TOTAL_PRICE = PRC_BTotal;
                SELL_PRICE_Baby.REV_PRC_DISCOUNT = 0;
                SELL_PRICE_Baby.REV_PRC_ADDITIONAL_PRICE = 0;
                SELL_PRICE_Baby.REV_PRC_CONTENT = "유아 요금";

                DB.REV_1.Add(SELL_PRICE_Baby);
            }

            DB.SaveChanges();

            /* 여행자 정보 입력 */
            var trvName = f["name_kor"].Split(',');
            var trvName_First = f["name_first"].Split(',');
            var trvName_Last = f["name_last"].Split(',');
            var trvGender = f["sex"].Split(',');
            var trvGB = f["GB"].Split(',');
            var trvPhone = f["phone"].Split(',');
            var trvBirth = f["birth"].Split(',');

            var revcuSeq = 1;

            var revcuData = DB.REV_2.Where(w => w.REV_DAY.Equals(revDate))
                                    .Where(w => w.REV_SEQ == lastRev)
                                    .GroupBy(g => new
                                    {
                                        g.CORP_CODE,
                                        g.REV_DAY,
                                        g.REV_SEQ
                                    })
                                    .Select(s => new
                                    {
                                        seq = s.Max(max => max.REV_CU_SEQ),
                                    });

            try { revcuSeq += revcuData.Single().seq; }
            catch { }

            for (int i = 0; i < trvName.Length; i++)
            {
                udtTraveler(revDate, lastRev, revcuSeq + i, "", 0, trvName[i], trvName_First[i], trvName_Last[i], trvGender[i], trvGB[i], trvPhone[i], trvBirth[i]);
            }

            var empData = DB.EMP_0.Where(w => w.EMP_NO == chg_emp_no)
                                  .Select(s => new
                                  {
                                      phone = s.EMP_PHONE1 + s.EMP_PHONE2 + s.EMP_PHONE3
                                  });

            string pCount = "성인 " + aCnt + "명";

            if (Convert.ToInt32(cCnt) > 0)
            {
                pCount += ", 소인 " + cCnt + "명";
            }

            if (Convert.ToInt32(bCnt) > 0)
            {
                pCount += ", 유아 " + bCnt + "명";
            }

            DateTime insDate = DateTime.Now;

            //string rName = CuData.Single().CU_NM_KOR;
            //string rTel = CuData.Single().tel,
            string rTime = insDate.ToString("yyyy년 MM월 dd일 HH시 mm분"),
                   toMail = "hj3bae@ntravels.co.kr";

            toMail = DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_EMAIL;

            sendrMail(code, PDT_TITLE, date, PRC_Total.ToString(), pCount, name, tel, reqTxt, rTime, toMail);

            ////////////////////////////////////////////

            var empTel = DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE1 +
                         DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE2 +
                         DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE3;

            string id = "",
                   cont = "";

            id = "10210";
            id = "12920";

            cont = "[엔타비 예약접수] \r\n" + 
                   "상품 예약이 접수가 되었습니다.\r\n" + 
                   " - 예약번호 : " + revDate + "_" + lastRev + "\r\n" + 
                   " - 예약일 : " + rTime + "\r\n" + 
                   " - 출발일 : " + date + "\r\n" + 
                   " - 상품명 : " + PDT_TITLE + "\r\n" + 
                   " - 대표 예약자 : " + name + "\r\n" + 
                   " - 대표 예약자 연락처 : " + tel + "\r\n" + 
                   " - 인원 : " + pCount + "\r\n" + 
                   " - 상품 요금 : " + PRC_Total.ToString() + " 원\r\n" + 
                   " - 예약자 한마디 : " + reqTxt;

            kakaoMSG(id, cont, empTel);

            if (empTel != "01073440940")
            {
                empTel = "01073440940";    //신팀장님

                kakaoMSG(id, cont, empTel);
            }

            id = "10217";
            cont = "[엔타비 예약접수] \r\n상품 예약이 접수가 되었습니다.\r\n" + 
                   " - 예약번호 : " + revDate + "_" + lastRev + "\r\n" + 
                   " - 출발일 : " + date + "\r\n" + 
                   " - 상세예약조회 : http://ntabi.co.kr \r\n\r\n" + 
                   "담당자 확인 후 다음 절차를 안내해드리겠습니다. \r\n" + 
                   "고객님의 소중한 여행을 저희에게 맡겨 주셔서 진심으로 감사 드립니다. \r\n\r\n" + 
                   "고객센터 : 1670-4601 \r\n" + 
                   "(운영시간 : 평일 9시~6시) \r\n\r\n" + 
                   "여행갈땐 엔타비 바로가기 www.ntabi.co.kr";

            kakaoMSG(id, cont, tel);

            //////////////////////////////////////////////

            /*
             *
             * 홈페이지 고객 예약시 고객 자동 등록
             * 
             */

            int SEQ = 1;

            var seqData = DB.EVE_0.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.EVE_REG_DATE.Equals(strDate))
                                  .GroupBy(g => new { g.EVE_REG_DATE })
                                  .Select(s => new { EVE_SEQ = s.Max(max => max.EVE_SEQ) });

            try
            {
                SEQ = Convert.ToInt32(seqData.Single().EVE_SEQ.ToString()) + 1;
            }
            catch (Exception)
            {

            }

            EVE_0 Data = new EVE_0();

            Data.CORP_CODE = "NTB";
            Data.EVE_DATE = date;//.Substring(0,4) + "-" + date.Substring(5,2) + "-" + date.Substring(6,2);
            Data.EVE_SEQ = SEQ;
            Data.EVE_NAME = name;
            Data.EVE_TEL = tel;
            Data.EVE_EMAIL = string.IsNullOrEmpty(email) ? "-" : email;
            Data.EVE_OFFICE_CODE = "HOM";
            Data.EVE_CNT = totalCnt;
            Data.EVE_ETC = string.IsNullOrEmpty(reqTxt) ? "-" : reqTxt;
            Data.EVE_AREA_CODE = PDT_AREA_CODE;
            Data.EVE_TITLE = PDT_TITLE;
            Data.EVE_REG_DATE = strDate;
            Data.EVE_PRICE = PRC_Total;

            Data.EVE_IST_EMP_NO = chg_emp_no;
            Data.EVE_IST_DATE = date;

            DB.EVE_0.Add(Data);

            DB.SaveChanges();

            /*
             *
             * 홈페이지 고객 예약시 고객 자동 등록
             * 
             */


            var empData2 = DB.EMP_0.ToList()
                                  .Where(w => w.EMP_NO.ToString().Equals(chg_emp_no.ToString()))
                                  .Select(s => new
                                  {
                                      txt = s.EMP_NAME + " | " + s.EMP_TEL1 + "-" + s.EMP_TEL2 + "-" + s.EMP_TEL3 + " | " + s.EMP_EMAIL,
                                  });

            ViewBag.empData = empData2;

            return View(dls);
        }

        public void setRevHotel(string p1, string p2, int p3, string p4, string p5, string p6, string p7, string p8, int p9, int p10, string p11, int p12, string p13, string p14, string p15, string p16, string p17, int p18, int p19, int p20)
        {
            TB_HtlResInfo hrData = new TB_HtlResInfo();

            hrData.CORP_CODE = p1;
            hrData.REV_DAY = p2;
            hrData.REV_SEQ = p3;
            hrData.HTL_SEQ = p4;
            hrData.HTL_NATION_CODE = p5;
            hrData.HTL_AREA_CODE = p6;
            hrData.HTL_TYPE = p7;
            hrData.HTL_ChkIn = p8;
            hrData.Stay_Seq = p9;
            hrData.HTL_StayDays = p10;
            hrData.CU_YY = p11;
            hrData.CU_SEQ = p12;
            hrData.CU_NAME = p13;
            hrData.CU_NAME_FIRST = p14;
            hrData.CU_NAME_LAST = p15;
            hrData.CU_HP = p16;
            hrData.CU_EMAIL = p17;
            hrData.ADULT_CNT = p18;
            hrData.CHILD_CNT = p19;
            hrData.BABY_CNT = p20;
            hrData.HTL_RoomType = "기본(더블)";
            //hrData.Htl_ResComment = p21;

            DB.TB_HtlResInfo.Add(hrData);
            DB.SaveChanges();
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

        // POST: Products/getOptions
        //public ActionResult getOptions(string code, string key, string date, string setTraff, string trf, string prc, string htl, string room)
        public ActionResult getOptions(string code, string key, string date, string inDays)
        {
            ViewBag.date = date;

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
                                    .Where(w => w.PDT_IN == Convert.ToInt32(inDays))
                                    .Where(w => w.PDT_IN != 1 ? w.PDT_CANRES == null || w.PDT_CANRES.Equals("N") : true)
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

            hotelList = hotelList.Join(hotelState,
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

            var HotelLists = hotelList.Join(DB.HTL_0.Where(w => w.HTL_PROC_CODE.Equals("02")),
                                            a => a.PDT_HTL_SEQ,
                                            b => b.HTL_SEQ,
                                           (a, b) => new
                                           {
                                               a.PDT_HTL_SEQ,
                                               b.HTL_NAME,
                                               a.inCnt,
                                           });

            //var PDT_AREA_CODE = DB.PDT_0.ToList()
            //                      .Where(w => w.CORP_CODE.Equals(corp))
            //                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
            //                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
            //                      .Where(w => w.PDT_YY.Equals(pdtyy))
            //                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
            //                      .Select(s => new
            //                      {
            //                          s.PDT_AREA_CODE
            //                      }).FirstOrDefault().PDT_AREA_CODE.ToString();

            //List<string> area = getAreaCode(PDT_AREA_CODE);

            //var chkDateLists = DB.HTL_2.ToList()
            //                           .Where(w => w.HTL_DATE.Equals(date))
            //                           .GroupBy(g => new { g.CORP_CODE, g.HTL_SEQ });

            //var HotelLists = DB.HTL_0.ToList()
            //                         .Where(w => area.Contains(w.HTL_AREA_CODE))
            //                         .Where(w => w.HTL_PROC_CODE.Equals("02"))      // 판매중
            //                         .Join(chkDateLists,
            //                               a => a.CORP_CODE + a.HTL_SEQ,
            //                               b => b.Key.CORP_CODE + b.Key.HTL_SEQ,
            //                              (a, b) => new
            //                              {
            //                                  a.HTL_SEQ,
            //                                  a.HTL_NAME
            //                              });

            //if (setTraff.Equals("1"))
            //{
            //    var trfData = getTraffData(code, key, date);

            //    Dictionary<string, string> trafLists = new Dictionary<string, string>();

            //    string txt = "",
            //           name = "",
            //           value = "",
            //           subseq = "",
            //           tempVal = "",
            //           traffype = "",
            //           titleTxt = "";

            //    foreach (object item in (dynamic)trfData.Data)
            //    {
            //        name = "";
            //        tempVal = "";
            //        traffype = "";

            //        try { name = item.GetType().GetProperties()[3].GetValue(item, null).ToString(); } catch { }
            //        try { subseq = item.GetType().GetProperties()[1].GetValue(item, null).ToString(); } catch { }
            //        try { tempVal = item.GetType().GetProperties()[0].GetValue(item, null).ToString(); } catch { }
            //        try { traffype = item.GetType().GetProperties()[2].GetValue(item, null).ToString(); } catch { }

            //        if (!string.IsNullOrEmpty(value) && value != tempVal) 
            //        {
            //            trafLists.Add(value, txt);

            //            txt = "";
            //        }

            //        switch (traffype)
            //        {
            //            case "AIR":
            //            case "SHP":
            //                if (subseq.Equals("1"))
            //                {
            //                    titleTxt = "출국 편 - ";
            //                }
            //                else if (subseq.Equals("2"))
            //                {
            //                    titleTxt = ", 귀국 편 - ";
            //                }
            //                break;
            //        }

            //        txt += titleTxt + "[" + name + "]";//, 귀국편 - [" + + "]"
            //        value = tempVal;
            //    }

            //    trafLists.Add(value, txt);

            //    ViewBag.trafLists = trafLists;
            //}

            //var priKey = getCode(code, key);

            //string corp = priKey[0];
            //string pdt_type = priKey[1];
            //string emp = priKey[2];
            //string pdtyy = priKey[3];
            //string pdtseq = priKey[4];

            //var daysData2 = DB.PDT_1.ToList()
            //                        .Where(w => w.CORP_CODE.Equals(corp))
            //                        .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
            //                        .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
            //                        .Where(w => w.PDT_YY.Equals(pdtyy))
            //                        .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
            //                        .Where(w => w.PDT_STATE_CODE != "03")
            //                        .Where(w => w.PDT_DATE.Equals(date))
            //                        .GroupBy(g => new { g.PRC_SEQ })
            //                        .Select(s => new
            //                        {
            //                            code,
            //                            PRC_SEQ = s.Key.PRC_SEQ
            //                        });

            //var prcData = DB.PRC_0.ToList()
            //                      .Where(w => w.CORP_CODE.Equals(corp))
            //                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
            //                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
            //                      .Where(w => w.PDT_YY.Equals(pdtyy))
            //                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
            //                      .Join(daysData2,
            //                      a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + a.PRC_SEQ,
            //                      b => b.code + b.PRC_SEQ,
            //                      (a, b) => new
            //                      {
            //                          a.PRC_SEQ,
            //                          a.PRC_Currency_CODE,
            //                          a.PRC_Adult,
            //                          a.PRC_Child,
            //                          a.PRC_Baby,
            //                          a.PRC_EXP,
            //                      });

            //return View(LocalTourLists);
            return View(HotelLists);
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

                //DataSet ds = new DataSet();

                //SqlDataAdapter sda = new SqlDataAdapter("SP_SearchotelPackPrice_Stat", dbconn);

                //sda.SelectCommand.Parameters.AddWithValue("@Corp_Code", Corp_Code);
                //sda.SelectCommand.Parameters.AddWithValue("@Pdt_Type_Code", Pdt_Type_Code);
                //sda.SelectCommand.Parameters.AddWithValue("@Pdt_ist_emp_No", Pdt_ist_emp_No);
                //sda.SelectCommand.Parameters.AddWithValue("@Pdt_yy", Pdt_yy);
                //sda.SelectCommand.Parameters.AddWithValue("@Pdt_Seq", Pdt_Seq);
                //sda.SelectCommand.Parameters.AddWithValue("@ToDate", tDate);
                //sda.SelectCommand.Parameters.AddWithValue("@Term", inCnt);
                //sda.SelectCommand.Parameters.AddWithValue("@ExJPY", 10.5);
                //sda.SelectCommand.Parameters.AddWithValue("@ExUSD", 12);

                //sda.Fill(ds);
                //sda.Dispose();



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
        
        public int getMinPrice(string code, string key, string date)
        {
            var priKeys = getCode(code, key);

            string pdttype = priKeys[1].ToString();
            string pdtyy = priKeys[3].ToString();

            int pdtemp = Convert.ToInt32(priKeys[2].ToString());
            short pdtseq = Convert.ToInt16(priKeys[4].ToString());

            DateTime now = DateTime.Now;

            now = now.AddDays(2);

            var thisMonthPriceData = DB.PDT_1.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                             .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                             .Where(w => w.PDT_YY.Equals(pdtyy))
                                             .Where(w => w.PDT_SEQ == pdtseq)
                                             .Where(w => w.PDT_DATE.Equals(date))
                                             .Select(s => new
                                             {
                                                 s.PRC_SEQ,
                                                 s.TRF_SEQ,
                                                 s.PDT_DATE,
                                             })
                                             .ToList();

            var priceData = DB.PRC_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                    .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                    .Where(w => w.PDT_YY.Equals(pdtyy))
                                    .Where(w => w.PDT_SEQ == pdtseq)
                                    .Select(s => new
                                    {
                                        s.PRC_SEQ,
                                        s.PRC_Currency_CODE,
                                        s.PRC_Adult,
                                        s.PRC_Profit,
                                    });

            var aaaaa = thisMonthPriceData.Join(priceData,
                                                a => a.PRC_SEQ,
                                                b => b.PRC_SEQ,
                                                (a, b) => new
                                                {
                                                    a.TRF_SEQ,
                                                    a.PDT_DATE,

                                                    price = exchangedToDouble(b.PRC_Currency_CODE, (b.PRC_Adult + b.PRC_Profit).ToString()), //b.PRC_Adult + b.PRC_Profit,
                                                })
                                                .ToList();
            return 0;


            var chkCombinHotels = DB.PDT_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                          .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                          .Where(w => w.PDT_YY.Equals(pdtyy))
                                          .Where(w => w.PDT_SEQ == pdtseq)
                                          .FirstOrDefault()
                                          .PDT_COMBINE_HTL;

            if (!string.IsNullOrEmpty(chkCombinHotels) && chkCombinHotels.Equals("Y"))
            {
                var trfData = DB.TRF_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                      .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == pdtseq)
                                      .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"));

                aaaaa = aaaaa.Join(trfData,
                                   a => a.TRF_SEQ,
                                   b => b.TRF_SEQ,
                                  (a, b) => new
                                  {
                                      a.TRF_SEQ,
                                      PDT_DATE = b.TRF_TYPE == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,

                                      a.price,
                                  })
                                  .ToList();

                var hotelData = DB.PDT_4.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq);

                foreach (object hotel in hotelData)
                {
                    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();
                    string hotelDate = aaaaa.FirstOrDefault().PDT_DATE;

                    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
                                                .Where(w => w.HTL_DATE.Equals(hotelDate))
                                                .Select(s => new
                                                {
                                                    s.HTL_DATE,
                                                    s.HTL_PRC_SEQ,
                                                })
                                                .ToList();

                    var hotelPriceData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
                                                 .Select(s => new
                                                 {
                                                     s.HTL_PRC_SEQ,
                                                     s.HTL_Currency_CODE,
                                                     HTL_PRICE = string.IsNullOrEmpty(s.HTL_PRICE.ToString()) ? "0" : s.HTL_PRICE.ToString(),
                                                     HTL_PRICE2 = string.IsNullOrEmpty(s.HTL_PRICE2.ToString()) ? "0" : s.HTL_PRICE2.ToString(),
                                                     HTL_PRICE3 = string.IsNullOrEmpty(s.HTL_PRICE3.ToString()) ? "0" : s.HTL_PRICE3.ToString(),
                                                     HTL_PRICE4 = string.IsNullOrEmpty(s.HTL_PRICE4.ToString()) ? "0" : s.HTL_PRICE4.ToString(),
                                                     s.HTL_PROFIT,
                                                 });

                    var hotelPrice = hotelDateData.Join(hotelPriceData,
                                                        a => a.HTL_PRC_SEQ,
                                                        b => b.HTL_PRC_SEQ,
                                                        (a, b) => new
                                                        {
                                                            a.HTL_DATE,
                                                            b.HTL_Currency_CODE,

                                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                                                        }).ToList();

                    if (!hotelPrice.Any())
                        continue;

                    aaaaa = aaaaa.Join(hotelPrice,
                                       a => a.PDT_DATE,
                                       b => b.HTL_DATE,
                                      (a, b) => new
                                      {
                                          a.TRF_SEQ,
                                          PDT_DATE = DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),

                                          price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),
                                      })
                                      .ToList();
                }
            }

            if (!aaaaa.Any())
            {
                return 0;
            }

            var price = aaaaa.GroupBy(g => new { g.PDT_DATE })
                             .FirstOrDefault()
                             .Min(min => min.price);

            return Convert.ToInt32(price.ToString());
        }

        public int getTotalPrice(string code, string key)
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
                                           s.PDT_DAYS_CODE,
                                           s.PDT_COMBINE_HTL,
                                       });

            string dayCnt = defaultLists.FirstOrDefault().PDT_DAYS_CODE.Substring(2);
            string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

            DateTime thisDate = DateTime.ParseExact("2018-07-14", "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-1);

            var chkPriceLists = DB.PDT_1.ToList()
                                        .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq)
                                        .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > thisDate)
                                        .Where(w => w.PDT_STATE_CODE != "03")
                                        .OrderBy(o => o.PDT_DATE)
                                        .Select(s => new
                                        {
                                            s.PDT_DATE,
                                            s.PDT_STATE_CODE,

                                            s.CORP_CODE,
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            s.TRF_SEQ,
                                            s.PRC_SEQ
                                        });

            var priceDefTable = chkPriceLists.Join(DB.PRC_0,
                                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.PDT_DATE,
                                                      a.PDT_STATE_CODE,

                                                      price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      a.TRF_SEQ,
                                                      a.PRC_SEQ
                                                  });

            var trafDatas = priceDefTable.Join(DB.TRF_0,
                                               a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                               (a, b) => new
                                               {
                                                   a.PDT_DATE,
                                                   a.PDT_STATE_CODE,
                                                   a.price,
                                                   b.TRF_TITLE,
                                                   b.TRF_TYPE,
                                                   b.TRF_SUB_SEQ
                                               });


            var trafTitle = trafDatas.GroupBy(g => new { g.PDT_DATE })
                                        .Select(s => new
                                        {
                                            s.Key.PDT_DATE,
                                            title = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).Any() ? "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "] - [" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).FirstOrDefault().TRF_TITLE + "]" : "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "]",
                                            type = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TYPE,
                                        });

            var priceTable = priceDefTable.Join(trafTitle,
                                            a => a.PDT_DATE,
                                            b => b.PDT_DATE,
                                            (a, b) => new
                                            {
                                                a.PDT_DATE,
                                                b.title,
                                                a.PDT_STATE_CODE,
                                                a.price,

                                                inDate = b.type == "SHP" ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                                            })
                                            .ToList();

            if (!string.IsNullOrEmpty(chkCombinHotels) && chkCombinHotels.Equals("Y"))
            {
                var hotelData = DB.PDT_4.ToList()
                                        .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq)
                                        .OrderBy(o => o.PDT_IN);

                int inCount = 1;

                foreach (object hotel in hotelData)
                {
                    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                    int tempInCnt = Convert.ToInt32(hotel.GetType().GetProperties()[4].GetValue(hotel, null).ToString());

                    var hotelPrice = hotelDateData.ToList()
                                                  .Join(DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq)),
                                                        a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                        b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                                        (a, b) => new
                                                        {
                                                            a.HTL_DATE,
                                                            b.HTL_Currency_CODE,
                                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
                                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                                                        });



                    priceTable = priceTable.Join(hotelPrice,
                                                a => a.inDate,
                                                b => b.HTL_DATE,
                                                (a, b) => new
                                                {
                                                    a.PDT_DATE,
                                                    a.title,
                                                    a.PDT_STATE_CODE,
                                                    price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                                                    inDate = DateTime.ParseExact(a.inDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                                                })
                                                .ToList();

                    inCount = tempInCnt;
                }
            }

            var returnData = priceTable.Select(s => new
            {
                sdt = DateTime.ParseExact(s.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MM.dd (ddd)"),
                edt = DateTime.ParseExact(s.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(Convert.ToInt32(dayCnt) - 1).ToString("MM.dd (ddd)"),
                s.title,
                s.PDT_STATE_CODE,
                s.price,
            })
            .GroupBy(g => new { g.sdt, g.edt, g.title })
            .Select(s => new
            {
                s.Key.sdt,
                s.Key.edt,
                s.Key.title,
                PDT_STATE_CODE = s.Where(w => w.price.ToString().Equals(s.Min(min => min.price).ToString())).Any() ? s.Where(w => w.price.ToString().Equals(s.Min(min => min.price).ToString())).FirstOrDefault().PDT_STATE_CODE : "",
                price = s.Min(min => min.price)
            })
            .Take(4)
            .ToList();

            return 0;
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

        public List<string> getAreaCode(string A)
        {
            List<string> area = new List<string>();

            switch (A)
            {
                case "KYU":
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
                case "GSA":
                    area.Add("SPN");
                    area.Add("GUM");
                    break;
                default:
                    area.Add(A);
                    break;
            }

            return area;
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

        public JsonResult getTrf(string code, string key, string date)
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

        public JsonResult getPrc(string code, string key, string date, int trfCD)
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
                                     //a.PRC_Currency_CODE,
                                     //PRC_Adult = a.PRC_Adult + a.PRC_Profit,
                                     //PRC_Child = a.PRC_Child + a.PRC_Profit,
                                     //PRC_Baby = a.PRC_Baby + a.PRC_Profit,
                                     a.PRC_EXP,
                                 });

            return Json(prcData);
        }

        public JsonResult getDays(string code, string key, string trfCode = "", string prcCode = "")
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

        public JsonResult JSON_getRoom(string HSeq, string OSeq)
        {
            List<string> rName = new List<string>();

            switch(HSeq)
            {
                default:
                    rName.Add("싱글");
                    rName.Add("더블");
                    rName.Add("트윈");
                    rName.Add("트리플");
                    break;
            }

            var roomData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(HSeq))
                                   .Where(w => w.HTL_PRC_SEQ.ToString().Equals(OSeq))
                                   .ToList()
                                   .Select(s => new
                                   {
                                       room1 = !string.IsNullOrEmpty(s.HTL_PRICE.ToString()) ? rName[0] : null,
                                       room2 = !string.IsNullOrEmpty(s.HTL_PRICE2.ToString()) ? rName[1] : null,
                                       room3 = !string.IsNullOrEmpty(s.HTL_PRICE3.ToString()) ? rName[2] : null,
                                       room4 = !string.IsNullOrEmpty(s.HTL_PRICE4.ToString()) ? rName[3] : null,


                                       s.HTL_PRICE,
                                       s.HTL_PRICE2,
                                       s.HTL_PRICE3,
                                       s.HTL_PRICE4
                                   });

            return Json(roomData);
        }

        public JsonResult getOptionData(string seq, string date)
        {
            var chkDateLists = DB.HTL_2.ToList()
                                       .Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                       .Where(w => w.HTL_DATE.Equals(date));

            var roomDatas = DB.HTL_1.ToList()
                                    .Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                    .Join(chkDateLists,
                                          a => a.CORP_CODE + a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                          b => b.CORP_CODE + b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                         (a, b) => new
                                         {
                                             //a.CORP_CODE,
                                             //a.HTL_SEQ,
                                             a.HTL_PRC_SEQ,
                                             //a.HTL_Currency_CODE,
                                             //Price = a.HTL_PRICE + a.HTL_PROFIT,
                                             //a.HTL_ROOM,
                                             a.HTL_EXP,
                                             //maxIn = 1,
                                         });

            //var temp1 = roomDatas.Join(DB.HTL_2.ToList()
            //                                   .Where(w => w.HTL_SEQ.ToString().Equals(seq)),
            //                           a => a.CORP_CODE + a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
            //                           b => b.CORP_CODE + b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
            //                           (a, b) => new
            //                           {
            //                               a.CORP_CODE,
            //                               a.HTL_SEQ,
            //                               a.HTL_PRC_SEQ,
            //                               a.HTL_Currency_CODE,
            //                               a.Price,
            //                               a.HTL_ROOM,
            //                               a.HTL_EXP,
            //                               a.maxIn,
            //                               b.HTL_DATE,
            //                           })
            //                           .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)); ;

            //var temp2 = temp1.Select((s, idx) => new
            //                        {
            //                            s.CORP_CODE,
            //                            s.HTL_SEQ,
            //                            s.HTL_PRC_SEQ,
            //                            s.HTL_Currency_CODE,
            //                            s.Price,
            //                            s.HTL_ROOM,
            //                            s.HTL_EXP,
            //                            s.maxIn,
            //                            s.HTL_DATE,
            //                            idx,
            //                            num = DateTime.ParseExact(s.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-idx),
            //                        });

            //var temp3 = temp2.GroupBy(g => g.num);

            //var temp4 = temp3.Select(s => new
            //                        {
            //                            s.FirstOrDefault().CORP_CODE,
            //                            s.FirstOrDefault().HTL_SEQ,
            //                            s.FirstOrDefault().HTL_PRC_SEQ,
            //                            s.FirstOrDefault().HTL_Currency_CODE,
            //                            s.FirstOrDefault().Price,
            //                            s.FirstOrDefault().HTL_ROOM,
            //                            s.FirstOrDefault().HTL_EXP,
            //                            maxIn = s.Count(cnt => cnt.HTL_SEQ.ToString().Equals(seq)),
            //                            date = s.Min(min => min.HTL_DATE)
            //                        });

            //var temp5 = temp4.Where(w => w.date.Equals(date));

            return Json(roomDatas);
        }

        public JsonResult JSON_getHotels(string code, string key, string date, string eDate = "")
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
                                     .GroupBy(g => new { g.CORP_CODE, g.HTL_SEQ })
                                     .ToList();

            if (eDate != "")
            {
                DateTime sDateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime eDateTime = DateTime.ParseExact(eDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TimeSpan TS = eDateTime - sDateTime;

                int diffDay = TS.Days;

                hotelState = DB.HTL_2.Where(w => w.HTL_STATE_CODE.Equals("01"))
                                     .ToList()
                                     .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= sDateTime)
                                     .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) < eDateTime)
                                     .GroupBy(g => new { g.CORP_CODE, g.HTL_SEQ })
                                     .ToList();
            }

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

        public JsonResult getTraffData(string code, string key, string date)
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
                                    .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq));

            var dateData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                   .Where(w => w.PDT_DATE.Equals(date));

            //var joindata = traffData.Join(dateData,
            //                              a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
            //                              b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
            //                              (a, b) => new
            //                              {
            //                                  a.TRF_SEQ,
            //                                  a.TRF_SUB_SEQ,
            //                                  a.TRF_TYPE,
            //                                  a.TRF_TITLE,
            //                                  a.TRF_SAREA,
            //                                  a.TRF_STIME,
            //                                  a.TRF_EAREA,
            //                                  a.TRF_ETIME,
            //                              })
            //                              .GroupBy(g => new
            //                              {
            //                                  g.TRF_SEQ,
            //                                  g.TRF_SUB_SEQ,
            //                                  g.TRF_TYPE,
            //                                  g.TRF_TITLE,
            //                                  g.TRF_SAREA,
            //                                  g.TRF_STIME,
            //                                  g.TRF_EAREA,
            //                                  g.TRF_ETIME,
            //                              })
            //                              .Select(s => new
            //                              {
            //                                  s.Key.TRF_SEQ,
            //                                  s.Key.TRF_SUB_SEQ,
            //                                  s.Key.TRF_TYPE,
            //                                  TRF_TITLE = (s.Key.TRF_TITLE),
            //                                  s.Key.TRF_SAREA,
            //                                  s.Key.TRF_STIME,
            //                                  s.Key.TRF_EAREA,
            //                                  s.Key.TRF_ETIME,
            //                                  title = getNNNNNNNAsjdkl(s.Key.TRF_TITLE)
            //                              });

            var joindata = traffData.Join(dateData,
                                          a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                          b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                          (a, b) => new
                                          {
                                              a.TRF_SEQ,
                                              a.TRF_SUB_SEQ,
                                              a.TRF_TYPE,
                                              a.TRF_TITLE,
                                              a.TRF_SAREA,
                                              a.TRF_STIME,
                                              a.TRF_EAREA,
                                              a.TRF_ETIME,
                                              title = getNNNNNNNAsjdkl(a.TRF_TITLE)
                                          });

            return Json(joindata);
        }

        public JsonResult getOptionsData(string code, string key, string trf_code, string date)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var prcData = DB.PRC_0.ToList()
                                    .Where(w => w.CORP_CODE.Equals(corp))
                                    .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                    .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                    .Where(w => w.PDT_YY.Equals(pdtyy))
                                    .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq));
                                    //.GroupBy(g => new
                                    //{
                                    //    g.CORP_CODE,
                                    //    g.PDT_TYPE_CODE,
                                    //    g.PDT_IST_EMP_NO,
                                    //    g.PDT_YY,
                                    //    g.PDT_SEQ,
                                    //    g.PRC_SEQ,
                                    //    g.PRC_Currency_CODE,
                                    //    g.PRC_Adult, //.APrice,
                                    //    g.PRC_Child, //.CPrice,
                                    //    g.PRC_Baby, //.BPrice,
                                    //    g.PRC_Profit,
                                    //    g.PRC_EXP,
                                    //})
                                    //.Select(s => new
                                    //{
                                    //    s.Key.CORP_CODE,
                                    //    s.Key.PDT_TYPE_CODE,
                                    //    s.Key.PDT_IST_EMP_NO,
                                    //    s.Key.PDT_YY,
                                    //    s.Key.PDT_SEQ,
                                    //    s.Key.PRC_SEQ,
                                    //    s.Key.PRC_Currency_CODE,
                                    //    s.Key.PRC_Adult,
                                    //    s.Key.PRC_Child,
                                    //    s.Key.PRC_Baby,
                                    //    s.Key.PRC_EXP,
                                    //    s.Key.PRC_Profit,
                                    //});

            var dateData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                   .Where(w => w.PDT_DATE.Equals(date))
                                   .Where(w => w.TRF_SEQ.ToString().Equals(trf_code));

            var joindata = prcData.Join(dateData,
                                          a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                          b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                          (a, b) => new
                                          {
                                              a.PRC_SEQ,
                                              a.PRC_Currency_CODE,
                                              APrice = a.PRC_Adult + a.PRC_Profit,
                                              CPrice = a.PRC_Child + a.PRC_Profit,
                                              BPrice = a.PRC_Baby,
                                              //BPrice = a.PRC_Baby + a.PRC_Profit,
                                              a.PRC_EXP,
                                          });

            return Json(joindata);
        }

        public JsonResult JSON_getFourDateLists(string code, string key, string date)
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
                                           s.PDT_DAYS_CODE,
                                           s.PDT_COMBINE_HTL,
                                       });

            int inCnt = Convert.ToInt32(defaultLists.FirstOrDefault().PDT_DAYS_CODE.Substring(0, 2));
            string dayCnt = defaultLists.FirstOrDefault().PDT_DAYS_CODE.Substring(2);
            string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

            DateTime thisDate = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(-1);

            var chkPriceLists = DB.PDT_1.ToList()
                                        .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq)
                                        .Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > thisDate)
                                        //.Where(w => w.PDT_STATE_CODE != "03")
                                        .OrderBy(o => o.PDT_DATE)
                                        //.Take(4)
                                        .Select(s => new
                                        {
                                            s.PDT_DATE,
                                            s.PDT_STATE_CODE,

                                            s.CORP_CODE,
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            s.TRF_SEQ,
                                            s.PRC_SEQ
                                        });

            var priceDefTable = chkPriceLists.Join(DB.PRC_0,
                                                   a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.PRC_SEQ,
                                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PRC_SEQ,
                                                  (a, b) => new
                                                  {
                                                      a.PDT_DATE,
                                                      a.PDT_STATE_CODE,

                                                      price = exchangedToDouble(b.PRC_Currency_CODE, (Convert.ToInt32(b.PRC_Adult.ToString()) + Convert.ToInt32(b.PRC_Profit.ToString())).ToString()),

                                                      a.CORP_CODE,
                                                      a.PDT_TYPE_CODE,
                                                      a.PDT_IST_EMP_NO,
                                                      a.PDT_YY,
                                                      a.PDT_SEQ,
                                                      a.TRF_SEQ,
                                                      a.PRC_SEQ
                                                  });

            var trafDatas = priceDefTable.Join(DB.TRF_0.Where(w => w.TRF_SUB_SEQ == 1),
                                               a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + a.TRF_SEQ,
                                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.TRF_SEQ,
                                               (a, b) => new
                                               {
                                                   a.PDT_DATE,
                                                   a.PDT_STATE_CODE,
                                                   a.price,
                                                   b.TRF_TITLE,
                                                   b.TRF_TYPE,
                                                   b.TRF_SUB_SEQ
                                               });


            var trafTitle = trafDatas.GroupBy(g => new { g.PDT_DATE })
                                        .Select(s => new
                                        {
                                            s.Key.PDT_DATE,
                                            title = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).Any() ? "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "] - [" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("2")).FirstOrDefault().TRF_TITLE + "]" : "[" + s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TITLE + "]",
                                            type = s.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1")).FirstOrDefault().TRF_TYPE,
                                        });

            List<string> chkTit = new List<string>();

            chkTit.Add("부관");
            chkTit.Add("PK");
            chkTit.Add("카멜");
            chkTit.Add("CM");

            var priceTable = priceDefTable.Join(trafTitle,
                                            a => a.PDT_DATE,
                                            b => b.PDT_DATE,
                                            (a, b) => new
                                            {
                                                a.PDT_DATE,
                                                b.title,
                                                a.PDT_STATE_CODE,
                                                a.price,

                                                inDate = (b.type == "SHP" && chkTit.Any(b.title.Contains)) ? DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : a.PDT_DATE,
                                                
                                                a.CORP_CODE,
                                                a.PDT_TYPE_CODE,
                                                a.PDT_IST_EMP_NO,
                                                a.PDT_YY,
                                                a.PDT_SEQ,
                                            })
                                            .ToList();

            if (!string.IsNullOrEmpty(chkCombinHotels) && chkCombinHotels.Equals("Y"))
            {
                var hotelData = DB.PDT_4.ToList()
                                        .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                        .Where(w => w.PDT_YY.Equals(pdtyy))
                                        .Where(w => w.PDT_SEQ == pdtseq)
                                        .OrderBy(o => o.PDT_IN);

                for(int i = 1; i <= inCnt; i++)
                {
                    var minPHotelData = hotelData.Where(w => w.PDT_IN == i);

                    var hotelData2 = priceTable.Join(hotelData,
                                                     a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO + "_" + i,
                                                     b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO + "_" + b.PDT_IN,
                                                    (a, b) => new
                                                    {
                                                        a.PDT_DATE,
                                                        a.title,
                                                        a.PDT_STATE_CODE,
                                                        a.price,
                                                        a.inDate,
                                                        b.PDT_HTL_SEQ
                                                    });

                    var temp0 = DB.HTL_1.ToList()
                                        .Join(minPHotelData,
                                              a => a.HTL_SEQ,
                                              b => b.PDT_HTL_SEQ,
                                             (a, b) => new 
                                             {
                                                 a.HTL_SEQ,
                                                 a.HTL_PRC_SEQ,
                                                 a.HTL_Currency_CODE,
                                                 price1 = a.HTL_PRICE,
                                                 price2 = a.HTL_PRICE2,
                                                 price3 = a.HTL_PRICE3,
                                                 price4 = a.HTL_PRICE4,
                                                 a.HTL_PROFIT 
                                             });

                    var temp1 = DB.HTL_2.ToList()
                                        .Where(w => w.HTL_STATE_CODE.Equals("01"))
                                        .Join(hotelData2,
                                              a => a.HTL_SEQ + a.HTL_DATE,
                                              b => b.PDT_HTL_SEQ + DateTime.ParseExact(b.inDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(i - 1).ToString("yyyy-MM-dd"),
                                             (a, b) => new
                                             {
                                                 a.HTL_SEQ,
                                                 a.HTL_PRC_SEQ,
                                                 inDate = DateTime.ParseExact(b.inDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(i - 1).ToString("yyyy-MM-dd"),
                                             });

                    var temp2 = temp0.ToList()
                                     .Join(temp1,
                                           a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                           b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                          (a, b) => new 
                                          {
                                              a.HTL_SEQ,
                                              a.HTL_PRC_SEQ,
                                              a.HTL_Currency_CODE,
                                              a.price1,
                                              a.price2,
                                              a.price3,
                                              a.price4,
                                              minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(a.price1.ToString()) ? "0" : a.price1.ToString()),
                                                                     Convert.ToInt32(string.IsNullOrEmpty(a.price2.ToString()) ? "0" : a.price2.ToString()),
                                                                     Convert.ToInt32(string.IsNullOrEmpty(a.price3.ToString()) ? "0" : a.price3.ToString()),
                                                                     Convert.ToInt32(string.IsNullOrEmpty(a.price4.ToString()) ? "0" : a.price4.ToString())) + a.HTL_PROFIT,
                                              b.inDate,
                                          });

                    var temp3 = temp2.GroupBy(g => new { g.inDate, g.HTL_Currency_CODE })
                                     .Select(s => new
                                     {
                                         s.Key.inDate,
                                         minPrice = exchangedToDouble(s.Key.HTL_Currency_CODE, s.Min(min => min.minPrice).ToString()),
                                     });

                    priceTable = priceTable.Join(temp3,
                                                 a => DateTime.ParseExact(a.inDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(i - 1).ToString("yyyy-MM-dd"),
                                                 b => b.inDate,
                                                (a, b) => new
                                                {
                                                    a.PDT_DATE,
                                                    a.title,
                                                    a.PDT_STATE_CODE,
                                                    price = a.price + b.minPrice,
                                                    a.inDate,
                                                    a.CORP_CODE,
                                                    a.PDT_TYPE_CODE,
                                                    a.PDT_IST_EMP_NO,
                                                    a.PDT_YY,
                                                    a.PDT_SEQ,
                                                })
                                                .ToList();
                }

                //foreach (object hotel in hotelData)
                //{
                //    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                //    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                //    int tempInCnt = Convert.ToInt32(hotel.GetType().GetProperties()[4].GetValue(hotel, null).ToString());

                //    var hotelPrice = hotelDateData.ToList()
                //                                  .Join(DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq)),
                //                                        a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                //                                        b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                //                                        (a, b) => new
                //                                        {
                //                                            a.HTL_DATE,
                //                                            b.HTL_Currency_CODE,
                //                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                //                                        });



                //    priceTable = priceTable.Join(hotelPrice,
                //                                a => a.inDate,
                //                                b => b.HTL_DATE,
                //                                (a, b) => new
                //                                {
                //                                    a.PDT_DATE,
                //                                    a.title,
                //                                    a.PDT_STATE_CODE,
                //                                    price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                    inDate = DateTime.ParseExact(a.inDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                //                                })
                //                                .ToList();

                //    inCount = tempInCnt;
                //}
            }

            var returnData = priceTable.Select(s => new
            {
                sdt = DateTime.ParseExact(s.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToString("MM.dd (ddd)"),
                edt = DateTime.ParseExact(s.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(Convert.ToInt32(dayCnt) - 1).ToString("MM.dd (ddd)"),
                s.title,
                s.PDT_STATE_CODE,
                //s.price,
                price = Math.Ceiling(Convert.ToDouble(s.price) * 0.01) * 100
            })
            .GroupBy(g => new { g.sdt, g.edt, g.title })
            .Select(s => new
            {
                s.Key.sdt,
                s.Key.edt,
                title = getNNNNNNNAsjdkl(s.Key.title),
                PDT_STATE_CODE = s.Where(w => w.price.ToString().Equals(s.Min(min => min.price).ToString())).Any() ? s.Where(w => w.price.ToString().Equals(s.Min(min => min.price).ToString())).FirstOrDefault().PDT_STATE_CODE : "",
                price = s.Min(min => min.price)
            })
            .Take(4)
            .ToList();

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
                                           s.PDT_DAYS_CODE,
                                       });

            int inCnt = Convert.ToInt32(defaultLists.FirstOrDefault().PDT_DAYS_CODE.Substring(0, 2));
            string chkCombinHotels = defaultLists.FirstOrDefault().PDT_COMBINE_HTL;

            var thisMonthPriceData5 = DB.PDT_1.ToList()
                                              .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                              .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                              .Where(w => w.PDT_YY.Equals(pdtyy))
                                              .Where(w => w.PDT_SEQ == pdtseq)
                                              .Where(w => w.PDT_STATE_CODE.Equals("02"));

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

                for(int i = 1; i <= inCnt; i++)
                {
                    var minPHotelData = hotelData.Where(w => w.PDT_IN == i);

                    var temtwmpawetmpa = thisMonthPriceData.Join(minPHotelData,
                                                                a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
                                                                b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
                                                                (a, b) => new 
                                                                {
                                                                    b.PDT_HTL_SEQ,
                                                                    a.PDT_DATE
                                                                }).ToList();

                    var asjlfashkjfl = DB.HTL_2.ToList()
                                               .Join(temtwmpawetmpa,
                                                    a => a.HTL_SEQ + a.HTL_DATE,
                                                    b => b.PDT_HTL_SEQ + b.PDT_DATE,//DateTime.ParseExact(b.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                                                    (a, b) => new
                                                    {
                                                        a.HTL_SEQ,
                                                        a.HTL_PRC_SEQ,
                                                        inDate = b.PDT_DATE,//DateTime.ParseExact(b.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                                                    })
                                                    .ToList();

                    var prisjaf = DB.HTL_1.ToList()
                                          .Join(asjlfashkjfl,
                                                a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                                                b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                                               (a, b) => new
                                               {
                                                   b.HTL_SEQ,
                                                   b.inDate,
                                                   a.HTL_PRICE,
                                                   a.HTL_PRICE2,
                                                   a.HTL_PRICE3,
                                                   a.HTL_PRICE4,
                                                   a.HTL_PROFIT,
                                                   a.HTL_Currency_CODE,
                                                   minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(a.HTL_PRICE.ToString()) ? "0" : a.HTL_PRICE.ToString()),
                                                                          Convert.ToInt32(string.IsNullOrEmpty(a.HTL_PRICE2.ToString()) ? "0" : a.HTL_PRICE2.ToString()),
                                                                          Convert.ToInt32(string.IsNullOrEmpty(a.HTL_PRICE3.ToString()) ? "0" : a.HTL_PRICE3.ToString()),
                                                                          Convert.ToInt32(string.IsNullOrEmpty(a.HTL_PRICE4.ToString()) ? "0" : a.HTL_PRICE4.ToString())) + a.HTL_PROFIT,
                                               });

                    var asjfljka = prisjaf.Select(s => new
                    {
                        s.inDate,
                        minPrice = exchangedToDouble(s.HTL_Currency_CODE, s.minPrice.ToString()),
                    });

                    var gajsjflda = asjfljka.GroupBy(g => new { g.inDate })
                                            .Select(s => new
                                            {
                                                s.Key.inDate,
                                                minPrice = s.Min(min => min.minPrice),
                                            });

                    thisMonthPriceData = thisMonthPriceData.Join(gajsjflda,
                                                                 a => a.PDT_DATE,
                                                                 b => b.inDate,
                                                                (a, b) => new
                                                                {
                                                                    a.month,
                                                                    a.days,
                                                                    price = a.price + b.minPrice,

                                                                    a.PDT_TYPE_CODE,
                                                                    a.PDT_IST_EMP_NO,
                                                                    a.PDT_YY,
                                                                    a.PDT_SEQ,
                                                                    PDT_DATE = DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                                                                    a.traff,
                                                                });

                }

                //foreach (object hotel in hotelData)
                //{
                //    string inDays = hotel.GetType().GetProperties()[5].GetValue(hotel, null).ToString();
                //    string crtHtlseq = hotel.GetType().GetProperties()[6].GetValue(hotel, null).ToString();

                //    var hotelDateData = DB.HTL_2.Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq));

                //    var hotelPriceData = DB.HTL_1.ToList().Where(w => w.HTL_SEQ.ToString().Equals(crtHtlseq))
                //                                 .Select(s => new
                //                                 {
                //                                     s.HTL_SEQ,
                //                                     s.HTL_PRC_SEQ,
                //                                     s.HTL_Currency_CODE,
                //                                     HTL_PRICE = string.IsNullOrEmpty(s.HTL_PRICE.ToString()) ? "0" : s.HTL_PRICE.ToString(),
                //                                     HTL_PRICE2 = string.IsNullOrEmpty(s.HTL_PRICE2.ToString()) ? "0" : s.HTL_PRICE2.ToString(),
                //                                     HTL_PRICE3 = string.IsNullOrEmpty(s.HTL_PRICE3.ToString()) ? "0" : s.HTL_PRICE3.ToString(),
                //                                     HTL_PRICE4 = string.IsNullOrEmpty(s.HTL_PRICE4.ToString()) ? "0" : s.HTL_PRICE4.ToString(),
                //                                     s.HTL_PROFIT,
                //                                 });

                //    var hotelPrice = hotelDateData.ToList()
                //                                  .Join(hotelPriceData,
                //                                        a => a.HTL_SEQ + "_" + a.HTL_PRC_SEQ,
                //                                        b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
                //                                        (a, b) => new
                //                                        {
                //                                            a.HTL_DATE,
                //                                            b.HTL_Currency_CODE,
                //                                            //minPrice = getMinPrice(Convert.ToInt32(b.HTL_PRICE), Convert.ToInt32(b.HTL_PRICE2), Convert.ToInt32(b.HTL_PRICE3), Convert.ToInt32(b.HTL_PRICE4)) + b.HTL_PROFIT,

                //                                            minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
                //                                                                   Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
                //                                        });

                //    thisMonthPriceData = thisMonthPriceData.Join(hotelPrice,
                //                                                 a => a.PDT_DATE,
                //                                                 b => b.HTL_DATE,
                //                                                (a, b) => new
                //                                                {
                //                                                    a.month,
                //                                                    a.days,
                //                                                    price = a.price + exchangedToDouble(b.HTL_Currency_CODE, b.minPrice.ToString()),

                //                                                    a.PDT_TYPE_CODE,
                //                                                    a.PDT_IST_EMP_NO,
                //                                                    a.PDT_YY,
                //                                                    a.PDT_SEQ,
                //                                                    PDT_DATE = DateTime.ParseExact(a.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd"),
                //                                                    a.traff,
                //                                                });
                //}
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

        public JsonResult getPriceData(string code, string key, string prcCode, string trfCode, string date, string inHotels = "", string eDate = "")
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
                                      BPrice = s.PRC_Baby == 0 ? 0 : exchangedToDouble(s.PRC_Currency_CODE, (s.PRC_Baby).ToString()),
                                      //BPrice = s.PRC_Baby == 0 ? 0 : exchangedToDouble(s.PRC_Currency_CODE, (s.PRC_Baby + s.PRC_Profit).ToString()),
                                  })
                                  .ToList();

            if (!string.IsNullOrEmpty(eDate))
            {
                string hSeq = inHotels;
                int price = 0;

                DateTime sDateTime = DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                DateTime eDateTime = DateTime.ParseExact(eDate.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture);

                TimeSpan TS = eDateTime - sDateTime;

                int diffDay = TS.Days;

                var tempData = DB.HTL_2.ToList()
                                       .Where(w => w.HTL_SEQ.ToString().Equals(hSeq))
                                       .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= sDateTime)
                                       .Where(w => DateTime.ParseExact(w.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) < eDateTime);


                var tempPrice = tempData.Join(DB.HTL_1,
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
                                                });

                var tempData2 = tempPrice.GroupBy(g => new { g.HTL_SEQ });

                var tempData3 = tempData2.Select(s => new { price = s.Sum(sum => sum.price) });

                price = Convert.ToInt32(tempData3.SingleOrDefault().price.ToString());

                returnData = returnData.Select(s => new
                {
                    s.DATE,
                    APrice = Math.Ceiling(Convert.ToDouble(s.APrice + price) * 0.01) * 100,
                    CPrice = Math.Ceiling(Convert.ToDouble(s.CPrice) * 0.01) * 100,// + price,
                    BPrice = Math.Ceiling(Convert.ToDouble(s.BPrice) * 0.01) * 100,// + price,
                })
                .ToList();
            }
            else if (!string.IsNullOrEmpty(inHotels))
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


                    //string tempPrice = DB.HTL_1.ToList()
                    //                           .Where(w => w.HTL_SEQ.ToString().Equals(hotels[i].Trim().ToString()))
                    //                           .SingleOrDefault()
                    //                           .HTL_PRICE2.ToString();

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

            //var chkSetHotels = DB.PDT_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
            //                           .Where(w => w.PDT_IST_EMP_NO == pdtemp)
            //                           .Where(w => w.PDT_YY.Equals(pdtyy))
            //                           .Where(w => w.PDT_SEQ == pdtseq)
            //                           .FirstOrDefault()
            //                           .PDT_COMBINE_HTL;

            //if (chkSetHotels.Equals("Y"))
            //{
            //    var traff = DB.TRF_0.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
            //                        .Where(w => w.PDT_IST_EMP_NO == pdtemp)
            //                        .Where(w => w.PDT_YY.Equals(pdtyy))
            //                        .Where(w => w.PDT_SEQ == pdtseq)
            //                        .Where(w => w.TRF_SEQ.ToString().Equals(trfCode))
            //                        .Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
            //                        .FirstOrDefault()
            //                        .TRF_TYPE
            //                        .ToString();

            //    var pdtData = DB.PDT_4.Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
            //                          .Where(w => w.PDT_IST_EMP_NO == pdtemp)
            //                          .Where(w => w.PDT_YY.Equals(pdtyy))
            //                          .Where(w => w.PDT_SEQ == pdtseq)
            //                          .ToList()
            //                          .Select(s => new 
            //                          {
            //                              DATE = date,
            //                              SIN_DATE = traff == "SHP" ? DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays(1).ToString("yyyy-MM-dd") : date,

            //                              s.PDT_IN,
            //                              s.PDT_HTL_SEQ
            //                          });

            //    var rData4 = pdtData.Join(DB.HTL_2.Where(w => w.HTL_STATE_CODE.Equals("01")),
            //                              a => a.PDT_HTL_SEQ + a.SIN_DATE,
            //                              b => b.HTL_SEQ + b.HTL_DATE,
            //                             (a, b) => new
            //                             {
            //                                 a.PDT_HTL_SEQ,

            //                                 a.DATE,
            //                                 a.SIN_DATE,

            //                                 HTL_DATE = DateTime.ParseExact(b.HTL_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((a.PDT_IN) - 1).ToString("yyyy-MM-dd"),
            //                             });


            //    var rData5 = rData4.Join(DB.HTL_2.Where(w => w.HTL_STATE_CODE.Equals("01")),
            //                             a => a.PDT_HTL_SEQ + "_" + a.HTL_DATE,
            //                             b => b.HTL_SEQ + "_" + b.HTL_DATE,
            //                            (a, b) => new
            //                            {
            //                                a.PDT_HTL_SEQ,

            //                                a.DATE,
            //                                a.SIN_DATE,
            //                                a.HTL_DATE,

            //                                b.HTL_PRC_SEQ,
            //                            });

            //    var rData6 = rData5.Join(DB.HTL_1,
            //                             a => a.PDT_HTL_SEQ + "_" + a.HTL_PRC_SEQ,
            //                             b => b.HTL_SEQ + "_" + b.HTL_PRC_SEQ,
            //                            (a, b) => new
            //                            {
            //                                a.DATE,
            //                                a.SIN_DATE,
            //                                a.HTL_DATE,

            //                                b.HTL_Currency_CODE,

            //                                minPrice = getMinPrice(Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE.ToString()) ? "0" : b.HTL_PRICE.ToString()),
            //                                                       Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE2.ToString()) ? "0" : b.HTL_PRICE2.ToString()),
            //                                                       Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE3.ToString()) ? "0" : b.HTL_PRICE3.ToString()),
            //                                                       Convert.ToInt32(string.IsNullOrEmpty(b.HTL_PRICE4.ToString()) ? "0" : b.HTL_PRICE4.ToString())) + b.HTL_PROFIT,
            //                            });

            //    var rData7 = rData6.Select(s => new
            //                       {
            //                           s.DATE,

            //                           hotelPrice = exchangedToDouble(s.HTL_Currency_CODE, s.minPrice.ToString()),
            //                       });

            //    var rData8 = rData7.GroupBy(g => new { g.DATE })
            //                       .Select(s => new
            //                       {
            //                           s.Key.DATE,

            //                           totalPrice = s.Sum(sum => sum.hotelPrice),
            //                       });

            //    var returnData2 = returnData.Join(rData8,
            //                                     a => a.DATE,
            //                                     b => b.DATE,
            //                                    (a, b) => new
            //                                    {
            //                                        a.DATE,
            //                                        APrice = a.APrice == 0 ? 0 : Convert.ToDouble(a.APrice + b.totalPrice),
            //                                        BPrice = a.BPrice == 0 ? 0 : Convert.ToDouble(a.BPrice + b.totalPrice),
            //                                        CPrice = a.CPrice == 0 ? 0 : Convert.ToDouble(a.CPrice + b.totalPrice),
            //                                    })
            //                                    .ToList();

            //    return Json(returnData2);
            //}

            return Json(returnData);
        }

        public JsonResult exchanged(string currency, string money)
        {
            double exchanged_USD = 12,
                   exchanged_JPY = 10.5,
                   exchanged_KRW = 1,
                   returnMoney = 0;

            switch(currency)
            {
                case "JPY":
                    returnMoney = Convert.ToInt32(money) * exchanged_JPY;
                    break;
                case "USD":
                    returnMoney = Convert.ToInt32(money) * exchanged_USD;
                    break;
                case "KRW":
                    returnMoney = Convert.ToInt32(money) * exchanged_KRW;
                    break;
            }

            return Json(returnMoney);
        }

        /*
         * 소켓 통신 
         */ 

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

                try
                {
                    var cntRoomData = DB.CHAT_Room.Select(s => new { cnt = s.CHAT_Room_ID }).Max(m => m.cnt);

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

        public void JSON_chkNewMessage(string chatID)
        {
            var readChat = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(chatID))
                                          .Where(w => w.CHAT_User_YY.Equals("ADMN"))
                                          .Where(w => w.CHAT_isNew.Equals("Y"));

            if (readChat.Any())
            {
                foreach (var item in readChat)
                {
                    item.CHAT_isNew = "N";
                }

                DB.SaveChanges();
            }
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

            switch(type)
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
                //.Where(w => w.PDT_TYPE_CODE.Equals("FT"))
                //.Where(w => w.PDT_IST_EMP_NO == 20)
                //.Where(w => w.PDT_YY.Equals("18"))
                //.Where(w => w.PDT_SEQ == 3)
                //.Take(5)
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

            /*

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
                                   });

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
                                   .OrderByDescending(o => o.PDT_UDT_DATE);
            }
            else
            {
                List<string> area = getAreaCode(A);

                PdtLists = PdtLists.Where(w => area.Contains(w.PDT_AREA_CODE));
            }

            PdtLists = PdtLists.Where(w => w.PDT_TYPE_CODE.Equals(tourType));

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

            PdtLists = PdtLists.Where(w => imgLists.Contains(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ));

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

            var firstLists = rData0.Join(DB.PDT_2,
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
                                            a.PDT_COMBINE_HTL,
                                            inCnt = a.PDT_DAYS_CODE.Substring(0, 2),
                                            b.PDT_IMG,
                                            a.PDT_DAYS_CODE,
                                            a.PDT_OPTIONS,
                                            a.PDT_ORDER_NO
                                        }).ToList();

            DateTime now = DateTime.Now;

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

            var chkDateLists = DB.PDT_1.ToList().Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now);

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

            var secondLists = firstLists.Join(minPriceLists,
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
                                                  minPrice = b.minPrice.ToString(),
                                                  a.PDT_COMBINE_HTL,
                                                  a.inCnt,
                                                  a.PDT_DAYS_CODE,
                                                  a.PDT_OPTIONS,
                                                  b.TRF_SEQ,
                                                  DATE = "",
                                                  a.PDT_ORDER_NO,
                                              });

            var getHotelDatas = secondLists.Join(DB.PDT_4,
                                                 a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                                 b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                (a, b) => new
                                                {
                                                    a.CORP_CODE,
                                                    a.PDT_TYPE_CODE,
                                                    a.PDT_IST_EMP_NO,
                                                    a.PDT_YY,
                                                    a.PDT_SEQ,
                                                    minPrice = a.PDT_COMBINE_HTL == "N" ? a.minPrice : a.minPrice//(Convert.ToInt32(a.minPrice) + getHotelMinPrice(a.CORP_CODE, a.PDT_TYPE_CODE, a.PDT_IST_EMP_NO, a.PDT_YY, a.PDT_SEQ, nowDate, "", Convert.ToInt32(a.inCnt))).ToString(),
                                                })
                                                .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                                .Select(s => new
                                                {
                                                    s.Key.CORP_CODE,
                                                    s.Key.PDT_TYPE_CODE,
                                                    s.Key.PDT_IST_EMP_NO,
                                                    s.Key.PDT_YY,
                                                    s.Key.PDT_SEQ,
                                                    minPrice = s.Min(min => min.minPrice),
                                                });

            secondLists = from f in secondLists
                          join j in getHotelDatas on f.CORP_CODE + f.PDT_TYPE_CODE + f.PDT_IST_EMP_NO + f.PDT_YY + f.PDT_SEQ + "_" + f.PDT_IST_EMP_NO equals j.CORP_CODE + j.PDT_TYPE_CODE + j.PDT_IST_EMP_NO + j.PDT_YY + j.PDT_SEQ + "_" + j.PDT_IST_EMP_NO into jj
                          from jf in jj.DefaultIfEmpty()
                          select new
                          {
                              f.CORP_CODE,
                              f.PDT_TYPE_CODE,
                              f.PDT_IST_EMP_NO,
                              f.PDT_YY,
                              f.PDT_SEQ,
                              f.PDT_TITLE,
                              f.PDT_CONTENT,
                              f.PDT_IMG,
                              minPrice = jf != null ? jf.minPrice : f.minPrice,
                              f.PDT_COMBINE_HTL,
                              f.inCnt,
                              f.PDT_DAYS_CODE,
                              f.PDT_OPTIONS,
                              f.TRF_SEQ,
                              DATE = "",
                              f.PDT_ORDER_NO,
                          };

            secondLists = secondLists.Join(dateLists,
                                           a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                           b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
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
                                              a.inCnt,
                                              a.PDT_DAYS_CODE,
                                              a.PDT_OPTIONS,
                                              a.TRF_SEQ,
                                              DATE = b.Date,
                                              a.PDT_ORDER_NO,
                                          });

            var traffData = DB.TRF_0.ToList();

            if (traff.Length == 3)
            {
                traffData = DB.TRF_0.Where(w => w.TRF_TYPE.Equals(traff)).ToList();
            }

            List<string> chkTrf = new List<string>();

            switch(trf)
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
                case "7C":
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
            }

            if (chkTrf.Count != 0)
            {

            traffData = traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
                                 .Where(w => chkTrf.Any(w.TRF_TITLE.Contains) )//chkTrf.Contains(w.TRF_TITLE))
                                 .ToList();
            }

            var rData00 = secondLists.Join(traffData,
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
                                      .OrderBy(o => new { o.minPrice, o.PDT_ORDER_NO });
                                      //.ThenBy(t => t.minPrice);

            return Json(rData00);
            */
        }


        public void udtTraveler(string rDay, int rSeq, int rcSeq, string cuyy, int cuSeq, string name = "", string first = "", string last = "", string sex = "", string gb = "", string tel = "", string birth = "", string content = "")
        {
            REV_2 Data = new REV_2();

            Data.CORP_CODE = "NTB";
            Data.REV_DAY = rDay;
            Data.REV_SEQ = rSeq;
            Data.REV_CU_SEQ = rcSeq;

            Data.CU_YY = cuyy == "" ? null : cuyy;

            if (cuSeq == 0)
            {
                Data.CU_SEQ = null;
            }
            else
            {
                Data.CU_SEQ = cuSeq;
            }

            Data.CU_NAME = name;
            Data.CU_NAME_FIRST = first;
            Data.CU_NAME_LAST = last;
            Data.CU_SEX = sex;
            Data.CU_GB = gb;
            Data.CU_TEL = tel;
            Data.CU_Birthdate = birth;
            Data.REV_CONTENT = content;

            DB.REV_2.Add(Data);

            DB.SaveChanges();
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

        /*
         * ----------고객용------------
         * 10072 : 회원 가입시
         * 10073 : 문의 글 작성시
         * 10212 : 답변 글 작성시
         * 10075 : 예약시 -> 10217
         * 10076 : 회원 탈퇴시
         * 10077 : 카드 결제시
         * 
         * ----------담당자용------------
         * 10209 : 문의 글 작성시
         * 10210 : 예약시
         * 10211 : 카드 결제시
         */
        public void kakaoMSG(string id, string content, string tel)
        {
            TSMS_AGENT_MESSAGE MSG = new TSMS_AGENT_MESSAGE();

            MSG.SERVICE_SEQNO = 1610001783;
            MSG.RESERVE7 = id;
            MSG.SEND_MESSAGE = content;
            MSG.SUBJECT = "엔타비입니다.";
            MSG.BACKUP_MESSAGE = content;
            MSG.BACKUP_PROCESS_CODE = "102";
            MSG.SEND_FLAG = "N";
            MSG.IMG_SEND_FLAG = "N";
            MSG.MESSAGE_TYPE = "002";
            MSG.CONTENTS_TYPE = "004";
            MSG.RECEIVE_MOBILE_NO = tel;
            MSG.CALLBACK_NO = tel;
            MSG.JOB_TYPE = "B00";
            MSG.COLOR_TYPE = "C01";
            MSG.REGISTER_DATE = DateTime.Now;
            MSG.REGISTER_BY = "TESTER";

            NDB.TSMS_AGENT_MESSAGE.Add(MSG);
            NDB.SaveChanges();
        }

        public void sendrMail(string code, string pName, string sDays, string tPrice, string pCount, string rName, string rTel, string rTxt, string rTime, string toMail)
        {
            string _senderID = "ntravels@ntravels.co.kr";
            string _senderName = "NTravels";

            var _title = "[엔타비] 예약이 접수 되었습니다.";

            var _body = "<table class='__se_tbl' border='0' cellpadding='0' cellspacing='1' style='background-color: rgb(199, 199, 199);'>" +
                           "<tbody><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;상품 코드</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + code + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;상품 명</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + pName + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;출발 일</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + sDays + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;상품 요금&nbsp;</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + tPrice + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;인원</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + pCount + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;대표 예약자</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + rName + "</span></p>" +
                               "</td>" +
                           "</tr><tr>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 159px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;대표 예약자 연락처</span></p>" +
                               "</td>" +
                               "<td style='padding: 3px 4px 2px; color: rgb(102, 102, 102); width: 279px; height: 19px; background-color: rgb(255, 255, 255);'>" +
                                   "<p><span style='color: rgb(0, 0, 0);'>&nbsp;</span><span style='color: rgb(0, 0, 0); font-size: 13.3333px; line-height: 1.5;'>" + rTel + "</span></p>" +
                               "</td>" +
                           "</tr></tbody>" +
                       "</table><p></p><p><br></p><p>* 예약자 한마디<br>" + rTxt + "</p><p></p><p><br></p><p>" + rTime + " 에 예약신청이 확인 되었습니다.</p>";

            try
            {
                MailMessage _message = new MailMessage();
                _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                _message.To.Add("hj3bae@ntravels.co.kr");
                _message.To.Add(toMail);
                _message.Subject = _title;
                _message.SubjectEncoding = System.Text.Encoding.UTF8;
                _message.Body = _body;
                _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                SmtpClient server = new SmtpClient("smtp.worksmobile.com", 587);
                //server.
                server.EnableSsl = true;
                server.UseDefaultCredentials = false;
                server.DeliveryMethod = SmtpDeliveryMethod.Network;
                server.Credentials = new System.Net.NetworkCredential("ntravels@ntravels.co.kr", "ntravels1!");
                server.Send(_message);
            }
            catch (ArgumentException error)
            {
                ViewBag.err = error;
            }
        }
    }
}