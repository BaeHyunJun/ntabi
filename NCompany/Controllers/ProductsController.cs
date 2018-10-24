using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class ProductsController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

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

            switch(code)
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

        public ActionResult Index(string type = "", string nation = "", string area = "", string city = "", string emp = "", string code = "", string options = "", string state = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var products = DB.PDT_0.ToList()
                                   .Select(s => new
                                    {
                                        code    = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                        order   = s.PDT_ORDER_NO,
                                        proc    = getOrderTxt(s.PDT_PROC_CODE),
                                        nation  = getNationTxt(s.PDT_NATION_CODE),
                                        area    = getAreaTxt(s.PDT_AREA_CODE),
                                        title   = s.PDT_TITLE,
                                        options = s.PDT_OPTIONS,
                                        s.PDT_Option1,
                                        s.PDT_Option2,
                                        s.PDT_Option3,
                                        s.PDT_Option4,
                                        s.PDT_Option5,
                                        s.PDT_Option6,
                                        key = s.PDT_IST_EMP_NO,
                                        s.PDT_TYPE_CODE,
                                        s.PDT_EMP_NO,
                                        s.PDT_DAYS_CODE,
                                        city = s.PDT_SCITY_CODE,
                                    });

            if (!string.IsNullOrEmpty(type))
            {
                products = products.Where(w => w.PDT_TYPE_CODE.Equals(type));
            }

            if (!string.IsNullOrEmpty(nation))
            {
                products = products.Where(w => w.nation.Equals(getNationTxt(nation)));
            }

            if (!string.IsNullOrEmpty(area))
            {
                products = products.Where(w => w.area.Equals(getAreaTxt(area)));
            }

            if (!string.IsNullOrEmpty(city))
            {
                products = products.Where(w => w.city.Equals(city));
            }

            if (string.IsNullOrEmpty(emp))
            {
                emp = user.Login.ToString();

                products = products.Where(w => w.PDT_EMP_NO == Convert.ToInt32(emp));
            }
            else if (emp == "all")
            {

            }
            else
            {
                products = products.Where(w => w.PDT_EMP_NO == Convert.ToInt32(emp));
            }

            if (!string.IsNullOrEmpty(code))
            {
                products = products.Where(w => w.PDT_DAYS_CODE.Equals(code));
            }

            if (!string.IsNullOrEmpty(options))
            {
                products = products.Where(w => w.options.Equals(options));
            }

            if (!string.IsNullOrEmpty(state))
            {
                products = products.Where(w => w.proc.Equals(getOrderTxt(state)));
            }

            return View(products);
        }

        public ActionResult defPdt(string code = "", string key = "", string url = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (code != "")
            {
                var priKey = getCode(code, key);

                string corp     = priKey[0];
                string type     = priKey[1];
                string emp      = priKey[2];
                string pdtyy    = priKey[3];
                string pdtseq   = priKey[4];

                var getPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                     .Select(s => new
                                     {
                                         type       = s.PDT_TYPE_CODE,
                                         nation     = s.PDT_NATION_CODE,
                                         area       = s.PDT_AREA_CODE,
                                         title      = s.PDT_TITLE,
                                         content    = s.PDT_CONTENT,
                                         emp        = s.PDT_EMP_NO,
                                         scity      = s.PDT_SCITY_CODE,
                                         proc       = s.PDT_PROC_CODE,
                                         order      = s.PDT_ORDER_NO,
                                         days       = s.PDT_DAYS_CODE,
                                         options    = s.PDT_OPTIONS,
                                         istdate    = s.PDT_IST_DATE,
                                         udtdate    = s.PDT_UDT_DATE,
                                         istemp     = s.PDT_IST_EMP_NO,
                                         udtemp     = s.PDT_UDT_EMP_NO,
                                         s.PDT_Option1,
                                         s.PDT_Option2,
                                         s.PDT_Option3,
                                         s.PDT_Option4,
                                         s.PDT_TOUR_CITY,
                                         s.PDT_MIN_COUNT,
                                         s.PDT_COMBINE_HTL,
                                     });

                ViewBag.pdtData = getPdt;

                var getInHotels = DB.PDT_4.Where(w => w.CORP_CODE.Equals(corp))
                                          .Where(w => w.PDT_TYPE_CODE.Equals(type))
                                          .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(emp))
                                          .Where(w => w.PDT_YY.ToString().Equals(pdtyy))
                                          .Where(w => w.PDT_SEQ.ToString().Equals(pdtseq))
                                          .Select(s => new
                                          {
                                              s.PDT_IN,
                                              s.PDT_HTL_SEQ,
                                              s.PDT_CANRES,
                                          });

                ViewBag.getInHotels = getInHotels;
            }

            ViewBag.rUrl = url;

            return View();
        }

        public ActionResult chkDays(string code, string key)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            int emp = Convert.ToInt32(priKey[2]);
            string pdtyy = priKey[3];
            int pdtseq = Convert.ToInt32(priKey[4]);

            var trfData = DB.TRF_0.Where(w => w.TRF_SUB_SEQ == 1)
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == emp)
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == pdtseq)
                                  //.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                            .Select(s => new
                            {
                                s.TRF_SEQ,
                                s.TRF_TITLE,
                                s.TRF_STIME,
                                s.TRF_ETIME,
                            });

            var prcData = DB.PRC_0
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == emp)
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == pdtseq)//.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                  .Select(s => new
                                  {
                                      s.PRC_SEQ,
                                      s.PRC_Currency_CODE,
                                      s.PRC_Adult,
                                      s.PRC_EXP,
                                  });

            var dateData = DB.PDT_1.ToList()
                                   .Where(w => w.CORP_CODE.Equals(corp))
                                   .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                   .Where(w => w.PDT_IST_EMP_NO == emp)
                                   .Where(w => w.PDT_YY.Equals(pdtyy))
                                   .Where(w => w.PDT_SEQ == pdtseq);

            //var dates = dateData.Select(s => new
            //{
            //    s.TRF_SEQ,
            //    s.PRC_SEQ,
            //    day = s.PDT_DATE,
            //    state = s.PDT_STATE_CODE,
            //});

            ViewBag.trf = trfData;
            ViewBag.prc = prcData;

            return View();
        }

        public ActionResult chkPrice(string code, string key)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

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

        public ActionResult chkTraff(string code, string key)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var dayData = DB.TRF_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                            .Select(s => new
                            {
                                s.TRF_SEQ,
                                s.TRF_SUB_SEQ,
                                //s.TRF_CO_CODE,
                                s.TRF_TYPE,
                                s.TRF_TITLE,
                                s.TRF_STIME,
                                s.TRF_ETIME,
                                s.TRF_SAREA,
                                s.TRF_EAREA,
                            });

            var natLists = DB.TB_ComCode.Where(w => w.CatCode.Equals("NAT"))
                                        .Select(s => new
                                        {
                                            s.ComCode,
                                            s.Code_Kor,
                                            s.SortNo,
                                        })
                                        .OrderBy(o => o.SortNo);

            ViewBag.natLists = natLists;

            return View(dayData);
        }

        public ActionResult chkSched(string code, string key, string type)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var schData = DB.PDT_2.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                  .Select(s => new 
                                  {
                                      code,
                                      s.PDT_IMG,
                                      s.PDT_SCH0,
                                      s.PDT_SCH1,
                                      s.PDT_SCH2,
                                      s.PDT_SCH3,
                                      s.PDT_SCH4,
                                      s.PDT_SCH5,
                                      s.PDT_SCH6,
                                  });

            var scheduleData = DB.PDT_3.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                       .Select(s => new
                                       {
                                           code,
                                           s.SCH_DAY,
                                           s.SCH_CONT
                                       });

            var resultData = schData.Join(scheduleData,
                                         a => a.code,
                                         b => b.code,
                                         (a, b) => new
                                         {
                                             a.PDT_IMG,
                                             a.PDT_SCH0,
                                             a.PDT_SCH1,
                                             a.PDT_SCH2,
                                             a.PDT_SCH3,
                                             a.PDT_SCH4,
                                             a.PDT_SCH5,
                                             a.PDT_SCH6,
                                             b.SCH_DAY,
                                             b.SCH_CONT,
                                         });

            return View(resultData);
        }

        public ActionResult setImage(string code, string key)
        {
            var priKey = getCode(code, key);

            string corp_code = priKey[0],
                   pdt_type_code = priKey[1],
                   pdt_ist_emp_no = priKey[2],
                   pdt_yy = priKey[3],
                   pdt_seq = priKey[4];

            var imgData = DB.PDT_5.Where(w => w.CORP_CODE.ToString().Equals(corp_code))
                                  .Where(w => w.PDT_TYPE_CODE.ToString().Equals(pdt_type_code))
                                  .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(pdt_ist_emp_no))
                                  .Where(w => w.PDT_YY.ToString().Equals(pdt_yy))
                                  .Where(w => w.PDT_SEQ.ToString().Equals(pdt_seq))
                                  .OrderBy(o => o.PDT_IMG_SEQ)
                                  .Select(s => new
                                  {
                                      s.PDT_IMG_URL
                                  });

            return View(imgData);
        }

        public ActionResult setOptions(string nation, string area, string days)
        {
            var hotelData = DB.HTL_0.Where(w => w.HTL_NATION_CODE.Equals(nation))
                                    .Where(w => w.HTL_AREA_CODE.Equals(area))
                                    //.Where(w => w.HTL_PROC_CODE)
                                    .Select(s => new 
                                    {
                                        s.HTL_SEQ,
                                        s.HTL_NAME,
                                    });


            return View(hotelData);
        }

        public ActionResult searchHotels(string title)
        {
            var hotelData = DB.HTL_0.Where(w => w.HTL_NAME.Contains(title))
                                    .Select(s => new
                                    {
                                        s.HTL_SEQ,
                                        s.HTL_NAME
                                    });

            return View(hotelData);
        }

        public ActionResult inQueryAir()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var chatRoomData = DB.CHAT_Room.OrderByDescending(o => o.CHAT_isNew).ToList();

            return View(chatRoomData);
        }

        public JsonResult updateSch(string code, string key, string type, string feature = "", string ct1 = "", string ct2 = "", string ct3 = "", string ct4 = "", string ct5 = "", string ct6 = "", string ct7 = "", string schedule = "", string attach = "")
        {
            if (type == "pc")
            {

            }
            else
            {
                if (udtMSch(code, key, feature, ct1, ct2, ct3, ct4, ct5, ct6, ct7, schedule, attach))
                {
                    return Json("모바일 버전 상품 정보가 변경 되었습니다.");
                }
            }

            return Json("상품 정보 업데이트에 실패 했습니다.");
        }

        public bool udtMSch(string code, string key, string feature, string ct1, string ct2, string ct3, string ct4, string ct5, string ct6, string ct7, string schedule, string attach, int day = 1)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string type = priKey[1];
            int emp = Convert.ToInt32(priKey[2]);
            string pdtyy = priKey[3];
            int pdtseq = Convert.ToInt32(priKey[4]);

            var schData = DB.PDT_2.ToList();
                                  //.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
            schData = schData.Where(w => w.CORP_CODE.Equals(corp)).ToList();
            schData = schData.Where(w => w.PDT_TYPE_CODE.Equals(type)).ToList();
            schData = schData.Where(w => w.PDT_IST_EMP_NO == emp).ToList();
            schData = schData.Where(w => w.PDT_YY.Equals(pdtyy)).ToList();
            schData = schData.Where(w => w.PDT_SEQ == pdtseq).ToList();

            if (schData.Any())
            {
                schData.Single().PDT_IMG = feature;                         // 대표이미지
                schData.Single().PDT_SCH0 = HttpUtility.UrlDecode(ct1);     // 예약 진행과정
                schData.Single().PDT_SCH1 = HttpUtility.UrlDecode(ct2);     // 확인 사항
                schData.Single().PDT_SCH2 = HttpUtility.UrlDecode(ct3);     // 특전
                schData.Single().PDT_SCH3 = HttpUtility.UrlDecode(ct4);     // 포함
                schData.Single().PDT_SCH4 = HttpUtility.UrlDecode(ct5);     // 불포함
                schData.Single().PDT_SCH5 = HttpUtility.UrlDecode(ct6);     // 취소수수료
                schData.Single().PDT_SCH6 = HttpUtility.UrlDecode(ct7);     // 추가내용


            }
            else
            {
                PDT_2 insSch = new PDT_2();

                insSch.CORP_CODE        = corp;
                insSch.PDT_TYPE_CODE    = type;
                insSch.PDT_IST_EMP_NO   = emp;
                insSch.PDT_YY           = pdtyy;
                insSch.PDT_SEQ          = pdtseq;

                insSch.PDT_IMG  = feature;                              // 대표 이미지
                insSch.PDT_SCH0 = HttpUtility.UrlDecode(ct1);           // 예약 진행과정
                insSch.PDT_SCH1 = HttpUtility.UrlDecode(ct2);           // 확인 사항
                insSch.PDT_SCH2 = HttpUtility.UrlDecode(ct3);           // 특전
                insSch.PDT_SCH3 = HttpUtility.UrlDecode(ct4);           // 포함
                insSch.PDT_SCH4 = HttpUtility.UrlDecode(ct5);           // 불포함
                insSch.PDT_SCH5 = HttpUtility.UrlDecode(ct6);           // 취소수수료
                insSch.PDT_SCH6 = HttpUtility.UrlDecode(ct7);           // 추가내용

                DB.PDT_2.Add(insSch);
            }

            var scheduleData = DB.PDT_3.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            if (scheduleData.Any())
            {
                scheduleData.Single().SCH_CONT = HttpUtility.UrlDecode(schedule);
                scheduleData.Single().SCH_DAY = day;
            }
            else
            {
                PDT_3 insSchedule = new PDT_3();

                insSchedule.CORP_CODE       = corp;
                insSchedule.PDT_TYPE_CODE   = type;
                insSchedule.PDT_IST_EMP_NO  = emp;
                insSchedule.PDT_YY          = pdtyy;
                insSchedule.PDT_SEQ         = pdtseq;

                insSchedule.SCH_CONT        = HttpUtility.UrlDecode(schedule);
                insSchedule.SCH_DAY         = day;

                DB.PDT_3.Add(insSchedule);
            }

            var chkData1 = DB.PDT_2.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));
            var chkData2 = DB.PDT_3.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            if (chkData1.Any() && chkData2.Any())
            {
                var udtPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

                udtPdt.Single().PDT_Option4 = "Y";
            }

            try
            {
                DB.SaveChanges();
                return true;
            } 
            catch(Exception e)
            {
                return false;
            }
        }

        //public void uploadFile(string fakepath)
        //{
        //    HttpFileCollectionBase uploadedFiles = Request.Files;

        //    var fileName = Path.GetFileName(uploadedFiles[i].FileName);

        //    string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

        //    string fileNames = nowDt + "_" + fileName;

        //    var path = Path.Combine(Server.MapPath("~/attachment"), fileNames);

        //    uploadedFiles[i].SaveAs(path);


        //    string fileloc = Request.Url.GetLeftPart(UriPartial.Authority) + "/attachment/" + fileNames;

        //    attach += fileloc + ";";
        //}

        public void updateImg(string code, string key, List<string> img)
        {
            var priKey = getCode(code, key);

            string corp_code = priKey[0],
                   pdt_type_code = priKey[1],
                   pdt_ist_emp_no = priKey[2],
                   pdt_yy = priKey[3],
                   pdt_seq = priKey[4];

            var imgData = DB.PDT_5.Where(w => w.CORP_CODE.ToString().Equals(corp_code))
                                  .Where(w => w.PDT_TYPE_CODE.ToString().Equals(pdt_type_code))
                                  .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(pdt_ist_emp_no))
                                  .Where(w => w.PDT_YY.ToString().Equals(pdt_yy))
                                  .Where(w => w.PDT_SEQ.ToString().Equals(pdt_seq));

            if (imgData.Any())
            {
                foreach (var item in imgData)
                {
                    DB.PDT_5.Remove(item);
                }

                DB.SaveChanges();
            }

            foreach (var url in img)
            {
                udtImg(corp_code, pdt_type_code, pdt_ist_emp_no, pdt_yy, pdt_seq, img.BinarySearch(url), url);
            }
        }

        public void udtImg(string corp_code, string pdt_type_code, string pdt_ist_emp_no, string pdt_yy, string pdt_seq, int idx, string url)
        {
            PDT_5 addData = new PDT_5();

            addData.CORP_CODE = corp_code;
            addData.PDT_TYPE_CODE = pdt_type_code;
            addData.PDT_IST_EMP_NO = Convert.ToInt32(pdt_ist_emp_no);
            addData.PDT_YY = pdt_yy;
            addData.PDT_SEQ = Convert.ToInt32(pdt_seq);
            addData.PDT_IMG_SEQ = idx;
            addData.PDT_IMG_URL = url;

            DB.PDT_5.Add(addData);

            DB.SaveChanges();
        }

        public ActionResult updateTrf(FormCollection f, string redirect, string code, string key)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }
            
            var seq     = f["seq[]"].Split(',');
            var subSeq  = f["subSeq[]"].Split(',');
            var title   = f["title[]"].Split(',');
            var type    = f["type[]"].Split(',');
            var stime   = f["stime[]"].Split(',');
            var etime   = f["etime[]"].Split(',');
            var sarea   = f["sarea[]"].Split(',');
            var earea   = f["earea[]"].Split(',');

            var trfData = DB.TRF_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            int idx = 1;

            string tt = "",
                   tp = "",
                   st = "",
                   et = "",
                   sa = "",
                   ea = "";

            if (trfData.Any())
            {
                foreach (var item in trfData)
                {
                    DB.TRF_0.Remove(item);
                }

                DB.SaveChanges();
            }

            for (var i = 0; seq.Length > i; i++)
            {
                tt = !string.IsNullOrEmpty(title[i].ToString()) ? title[i].ToString() : "";
                tp = !string.IsNullOrEmpty(type[i].ToString())  ? type[i].ToString()  : "";
                st = !string.IsNullOrEmpty(stime[i].ToString()) ? stime[i].ToString() : "";
                et = !string.IsNullOrEmpty(etime[i].ToString()) ? etime[i].ToString() : "";
                sa = !string.IsNullOrEmpty(sarea[i].ToString()) ? sarea[i].ToString() : "";
                ea = !string.IsNullOrEmpty(earea[i].ToString()) ? earea[i].ToString() : "";

                if (subSeq[i] == "2")
                {
                    idx--;
                }

                udtTraff(code, key, (idx).ToString(), tt, tp, st, et, sa, ea, subSeq[i]);

                idx++;
            }

            if (trfData.Any())
            {
                var udtPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

                udtPdt.Single().PDT_Option1 = "Y";

                DB.SaveChanges();
            }

            return Redirect(HttpUtility.UrlDecode(redirect));
        }

        public ActionResult updatePrc(FormCollection f, string redirect, string code, string key)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var seq         = f["seq[]"].Split(',');
            var currency    = f["currency[]"].Split(',');
            var adultPrice  = f["adult[]"].Split(',');
            var childPrice  = f["child[]"].Split(',');
            var babyPrice   = f["baby[]"].Split(',');
            var profit      = f["profit[]"].Split(',');
            var content     = f["content[]"].Split(',');

            var prcData = DB.PRC_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            string idx = "";

            string cr = "",
                   ap = "",
                   cp = "",
                   bp = "",
                   pf = "",
                   ct = "";

            //if (prcData.Any())
            //{
            //    foreach (var item in prcData)
            //    {
            //        DB.PRC_0.Remove(item);
            //    }

            //    DB.SaveChanges();
            //}

            for (var i = 0; seq.Length > i; i++)
            {
                idx = !string.IsNullOrEmpty(seq[i].ToString())          ? seq[i].ToString()         : "";
                cr = !string.IsNullOrEmpty(currency[i].ToString())      ? currency[i].ToString()    : "";
                ap = !string.IsNullOrEmpty(adultPrice[i].ToString())    ? adultPrice[i].ToString()  : "";
                cp = !string.IsNullOrEmpty(childPrice[i].ToString())    ? childPrice[i].ToString()  : "0";
                bp = !string.IsNullOrEmpty(babyPrice[i].ToString())     ? babyPrice[i].ToString()   : "0";
                pf = !string.IsNullOrEmpty(profit[i].ToString())        ? profit[i].ToString()      : "0";
                ct = !string.IsNullOrEmpty(content[i].ToString())       ? content[i].ToString()     : "";

                udtPrice(code, key, idx, cr, ap, cp, bp, pf, ct);

                //idx += 1;
            }

            if (prcData.Any())
            {
                var udtPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

                udtPdt.Single().PDT_Option2 = "Y";

                DB.SaveChanges();
            }

            return Redirect(HttpUtility.UrlDecode(redirect));
        }

        public ActionResult updatePdt(FormCollection f, string redirect, string code = "", string key = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            //var getPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_NATION_CODE + w.PDT_AREA_CODE + w.PDT_YY + w.PDT_SEQ).Equals(code));

            string type     = f["type"].ToString(),
                   nation   = f["nation"].ToString(),
                   area     = f["area"].ToString(),
                   title    = f["title"].ToString(),
                   content  = f["content"].ToString(),
                   emp      = f["emp"].ToString(),
                   city     = f["city"].ToString(),
                   proc     = f["proc"] != null ? f["proc"].ToString() : "01",
                   order    = f["order"].ToString(),
                   days     = f["days"].ToString(),
                   options  = f["options"].ToString(),
                   cities   = f["cities"].ToString(),
                   minCnt   = f["minCnt"].ToString(),
                   inHotels = f["inHotels"].ToString(),
                   combineHtl = f["combine"] != null ? f["combine"].ToString() : "N";

            int emp_no = Convert.ToInt32(emp);

            DateTime now = DateTime.Now;

            string yy = now.ToString("yy");

            var latPdt = DB.PDT_0.Where(w => w.CORP_CODE.Equals(corp_code))
                                 .Where(w => w.PDT_TYPE_CODE.Equals(type))
                                 .Where(w => w.PDT_IST_EMP_NO == emp_no)
                                 .Where(w => w.PDT_YY.Equals(yy))
                                 .OrderByDescending(o => o.PDT_SEQ)
                                 .Select(s => new
                                 {
                                     seq = s.PDT_SEQ,
                                 }).Take(1);

            int idx = 1;

            try { idx = int.Parse(latPdt.Single().seq.ToString()) + 1; }
            catch { }

            if (code == "")
            {
                PDT_0 productDB = new PDT_0();

                productDB.CORP_CODE = corp_code;
                productDB.PDT_TYPE_CODE = type;
                productDB.PDT_EMP_NO = Convert.ToInt32(emp);
                productDB.PDT_YY = now.ToString("yy");
                productDB.PDT_SEQ = Convert.ToInt16(idx.ToString());
                productDB.PDT_NATION_CODE = nation;
                productDB.PDT_AREA_CODE = area;
                productDB.PDT_TITLE = title;
                productDB.PDT_CONTENT = content;
                productDB.PDT_SCITY_CODE = city;
                productDB.PDT_PROC_CODE = proc;
                productDB.PDT_ORDER_NO = Convert.ToInt16(order);
                productDB.PDT_DAYS_CODE = days;
                productDB.PDT_OPTIONS = options;
                productDB.PDT_IST_DATE = now.ToString("yyyy.MM.dd");
                productDB.PDT_IST_EMP_NO = Convert.ToInt32(emp);
                productDB.PDT_UDT_DATE = now.ToString("yyyy.MM.dd");
                productDB.PDT_UDT_EMP_NO = Convert.ToInt32(emp);
                productDB.PDT_TOUR_CITY = cities;
                productDB.PDT_MIN_COUNT = Convert.ToInt32(minCnt);
                productDB.PDT_COMBINE_HTL = combineHtl;

                DB.PDT_0.Add(productDB);

                var inHotelsLists = DB.PDT_4.ToList()
                                            .Where(w => w.CORP_CODE.Equals(corp_code))
                                            .Where(w => w.PDT_TYPE_CODE.Equals(type))
                                            .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                            .Where(w => w.PDT_YY.Equals(now.ToString("yy")))
                                            .Where(w => w.PDT_SEQ == Convert.ToInt32(idx.ToString()));

                foreach (var item in inHotelsLists)
                {
                    DB.PDT_4.Remove(item);
                }

                string HTL_data = inHotels.Trim();
                string[] hotelIn = HTL_data.Split(',');

                for (int htl_idx = 0; htl_idx < hotelIn.Length; htl_idx++)
                {
                    if (string.IsNullOrEmpty(hotelIn[htl_idx]))
                        continue;

                    string[] hotelSeq = hotelIn[htl_idx].Split(':');

                    PDT_4 inHotelDB = new PDT_4();

                    inHotelDB.CORP_CODE = corp_code;
                    inHotelDB.PDT_TYPE_CODE = type;
                    inHotelDB.PDT_IST_EMP_NO = Convert.ToInt32(emp);
                    inHotelDB.PDT_YY = now.ToString("yy");
                    inHotelDB.PDT_SEQ = Convert.ToInt16(idx.ToString());
                    inHotelDB.PDT_IN = Convert.ToInt32(hotelSeq[0]);
                    inHotelDB.PDT_HTL_SEQ = Convert.ToInt16(hotelSeq[1].Trim());
                    inHotelDB.IST_DATE = now.ToString("yyyy.MM.dd");
                    inHotelDB.IST_EMP_NO = Convert.ToInt32(emp);
                    inHotelDB.UDT_DATE = now.ToString("yyyy.MM.dd");
                    inHotelDB.UDT_EMP_NO = Convert.ToInt32(emp);
                    try
                    {
                        inHotelDB.PDT_CANRES = hotelSeq[2].ToString().Trim();
                    }
                    catch
                    {
                        inHotelDB.PDT_CANRES = "N";
                    }

                    DB.PDT_4.Add(inHotelDB);
                }
            }
            else
            {
                var priKey123 = getCode(code, key);

                string corp123 = priKey123[0];
                string type123 = priKey123[1];
                string emp123 = priKey123[2];
                string pdtyy123 = priKey123[3];
                string pdtseq123 = priKey123[4];

                var udtPdt = DB.PDT_0.Where(w => w.CORP_CODE.Equals(corp123))
                                     .Where(w => w.PDT_TYPE_CODE.Equals(type123))
                                     .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(emp123))
                                     .Where(w => w.PDT_YY.ToString().Equals(pdtyy123))
                                     .Where(w => w.PDT_SEQ.ToString().Equals(pdtseq123));

                udtPdt.Single().PDT_EMP_NO = Convert.ToInt32(emp);
                udtPdt.Single().PDT_NATION_CODE = nation;
                udtPdt.Single().PDT_AREA_CODE = area;
                udtPdt.Single().PDT_TITLE = title;
                udtPdt.Single().PDT_CONTENT = content;
                udtPdt.Single().PDT_SCITY_CODE = city;
                udtPdt.Single().PDT_PROC_CODE = proc;
                udtPdt.Single().PDT_ORDER_NO = Convert.ToInt16(order);
                udtPdt.Single().PDT_DAYS_CODE = days;
                udtPdt.Single().PDT_OPTIONS = options;
                udtPdt.Single().PDT_UDT_DATE = now.ToString("yyyy.MM.dd");
                udtPdt.Single().PDT_UDT_EMP_NO = Convert.ToInt32(emp);
                udtPdt.Single().PDT_TOUR_CITY = cities;
                udtPdt.Single().PDT_MIN_COUNT = Convert.ToInt32(minCnt);
                udtPdt.Single().PDT_COMBINE_HTL = combineHtl;

                var inHotelsLists = DB.PDT_4.ToList()
                                            .Where(w => w.CORP_CODE.Equals(corp123))
                                            .Where(w => w.PDT_TYPE_CODE.Equals(type123))
                                            .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp123))
                                            .Where(w => w.PDT_YY.Equals(pdtyy123))
                                            .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq123.ToString()));

                foreach (var item in inHotelsLists)
                {
                    DB.PDT_4.Remove(item);
                }

                string HTL_data = inHotels.Trim();
                string[] hotelIn = HTL_data.Split(',');

                for (int htl_idx = 0; htl_idx < hotelIn.Length; htl_idx++)
                {
                    if (string.IsNullOrEmpty(hotelIn[htl_idx]))
                        continue;

                    string[] hotelSeq = hotelIn[htl_idx].Split(':');

                    PDT_4 inHotelDB = new PDT_4();

                    inHotelDB.CORP_CODE = corp123;
                    inHotelDB.PDT_TYPE_CODE = type123;
                    inHotelDB.PDT_IST_EMP_NO = Convert.ToInt32(emp123);
                    inHotelDB.PDT_YY = pdtyy123;
                    inHotelDB.PDT_SEQ = Convert.ToInt16(pdtseq123.ToString());
                    inHotelDB.PDT_IN = Convert.ToInt32(hotelSeq[0]);
                    inHotelDB.PDT_HTL_SEQ = Convert.ToInt16(hotelSeq[1].Trim());
                    inHotelDB.IST_DATE = now.ToString("yyyy.MM.dd");
                    inHotelDB.IST_EMP_NO = Convert.ToInt32(emp);
                    inHotelDB.UDT_DATE = now.ToString("yyyy.MM.dd");
                    inHotelDB.UDT_EMP_NO = Convert.ToInt32(emp);
                    try
                    {
                        inHotelDB.PDT_CANRES = hotelSeq[2].ToString().Trim();
                    }
                    catch
                    {
                        inHotelDB.PDT_CANRES = "N";
                    }

                    DB.PDT_4.Add(inHotelDB);
                }

            }

            DB.SaveChanges();

            return Redirect(HttpUtility.UrlDecode(redirect));
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

        public void udtPrice(string code, string key, string seq, string currency, string adult, string child = "0", string baby = "0", string profit = "0", string content = "")
        {
            var priKey = getCode(code, key);

            string corp     = priKey[0];
            string type     = priKey[1];
            string pdtyy    = priKey[3];

            int emp      = Convert.ToInt32(priKey[2]);

            short pdtseq   = Convert.ToInt16(priKey[4]);

            var prcData = DB.PRC_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdtseq);

            int maxIdx = 1; 

            if (string.IsNullOrEmpty(seq))
            {
                try
                {
                    maxIdx = prcData.GroupBy(g => new
                                     {
                                         g.CORP_CODE,
                                         g.PDT_TYPE_CODE,
                                         g.PDT_IST_EMP_NO,
                                         g.PDT_YY,
                                         g.PDT_SEQ
                                     })
                                     .FirstOrDefault()
                                     .Max(max => max.PRC_SEQ) + 1;
                }
                catch
                {

                }

                PRC_0 newPrice = new PRC_0();

                newPrice.CORP_CODE = corp;
                newPrice.PDT_TYPE_CODE = type;
                newPrice.PDT_IST_EMP_NO = emp;
                newPrice.PDT_YY = pdtyy;
                newPrice.PDT_SEQ = pdtseq;
                newPrice.PRC_SEQ = maxIdx;
                newPrice.PRC_Currency_CODE = currency;
                newPrice.PRC_Adult = Convert.ToInt32(adult);
                newPrice.PRC_Child = Convert.ToInt32(child);
                newPrice.PRC_Baby = Convert.ToInt32(baby);
                newPrice.PRC_Profit = Convert.ToInt32(profit);
                newPrice.PRC_EXP = content;

                DB.PRC_0.Add(newPrice);
            } 
            else
            {
                maxIdx = Convert.ToInt32(seq);

                prcData = prcData.Where(w => w.PRC_SEQ == maxIdx);

                prcData.Single().PRC_Currency_CODE = currency;
                prcData.Single().PRC_Adult = Convert.ToInt32(adult);
                prcData.Single().PRC_Child = Convert.ToInt32(child);
                prcData.Single().PRC_Baby = Convert.ToInt32(baby);
                prcData.Single().PRC_Profit = Convert.ToInt32(profit);
                prcData.Single().PRC_EXP = content;
            }

            DB.SaveChanges();
        }

        public void udtTraff(string code, string key, string seq, string title, string type, string stime, string etime, string sarea, string earea, string subSeq = "1")
        {
            var priKey = getCode(code, key);

            string corp     = priKey[0];
            string pdt_type = priKey[1];
            string emp      = priKey[2];
            string pdtyy    = priKey[3];
            string pdtseq   = priKey[4];

            TRF_0 newTraff  = new TRF_0();

            newTraff.CORP_CODE = corp;
            newTraff.PDT_TYPE_CODE = pdt_type;
            newTraff.PDT_IST_EMP_NO = Convert.ToInt32(emp);
            newTraff.PDT_YY = pdtyy;
            newTraff.PDT_SEQ = Convert.ToInt16(pdtseq);
            newTraff.TRF_SUB_SEQ = Convert.ToInt16(subSeq);

            newTraff.TRF_SEQ = Convert.ToInt32(seq);
            newTraff.TRF_TITLE = title;
            newTraff.TRF_TYPE = type;
            newTraff.TRF_STIME = stime;
            newTraff.TRF_ETIME = etime;
            newTraff.TRF_SAREA = sarea;
            newTraff.TRF_EAREA = earea;

            DB.TRF_0.Add(newTraff);
            DB.SaveChanges();
        }

        public void udtDay(string code, string key, string trfCode, string prcCode, string type, string day)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string emp = priKey[2];
            string pdtyy = priKey[3];
            string pdtseq = priKey[4];

            var chkPdtDays = DB.PDT_1.ToList()
                                     .Where(w => w.CORP_CODE.Equals(corp))
                                     .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                     .Where(w => w.PDT_IST_EMP_NO == Convert.ToInt32(emp))
                                     .Where(w => w.PDT_YY.Equals(pdtyy))
                                     .Where(w => w.PDT_SEQ == Convert.ToInt32(pdtseq))
                                     .Where(w => w.TRF_SEQ == Convert.ToInt32(trfCode))
                                     .Where(w => w.PRC_SEQ == Convert.ToInt32(prcCode))
                                     .Where(w => w.PDT_DATE.Equals(day));
            if (type == "05")
            {
                foreach (var item in chkPdtDays)
                {
                    DB.PDT_1.Remove(item);
                }
            }
            else if (chkPdtDays.Any())
            {
                chkPdtDays.Single().PDT_STATE_CODE = type;
            }
            else
            {
                PDT_1 pdtDays = new PDT_1();

                pdtDays.CORP_CODE = corp;
                pdtDays.PDT_TYPE_CODE = pdt_type;
                pdtDays.PDT_IST_EMP_NO = Convert.ToInt32(emp);
                pdtDays.PDT_YY = pdtyy;
                pdtDays.PDT_SEQ = Convert.ToInt16(pdtseq);
                pdtDays.TRF_SEQ = Convert.ToInt32(trfCode);
                pdtDays.PRC_SEQ = Convert.ToInt32(prcCode);
                pdtDays.PDT_DATE = day;
                pdtDays.PDT_STATE_CODE = type;

                DB.PDT_1.Add(pdtDays);
            }

            DB.SaveChanges();
        }

        public void udtDays(string code, string key, string trfCode, string prcCode, string type, string[] days)
        {
            int count = days.Count();

            for (int i = 0; i < count; i++)
            {
                string day = days[i];

                udtDay(code, key, trfCode, prcCode, type, day);
            }

            var udtPdt = DB.PDT_0.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code));

            udtPdt.Single().PDT_Option3 = "Y";

            DB.SaveChanges();

            //return getDays(code, key, trfCode, prcCode);
        }

        public JsonResult JSON_getImage(string code, string key)
        {
            var iData = DB.PDT_2.Where(w => (w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ).Equals(code))
                                .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(key))
                                .FirstOrDefault()
                                .PDT_IMG
                                .ToString();

            return Json(iData);
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
                        });

            return Json(dates);
        }

        public JsonResult JSON_getCode(string category, string nation = "", string area = "")
        {
            var comLists = DB.TB_ComCode.Where(w => w.CatCode.Equals(category))
                                        .Select(s => new
                                        {
                                            s.ComCode,
                                            s.AlterCode,
                                            s.Code_Kor,
                                            s.SortNo,
                                        })
                                        .OrderBy(o => o.SortNo);

            if (!string.IsNullOrEmpty(nation))
            {
                //comLists = comLists.Where(w => w.Limit.Equals(limit))
                //                 .OrderBy(o => o.SortNo);

                var areaTree = DB.TB_AreaTree.Where(w => w.Nation.Equals(nation))
                                             .GroupBy(g => new { g.Area })
                                             .Select(s => new
                                             {
                                                 s.Key.Area
                                             });

                comLists = comLists.Join(areaTree,
                                         a => a.ComCode,
                                         b => b.Area,
                                        (a, b) => new
                                        {
                                            a.ComCode,
                                            a.AlterCode,
                                            a.Code_Kor,
                                            a.SortNo
                                        })
                                        .OrderBy(o => o.SortNo);
            }
            else if (!string.IsNullOrEmpty(area))
            {
                var areaTree = DB.TB_AreaTree.Where(w => w.Area.Equals(area))
                                             .Select(s => new
                                             {
                                                 s.City
                                             });

                comLists = comLists.Join(areaTree,
                                         a => a.ComCode,
                                         b => b.City,
                                        (a, b) => new
                                        {
                                            a.ComCode,
                                            a.AlterCode,
                                            a.Code_Kor,
                                            a.SortNo
                                        })
                                        .OrderBy(o => o.SortNo);
            }
            else if (category != "NAT")// (category == "ARE" || category == "CTY")
            {
                comLists = null;
            }

            return Json(comLists);
        }

        public JsonResult JSON_getNations()
        {
            Dictionary<string, string> nations = new Dictionary<string, string>();
            
            nations.Add("JP", "일본");
            nations.Add("HM", "홍콩/마카오");
            nations.Add("TW", "대만");
            nations.Add("SA", "동남아");
            nations.Add("HN", "하와이");
            nations.Add("GS", "괌/사이판");

            var natLists = DB.TB_ComCode.Where(w => w.CatCode.Equals("NAT"))
                                        .Select(s => new
                                        {
                                            s.ComCode,
                                            s.AlterCode,
                                            s.Code_Kor,
                                            s.SortNo
                                        })
                                        .OrderBy(o => o.SortNo);

            return Json(natLists);
        }

        public JsonResult JSON_getAreas(string nation)
        {
            Dictionary<string, string> Areas = new Dictionary<string, string>();

            switch (nation)
            {
                case "JP":
                    Areas.Add("FUK", "규슈");
                    Areas.Add("TYO", "도쿄");
                    Areas.Add("OSA", "오사카");
                    Areas.Add("CTS", "홋카이도");
                    Areas.Add("TSM", "대마도");
                    Areas.Add("OKA", "오키나와");
                    Areas.Add("NGY", "나고야");
                    Areas.Add("TAK", "다카야마");
                    break;
                case "HM":
                    Areas.Add("HKG", "홍콩");
                    Areas.Add("MFM", "마카오");
                    break;
                case "TW":
                    Areas.Add("TPE", "타이페이");
                    break;
                case "SA":
                    Areas.Add("DAD", "다낭");
                    Areas.Add("CEB", "세부");
                    break;
                case "HN":
                    Areas.Add("HNL", "호놀루루");
                    break;
                case "GS":
                    Areas.Add("GUM", "괌");
                    Areas.Add("SPN", "사이판");
                    break;
                default:
                    break;
            }

            return Json(Areas);
        }

        public JsonResult JSON_setInHotel(string code, string key, string hotel)
        {
            string data = hotel.Trim();
            string[] hotelIn = data.Split(',');

            for (int idx = 0; idx < hotelIn.Length; idx++)
            {
                string[] hotelSeq = hotelIn[idx].Split(':');


            }

            return Json("");
        }

        public JsonResult JSON_getChatList(string no)
        {
            var messages = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(no)).ToList()
                                          .Join(DB.CHAT_Room,
                                                a => a.CHAT_Room_ID,
                                                b => b.CHAT_Room_ID,
                                               (a, b) => new
                                               {
                                                   user     = b.CHAT_Name,
                                                   message  = a.CHAT_Message1,
                                                   Date     = DateTime.ParseExact(a.CHAT_Message_Date + " " + a.CHAT_Message_Time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM월 dd일 hh:mm"),
                                                   isAdmin  = a.CHAT_User_YY,
                                               });
                                          //.Select(s => new
                                          //{
                                          //    message = s.CHAT_Message1,
                                          //    Date = DateTime.ParseExact(s.CHAT_Message_Date + " " + s.CHAT_Message_Time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToString("MM월 dd일 hh:mm"),
                                          //});

            return Json(messages);
        }

        public JsonResult JSON_getHotels(string nation, string area)
        {
            var hotelData = DB.HTL_0.Where(w => w.HTL_NATION_CODE.Equals(nation))
                                    .Where(w => w.HTL_AREA_CODE.Equals(area))
                                    .Select(s => new
                                    {
                                        seq = s.HTL_SEQ,
                                        name = s.HTL_NAME,
                                    });

            return Json(hotelData);
        }

        public JsonResult JSON_searchHotelName(string title)
        {
            var hotelData = DB.HTL_0.Where(w => w.HTL_NAME.Contains(title))
                                    .Select(s => new
                                    {
                                        seq = s.HTL_SEQ,
                                        name = s.HTL_NAME
                                    });

            return Json(hotelData);
        }

        public JsonResult JSON_searchHotelSeq(int seq)
        {
            var hotelData = DB.HTL_0.Where(w => w.HTL_SEQ == seq)
                                    .Select(s => new
                                    {
                                        seq = s.HTL_SEQ,
                                        name = s.HTL_NAME
                                    });

            return Json(hotelData);
        }

        public JsonResult removeDays(string code, string key, int seq)
        {
            var priKey = getCode(code, key);

            string corp = priKey[0];
            string pdt_type = priKey[1];
            string pdtyy = priKey[3];

            int emp = Convert.ToInt32(priKey[2]);
            int pdtseq = Convert.ToInt32(priKey[4]);



            var prcData = DB.PRC_0.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdtseq)
                                  .Where(w => w.PRC_SEQ == seq);

            if (prcData.Any())
            {
                foreach (var item in prcData)
                {
                    DB.PRC_0.Remove(item);
                }

                DB.SaveChanges();
            }

            var dayData = DB.PDT_1.Where(w => w.CORP_CODE.Equals(corp))
                                  .Where(w => w.PDT_TYPE_CODE.Equals(pdt_type))
                                  .Where(w => w.PDT_IST_EMP_NO == emp)
                                  .Where(w => w.PDT_YY.Equals(pdtyy))
                                  .Where(w => w.PDT_SEQ == pdtseq)
                                  .Where(w => w.PRC_SEQ == seq);

            if (dayData.Any())
            {
                foreach (var item in dayData)
                {
                    DB.PDT_1.Remove(item);
                }

                DB.SaveChanges();
            }

            return Json("삭제 완료");
        }

        public void JSON_chkNewMessage(string chatID)
        {
            var readChat = DB.CHAT_Message.Where(w => w.CHAT_Room_ID.ToString().Equals(chatID))
                                          .Where(w => w.CHAT_User_YY != ("ADMN"))
                                          .Where(w => w.CHAT_isNew.Equals("Y"));

            if (readChat.Any())
            {
                foreach (var item in readChat)
                {
                    item.CHAT_isNew = "N";
                }

                DB.SaveChanges();
            }

            var readRoom = DB.CHAT_Room.Where(w => w.CHAT_Room_ID.ToString().Equals(chatID))
                                       .Where(w => w.CHAT_isNew.Equals("Y"));

            if (readRoom.Any())
            {
                foreach (var item in readRoom)
                {
                    item.CHAT_isNew = "N";
                }

                DB.SaveChanges();
            }
        }
    }
} 