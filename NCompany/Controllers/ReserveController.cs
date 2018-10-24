using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class ReserveController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB NDB = new NtabiDB();

        string corp = "NTB";

        public string getName(int emp)
        {
            string name = "";

            name = DB.EMP_0.Where(w => w.EMP_NO == emp).Single().EMP_NAME;

            return name;
        }

        public string getDetail(string type, string detail)
        {
            string name = "";

            try
            {
                name = DB.DETAIL.Where(w => w.DET_TYPE.Equals(type))
                                .Where(w => w.DET_SEQ.ToString().Equals(detail))
                                .SingleOrDefault()
                                .DET_NAME.ToString();
            }
            catch (Exception e)
            {
                name = "";
            }

            if (type == "REFD")
            {
                switch(detail)
                {
                    case "-":
                    case "CASH":
                    case "cash":
                        name = "현금";
                        break;
                    case "CARD":
                    case "card":
                        name = "카드 취소";
                        break;
                    case "BANK":
                    case "bank":
                        name = "은행 입금";
                        break;
                }
            }

            return name;
        }

        public string getAccGB(string type)
        {
            string txt = "";

            switch(type)
            {
                case "CASH":
                    txt = "현금 결제";
                    break;
                case "BANK":
                    txt = "은행 입금";
                    break;
                case "CARD":
                    txt = "카드 결제";
                    break;
                case "EFTS":
                    txt = "전자 결제";
                    break;
                case "TBPY":
                    txt = "타비 페이";
                    break;
                case "REFD":
                    txt = "환불";
                    break;
            }

            return txt;
        }

        public string getAreaTxt(string code)
        {
            string txt = "";

            switch (code)
            {
                case "KYU":
                case "FUK":
                    txt = "규슈";
                    break;
                case "TOK":
                case "TYO":
                    txt = "도쿄";
                    break;
                case "OSA":
                    txt = "오사카";
                    break;
                case "HOK":
                case "CTS":
                    txt = "홋카이도";
                    break;
                case "TSU":
                case "TSM":
                    txt = "대마도";
                    break;
                case "OKI":
                case "OKA":
                    txt = "오키나와";
                    break;
                case "NGY":
                case "NGO":
                    txt = "나고야";
                    break;
                case "HKG":
                    txt = "홍콩";
                    break;
                case "TPE":
                    txt = "타이페이";
                    break;
                case "DAD":
                    txt = "다낭";
                    break;
                case "CEB":
                    txt = "세부";
                    break;
                case "HNL":
                    txt = "호놀룰루";
                    break;
                case "GUM":
                    txt = "괌";
                    break;
                case "SPN":
                    txt = "사이판";
                    break;
                default:
                    txt = code;
                    break;
            }

            return txt;
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
                case "NGO":
                    area.Add("NGY");
                    area.Add("NGO");
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

        public string convertDate8To10(string date)
        {
            if (date.Length != 8)
                return date;

            string result = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);

            return result;
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


        // GET: Reserve
        public ActionResult Index(string state = "", string rday = "", string sday1 = "", string sday2 = "", string name = "", string emp = "", string code = "", string key = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var ReData = DB.REV_0.Select(s => new
                                  {
                                      key = s.REV_DAY + "_" + s.REV_SEQ,
                                      s.REV_STATE,
                                      REV_DAY = s.REV_DAY.Substring(0, 4) + "-" + s.REV_DAY.Substring(4, 2) + "-" + s.REV_DAY.Substring(6, 2),
                                      s.REV_STARTDAY,
                                      s.CU_NAME,
                                      //PDT_CODE = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                      s.PDT_TITLE,
                                      s.ADULT_CNT,
                                      s.CHILD_CNT,
                                      s.BABY_CNT,
                                      s.REV_PRICE,
                                      s.REV_CHK_PRICE,
                                      s.CHG_EMP_NO,
                                      s.CHK_VOU,
                                      s.PDT_TYPE_CODE,
                                      s.PDT_IST_EMP_NO,
                                      s.PDT_YY,
                                      s.PDT_SEQ,
                                  })
                                  .ToList();

            if (!string.IsNullOrEmpty(state))
            {
                ReData = ReData.Where(w => w.REV_STATE.Equals(state)).ToList();
            }
            else
            {
                ReData = ReData.Where(w => w.REV_STATE != "10").ToList();
            }

            if (!string.IsNullOrEmpty(rday))
            {
                ReData = ReData.Where(w => w.REV_DAY.Equals(rday)).ToList();
            }

            if (!string.IsNullOrEmpty(sday1))
            {
                DateTime dt1 = DateTime.ParseExact(sday1, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                ReData = ReData.Where(w => DateTime.ParseExact(w.REV_STARTDAY, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= dt1).ToList();
            }

            if (!string.IsNullOrEmpty(sday2))
            {
                DateTime dt2 = DateTime.ParseExact(sday2, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                ReData = ReData.Where(w => DateTime.ParseExact(w.REV_STARTDAY, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= dt2).ToList();
            }

            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(key))
            {
                var priKey = getCode(code, key);

                string type = priKey[1],
                       pdtyy = priKey[3];

                int empNo  = Convert.ToInt32(priKey[2]),
                    pdtseq = Convert.ToInt32(priKey[4]);

                ReData = ReData.Where(w => w.PDT_TYPE_CODE.Equals(type))
                               .Where(w => w.PDT_IST_EMP_NO == empNo)
                               .Where(w => w.PDT_YY.Equals(pdtyy))
                               .Where(w => w.PDT_SEQ == pdtseq)
                               .ToList();
            }

            if (!string.IsNullOrEmpty(name))
            {
                ReData = ReData.Where(w => w.CU_NAME.Contains(name)).ToList();
            }

            if (string.IsNullOrEmpty(emp))
            {
                emp = user.Login.ToString();

                ReData = ReData.Where(w => w.CHG_EMP_NO == Convert.ToInt32(emp)).ToList();
            }
            else if (emp == "all")
            {

            }
            else
            {
                ReData = ReData.Where(w => w.CHG_EMP_NO == Convert.ToInt32(emp)).ToList();
            }

            //var ReData1 = ReData.Join(DB.PDT_0,
            //                         a => a.PDT_CODE,
            //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                         (a, b) => new
            //                         {
            //                             a.key,
            //                             a.REV_STATE,
            //                             a.REV_DAY,
            //                             a.REV_STARTDAY,
            //                             a.CU_NAME,
            //                             b.PDT_TITLE,
            //                             a.ADULT_CNT,
            //                             a.CHILD_CNT,
            //                             a.BABY_CNT,
            //                             a.REV_PRICE,
            //                             a.REV_CHK_PRICE,
            //                             a.CHG_EMP_NO
            //                         });

            var ReData2 = ReData.Join(DB.EMP_0,
                                     a => a.CHG_EMP_NO,
                                     b => b.EMP_NO,
                                     (a, b) => new
                                     {
                                         a.key,
                                         a.REV_STATE,
                                         a.REV_DAY,
                                         a.REV_STARTDAY,
                                         a.CU_NAME,
                                         a.PDT_TITLE,
                                         a.ADULT_CNT,
                                         a.CHILD_CNT,
                                         a.BABY_CNT,
                                         a.REV_PRICE,
                                         a.REV_CHK_PRICE,
                                         b.EMP_NAME,
                                         a.CHK_VOU,
                                     })
                                     .OrderByDescending(o => o.REV_DAY)
                                     .OrderBy(o => o.REV_STATE);

            return View(ReData2);
        }

        public ActionResult searchForm(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();

            string state = f["state"].ToString(),
                   rday = f["rday"].ToString(),
                   sday = f["sday"].ToString(),
                   name = f["name"].ToString(),
                   emp = f["emp"].ToString();

            return Redirect("/Reserve?state=" + state + "&rday=" + rday + "&sday=" + sday + "&name=" + name + "&emp=" + emp);
        }

        public ActionResult detRev(string code = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (code != "")
            {
                var reData = DB.REV_0.Where(w => (w.REV_DAY + "_" + w.REV_SEQ).Equals(code))
                                     .Select(s => new
                                     {
                                         revCode = code,
                                         s.REV_CNT,
                                         s.REV_STATE,
                                         s.CHG_EMP_NO,
                                         s.IST_EMP_NO,
                                         s.SEL_EMP_NO,
                                         s.UDT_EMP_NO,
                                         s.IST_DATE,
                                         s.SEL_DATE,
                                         s.UDT_DATE,
                                         s.REV_STARTDAY,
                                         s.PDT_TITLE,
                                         s.PDT_DAYS_CODE,
                                         cuCode = s.CU_YY + s.CU_SEQ,
                                         pdtCode = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                         key = s.PDT_IST_EMP_NO,
                                         aCnt = s.ADULT_CNT,
                                         cCnt = s.CHILD_CNT,
                                         bCnt = s.BABY_CNT,
                                     });

                //var reData2 = reData.Join(DB.PDT_0,
                //                         a => a.pdtCode,
                //                         b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                //                         (a, b) => new
                //                         {
                //                             a.revCode,
                //                             a.REV_CNT,
                //                             a.REV_STATE,
                //                             a.CHG_EMP_NO,
                //                             a.IST_EMP_NO,
                //                             a.SEL_EMP_NO,
                //                             a.UDT_EMP_NO,
                //                             a.IST_DATE,
                //                             a.SEL_DATE,
                //                             a.UDT_DATE,
                //                             a.REV_STARTDAY,
                //                             a.PDT_TITLE,
                //                             b.PDT_DAYS_CODE,
                //                             a.cuCode,
                //                             a.pdtCode,
                //                             a.key,
                //                         });

                string cuCode = reData.Single().cuCode;

                var cuData = NDB.CU001.Where(w => (w.CU_YY + w.CU_SEQ).Equals(cuCode))
                                      .Select(s => new
                                      {
                                          s.CU_NM_KOR,
                                          s.CU_NM_ENG_FIRST,
                                          s.CU_NM_ENG_LAST,
                                          s.EMAIL,
                                          tel = s.HANDPHONE1 + "-" + s.HANDPHONE2 + "-" + s.HANDPHONE3,
                                          s.CU_YY,
                                          s.CU_SEQ,
                                      });

                ViewBag.cuData = cuData;
                ViewBag.reData = reData;
            }

            return View();
        }

        public ActionResult setAccount(string revDay, string revSeq)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            //var revDay = f["REV_DAY"],
            //    revSeq = f["REV_SEQ"],
            //    type = f["type"],
            //    detail = f["detail"],
            //    price = f["price"],
            //    date = f["date"],
            //    name = f["name"]

            ////var data1 = DB.REV_3

            //return Redirect("/Reserve");

            return View();
        }

        public ActionResult chkVoucher(string code)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var reData = DB.REV_0.Where(w => (w.REV_DAY + "_" + w.REV_SEQ).Equals(code))
                                 .Select(s => new
                                 {
                                     s.CORP_CODE,
                                     s.PDT_TYPE_CODE,
                                     s.PDT_IST_EMP_NO,
                                     s.PDT_YY,
                                     s.PDT_SEQ,
                                     s.REV_DAY,
                                     s.REV_SEQ
                                 });

            var schData = DB.VOU_0.Join(reData,
                                       a => a.CORP_CODE + a.REV_DAY + a.REV_SEQ,
                                       b => b.CORP_CODE + b.REV_DAY + b.REV_SEQ,
                                       (a, b) => new
                                       {
                                           a.CORP_CODE,
                                           a.REV_DAY,
                                           a.REV_SEQ,
                                           a.VOU_SEQ,
                                           a.VOU_CONT1,
                                           a.VOU_CONT2,
                                           a.VOU_CONT3,
                                           a.VOU_CONT4,
                                           a.VOU_LATITUDE,
                                           a.VOU_LONGITUDE,
                                           a.VOU_PHOTO1,
                                           a.VOU_PHOTO2,
                                           a.VOU_PHOTO3
                                       });

            return View(schData);
        }








        public ActionResult updateData(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string type = f["type"],
                   revDay = f["REV_DAY"],
                   revSeq = f["REV_SEQ"];

            switch(type)
            {
                case "updatePrice":
                    deletePrice(revDay, revSeq);
                    updatePrice(f);
                    break;
                case "updateTraveler":
                    deleteTraveler(revDay, revSeq);
                    updateTraveler(f);
                    break;
            }

            return Redirect("/Reserve");
        }

        public void deleteTraveler(string revDay, string revSeq)
        {
            string REV_DAY = revDay;

            int REV_SEQ = Convert.ToInt32(revSeq);

            var revprcData = DB.REV_2.Where(w => w.REV_DAY.Equals(REV_DAY))
                                     .Where(w => w.REV_SEQ == REV_SEQ);

            if (revprcData.Any())
            {
                foreach (var item in revprcData)
                {
                    DB.REV_2.Remove(item);
                }
            }

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                  .Where(w => w.REV_SEQ == REV_SEQ);

            revData.Single().ADULT_CNT = 0;
            revData.Single().CHILD_CNT = 0;
            revData.Single().BABY_CNT = 0;

            DB.SaveChanges();
        }

        public void deletePrice(string revDay, string revSeq)
        {
            string REV_DAY = revDay;

            int REV_SEQ = Convert.ToInt32(revSeq);

            var revprcData = DB.REV_1.Where(w => w.REV_DAY.Equals(REV_DAY))
                                     .Where(w => w.REV_SEQ == REV_SEQ);

            if (revprcData.Any())
            {
                foreach (var item in revprcData)
                {
                    DB.REV_1.Remove(item);
                }
            }

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                  .Where(w => w.REV_SEQ == REV_SEQ);

            revData.Single().REV_PRICE      = 0;
            revData.Single().REV_CHK_PRICE  = "N";

            DB.SaveChanges();
        }

        public void updateTraveler(FormCollection f)
        {
            var revDay = f["REV_DAY"];
            var revSeq = f["REV_SEQ"];
            var arrCode = f["user"].Split(',');
            var arrName = f["cuName"].Split(',');
            var arrFirst = f["cuFirst"].Split(',');
            var arrLast = f["cuLast"].Split(',');
            var arrSex = f["sex"].Split(',');
            var arrGB = f["gb"].Split(',');
            var arrTel = f["cuTel"].Split(',');
            var arrContent = f["content"].Split(',');

            int REV_SEQ = Convert.ToInt32(revSeq),
                revcuSeq = 1,
                CU_SEQ = 0,
                aCnt = 0,
                cCnt = 0,
                bCnt = 0;

            string REV_DAY = revDay.ToString(),
                   CU_YY = "",
                   CU_NAME = "",
                   CU_FIRST = "",
                   CU_LAST = "",
                   CU_SEX = "",
                   CU_GB = "",
                   CU_TEL = "",
                   CU_CONTENT = "";

            for (int i = 0; i < arrName.Length; i++)
            {
                CU_YY = "";
                CU_NAME = "";
                CU_FIRST = "";
                CU_LAST = "";
                CU_SEX = "";
                CU_GB = "";
                CU_TEL = "";
                CU_CONTENT = "";

                CU_SEQ = 0;

                revcuSeq = i + 1;
                CU_NAME = arrName[i].ToString();
                CU_FIRST = arrFirst[i].ToString();
                CU_LAST = arrLast[i].ToString();
                CU_SEX = arrSex[i].ToString();
                CU_GB = arrGB[i].ToString();
                CU_TEL = arrTel[i].ToString();
                CU_CONTENT = arrContent[i].ToString();

                try
                {
                    CU_YY = arrCode[i].ToString().Substring(0, 4);
                    CU_SEQ = Convert.ToInt32(arrCode[i].ToString().Substring(4));
                }
                catch(Exception)
                {
                }

                switch(CU_GB)
                {
                    case "A":
                        aCnt++;
                        break;
                    case "C":
                        cCnt++;
                        break;
                    case "B":
                        bCnt++;
                        break;
                }

                udtTraveler(REV_DAY, REV_SEQ, revcuSeq, CU_YY, CU_SEQ, CU_NAME, CU_FIRST, CU_LAST, CU_SEX, CU_GB, CU_TEL, CU_CONTENT);
            }

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                  .Where(w => w.REV_SEQ == REV_SEQ);

            revData.Single().ADULT_CNT = aCnt;
            revData.Single().CHILD_CNT = cCnt;
            revData.Single().BABY_CNT = bCnt;

            DB.SaveChanges();
        }

        public void updatePrice(FormCollection f)
        {
            var revDay = f["REV_DAY"];
            var revSeq = f["REV_SEQ"];
            var arrPrcKey = f["prcKey"].Split(',');
            var arrPrcContent = f["prcContent"].Split(',');
            var arrRevprcContent = f["revprcContent"].Split(',');
            var arrRevprcGB = f["revprcGB"].Split(',');
            var arrPrcCurrencyCode = f["prcCurrencyCode"].Split(',');
            var arrPrice = f["price"].Split(',');
            var arrDiscount = f["discount"].Split(',');
            var arrAdditional = f["additional"].Split(',');
            var arrCnt = f["cnt"].Split(',');

            string rDay = revDay.ToString(),
                   content = "",
                   gb = "",
                   type = "",
                   yy = "",
                   pContent = "",
                   currency = "";

            int rSeq = Convert.ToInt32(revSeq),
                rpSeq = 1,
                price = 0,
                discount = 0,
                additional = 0,
                cnt = 1,
                tPrice = 0,
                emp = 0,
                pdtSeq = 0,
                prcSeq = 0;

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(rDay))
                                  .Where(w => w.REV_SEQ == rSeq);

            try
            {
                type = revData.Single().PDT_TYPE_CODE.ToString();
                yy = revData.Single().PDT_YY.ToString();
                emp = Convert.ToInt32(revData.Single().PDT_IST_EMP_NO);
                pdtSeq = Convert.ToInt32(revData.Single().PDT_SEQ);
            }
            catch(Exception)
            {

            }

            int rPrice = 0;

            for (int i = 0; i < arrPrice.Length; i++)
            {
                content = "";
                gb = "";
                pContent = "";
                currency = "";
                price = 0;
                discount = 0;
                additional = 0;
                cnt = 1;
                tPrice = 0;
                prcSeq = 0;

                content = arrRevprcContent[i].ToString();
                gb = arrRevprcGB[i].ToString();
                pContent = arrPrcContent[i].ToString();
                currency = arrPrcCurrencyCode[i].ToString();

                rpSeq = (i + 1);
                price = string.IsNullOrEmpty(arrPrice[i]) ? 0 : Convert.ToInt32(arrPrice[i]);
                discount = string.IsNullOrEmpty(arrDiscount[i]) ? 0 : Convert.ToInt32(arrDiscount[i]);
                additional = string.IsNullOrEmpty(arrAdditional[i]) ? 0 : Convert.ToInt32(arrAdditional[i]);
                cnt = string.IsNullOrEmpty(arrCnt[i]) ? 0 : Convert.ToInt32(arrCnt[i]);
                prcSeq = string.IsNullOrEmpty(arrPrcKey[i]) ? 0 : Convert.ToInt32(arrPrcKey[i]);

                tPrice = (price + additional - discount) * cnt;

                udtPrice(rDay, rSeq, rpSeq, price, discount, additional, cnt, tPrice, content, gb, type, emp, yy, pdtSeq, prcSeq, pContent, currency);

                rPrice += tPrice;
            }

            revData.Single().REV_PRICE = rPrice;
            DB.SaveChanges();
        }

        public JsonResult updateVoucher(string REV_DAY, string REV_SEQ, string VOU_SEQ, string CONT1, string CONT2, string CONT3, string CONT4, string LAT, string LON, string PHOTO1, string PHOTO2, string PHOTO3)
        {
            string txt = "";

            int rSeq = Convert.ToInt32(REV_SEQ),
                vSeq = Convert.ToInt32(VOU_SEQ);

            var chkVoucher = DB.VOU_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                     .Where(w => w.REV_SEQ == rSeq)
                                     .Where(w => w.VOU_SEQ == vSeq);

            if (chkVoucher.Any())
            {
                chkVoucher.Single().VOU_CONT1 = HttpUtility.UrlDecode(CONT1);
                chkVoucher.Single().VOU_CONT2 = HttpUtility.UrlDecode(CONT2);
                chkVoucher.Single().VOU_CONT3 = HttpUtility.UrlDecode(CONT3);
                chkVoucher.Single().VOU_CONT4 = HttpUtility.UrlDecode(CONT4);
                chkVoucher.Single().VOU_LATITUDE = LAT;
                chkVoucher.Single().VOU_LONGITUDE = LON;
                chkVoucher.Single().VOU_PHOTO1 = PHOTO1;
                chkVoucher.Single().VOU_PHOTO2 = PHOTO2;
                chkVoucher.Single().VOU_PHOTO3 = PHOTO3;
            }
            else
            {
                VOU_0 Data = new VOU_0();

                Data.CORP_CODE = corp;
                Data.REV_DAY = REV_DAY;
                Data.REV_SEQ = rSeq;
                Data.VOU_SEQ = vSeq;
                Data.VOU_CONT1 = HttpUtility.UrlDecode(CONT1);
                Data.VOU_CONT2 = HttpUtility.UrlDecode(CONT2);
                Data.VOU_CONT3 = HttpUtility.UrlDecode(CONT3);
                Data.VOU_CONT4 = HttpUtility.UrlDecode(CONT4);
                Data.VOU_LATITUDE = LAT;
                Data.VOU_LONGITUDE = LON;
                Data.VOU_PHOTO1 = PHOTO1;
                Data.VOU_PHOTO2 = PHOTO2;
                Data.VOU_PHOTO3 = PHOTO3;

                DB.VOU_0.Add(Data);
            }

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                  .Where(w => w.REV_SEQ == rSeq);

            revData.Single().CHK_VOU = "Y";

            try
            {
                DB.SaveChanges();

                txt = "바우처 저장을 완료 했습니다.";
            }
            catch(Exception e)
            {
                txt = "바우처 저장에 오류가 발생했습니다 : " + e;
            }

            return Json(txt);
        }

        public void udtTraveler(string rDay, int rSeq, int rcSeq, string cuyy, int cuSeq, string name, string first, string last, string sex, string gb, string tel, string content)
        {
            REV_2 Data = new REV_2();

            Data.CORP_CODE = corp;
            Data.REV_DAY = rDay;
            Data.REV_SEQ = rSeq;
            Data.REV_CU_SEQ = rcSeq;
            Data.CU_YY = cuyy;
            Data.CU_SEQ = cuSeq;
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

        public void udtPrice(string rDay, int rSeq, int rpSeq, int price, int discount, int additional, int cnt, int tPrice, string content, string gb, string type, int emp, string yy, int pdtSeq, int prcSeq, string pContent, string currency)
        {
            REV_1 Data = new REV_1();

            Data.CORP_CODE = corp;
            Data.REV_DAY = rDay;
            Data.REV_SEQ = rSeq;
            Data.REV_PRC_SEQ = rpSeq;
            Data.REV_PRC_PRICE = price;
            Data.REV_PRC_DISCOUNT = discount;
            Data.REV_PRC_ADDITIONAL_PRICE = additional;
            Data.REV_PRC_CNT = cnt;
            Data.REV_PRC_TOTAL_PRICE = tPrice;
            Data.REV_PRC_CONTENT = content;
            Data.REV_PRC_GB = gb;
            Data.PDT_TYPE_CODE = type;
            Data.PDT_IST_EMP_NO = emp;
            Data.PDT_YY = yy;
            Data.PDT_SEQ = pdtSeq;
            Data.PRC_SEQ = prcSeq;
            Data.PRC_CONTENT = pContent;
            Data.PRC_CURRENCY_CODE = currency;

            DB.REV_1.Add(Data);

            DB.SaveChanges();
        }

        public ActionResult updateRev(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string pdtTitle = f["pdtTitle"],
                   pdtCode  = f["code"],
                   pdtKey   = f["key"],
                   startDay = f["sDay"],
                   dayCode  = f["dayCode"],
                   endDay   = f["eDay"],
                   cuCode   = f["user"],
                   cuName   = f["cuName"],
                   cuFirst  = f["cuFirst"],
                   cuLast   = f["cuLast"],
                   cuMail   = f["cuMail"],
                   cuTel    = f["cuTel"],
                   revCode  = f["revCode"],
                   revCnt   = f["adultCnt"],
                   childCnt   = f["childCnt"],
                   babyCnt   = f["babyCnt"],
                   revState = f["revState"],
                   corp     = "NTB",
                   type     = "",
                   pdtyy    = "",
                   cuyy     = "";

            int cuSeq = 0,
                emp = 0,
                pdtseq = 0;

            DateTime nowDate = DateTime.Now;

            if (!string.IsNullOrEmpty(pdtCode))
            {
                var priKey = getCode(pdtCode, pdtKey);

                //corp    = priKey[0];
                type    = priKey[1];
                emp     = Convert.ToInt32(priKey[2]);
                pdtyy   = priKey[3];
                pdtseq  = Convert.ToInt32(priKey[4]);
            }

            string[] subsPhone = { "", "", "" };

            try
            {
                subsPhone = cuTel.Split('-');
            } catch(Exception)
            {

            }

            if (!string.IsNullOrEmpty(cuCode))
            {
                cuyy    = cuCode.Substring(0, 4);
                cuSeq   = Convert.ToInt32(cuCode.Substring(4));

                /* 예약자 */

                var reUser0 = NDB.CU001.Where(w => w.CU_YY.Equals(cuyy))
                                       .Where(w => w.CU_SEQ == cuSeq);

                reUser0.Single().CU_NM_KOR          = cuName;
                reUser0.Single().EMAIL              = cuMail;
                reUser0.Single().CU_NM_ENG_FIRST    = cuFirst;
                reUser0.Single().CU_NM_ENG_LAST     = cuLast;
                reUser0.Single().HANDPHONE1         = subsPhone[0];
                reUser0.Single().HANDPHONE2         = subsPhone[1];
                reUser0.Single().HANDPHONE3         = subsPhone[2];

                NDB.SaveChanges();
            }
            else
            {
                // 고객 새로 등록하기
                string CU_YY = nowDate.ToString("yyyy");

                int CU_SEQ = 1;

                var cuData = NDB.CU001.Where(w => w.CU_YY.Equals(CU_YY))
                                      .GroupBy(g => new
                                      {
                                          g.CU_YY
                                      })
                                      .Select(s => new
                                      {
                                          seq = s.Max(max => max.CU_SEQ),
                                      });

                try { CU_SEQ = cuData.Single().seq + 1; }
                catch { }

                CU001 customer = new CU001();

                customer.CU_YY              = CU_YY;
                customer.CU_SEQ             = CU_SEQ;
                customer.CU_NM_KOR          = cuName;
                customer.CU_NM_ENG_FIRST    = cuFirst;
                customer.CU_NM_ENG_LAST     = cuLast;
                customer.EMAIL              = cuMail;
                customer.HANDPHONE1         = subsPhone[0];
                customer.HANDPHONE2         = subsPhone[1];
                customer.HANDPHONE3         = subsPhone[2];

                NDB.CU001.Add(customer);

                NDB.SaveChanges();
            }

            string rev_Date = "",
                   ist_date = "",
                   udt_date = "",
                   sel_date = "",
                   chkPrice = "N";

            int lastRev = 1,
                aCnt = 1,
                cCnt = 0,
                bCnt = 0,
                totalPrice = 0,
                ist_emp_no = 0,
                udt_emp_no = 0,
                sel_emp_no = 0,
                chg_emp_no = 0;

            aCnt = Convert.ToInt32(revCnt);
            cCnt = Convert.ToInt32(childCnt);
            bCnt = Convert.ToInt32(babyCnt);

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO
                                  });

            udt_emp_no = Convert.ToInt32(empData.Single().EMP_NO.ToString());

            udt_date = DateTime.Now.ToString("yyyy-MM-dd");
            rev_Date = DateTime.Now.ToString("yyyyMMdd"); ;

            if (!string.IsNullOrEmpty(revCode))
            {
                var rev_Code = revCode.Split('_');

                string revYY = rev_Code[0];

                int revSeq = Convert.ToInt32(rev_Code[1]);

                var reData = DB.REV_0.Where(w => w.REV_DAY.Equals(revYY))
                                     .Where(w => w.REV_SEQ == revSeq);

                reData.Single().PDT_TYPE_CODE   = type;
                reData.Single().PDT_IST_EMP_NO  = emp;
                reData.Single().PDT_YY          = pdtyy;
                reData.Single().PDT_SEQ         = pdtseq;
                reData.Single().PDT_TITLE       = pdtTitle;
                reData.Single().PDT_DAYS_CODE   = dayCode;

                reData.Single().CU_YY       = cuyy;
                reData.Single().CU_SEQ      = cuSeq;
                reData.Single().CU_NAME     = cuName;

                reData.Single().REV_CNT         = aCnt + cCnt + bCnt;
                reData.Single().REV_STATE       = revState;
                reData.Single().REV_STARTDAY    = startDay;

                reData.Single().ADULT_CNT   = aCnt;
                reData.Single().CHILD_CNT   = cCnt;
                reData.Single().BABY_CNT    = bCnt;

                reData.Single().UDT_EMP_NO  = udt_emp_no;
                reData.Single().UDT_DATE    = udt_date;
            }
            else
            {
                ist_emp_no = Convert.ToInt32(empData.Single().EMP_NO.ToString());
                sel_emp_no = Convert.ToInt32(empData.Single().EMP_NO.ToString());
                chg_emp_no = Convert.ToInt32(empData.Single().EMP_NO.ToString());

                ist_date = udt_date;
                sel_date = udt_date;

                var chkRev = DB.REV_0.Where(w => w.REV_DAY.Equals(rev_Date));

                if (chkRev.Any())
                {
                    lastRev += chkRev.Count();
                }

                REV_0 newReserv = new REV_0();

                newReserv.CORP_CODE = corp;

                newReserv.PDT_TYPE_CODE     = type;
                newReserv.PDT_IST_EMP_NO    = emp;
                newReserv.PDT_YY            = pdtyy;
                newReserv.PDT_SEQ           = pdtseq;
                newReserv.PDT_TITLE         = pdtTitle;
                newReserv.PDT_DAYS_CODE     = dayCode;

                newReserv.CU_YY         = cuyy;
                newReserv.CU_SEQ        = cuSeq;
                newReserv.CU_NAME       = cuName;

                newReserv.REV_DAY       = rev_Date;
                newReserv.REV_SEQ       = lastRev;
                newReserv.REV_CNT       = aCnt + cCnt + bCnt;
                newReserv.REV_STATE     = revState;
                newReserv.REV_STARTDAY  = startDay;

                newReserv.ADULT_CNT     = aCnt;
                newReserv.CHILD_CNT     = cCnt;
                newReserv.BABY_CNT      = bCnt;
                newReserv.REV_PRICE     = totalPrice;
                newReserv.REV_CHK_PRICE = chkPrice;

                newReserv.IST_EMP_NO = ist_emp_no;
                newReserv.CHG_EMP_NO = chg_emp_no;
                newReserv.SEL_EMP_NO = sel_emp_no;
                newReserv.UDT_EMP_NO = udt_emp_no;

                newReserv.IST_DATE = ist_date;
                newReserv.UDT_DATE = udt_date;
                newReserv.SEL_DATE = sel_date;

                DB.REV_0.Add(newReserv);
            }

            DB.SaveChanges();

            //var seq = f["seq[]"].Split(',');
            //var currency = f["currency[]"].Split(',');
            //var adultPrice = f["adult[]"].Split(',');
            //var childPrice = f["child[]"].Split(',');
            //var babyPrice = f["baby[]"].Split(',');
            //var profit = f["profit[]"].Split(',');
            //var content = f["content[]"].Split(',');

            //var prcData = DB.PRC_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            //int idx = 1;

            //string cr = "",
            //       ap = "",
            //       cp = "",
            //       bp = "",
            //       pf = "",
            //       ct = "";

            //if (prcData.Any())
            //{
            //    foreach (var item in prcData)
            //    {
            //        DB.PRC_0.Remove(item);
            //    }

            //    DB.SaveChanges();
            //}

            //for (var i = 0; seq.Length > i; i++)
            //{
            //    cr = !string.IsNullOrEmpty(currency[i].ToString()) ? currency[i].ToString() : "";
            //    ap = !string.IsNullOrEmpty(adultPrice[i].ToString()) ? adultPrice[i].ToString() : "";
            //    cp = !string.IsNullOrEmpty(childPrice[i].ToString()) ? childPrice[i].ToString() : "0";
            //    bp = !string.IsNullOrEmpty(babyPrice[i].ToString()) ? babyPrice[i].ToString() : "0";
            //    pf = !string.IsNullOrEmpty(profit[i].ToString()) ? profit[i].ToString() : "0";
            //    ct = !string.IsNullOrEmpty(content[i].ToString()) ? content[i].ToString() : "";

            //    udtPrice(code, key, (idx).ToString(), cr, ap, cp, bp, pf, ct);

            //    idx += 1;
            //}

            //if (prcData.Any())
            //{
            //    var udtPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            //    udtPdt.Single().PDT_Option2 = "Y";

            //    DB.SaveChanges();
            //}

            return Redirect("/Reserve");
        }







        public ActionResult getOptions(string REV_DAY, string REV_SEQ)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string revYY = REV_DAY;

            int revSeq = Convert.ToInt32(REV_SEQ);

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(revYY))
                                  .Where(w => w.REV_SEQ == revSeq)
                                  .Select(s => new
                                  {
                                      code = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ
                                  });

            string code = revData.Single().code;

            var prcData = DB.PRC_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                  .Select(s => new
                                  {
                                      s.PRC_SEQ,
                                      s.PRC_Currency_CODE,
                                      s.PRC_Adult,
                                      s.PRC_Child,
                                      s.PRC_Baby,
                                      s.PRC_Profit,
                                      s.PRC_EXP,
                                  });

            return View(prcData);
        }

        public ActionResult getProduct()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var pdtData = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                  .Select(s => new
                                  {
                                      code = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                      title = s.PDT_TITLE,
                                      key = s.PDT_IST_EMP_NO,
                                  });

            return View(pdtData);
        }

        public ActionResult getCustomer()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            //var cuData = NDB.CU001.Select(s => new
            //                      {
            //                          code = s.CU_YY + s.CU_SEQ,
            //                          name = s.CU_NM_KOR,
            //                          first = s.CU_NM_ENG_FIRST,
            //                          last = s.CU_NM_ENG_LAST,
            //                          mail = s.EMAIL,
            //                          tel = s.HANDPHONE1 + "-" + s.HANDPHONE2 + "-" + s.HANDPHONE3,
            //                      });

            return View();
        }

        public ActionResult setACC(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string type = f["type"],
                   currency = f["currency"],
                   date = f["date"],
                   name = f["name"],
                   revDay = f["REV_DAY"];

            string detail = f["detail"].ToString();

            // var chkDetail = Regex.Replace(strDetail, @"\D", "");

            date = Regex.Replace(date, @"\D", "");

            int //detail1 = chkDetail == "" ? 0 : Convert.ToInt32(chkDetail),
                price = Convert.ToInt32(f["price"].ToString()),
                revSeq = Convert.ToInt32(f["REV_SEQ"].ToString());

            string corp = "NTB";

            string now = DateTime.Now.ToString("yyyyMMdd");

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO
                                  });

            int idx = 1;

            var chkData = DB.REV_3.Where(w => w.REV_DAY.Equals(revDay))
                                  .Where(w => w.REV_SEQ == revSeq)
                                  .GroupBy(g => new { g.REV_DAY, g.REV_SEQ })
                                  .Select(s => new
                                  {
                                      idx = s.Max(max => max.REV_ACC_SEQ)
                                  });

            if (chkData.Any())
            {
                idx += chkData.Single().idx;
            }

            REV_3 Data = new REV_3();

            Data.CORP_CODE = corp;
            Data.REV_DAY = revDay;
            Data.REV_SEQ = revSeq;
            Data.REV_ACC_SEQ = idx;
            Data.REV_ACC_DATE = date;
            Data.REV_ACC_REG_DATE = now;
            Data.REV_ACC_IST_EMP_NO = empData.Single().EMP_NO;
            Data.REV_ACC_GB_CODE = type;
            Data.REV_ACC_DET_SEQ = detail.ToString();
            Data.REV_ACC_PRICE = price;
            Data.REV_ACC_NAME = name;
            Data.REV_ACC_CONTENT = "";
            Data.ACC_DAY = "";
            Data.ACC_SEQ = 0;
            Data.ACC_SUB_SEQ = 0;

            DB.REV_3.Add(Data);

            var reData = DB.REV_0.Where(w => w.REV_DAY.Equals(revDay))
                                 .Where(w => w.REV_SEQ == revSeq);

            var rfData = DB.REV_3.Where(w => w.REV_DAY.Equals(revDay))
                                 .Where(w => w.REV_SEQ == revSeq)
                                 .GroupBy(g => new
                                 {
                                     g.REV_DAY,
                                     g.REV_SEQ
                                 })
                                 .Select(s => new
                                 {
                                     price = s.Sum(sum => sum.REV_ACC_PRICE)
                                 });

            int dcPrice = 0;

            if (rfData.Any())
                dcPrice = Convert.ToInt32(rfData.Single().price.ToString());

            int maxPrice = Convert.ToInt32(reData.Single().REV_PRICE.ToString());

            if (dcPrice > maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "F";
            }
            else if (dcPrice < maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "N";
            }
            else
            {
                reData.Single().REV_CHK_PRICE = "Y";
            }

            DB.SaveChanges();

            chkPrice(revDay, revSeq);

            return Redirect("/Reserve");
        }

        public void chkPrice(string date, int seq)
        {
            var raData = DB.REV_3.Where(w => w.REV_DAY.Equals(date))
                                 .Where(w => w.REV_SEQ == seq)
                                 .Where(w => w.REV_ACC_GB_CODE != "REFD")
                                 .GroupBy(g => new
                                 {
                                     g.REV_DAY,
                                     g.REV_SEQ
                                 })
                                 .Select(s => new
                                 {
                                     price = s.Sum(sum => sum.REV_ACC_PRICE)
                                 });

            var rfData = DB.REV_3.Where(w => w.REV_DAY.Equals(date))
                                 .Where(w => w.REV_SEQ == seq)
                                 .Where(w => w.REV_ACC_GB_CODE.Equals("REFD"))
                                 .GroupBy(g => new
                                 {
                                     g.REV_DAY,
                                     g.REV_SEQ
                                 })
                                 .Select(s => new
                                 {
                                     price = s.Sum(sum => sum.REV_ACC_PRICE)
                                 });

            var reData = DB.REV_0.Where(w => w.REV_DAY.Equals(date))
                                 .Where(w => w.REV_SEQ == seq);

            int dcPrice = 0;

            if (rfData.Any())
                dcPrice = Convert.ToInt32(rfData.Single().price.ToString());

            int accPrice = Convert.ToInt32(raData.Single().price.ToString()) - dcPrice,//Convert.ToInt32(rfData.Single().price.ToString()),
                maxPrice = Convert.ToInt32(reData.Single().REV_PRICE.ToString());

            if (accPrice > maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "F";
            } 
            else if (accPrice < maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "N";
            }
            else
            {
                reData.Single().REV_CHK_PRICE = "Y";
            }

            DB.SaveChanges();

        }









        public JsonResult Json_getProducts(string type, string nation, string area)
        {
            var pdtData = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                  .Select(s => new
                                         {
                                             code = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                             key = s.PDT_IST_EMP_NO,
                                             title = s.PDT_TITLE,
                                             s.PDT_TYPE_CODE,
                                             s.PDT_NATION_CODE,
                                             s.PDT_AREA_CODE
                                         });

            if (!string.IsNullOrEmpty(type))
            {
                pdtData = pdtData.Where(w => w.PDT_TYPE_CODE.Equals(type));
            }

            if (!string.IsNullOrEmpty(nation))
            {
                pdtData = pdtData.Where(w => w.PDT_NATION_CODE.Equals(nation));
            }

            if (!string.IsNullOrEmpty(area))
            {
                var aaa = getAreaCode(area);
                //pdtData = pdtData.Where(w => w.PDT_AREA_CODE.Equals(area));
                pdtData = pdtData.Where(w => aaa.Contains(w.PDT_AREA_CODE));
            }

            return Json(pdtData);
        }

        public JsonResult Json_getCustomer(string name, string tel)
        {
            var cuData = NDB.CU001.Where(w => w.DEL_FG.Equals("N"))
                                  .Select(s => new
                                  {
                                      code = s.CU_YY + s.CU_SEQ,
                                      name = s.CU_NM_KOR,
                                      first = s.CU_NM_ENG_FIRST,
                                      last = s.CU_NM_ENG_LAST,
                                      mail = s.EMAIL,
                                      tel = s.HANDPHONE1 + "-" + s.HANDPHONE2 + "-" + s.HANDPHONE3,
                                  });

            if (!string.IsNullOrEmpty(name))
            {
                cuData = cuData.Where(w => w.name.Equals(name));
            }

            if (!string.IsNullOrEmpty(tel))
            {
                cuData = cuData.Where(w => w.tel.Equals(tel));
            }

            return Json(cuData);
        }

        public JsonResult JSON_getSellPrice(string code)
        {
            var rev_Code = code.Split('_');

            string revYY = rev_Code[0];

            int revSeq = Convert.ToInt32(rev_Code[1]);

            var rpData = DB.REV_1.Where(w => w.REV_DAY.Equals(revYY))
                                 .Where(w => w.REV_SEQ == revSeq)
                                 .Select(s => new
                                 {
                                     pdtCode = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                     pdtKey = s.PDT_IST_EMP_NO,
                                     prcKey = s.PRC_SEQ,
                                     s.PRC_CONTENT,
                                     s.REV_PRC_CONTENT,
                                     s.REV_PRC_GB,
                                     s.PRC_CURRENCY_CODE,
                                     s.REV_PRC_PRICE,
                                     s.REV_PRC_DISCOUNT,
                                     s.REV_PRC_ADDITIONAL_PRICE,
                                     s.REV_PRC_CNT,
                                     s.REV_PRC_TOTAL_PRICE,
                                     aPrice = s.REV_PRC_PRICE,
                                     cPrice = 0,
                                     bPrice = 0,
                                 });

            try
            {
                int prcKey = Convert.ToInt32(rpData.Single().prcKey.ToString());

                if (prcKey < 1)
                    return Json(rpData);

                var rpData2 = rpData.Join(DB.PRC_0,
                                         a => a.pdtCode + "_" + a.prcKey,
                                         b => (b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PRC_SEQ),
                                         (a, b) => new
                                         {
                                             a.pdtCode,
                                             a.pdtKey,
                                             a.prcKey,
                                             a.PRC_CONTENT,
                                             a.REV_PRC_CONTENT,
                                             a.REV_PRC_GB,
                                             a.PRC_CURRENCY_CODE,
                                             a.REV_PRC_PRICE,
                                             a.REV_PRC_DISCOUNT,
                                             a.REV_PRC_ADDITIONAL_PRICE,
                                             a.REV_PRC_CNT,
                                             a.REV_PRC_TOTAL_PRICE,
                                             aPrice = b.PRC_Adult,
                                             cPrice = b.PRC_Child,
                                             bPrice = b.PRC_Baby,
                                         });

                return Json(rpData2);
            }
            catch(Exception e)
            {
            }

            return Json(rpData);
        }

        public JsonResult JSON_getHotels(string code)
        {
            var rev_Code = code.Split('_');

            string revYY = rev_Code[0];

            int revSeq = Convert.ToInt32(rev_Code[1]);
            
            var data = DB.TB_HtlResInfo.Where(w => w.REV_DAY.Equals(revYY))
                                       .Where(w => w.REV_SEQ == revSeq)
                                       .ToList()
                                       .Select(s => new
                                       {
                                           s.REV_DAY,
                                           s.REV_SEQ,
                                           s.HTL_SEQ,
                                           s.HTL_NATION_CODE,
                                           s.HTL_AREA_CODE,
                                           s.HTL_TYPE,
                                           s.HTL_ChkIn,
                                           s.Stay_Seq,
                                           s.HTL_StayDays,
                                           s.CU_YY,
                                           s.CU_SEQ,
                                           s.CU_NAME,
                                           s.CU_NAME_FIRST,
                                           s.CU_NAME_LAST,
                                           s.CU_HP,
                                           s.CU_EMAIL,
                                           s.ADULT_CNT,
                                           s.CHILD_CNT,
                                           s.BABY_CNT,
                                           s.HTL_RoomType,
                                           s.Htl_ResComment
                                       });

            return Json(data);
        }

        public JsonResult JSON_getAcc(string code)
        {
            var rev_Code = code.Split('_');

            string revYY = rev_Code[0];

            int revSeq = Convert.ToInt32(rev_Code[1]);

            var data = DB.REV_3.Where(w => w.REV_DAY.Equals(revYY))
                                 .Where(w => w.REV_SEQ == revSeq)
                                 .ToList()
                                 .Select(s => new
                                 {
                                     s.REV_ACC_SEQ,
                                     REV_ACC_DATE = convertDate8To10(s.REV_ACC_DATE),
                                     REV_ACC_REG_DATE = convertDate8To10(s.REV_ACC_REG_DATE),
                                     REV_ACC_IST_EMP_NO = getName(Convert.ToInt32(s.REV_ACC_IST_EMP_NO.ToString())),
                                     REV_ACC_GB_CODE = getAccGB(s.REV_ACC_GB_CODE),
                                     REV_ACC_DET_SEQ = getDetail(s.REV_ACC_GB_CODE, s.REV_ACC_DET_SEQ),
                                     s.REV_ACC_PRICE,
                                     s.REV_ACC_NAME,
                                     s.REV_ACC_CONTENT,
                                 });

            return Json(data);
        }

        public JsonResult JSON_getTraveler(string code)
        {
            var rev_Code = code.Split('_');

            string revYY = rev_Code[0];

            int revSeq = Convert.ToInt32(rev_Code[1]);

            var rcData = DB.REV_2.Where(w => w.REV_DAY.Equals(revYY))
                                 .Where(w => w.REV_SEQ == revSeq);

            return Json(rcData);
        }

        public JsonResult JSON_getVouData(string REV_DAY, string REV_SEQ, string type)
        {
            string REVDAY = REV_DAY; 
            int REVSEQ = Convert.ToInt32(REV_SEQ);

            var Data = DB.REV_0.Where(w => w.REV_DAY.Equals(REVDAY))
                               .Where(w => w.REV_SEQ == REVSEQ)
                               .Select(s => new
                               {
                                   s.CORP_CODE,
                                   s.PDT_TYPE_CODE,
                                   s.PDT_IST_EMP_NO,
                                   s.PDT_YY,
                                   s.PDT_SEQ
                               });

            var Data2 = DB.PDT_2.Join(Data,
                                     a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                     b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                     (a, b) => new
                                     {
                                         a.PDT_SCH3,
                                         a.PDT_SCH4,
                                         a.PDT_SCH5
                                     });

            string cont = "";

            switch(type)
            {
                case "include":
                    cont = Data2.Single().PDT_SCH3.ToString();
                    break;
                case "notincluded":
                    cont = Data2.Single().PDT_SCH4.ToString();
                    break;
                //case "careful":
                //    cont = Data2.Single().PDT_SCH3.ToString();
                //    break;
                case "cancellation":
                    cont = Data2.Single().PDT_SCH5.ToString();
                    break;
            }

            return Json(cont);
        }

        public JsonResult JSON_chkVoucher(string code)
        {
            string REV_DAY = code.Split('_')[0];

            int REV_SEQ = Convert.ToInt32(code.Split('_')[1]);

            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                  .Where(w => w.REV_SEQ == REV_SEQ)
                                  .Select(s => new
                                  {
                                      s.CHK_VOU
                                  });

            string txt = "N";

            try
            {
                if (revData.Single().CHK_VOU == "Y")
                {
                    txt = "Y";
                }
            }
            catch(Exception e)
            {

            }

            return Json(txt);
        }

        public JsonResult JSON_getVoucher(string day, int seq)
        {
            var revData = DB.REV_0.Where(w => w.REV_DAY.Equals(day))
                                  .Where(w => w.REV_SEQ == seq)
                                  .Select(s => new
                                  {
                                      s.REV_DAY,
                                      s.REV_SEQ,
                                      s.REV_STARTDAY,
                                      s.PDT_TITLE,
                                      s.CU_YY,
                                      s.CU_SEQ,
                                  });

            var rcData = DB.REV_2.Where(w => w.REV_DAY.Equals(day))
                                 .Where(w => w.REV_SEQ == seq)
                                 .Select(s => new
                                 {
                                     s.REV_CU_SEQ,
                                     s.CU_NAME
                                 });

            int rcCnt = rcData.Count() - 1;
            string rcName = rcData.Where(w => w.REV_CU_SEQ == 1).Single().CU_NAME.ToString();

            var vouData = DB.VOU_0.Where(w => w.REV_DAY.Equals(day))
                                  .Where(w => w.REV_SEQ == seq)
                                  .Where(w => w.VOU_SEQ == 1)
                                  .Select(s => new
                                  {
                                      s.REV_DAY,
                                      s.REV_SEQ,
                                      s.VOU_CONT1,
                                      s.VOU_CONT2,
                                      s.VOU_CONT3,
                                      s.VOU_CONT4,
                                      s.VOU_CONT5,
                                      s.VOU_LATITUDE,
                                      s.VOU_LONGITUDE,
                                      s.VOU_PHOTO1,
                                      s.VOU_PHOTO2,
                                      s.VOU_PHOTO3,
                                  });

            var result = vouData.Join(revData,
                                     a => a.REV_DAY + "_" + a.REV_SEQ,
                                     b => b.REV_DAY + "_" + b.REV_SEQ,
                                     (a, b) => new
                                     {
                                         a.VOU_CONT1,
                                         a.VOU_CONT2,
                                         a.VOU_CONT3,
                                         a.VOU_CONT4,
                                         a.VOU_CONT5,
                                         a.VOU_LATITUDE,
                                         a.VOU_LONGITUDE,
                                         a.VOU_PHOTO1,
                                         a.VOU_PHOTO2,
                                         a.VOU_PHOTO3,
                                         b.REV_STARTDAY,// = b.REV_STARTDAY.Replace('-', '.'),
                                         b.PDT_TITLE,
                                         rcCnt = rcCnt,
                                         rcName = rcName,
                                     });

            return Json(result);
        }

        public JsonResult JSON_getDetail(string code)
        {
            var Data = DB.DETAIL.Where(w => w.DET_TYPE.Equals(code))
                                .Select(s => new
                                {
                                    s.DET_TYPE,
                                    s.DET_SEQ,
                                    s.DET_NAME
                                });

            return Json(Data);
        }


    }
}