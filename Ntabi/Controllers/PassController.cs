using Ntabi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
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
        public ActionResult Index()
        {
            return View();
        }

        // GET: Pass/Lists
        public ActionResult Lists(string t = "")
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
            var priKey      = getCode(C, K);

            string corp     = priKey[0];
            string pdt_type = priKey[1];
            string pdtyy    = priKey[3];

            int emp         = Convert.ToInt32(priKey[2]);
            int pdtseq      = Convert.ToInt32(priKey[4]);

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

            //var pdt_SchLists = rData0.ToList()
            //                         .Join(DB.PDT_2,
            //                               a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
            //                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                              (a, b) => new
            //                              {
            //                                   a.CORP_CODE,
            //                                   a.PDT_TYPE_CODE,
            //                                   a.PDT_IST_EMP_NO,
            //                                   a.PDT_YY,
            //                                   a.PDT_SEQ,
            //                                   a.PDT_TITLE,
            //                                   a.PDT_CONTENT,
            //                                   a.PDT_ORDER_NO,
            //                                   a.PDT_IMG,


            //                                a.CORP_CODE,
            //                                a.PDT_TYPE_CODE,
            //                                a.PDT_IST_EMP_NO,
            //                                a.PDT_YY,
            //                                a.PDT_SEQ,
            //                                a.Title,
            //                                a.Content,
            //                                a.Option,
            //                                a.PDT_IMG,
            //                                a.PDT_SCH0,
            //                                a.PDT_SCH1,
            //                                a.PDT_SCH2,
            //                                a.PDT_SCH3,
            //                                a.PDT_SCH4,
            //                                a.PDT_SCH5,
            //                                a.PDT_SCH6,
            //                                a.PDT_SCH7,
            //                                a.key,
            //                                b.SCH_DAY,
            //                                b.SCH_CONT,
            //                                a.area,
            //                                a.city,
            //                                a.minCnt,
            //                                a.PDT_DAYS_CODE,
            //                            });

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
        // GET: Products/Reserve
        public ActionResult Reserve(FormCollection f)
        {
            try
            {
                string code = f["pdtCode"].ToString(),
                       key = f["pdtKey"].ToString(),
                       prcCode = f["prcCD"].ToString(),
                       ACnt = f["adultCnt"].ToString(),
                       CCnt = f["childCnt"].ToString(),
                       BCnt = f["babyCnt"].ToString(),
                       APrice = f["APrice"].ToString(),
                       CPrice = f["CPrice"].ToString(),
                       BPrice = f["BPrice"].ToString(),
                       currency = f["currency"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

                string revDate = DateTime.Now.ToString("yyyy-MM-dd");

                var pdtData = DB.PDT_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Select(s => new
                                      {
                                          code = code,
                                          key = key,
                                          title = s.PDT_TITLE,
                                          nation = s.PDT_NATION_CODE,
                                          area = s.PDT_AREA_CODE,
                                          inCnt = s.PDT_DAYS_CODE,
                                          stater = s.PDT_SCITY_CODE,
                                          ACnt = ACnt,
                                          APrice = APrice,
                                          CCnt = CCnt,
                                          CPrice = CPrice,
                                          BCnt = BCnt,
                                          BPrice = BPrice,
                                          revDate = revDate,
                                          empNo = s.PDT_EMP_NO,
                                      });

                string empNo = pdtData.SingleOrDefault().empNo.ToString();

                var prcData = DB.PRC_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Where(w => w.PRC_SEQ.ToString().Equals(prcCode))
                                      .Select(s => new
                                      {
                                          code = code,
                                          key = key,
                                          prcCode = prcCode,
                                          s.PRC_SEQ,
                                          s.PRC_Currency_CODE,
                                          APrice = s.PRC_Adult > 0 ? s.PRC_Adult + s.PRC_Profit : 0,
                                          CPrice = s.PRC_Child > 0 ? s.PRC_Child + s.PRC_Profit : 0,
                                          BPrice = s.PRC_Baby > 0 ? s.PRC_Baby + s.PRC_Profit : 0,
                                          s.PRC_EXP,
                                      });

                var empData = DB.EMP_0.ToList()
                                      .Where(w => w.EMP_NO.ToString().Equals(empNo))
                                      .Select(s => new
                                      {
                                          NAME = s.EMP_NAME,
                                          TEL = s.EMP_TEL1 + "-" + s.EMP_TEL2 + "-" + s.EMP_TEL3,
                                          MAIL = s.EMP_EMAIL,
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

                ViewBag.empData = empData;
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
                   inHotels = f["inHotels"];

            int prcCD = Convert.ToInt32(f["prcCD"]),
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

            int totalCnt = Convert.ToInt32(aCnt);
            int PRC_Adult = (Convert.ToInt32(prcData.Single().PRC_Adult.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(aCnt),
                PRC_Child = (Convert.ToInt32(prcData.Single().PRC_Child.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(cCnt),
                PRC_Baby = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(bCnt),
                PRC_ATotal = PRC_Adult * Convert.ToInt32(aCnt),
                PRC_CTotal = PRC_Child * Convert.ToInt32(cCnt),
                PRC_BTotal = PRC_Baby * Convert.ToInt32(bCnt),
                PRC_Total = PRC_ATotal + PRC_CTotal + PRC_BTotal;

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

            DB.SaveChanges();

            /* 여행자 정보 입력 */
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


            udtTraveler(revDate, lastRev, revcuSeq, "", 0, name, "", "", "", "A", tel, "");


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

            //sendrMail(code, PDT_TITLE, date, PRC_Total.ToString(), pCount, name, tel, reqTxt, rTime, toMail);


            ///*
            // *
            // * 홈페이지 고객 예약시 고객 자동 등록
            // * 
            // */

            //int SEQ = 1;

            //var seqData = DB.EVE_0.Where(w => w.CORP_CODE.Equals("NTB"))
            //                      .Where(w => w.EVE_REG_DATE.Equals(strDate))
            //                      .GroupBy(g => new { g.EVE_REG_DATE })
            //                      .Select(s => new { EVE_SEQ = s.Max(max => max.EVE_SEQ) });

            //try
            //{
            //    SEQ = Convert.ToInt32(seqData.Single().EVE_SEQ.ToString()) + 1;
            //}
            //catch (Exception)
            //{

            //}

            //EVE_0 Data = new EVE_0();

            //Data.CORP_CODE = "NTB";
            //Data.EVE_DATE = date;//.Substring(0,4) + "-" + date.Substring(5,2) + "-" + date.Substring(6,2);
            //Data.EVE_SEQ = SEQ;
            //Data.EVE_NAME = name;
            //Data.EVE_TEL = tel;
            //Data.EVE_EMAIL = string.IsNullOrEmpty(email) ? "-" : email;
            //Data.EVE_OFFICE_CODE = "HOM";
            //Data.EVE_CNT = totalCnt;
            //Data.EVE_ETC = string.IsNullOrEmpty(reqTxt) ? "-" : reqTxt;
            //Data.EVE_AREA_CODE = PDT_AREA_CODE;
            //Data.EVE_TITLE = PDT_TITLE;
            //Data.EVE_REG_DATE = strDate;
            //Data.EVE_PRICE = PRC_Total;

            //Data.EVE_IST_EMP_NO = chg_emp_no;
            //Data.EVE_IST_DATE = date;

            //DB.EVE_0.Add(Data);

            //DB.SaveChanges();

            ///*
            // *
            // * 홈페이지 고객 예약시 고객 자동 등록
            // * 
            // */

            var empData2 = DB.EMP_0.ToList()
                                  .Where(w => w.EMP_NO.ToString().Equals(chg_emp_no.ToString()))
                                  .Select(s => new
                                  {
                                      txt = s.EMP_NAME + " | " + s.EMP_TEL1 + "-" + s.EMP_TEL2 + "-" + s.EMP_TEL3 + " | " + s.EMP_EMAIL,
                                  });

            ViewBag.empData = empData2;

            return View(dls);
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
    }
}