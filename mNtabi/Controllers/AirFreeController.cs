using mNtabi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class AirFreeController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NTB";

        // GET: AirFree
        public ActionResult Index()
        {
            var PdtLists = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                   .Where(w => w.PDT_OPTIONS.Equals("airf"))
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
                                           a.PDT_NATION_CODE,
                                           a.PDT_AREA_CODE,
                                           b.PDT_IMG
                                       })
                                       .OrderBy(o => o.PDT_ORDER_NO);

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

            traffData = traffData.Where(w => w.TRF_SUB_SEQ.ToString().Equals("1"))
                //.Where(w => chkTrf.Any(w.TRF_TITLE.Contains))//chkTrf.Contains(w.TRF_TITLE))
                                 .ToList();

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
                                      });
            //.OrderBy(o => new { o.minPrice, o.PDT_ORDER_NO });
            return View(rData00);
            //return View();
        }
    }
}