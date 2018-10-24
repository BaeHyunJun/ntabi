using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class HotelController : Controller
    {
        DataBase DB = new DataBase();

        string corp_code = "NTB";

        public string getName(int emp)
        {
            string name = "";

            name = DB.EMP_0.Where(w => w.EMP_NO == emp).Single().EMP_NAME;

            return name;
        }

        public string getOrderTxt(string code)
        {
            string txt = "";

            switch (code)
            {
                case "01":
                    txt = "준비중";
                    break;
                case "02":
                    txt = "판매중";
                    break;
                case "03":
                    txt = "마감";
                    break;
                default:
                    txt = code;
                    break;
            }

            return txt;
        }

        public string getNationTxt(string code)
        {
            string txt = "";

            switch (code)
            {
                case "JP":
                    txt = "일본";
                    break;
                case "HM":
                    txt = "홍콩/마카오";
                    break;
                case "TW":
                    txt = "대만";
                    break;
                case "SA":
                    txt = "동남아";
                    break;
                case "HN":
                    txt = "하와이";
                    break;
                case "GS":
                    txt = "괌/사이판";
                    break;
                default:
                    txt = code;
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

        // GET: Hotel
        public ActionResult Index(string nation = "", string area = "", string state = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var hotels = DB.HTL_0.ToList()
                                 .Select(s => new
                                 {
                                     s.HTL_SEQ,
                                     nation = getNationTxt(s.HTL_NATION_CODE),
                                     area = getAreaTxt(s.HTL_AREA_CODE),
                                     s.HTL_NAME,
                                     proc = getOrderTxt(s.HTL_PROC_CODE),
                                     s.HTL_CHK_PRICE,
                                     s.HTL_CHK_DAYS,
                                     s.HTL_CHK_CONTENT,
                                 });

            if (!string.IsNullOrEmpty(nation))
            {
                hotels = hotels.Where(w => w.nation.Equals(getNationTxt(nation)));
            }

            if (!string.IsNullOrEmpty(area))
            {
                hotels = hotels.Where(w => w.area.Equals(getAreaTxt(area)));
            }

            if (!string.IsNullOrEmpty(state))
            {
                hotels = hotels.Where(w => w.proc.Equals(getOrderTxt(state)));
            }

            return View(hotels);
        }

        public ActionResult udtHotels(string seq)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (!string.IsNullOrEmpty(seq))
            {
                var hotelData = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                        .Select(s => new
                                        {
                                            s.HTL_SEQ,
                                            s.HTL_NATION_CODE,
                                            s.HTL_AREA_CODE,
                                            s.HTL_PROC_CODE,
                                            s.HTL_NAME,
                                            s.HTL_IST_DATE,
                                            s.HTL_IST_EMP_NO,
                                            s.HTL_UDT_DATE,
                                            s.HTL_UDT_EMP_NO,
                                            s.HTL_CHK_PRICE,
                                            s.HTL_CHK_DAYS,
                                            s.HTL_CHK_CONTENT,
                                            s.HTL_TYPE,
                                        });

                ViewBag.hotelData = hotelData;
            }

            return View();
        }

        public ActionResult chkDays(string seq)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var prcData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                  .Select(s => new
                                  {
                                      s.HTL_PRC_SEQ,
                                      s.HTL_Currency_CODE,
                                      s.HTL_PRICE,
                                      s.HTL_PROFIT,
                                      s.HTL_ROOM,
                                      s.HTL_EXP,
                                      s.HTL_PRICE2,
                                      s.HTL_PRICE3,
                                      s.HTL_PRICE4,
                                  });

            return View(prcData);
        }

        public ActionResult chkPrice(string seq)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var hotelType = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                    .Select(s => new
                                    {
                                        s.HTL_TYPE,
                                    })
                                    .FirstOrDefault()
                                    .HTL_TYPE
                                    .ToString();

            var prcData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                  .Select(s => new
                                  {
                                      s.HTL_PRC_SEQ,
                                      s.HTL_Currency_CODE,
                                      s.HTL_PRICE,
                                      s.HTL_PROFIT,
                                      s.HTL_ROOM,
                                      s.HTL_EXP,
                                      s.HTL_PRICE2,
                                      s.HTL_PRICE3,
                                      s.HTL_PRICE4,
                                      //TYPE = "",
                                      //s.HTL_SEQ,
                                  });

            //prcData = prcData.Join(hotelType,
            //                       a => a.HTL_SEQ,
            //                       b => b.HTL_SEQ,
            //                      (a, b) => new
            //                      {
            //                          a.HTL_PRC_SEQ,
            //                          a.HTL_Currency_CODE,
            //                          a.HTL_PRICE,
            //                          a.HTL_PROFIT,
            //                          a.HTL_ROOM,
            //                          a.HTL_EXP,
            //                          a.HTL_PRICE2,
            //                          a.HTL_PRICE3,
            //                          a.HTL_PRICE4,
            //                          TYPE = b.HTL_TYPE,
            //                          a.HTL_SEQ,
            //                      });

            ViewBag.hotelType = hotelType;

            return View(prcData);
        }

        public ActionResult chkContent(string seq)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult updatePrc(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var prc_seq = f["prc_seq[]"].Split(',');
            var currency = f["currency[]"].Split(',');
            var price1 = f["price1[]"].Split(',');
            var price2 = f["price2[]"].Split(',');
            var price3 = f["price3[]"].Split(',');
            var price4 = f["price4[]"].Split(',');
            var profit = f["profit[]"].Split(',');
            //var room = f["room[]"].Split(',');
            var content = f["content[]"].Split(',');
            var redirect = f["redirect"].ToString();
            var seq = f["seq"].ToString();

            var prcData = DB.HTL_1.Where(w => w.HTL_SEQ.ToString().Equals(seq));

            int idx = 1;

            string cr = "",
                   pc = "",
                   pf = "",
                   rm = "",
                   ct = "",
                   pc2 = "",
                   pc3 = "",
                   pc4 = "";

            if (prcData.Any())
            {
                foreach (var item in prcData)
                {
                    DB.HTL_1.Remove(item);
                }

                DB.SaveChanges();
            }

            for (var i = 0; prc_seq.Length > i; i++)
            {
                cr = !string.IsNullOrEmpty(currency[i].ToString()) ? currency[i].ToString() : "";
                pc = !string.IsNullOrEmpty(price1[i].ToString()) ? price1[i].ToString() : "";
                pf = !string.IsNullOrEmpty(profit[i].ToString()) ? profit[i].ToString() : "0";
                //rm = !string.IsNullOrEmpty(room[i].ToString()) ? room[i].ToString() : "";
                ct = !string.IsNullOrEmpty(content[i].ToString()) ? content[i].ToString() : "";

                pc2 = !string.IsNullOrEmpty(price2[i].ToString()) ? price2[i].ToString() : "";
                pc3 = !string.IsNullOrEmpty(price3[i].ToString()) ? price3[i].ToString() : "";
                pc4 = !string.IsNullOrEmpty(price4[i].ToString()) ? price4[i].ToString() : "";

                //udtPrice(seq, (idx).ToString(), cr, pc, pf, rm, ct, pc2, pc3, pc4);
                udtPrice(seq, (idx).ToString(), cr, pc, pf, ct, pc2, pc3, pc4);

                idx += 1;
            }

            if (prcData.Any())
            {
                var udtPdt = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(seq));

                udtPdt.Single().HTL_CHK_PRICE = "Y";

                DB.SaveChanges();
            }

            return Redirect(redirect);
        }

        public ActionResult updateHotel(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string seq = f["seq"].ToString(),
                   nation = f["nation"] != null && !string.IsNullOrEmpty(f["nation"]) ? f["nation"].ToString() : "01",//f["nation"].ToString(),
                   area = f["area"] != null && !string.IsNullOrEmpty(f["area"]) ? f["area"].ToString() : "01",//f["area"].ToString(),
                   title = f["title"].ToString(),
                   proc = f["proc"] != null && !string.IsNullOrEmpty(f["proc"]) ? f["proc"].ToString() : "01",
                   redirect = f["redirect"].ToString(),
                   type = f["type"].ToString(),
                   emp = user.Login.ToString();

            var hotelData = DB.HTL_0.OrderByDescending(o => o.HTL_SEQ)
                                    .Select(s => new
                                    {
                                        seq = s.HTL_SEQ,
                                    }).Take(1);

            int idx = 1;

            try { idx = int.Parse(hotelData.Single().seq.ToString()) + 1; }
            catch { }

            DateTime now = DateTime.Now;

            if (seq == "")
            {
                HTL_0 hotelDB = new HTL_0();

                hotelDB.CORP_CODE = corp_code;
                hotelDB.HTL_SEQ = Convert.ToInt16(idx.ToString());
                hotelDB.HTL_NATION_CODE = nation;
                hotelDB.HTL_AREA_CODE = area;
                hotelDB.HTL_TYPE = type;
                hotelDB.HTL_NAME = title;
                hotelDB.HTL_PROC_CODE = proc;
                hotelDB.HTL_IST_DATE = now.ToString("yyyy.MM.dd");
                hotelDB.HTL_IST_EMP_NO = Convert.ToInt32(emp);
                hotelDB.HTL_UDT_DATE = now.ToString("yyyy.MM.dd");
                hotelDB.HTL_UDT_EMP_NO = Convert.ToInt32(emp);

                DB.HTL_0.Add(hotelDB);
            }
            else
            {
                var udtPdt = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(seq));

                udtPdt.Single().HTL_TYPE = type;
                udtPdt.Single().HTL_NAME = title;
                udtPdt.Single().HTL_PROC_CODE = proc;
                udtPdt.Single().HTL_UDT_DATE = now.ToString("yyyy.MM.dd");
                udtPdt.Single().HTL_UDT_EMP_NO = Convert.ToInt32(emp);
            }

            DB.SaveChanges();

            return Redirect(redirect);

            //return View();
        }

        public void udtPrice(string seq, string idx, string currency, string price1 = "", string profit = "0", string content = "", string price2 = "", string price3 = "", string price4 = "")
        {
            HTL_1 newPrice = new HTL_1();

            newPrice.CORP_CODE = corp_code;
            newPrice.HTL_SEQ = Convert.ToInt32(seq);
            newPrice.HTL_PRC_SEQ = Convert.ToInt32(idx);
            newPrice.HTL_Currency_CODE = currency;
            newPrice.HTL_PRICE = price1 == "" ? 0 : Convert.ToInt32(price1);
            newPrice.HTL_PROFIT = Convert.ToInt32(profit);
            //newPrice.HTL_ROOM = room;
            newPrice.HTL_EXP = content;
            newPrice.HTL_PRICE2 = price2 == "" ? 0 : Convert.ToInt32(price2);
            newPrice.HTL_PRICE3 = price3 == "" ? 0 : Convert.ToInt32(price3);
            newPrice.HTL_PRICE4 = price4 == "" ? 0 : Convert.ToInt32(price4);

            DB.HTL_1.Add(newPrice);
            DB.SaveChanges();
        }

        public void udtDay(string seq, string prcCode, string type, string day)
        {
            var chkPdtDays = DB.HTL_2.ToList()
                                     .Where(w => w.CORP_CODE.Equals(corp_code))
                                     .Where(w => w.HTL_SEQ.ToString().Equals(seq))
                                     .Where(w => w.HTL_PRC_SEQ.ToString().Equals(prcCode))
                                     .Where(w => w.HTL_DATE.Equals(day));
            if (type == "05")
            {
                foreach (var item in chkPdtDays)
                {
                    DB.HTL_2.Remove(item);
                }
            }
            else if (chkPdtDays.Any())
            {
                chkPdtDays.Single().HTL_STATE_CODE = type;
            }
            else
            {
                HTL_2 pdtDays = new HTL_2();

                pdtDays.CORP_CODE = corp_code;
                pdtDays.HTL_SEQ = Convert.ToInt32(seq);
                pdtDays.HTL_PRC_SEQ = Convert.ToInt32(prcCode);
                pdtDays.HTL_STATE_CODE = type;
                pdtDays.HTL_DATE = day;

                DB.HTL_2.Add(pdtDays);
            }

            DB.SaveChanges();
        }

        public void udtDays(string seq, string prcCode, string type, string[] days)
        {
            int count = days.Count();

            for (int i = 0; i < count; i++)
            {
                string day = days[i];

                udtDay(seq, prcCode, type, day);
            }

            var udtPdt = DB.HTL_0.Where(w => w.HTL_SEQ.ToString().Equals(seq));

            udtPdt.Single().HTL_CHK_DAYS = "Y";

            DB.SaveChanges();
        }

        public JsonResult getDays(string seq, string prcCode = "")
        {
            var dateData = DB.HTL_2.ToList()
                                   .Where(w => w.HTL_SEQ.ToString().Equals(seq));

            if (!string.IsNullOrEmpty(prcCode))
            {
                dateData = dateData.Where(w => w.HTL_PRC_SEQ.ToString().Equals(prcCode));
            }

            var dates = dateData.Select(s => new
                                       {
                                           s.HTL_PRC_SEQ,
                                           day = s.HTL_DATE,
                                           state = s.HTL_STATE_CODE,
                                       });

            return Json(dates);
        }
    }
}