using NDayTrip.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace NDayTrip.Controllers
{
    public class ProductsController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NDT";

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
                case "TOK":
                case "TYO":
                    area.Add("TOK");
                    area.Add("TYO");
                    break;
                case "OSA":
                    area.Add("OSA");
                    break;
                case "HOK":
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
                case "NGY":
                    area.Add("NGY");
                    break;
                case "HKG":
                    area.Add("HKG");
                    break;
                case "TPE":
                    area.Add("TPE");
                    break;
                case "DAD":
                    area.Add("DAD");
                    break;
                case "CEB":
                    area.Add("CEB");
                    break;
                case "HNL":
                    area.Add("HNL");
                    break;
                case "GUM":
                    area.Add("GUM");
                    break;
                case "SPN":
                    area.Add("SPN");
                    break;
            }

            return area;
        }

        // GET: Products
        public ActionResult Index()
        {
            return View();
        }

        // GET: Products/Lists
        public ActionResult Lists(string loc)
        {
            string area_Code = "";

            switch(loc)
            {
                case "TOKYO":
                    area_Code = "TOK";
                    break;
                case "OKINAWA":
                    area_Code = "OKI";
                    break;
                case "OSAKA":
                    area_Code = "OSA";
                    break;
                case "TSUSHIMA":
                    area_Code = "TSU";
                    break;
                case "FUKUOKA":
                    area_Code = "KYU";
                    break;
                case "HOKKAIDO":
                    area_Code = "HOK";
                    break;
                case "NAGOYA":
                    area_Code = "NGY";
                    break;
            }

            var aaa = getAreaCode(area_Code);

            var defaultLists = DB.PDT_0.Where(w => aaa.Contains(w.PDT_AREA_CODE))
                                       .Where(w => w.PDT_TYPE_CODE.Equals("LT"))
                                       .Where(w => w.PDT_Option1.Equals("Y"))
                                       .Where(w => w.PDT_Option2.Equals("Y"))
                                       .Where(w => w.PDT_Option3.Equals("Y"))
                                       .Where(w => w.PDT_Option4.Equals("Y"))
                                       .Where(w => w.PDT_PROC_CODE.Equals("02"))
                                       .Select(s => new
                                       {
                                           Code     = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO  + s.PDT_YY + s.PDT_SEQ,
                                           Title    = s.PDT_TITLE,
                                           Content  = s.PDT_CONTENT,
                                           Option   = s.PDT_OPTIONS,
                                           s.PDT_ORDER_NO
                                       })
                                       .OrderBy(o => o.PDT_ORDER_NO)
                                       .ToList();

            DateTime now = DateTime.Now;

            var chkDayLists = defaultLists.Join(DB.PDT_1,
                                               a => a.Code,
                                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO  + b.PDT_YY + b.PDT_SEQ,
                                               (a, b) => new
                                               {
                                                   a.Code,
                                                   a.Title,
                                                   a.Content,
                                                   a.Option,
                                                   b.PDT_DATE,
                                               }).ToList().Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                               .GroupBy(g => new { g.Code, g.Title, g.Content, g.Option })
                                               .Select(s => new
                                               {
                                                   s.Key.Code,
                                                   s.Key.Title,
                                                   s.Key.Content,
                                                   s.Key.Option,
                                               });

            var pdt_ImgLists = chkDayLists.Join(DB.PDT_2,
                                                a => a.Code,
                                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO  + b.PDT_YY + b.PDT_SEQ,
                                                (a, b) => new
                                                {
                                                    a.Code,
                                                    a.Title,
                                                    a.Content,
                                                    a.Option,
                                                    b.PDT_IMG,
                                                });

            var minPriceLists = DB.PRC_0.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                        .Select(s => new
                                        {
                                            Code        = s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO  + s.Key.PDT_YY + s.Key.PDT_SEQ,
                                            Currency    = s.Min(min => min.PRC_Currency_CODE),
                                            Price       = s.Min(min => min.PRC_Adult + min.PRC_Profit),
                                            key         = s.Key.PDT_IST_EMP_NO,
                                        });

            var PdtLists = pdt_ImgLists.Join(minPriceLists,
                                            a => a.Code,
                                            b => b.Code,
                                            (a, b) => new
                                            {
                                                a.Code,
                                                a.Title,
                                                a.Content,
                                                a.Option,
                                                a.PDT_IMG,
                                                b.Currency,
                                                b.Price,
                                                b.key,
                                            });

            return View(PdtLists);
        }

        // GET: View
        public ActionResult Views(string code, string key)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Redirect("/");
            }

            var priKeys = getCode(code, key);

            string pdttype = priKeys[1].ToString();
            string pdtyy = priKeys[3].ToString();

            int pdtemp = Convert.ToInt32(priKeys[2].ToString());
            short pdtseq = Convert.ToInt16(priKeys[4].ToString());

            var defaultLists = DB.PDT_0//.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
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
                                       });

            var pdt_ImgLists = defaultLists.Join(DB.PDT_2,
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
                                                });

            var pdt_SchLists = pdt_ImgLists.Join(DB.PDT_3,
                                                a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO  + b.PDT_YY + b.PDT_SEQ,
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
                                                    b.SCH_CONT
                                                });

            var minPriceLists = DB.PRC_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                        .GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                        .Select(s => new
                                        {
                                            //Code = s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO + s.Key.PDT_YY + s.Key.PDT_SEQ,
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
                                                Code = code,
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
                                            });

            var getDateData = DB.PDT_1.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
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
                                       });

            ViewBag.emp = empData;

            var rvData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                      .Where(w => w.post_type.Equals("review"))
                //.Where(w => ("NTB" + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdttype))
                                      .Where(w => w.PDT_IST_EMP_NO == pdtemp)
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == pdtseq)
                                      .Where(w => w.DEL_FG == "N")
                                      .Select(s => new
                                      {
                                          s.post_content,
                                          s.post_options,
                                          //s.ist_date,
                                          date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                          //s.CU_YY,
                                          //s.CU_SEQ
                                          s.name
                                      })
                                      .ToList();

            //var rvData2 = rvData.Join(nDB.CU001.ToList(),
            //                         a => a.CU_YY + a.CU_SEQ,
            //                         b => b.CU_YY + b.CU_SEQ,
            //                         (a, b) => new
            //                         {
            //                             a.post_content,
            //                             a.post_options,
            //                             date = a.ist_date.Substring(0, 4) + "-" + a.ist_date.Substring(4, 2) + "-" + a.ist_date.Substring(6, 2),
            //                             b.CU_NM_KOR,
            //                             b.EMAIL
            //                         });

            ViewBag.rvData = rvData;

            return View(PdtLists);
        }

        public ActionResult reserve(FormCollection f)
        {
            string date = f["date"],
                   prcCD = f["prcCD"],
                   trfCD = f["trfCD"],
                   code = f["code"],
                   key = f["key"],
                   aCnt = f["adult_cnt"],
                   cCnt = f["child_cnt"],
                   bCnt = f["baby_cnt"];

            var user = Session["user"] as User;

            if (string.IsNullOrEmpty(code) || user == null)
            {
                return Redirect("/");
            }

            var pdtData = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO  + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                  .Join(DB.EMP_0,
                                       a => a.PDT_EMP_NO,
                                       b => b.EMP_NO,
                                       (a, b) => new
                                       {
                                           title    = a.PDT_TITLE,
                                           emp_Name = b.EMP_NAME,
                                           emp_Tel  = b.EMP_TEL1 + "-" + b.EMP_TEL2 + "-" + b.EMP_TEL3,
                                           emp_Mail = b.EMP_EMAIL
                                       });

            var userData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login))
                                    .Select(s => new { 
                                        s.CU_NM_KOR,
                                        s.CU_NM_ENG_FIRST,
                                        s.CU_NM_ENG_LAST,
                                        s.EMAIL,
                                        s.HANDPHONE1,
                                        s.HANDPHONE2,
                                        s.HANDPHONE3,
                                        s.BIRTHDAY,
                                        s.CU_HOME_ADDR,
                                        s.SEX,
                                        s.CU_YY,
                                        s.CU_SEQ,
                                    });

            ViewBag.trf = getTrf(code, date);
            ViewBag.prc = getOptions(code, date);
            ViewBag.prcCD = prcCD;
            ViewBag.date = date;
            ViewBag.aCnt = aCnt;
            ViewBag.cCnt = cCnt;
            ViewBag.bCnt = bCnt;
            ViewBag.code = code;
            ViewBag.key = key;
            ViewBag.user = userData;

            return View(pdtData);
        }

        public ActionResult setRev(FormCollection f)
        {
            string date     = f["date"],
                   code     = f["code"],
                   key      = f["key"],
                   aCnt     = f["aCnt"],
                   cCnt     = f["cCnt"],
                   bCnt     = f["bCnt"],
                   user_YY  = f["user_YY"],
                   reqTxt   = f["reqTxt"];

            int prcCD    = Convert.ToInt32(f["prcCD"]),
                trfCD    = Convert.ToInt32(f["trfCD"]),
                user_SEQ = Convert.ToInt32(f["user_SEQ"]);

            string email = f["revEmail"],
                   tel = f["revTel1"] + "-" + f["revTel2"] + "-" + f["revTel3"];

            var user = Session["user"] as User;

            if (string.IsNullOrEmpty(code) || user == null)
            {
                return Redirect("/");
            }

            var priKey      = getCode(code, key);

            string corp     = priKey[0];
            string pdt_type = priKey[1];
            string emp      = priKey[2];
            string pdtyy    = priKey[3];
            string pdtseq   = priKey[4];

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

            int emp_no  = Convert.ToInt32(emp),
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

            int totalCnt    = Convert.ToInt32(aCnt);
            int PRC_Adult   = (Convert.ToInt32(prcData.Single().PRC_Adult.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(aCnt),
                PRC_Child   = (Convert.ToInt32(prcData.Single().PRC_Child.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(cCnt),
                PRC_Baby    = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(bCnt),
                PRC_ATotal  = PRC_Adult * Convert.ToInt32(aCnt),
                PRC_CTotal  = PRC_Child * Convert.ToInt32(cCnt),
                PRC_BTotal  = PRC_Baby * Convert.ToInt32(bCnt),
                PRC_Total   = PRC_ATotal + PRC_CTotal + PRC_BTotal;

            string PRC_CURRENCY_CODE = prcData.Single().PRC_Currency_CODE.ToString(),
                   PRC_CONTENT = prcData.Single().PRC_EXP.ToString();

            if (cCnt != "0")
            {
                totalCnt += Convert.ToInt32(cCnt);
            }

            if (bCnt != "0")
            {
                totalCnt += Convert.ToInt32(bCnt);
            }

            var CuData = nDB.CU001.Where(w => w.CU_YY.Equals(user_YY))
                                  .Where(w => w.CU_SEQ == user_SEQ)
                                  .Select(s => new
                                  {
                                      s.CU_NM_KOR,
                                      tel = s.CO_TEL1 + s.CO_TEL2 + s.CO_TEL3
                                  });

            REV_0 newReserv = new REV_0();

            newReserv.CORP_CODE         = corp;
            newReserv.PDT_TYPE_CODE     = pdt_type;
            newReserv.PDT_IST_EMP_NO    = Convert.ToInt32(emp);
            newReserv.PDT_YY            = pdtyy;
            newReserv.PDT_SEQ           = Convert.ToInt32(pdtseq);
            newReserv.PDT_TITLE         = PDT_TITLE;
            newReserv.PDT_DAYS_CODE     = PDT_DAYS_CODE;
            newReserv.CU_YY             = user_YY;
            newReserv.CU_SEQ            = Convert.ToInt32(user_SEQ);
            newReserv.CU_NAME           = CuData.Single().CU_NM_KOR;

            newReserv.REV_DAY           = revDate;
            newReserv.REV_SEQ           = lastRev;
            newReserv.REV_CNT           = totalCnt;
            newReserv.REV_STATE         = "20";
            newReserv.REV_STARTDAY      = date;
            newReserv.ADULT_CNT         = Convert.ToInt32(aCnt);
            newReserv.CHILD_CNT         = Convert.ToInt32(cCnt);
            newReserv.BABY_CNT          = Convert.ToInt32(bCnt);
            newReserv.REV_PRICE         = PRC_Total;
            newReserv.REV_CHK_PRICE     = "N";
            
            newReserv.IST_EMP_NO        = chg_emp_no;                   // 등록사원
            newReserv.CHG_EMP_NO        = chg_emp_no;                   // 담당사원
            newReserv.SEL_EMP_NO        = chg_emp_no;                   // 판매사원
            newReserv.UDT_EMP_NO        = chg_emp_no;                   // 수정사원

            newReserv.IST_DATE          = strDate;
            newReserv.UDT_DATE          = strDate;
            newReserv.SEL_DATE          = strDate;

            DB.REV_0.Add(newReserv);

            DB.SaveChanges();

            /* 판매 금액 입력 */
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
            var trvName         = f["trvName"].Split(',');
            var trvName_First   = f["trvName_First"].Split(',');
            var trvName_Last    = f["trvName_Last"].Split(',');
            var trvGender       = f["trvGender"].Split(',');
            var trvGB           = f["trvGB"].Split(',');

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

            for(int i = 0; i < trvName.Length; i++)
            {
                udtTraveler(revDate, lastRev, revcuSeq + i, "", 0, trvName[i], trvName_First[i], trvName_Last[i], trvGender[i], trvGB[i]);
            }

            var empData = DB.EMP_0.Where(w => w.EMP_NO == chg_emp_no)
                                  .Select(s => new
                                  {
                                      phone = s.EMP_PHONE1 + s.EMP_PHONE2 + s.EMP_PHONE3
                                  });

            //string id = "10210";

            //string cont = "[엔데이트립 예약접수] \r\n상품 예약이 접수가 되었습니다.\r\n - 예약번호 : " + revDate + lastRev + "\r\n - 예약일 : " + strDate + "\r\n - 출발일 : " + date + "\r\n - 상품명 : " + PDT_TITLE + "\r\n - 대표 예약자 : " + CuData.Single().CU_NM_KOR + "\r\n - 대표 예약자 연락처 : " + CuData.Single().tel + "\r\n - 인원 : " + totalCnt + "\r\n - 상품 요금 : " + PRC_Total + " ( 1인 기준 )\r\n - 예약자 한마디 : " + reqTxt;
            //       cont = "[엔타비 예약접수] \r\n상품 예약이 접수가 되었습니다.\r\n - 예약번호 : " + nRe.RES_DAY + nRe.RES_SEQ + "\r\n - 예약일 : " + rTime + "\r\n - 출발일 : " + sDays + "\r\n - 상품명 : " + pName + "\r\n - 대표 예약자 : " + rName + "\r\n - 대표 예약자 연락처 : " + rTel + "\r\n - 인원 : " + pCount + "\r\n - 상품 요금 : " + tPrice + " ( 1인 기준 )\r\n - 예약자 한마디 : " + rTxt;

            //string tel = empData.Single().phone;

            //kakaoMSG(id, cont, tel);

            //return Redirect("/MyPage");
            //newReserv.ADULT_CNT         = Convert.ToInt32(aCnt);
            //newReserv.CHILD_CNT         = Convert.ToInt32(cCnt);
            //newReserv.BABY_CNT          = Convert.ToInt32(bCnt);

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

            string rName = CuData.Single().CU_NM_KOR;
            string rTel = CuData.Single().tel,
                   rTime = insDate.ToString("yyyy년 MM월 dd일 HH시 mm분"),
                   toMail = "hj3bae@ntravels.co.kr";

            toMail = DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_EMAIL;

            sendrMail(code, PDT_TITLE, date, PRC_Total.ToString(), pCount, rName, rTel, reqTxt, rTime, toMail);

            ////////////////////////////////////////////////////////////////////////

            var empTel = DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE1 +
                         DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE2 +
                         DB.EMP_0.Where(w => w.EMP_NO.Equals(chg_emp_no)).FirstOrDefault().EMP_PHONE3;

            string id = "",
                   cont = "";

            id = "10210";
            id = "12920";

            cont = "[엔데이트립 예약접수] \r\n" +
                   "상품 예약이 접수가 되었습니다.\r\n" +
                   " - 예약번호 : " + revDate + "_" + lastRev + "\r\n" +
                   " - 예약일 : " + rTime + "\r\n" +
                   " - 출발일 : " + date + "\r\n" +
                   " - 상품명 : " + PDT_TITLE + "\r\n" +
                   " - 대표 예약자 : " + rName + "\r\n" +
                   " - 대표 예약자 연락처 : " + tel + "\r\n" +
                   " - 인원 : " + pCount + "\r\n" +
                   " - 상품 요금 : " + PRC_Total.ToString() + " 원\r\n" +
                   " - 예약자 한마디 : " + reqTxt;

            kakaoMSG(id, cont, empTel);

            //if (empTel != "01073440940")
            //{
            //    empTel = "01073440940";    //신팀장님

            //    kakaoMSG(id, cont, empTel);
            //}

            id = "10217";
            cont = "[엔데이트립 예약접수] \r\n상품 예약이 접수가 되었습니다.\r\n" +
                   " - 예약번호 : " + revDate + "_" + lastRev + "\r\n" +
                   " - 출발일 : " + date + "\r\n" +
                   " - 상세예약조회 : http://ntabi.co.kr \r\n\r\n" +
                   "담당자 확인 후 다음 절차를 안내해드리겠습니다. \r\n" +
                   "고객님의 소중한 여행을 저희에게 맡겨 주셔서 진심으로 감사 드립니다. \r\n\r\n" +
                   "고객센터 : 1670-4601 \r\n" +
                   "(운영시간 : 평일 9시~6시) \r\n\r\n" +
                   "여행갈땐 엔타비 바로가기 www.ntabi.co.kr";

            kakaoMSG(id, cont, tel);

            //////////////////////////////////////////////////////////////

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
            Data.EVE_NAME = rName;
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

            return Content("<script>alert('예약은 확정이 아니라 담당자의 최종 확인 후 연락 바로 드립니다.');location.href='/MyPage/Views?d=" + revDate + "&k=" + lastRev + "';</script>");
        }

        public void udtTraveler(string rDay, int rSeq, int rcSeq, string cuyy, int cuSeq, string name = "", string first = "", string last = "", string sex = "", string gb = "", string tel = "", string content = "")
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
            Data.REV_CONTENT = content;

            DB.REV_2.Add(Data);

            DB.SaveChanges();
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

        public JsonResult JSON_udtReview(string pCode, string pKey, string cont, string star)
        {
            var priKey = getCode(pCode, pKey);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            //string emp = priKey[2];
            string pdtyy = priKey[3];
            //string pdtseq = priKey[4];

            int emp = Convert.ToInt32(priKey[2].ToString());

            short pdtseq = Convert.ToInt16(priKey[4].ToString());

            string type = "review";
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            var user = Session["user"] as User;

            var memData = from mf in nDB.CU001
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

            int postID = 1;

            var boardData = from bf in DB.NT_Board_2
                            where bf.post_type == type && bf.CORP_CODE == corp_code
                            orderby bf.post_ID descending
                            select new
                            {
                                postid = bf.post_ID
                            };

            try { postID = int.Parse(boardData.Take(1).Single().postid.ToString()) + 1; } catch { }

            NT_Board_2 board = new NT_Board_2();

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

            //board.PDT_TYPE_CODE = null;
            //board.PDT_IST_EMP_NO = null;
            //board.PDT_YY = null;
            //board.PDT_SEQ = null;

            board.post_category = null;

            switch (type)
            {
                case "qna":
                    break;
            }

            DB.NT_Board_2.Add(board);
            DB.SaveChanges();

            return Json("후기 등록을 완료했습니다.");
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
                                   .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                   .Where(w => w.PDT_STATE_CODE != "03");

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

                var resultDays = testDays//.Where(w => w.state != "03" && !string.IsNullOrEmpty(w.state))
                                        .ToList()
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

        public JsonResult getTrf(string code, string date)
        {
            var daysData = DB.PDT_1.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                   .Where(w => w.PDT_STATE_CODE != "03")
                                   .Where(w => w.PDT_DATE.Equals(date))
                                   .GroupBy(g => new { g.TRF_SEQ })
                                   .Select(s => new
                                   {
                                       code,
                                       TRF_SEQ = s.Key.TRF_SEQ
                                   });

            var trfData = DB.TRF_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO  + w.PDT_YY + w.PDT_SEQ).Equals(code))
                            .Join(daysData,
                                 a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO  + a.PDT_YY + a.PDT_SEQ + a.TRF_SEQ,
                                 b => b.code + b.TRF_SEQ,
                                 (a, b) => new
                                 {
                                     a.TRF_SEQ,
                                     a.TRF_TITLE,
                                     a.TRF_STIME,
                                     a.TRF_ETIME,
                                 });

            return Json(trfData);
        }

        public JsonResult getOptions(string code, string date)
        {
            var daysData = DB.PDT_1.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO  + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                   .Where(w => w.PDT_STATE_CODE != "03")
                                   .Where(w => w.PDT_DATE.Equals(date))
                                   .GroupBy(g => new { g.PRC_SEQ })
                                   .Select(s => new
                                   {
                                       code,
                                       PRC_SEQ = s.Key.PRC_SEQ
                                   });

            var prcData = DB.PRC_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO  + w.PDT_YY + w.PDT_SEQ).Equals(code))
                            .Join(daysData,
                                 a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO  + a.PDT_YY + a.PDT_SEQ + a.PRC_SEQ,
                                 b => b.code + b.PRC_SEQ,
                                 (a, b) => new
                                 {
                                     a.PRC_SEQ,
                                     a.PRC_Currency_CODE,
                                     a.PRC_Adult,
                                     a.PRC_Child,
                                     a.PRC_Baby,
                                     a.PRC_EXP,
                                 });

            return Json(prcData);
        }

        public string transCurrency(string code, string type = "code") {
            var result = "";

            switch(code)
            {
                case "KRW":
                    result = type == "code" ? "₩" : "원";
                    break;
                case "JPY":
                    result = type == "code" ? "¥" : "엔";
                    break;
                case "USD":
                    result = type == "code" ? "$" : "달러";
                    break;
            }

            return result;
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

            nDB.TSMS_AGENT_MESSAGE.Add(MSG);
            nDB.SaveChanges();
        }

        public void sendrMail(string code, string pName, string sDays, string tPrice, string pCount, string rName, string rTel, string rTxt, string rTime, string toMail)
        {
            string _senderID = "service@ntabi.co.kr";
            string _senderName = "NTravels";

            var _title = "[엔데이트립] 예약이 접수 되었습니다.";

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

                SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                server.EnableSsl = true;
                server.UseDefaultCredentials = false;
                server.DeliveryMethod = SmtpDeliveryMethod.Network;
                server.Credentials = new System.Net.NetworkCredential("ntabinday", "ntabi4605");
                server.Send(_message);
            }
            catch (ArgumentException error)
            {
                ViewBag.err = error;
            }
        }
    }
}