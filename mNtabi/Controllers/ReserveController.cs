using mNtabi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class ReserveController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB NDB = new NtabiDB();

        // GET: Reserve/agree
        public ActionResult agree(FormCollection f)
        {
            if (f["type"] != "pass")
            {
                string code = f["pdtCode"].ToString(),
                       key = f["pdtKey"].ToString(),
                       trfCode = f["trfCD"].ToString(),
                       prcCode = f["prcCD"].ToString(),
                       empNo = f["empNo"].ToString(),
                       revDate = f["revDate"].ToString(),
                       ACnt = f["adultCnt"].ToString(),
                       CCnt = f["childCnt"].ToString(),
                       BCnt = f["babyCnt"].ToString(),
                    //hotels   = f["hotel[]"].ToString(),
                    //rooms    = f["room[]"].ToString(),
                       inCnt = f["inCnt"].ToString(),
                       APrice = f["APrice"].ToString(),
                       CPrice = f["CPrice"].ToString(),
                       BPrice = f["BPrice"].ToString(),
                       inHotels = f["inHotels"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

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
                                          s.PDT_TOUR_CITY,
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
                                          code = code,
                                          key = key,
                                          trfCode = trfCode,
                                          TRF_TITLE = getNNNNNNNAsjdkl(s.TRF_TITLE),
                                          s.TRF_STIME,
                                          s.TRF_SAREA,
                                          s.TRF_ETIME,
                                          s.TRF_EAREA,
                                          s.TRF_TYPE,
                                          s.TRF_SUB_SEQ,
                                      });

                ViewBag.trfData = trfData;
                ViewBag.pdtData = pdtData;
            }
            else
            {
                string code = f["pdtCode"].ToString(),
                       key = f["pdtKey"].ToString(),
                       ACnt = f["adultCnt"].ToString(),
                       CCnt = f["childCnt"].ToString(),
                       BCnt = f["babyCnt"].ToString(),
                       APrice = f["APrice"].ToString(),
                       CPrice = f["CPrice"].ToString(),
                       BPrice = f["BPrice"].ToString(),
                       inHotels = f["currency"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

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
                                          empNo = s.PDT_EMP_NO,
                                          s.PDT_TOUR_CITY,
                                      });

                ViewBag.pdtData = pdtData;
            }

            return View();
        }

        // GET: Reserve/rev
        public ActionResult rev(FormCollection f)
        {
            if (f["type"] == "pdt")
            {
                string code = f["pdtCode"].ToString(),
                       key = f["pdtKey"].ToString(),
                       trfCode = f["trfCD"].ToString(),
                       prcCode = f["prcCD"].ToString(),
                       empNo = f["empNo"].ToString(),
                       revDate = f["revDate"].ToString(),
                       ACnt = f["adultCnt"].ToString(),
                       CCnt = f["childCnt"].ToString(),
                       BCnt = f["babyCnt"].ToString(),
                       inCnt = f["inCnt"].ToString(),
                       APrice = f["APrice"].ToString(),
                       CPrice = f["CPrice"].ToString(),
                       BPrice = f["BPrice"].ToString(),
                       inHotels = f["inHotels"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

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

                var trfData = DB.TRF_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Where(w => w.TRF_SEQ.ToString().Equals(trfCode))
                                      .Select(s => new
                                      {
                                          code = code,
                                          key = key,
                                          trfCode = trfCode,
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
                                          code = code,
                                          key = key,
                                          prcCode = prcCode,
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

                Dictionary<int, List<string>> HLists = new Dictionary<int, List<string>>();

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

                ViewBag.rHData = HLists;
                ViewBag.empData = empData;
                ViewBag.trfData = trfData;
                ViewBag.userData = userData;
                ViewBag.prcData = prcData;

                return View(pdtData);
            }
            else if (f["type"] == "pass")
            {
                string code = f["pdtCode"].ToString(),
                       key = f["pdtKey"].ToString(),
                       ACnt = f["adultCnt"].ToString(),
                       CCnt = f["childCnt"].ToString(),
                       BCnt = f["babyCnt"].ToString(),
                       APrice = f["APrice"].ToString(),
                       CPrice = f["CPrice"].ToString(),
                       BPrice = f["BPrice"].ToString(),
                       inHotels = f["currency"].ToString(),
                       option = f["option"].ToString();

                var priKey = getCode(code, key);

                string corp = priKey[0];
                string pdt_type = priKey[1];
                string emp = priKey[2];
                string pdtyy = priKey[3];
                string pdtseq = priKey[4];

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
                                          empNo = s.PDT_EMP_NO,
                                      });


                var prcData = DB.PRC_0.ToList()
                                      .Where(w => w.CORP_CODE.Equals(corp))
                                      .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                      .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                      .Where(w => w.PDT_YY.Equals(pdtyy))
                                      .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                      .Where(w => w.PRC_SEQ.ToString().Equals(option))
                                      .Select(s => new
                                      {
                                          code = code,
                                          key = key,
                                          option = option,
                                          s.PRC_SEQ,
                                          s.PRC_Currency_CODE,
                                          APrice = s.PRC_Adult > 0 ? s.PRC_Adult + s.PRC_Profit : 0,
                                          CPrice = s.PRC_Child > 0 ? s.PRC_Child + s.PRC_Profit : 0,
                                          BPrice = s.PRC_Baby > 0 ? s.PRC_Baby : 0,
                                          //BPrice = s.PRC_Baby > 0 ? s.PRC_Baby + s.PRC_Profit : 0,
                                          s.PRC_EXP,
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

                ViewBag.userData = userData;
                ViewBag.prcData = prcData;

                return View(pdtData);
            }
            return Content("<script>alert('잘못된 접근 경로 입니다.'); location.href='/';</script>");
        }

        // GET: Reserve/Confirm
        public ActionResult Confirm(FormCollection f)
        {
            string revType = f["type"],
                   code = f["code"],
                   key = f["key"],
                   aCnt = f["aCnt"],
                   cCnt = f["cCnt"],
                   bCnt = f["bCnt"],
                   user_YY = f["user_YY"];

            int trfCD = -1,
                prcCD = -1,
                user_SEQ = Convert.ToInt32(f["user_SEQ"]);

            string name = f["rName"],
                   email = f["rEmail"],
                   tel = f["rPhone"],
                   birth = f["rBirth"];

            string date = "", 
                   reqTxt = "", 
                   inHotels = "", 
                   currency = "";

            if (f["type"] == "pdt")
            {
                date = f["date"];
                reqTxt = f["reqTxt"];
                inHotels = f["inHotels"];

                trfCD = Convert.ToInt32(f["trfCD"]);
                prcCD = Convert.ToInt32(f["prcCD"]);
            } 
            else if (f["type"] == "pass")
            {
                currency = f["currency"];

                prcCD = Convert.ToInt32(f["option"]);
            }

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
                PRC_Baby = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString())),// * Convert.ToInt32(bCnt),
                //PRC_Baby = (Convert.ToInt32(prcData.Single().PRC_Baby.ToString()) + Convert.ToInt32(prcData.Single().PRC_Profit.ToString())),// * Convert.ToInt32(bCnt),
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
            newReserv.REV_STARTDAY = string.IsNullOrEmpty(date) ? strDate : date;
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

            if (revType == "pdt")
            {
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

            ////////////////////////////////////////////////////////////////////////

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

                    setRevHotel(corp,
                                revDate,
                                lastRev,
                                hotels[i].Trim(),
                                tempData0.SingleOrDefault().HTL_NATION_CODE.ToString(),
                                tempData0.SingleOrDefault().HTL_AREA_CODE.ToString(),
                                tempData0.SingleOrDefault().HTL_TYPE.ToString(),
                                DateTime.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture).AddDays((chkIn) + i).ToString("yyyy-MM-dd"),
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
            }
            /*
             * 호텔 선택시 등록
             */

            var empData2 = DB.EMP_0.ToList()
                                  .Where(w => w.EMP_NO.ToString().Equals(chg_emp_no.ToString()))
                                  .Select(s => new
                                  {
                                      txt = s.EMP_NAME + " | " + s.EMP_TEL1 + "-" + s.EMP_TEL2 + "-" + s.EMP_TEL3 + " | " + s.EMP_EMAIL,
                                  });

            ViewBag.empData = empData2;

            var trfData = DB.TRF_0.ToList()
                                  .Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                  .Where(w => w.TRF_SEQ.ToString().Equals(trfCD))
                                  .Select(s => new
                                  {
                                      code = code,
                                      key = key,
                                      trfCode = trfCD,
                                      TRF_TITLE = getNNNNNNNAsjdkl(s.TRF_TITLE),
                                      s.TRF_STIME,
                                      s.TRF_SAREA,
                                      s.TRF_ETIME,
                                      s.TRF_EAREA,
                                      s.TRF_TYPE,
                                      s.TRF_SUB_SEQ,
                                  });

            ViewBag.trfData = trfData;

            return View(dls);
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
                Data.CU_SEQ = 0;
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
    }
}