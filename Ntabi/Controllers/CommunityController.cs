using Ntabi.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
{
    public class CommunityController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NTB";

        // GET: Community
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lists(string type = "notice", int idx = 1)
        {
            var boardData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                         .Where(w => w.post_type.Equals(type))
                                         .Where(w => w.DEL_FG.Equals("N"))
                                         .Where(w => w.post_parent == null)
                                         .Select(s => new
                                         {
                                             s.post_type,
                                             s.post_ID,
                                             s.post_title,
                                             ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                             s.post_status,
                                             name = type != "notice" ? s.name.Substring(0, 1) + "*" + s.name.Substring(s.name.Length - 1, 1) : s.name,
                                             s.CU_YY,
                                             s.CU_SEQ,
                                             s.post_thumb,
                                             s.post_options,
                                             s.post_category,
                                             //s.CORP_CODE,
                                             //s.PDT_TYPE_CODE,
                                             //s.PDT_IST_EMP_NO,
                                             //s.PDT_YY,
                                             //s.PDT_SEQ,
                                             //pdt_name = "",
                                         })
                                         .OrderByDescending(o => o.post_ID);

            var rData = from rf in DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                        where rf.post_parent != null && rf.DEL_FG.Equals("N")
                        group rf by new { rf.post_type, rf.post_parent } into g
                        select new
                        {
                            post_type = g.Key.post_type,
                            post_parent = g.Key.post_parent,
                            rcont = "답변완료",
                            cnt = g.Count()
                        };

            var listData3 = from lf1 in boardData
                            join lf2 in rData on lf1.post_type + lf1.post_ID equals lf2.post_type + lf2.post_parent into j
                            from rf in j.DefaultIfEmpty()
                            orderby lf1.post_ID descending
                            select new
                            {
                                lf1.post_type,
                                lf1.post_ID,
                                lf1.post_title,
                                lf1.ist_date,
                                lf1.post_status,
                                lf1.name,
                                lf1.CU_YY,
                                lf1.CU_SEQ,
                                lf1.post_thumb,
                                lf1.post_options,
                                lf1.post_category,
                                rcont = rf.rcont
                                //parent = rf.,
                            };

            if (type == "customer")
            {
                var user = Session["user"] as User;

                var memData = from mf in nDB.CU001
                              where mf.ASHOP_SITE_CD + mf.CU_YY + mf.CU_SEQ == user.Login
                              select new
                              {
                                  CU_YY = mf.CU_YY,
                                  CU_SEQ = mf.CU_SEQ
                              };

                string CU_YY = "";// memData.Single().CU_YY;

                int CU_SEQ = 0; //memData.SingleOrDefault().CU_SEQ;

                try
                {
                    CU_SEQ = memData.SingleOrDefault().CU_SEQ;
                    CU_YY = memData.Single().CU_YY;
                }
                catch
                {

                }

                listData3 = listData3.Where(w => w.CU_YY.Equals(CU_YY))
                                     .Where(w => w.CU_SEQ == CU_SEQ);
            }

            int bdCnt = listData3.Count(),
                ListNum = 10;

            var result = listData3.Skip((idx - 1) * ListNum)
                                  .Take(ListNum)
                                  .ToList();

            ViewBag.Cnt = bdCnt;
            ViewBag.pageNum = idx;

            return View(result);
        }

        public ActionResult Views(string type, int key, int idx = 1)
        {
            var viewData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                        .Where(w => w.post_type.Equals(type))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent == null)
                                        .Where(w => w.post_ID == key)
                                        .Select(s => new
                                        {
                                            s.post_type,
                                            s.post_ID,
                                            s.post_title,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                            s.post_status,
                                            s.name,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.post_thumb,
                                            s.post_options,
                                            s.post_category,
                                            s.CORP_CODE,
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            pdt_name = "",
                                            s.post_content,
                                            s.DptCode,
                                            s.RtnCode,
                                            s.TripNation,
                                            s.TripArea,
                                            s.DptDate,
                                            s.Keyword,
                                            s.ForTrip,
                                            s.TransferCode,
                                            s.ReqQuotation,
                                            s.TempCol01,
                                            s.TempCol02,
                                            s.AdultCount,
                                            s.ChildCount,
                                            s.BabyCount,
                                            s.TripPeriod,
                                            s.TripBudget,
                                        })
                                        .ToList();

            if (type == "review")
            {
                viewData = viewData.Join(DB.PDT_0,
                                           a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                           b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                          (a, b) => new
                                          {
                                              a.post_type,
                                              a.post_ID,
                                              a.post_title,
                                              a.ist_date,
                                              a.post_status,
                                              a.name,
                                              a.CU_YY,
                                              a.CU_SEQ,
                                              a.post_thumb,
                                              a.post_options,
                                              a.post_category,
                                              a.CORP_CODE,
                                              a.PDT_TYPE_CODE,
                                              a.PDT_IST_EMP_NO,
                                              a.PDT_YY,
                                              a.PDT_SEQ,
                                              pdt_name = b.PDT_TITLE,
                                              a.post_content,
                                              a.DptCode,
                                              a.RtnCode,
                                              a.TripNation,
                                              a.TripArea,
                                              a.DptDate,
                                              a.Keyword,
                                              a.ForTrip,
                                              a.TransferCode,
                                              a.ReqQuotation,
                                              a.TempCol01,
                                              a.TempCol02,
                                              a.AdultCount,
                                              a.ChildCount,
                                              a.BabyCount,
                                              a.TripPeriod,
                                              a.TripBudget,
                                          })
                                         .OrderByDescending(o => o.post_ID)
                                         .ToList();
            }


            var boardData = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                         .Where(w => w.post_type.Equals(type))
                                         .Where(w => w.DEL_FG.Equals("N"))
                                         .Where(w => w.post_parent == null)
                                         .Select(s => new
                                         {
                                             s.post_type,
                                             s.post_ID,
                                             s.post_title,
                                             ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                             s.post_status,
                                             s.name,
                                             s.CU_YY,
                                             s.CU_SEQ,
                                             s.post_thumb,
                                             s.post_options,
                                             s.post_category,
                                             s.post_content,
                                             s.CORP_CODE,
                                             s.PDT_TYPE_CODE,
                                             s.PDT_IST_EMP_NO,
                                             s.PDT_YY,
                                             s.PDT_SEQ,
                                         })
                                         .OrderByDescending(o => o.post_ID);

            var comBoard = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                        .Where(w => w.post_type.Equals(type))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent == key)
                                        .Select(s => new
                                        {
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                            s.PDT_TYPE_CODE,
                                            s.PDT_IST_EMP_NO,
                                            s.PDT_YY,
                                            s.PDT_SEQ,
                                            category = s.post_category,
                                        })
                                        .ToList();

            var data = (from a in viewData
                        join b in comBoard on a.post_ID equals b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.name,
                            a.CU_YY,
                            a.CU_SEQ,
                            a.post_thumb,
                            a.post_options,
                            a.post_category,
                            a.CORP_CODE,
                            a.PDT_TYPE_CODE,
                            a.PDT_IST_EMP_NO,
                            a.PDT_YY,
                            a.PDT_SEQ,
                            a.pdt_name,
                            a.post_content,
                            review = p,

                            a.DptCode,
                            a.RtnCode,
                            a.TripNation,
                            a.TripArea,
                            a.DptDate,
                            a.Keyword,
                            a.ForTrip,
                            a.TransferCode,
                            a.ReqQuotation,
                            a.TempCol01,
                            a.TempCol02,
                            a.AdultCount,
                            a.ChildCount,
                            a.BabyCount,
                            a.TripPeriod,
                            a.TripBudget,
                        })
                        .ToList();

            var data2 = data.GroupBy(g => new { 
                                g.CORP_CODE, 
                                g.post_thumb, 
                                g.post_options, 
                                g.post_category, 
                                g.pdt_name, 
                                g.post_type, 
                                g.post_ID, 
                                g.post_title,
                                g.ist_date, 
                                g.post_status, 
                                g.name, 
                                g.post_content, 
                                g.PDT_IST_EMP_NO, 
                                g.PDT_TYPE_CODE, 
                                g.PDT_SEQ, 
                                g.PDT_YY, 
                                g.CU_SEQ,
                                g.CU_YY,

                                g.DptCode,
                                g.RtnCode,
                                g.TripNation,
                                g.TripArea,
                                g.DptDate,
                                g.Keyword,
                                g.ForTrip,
                                g.TransferCode,
                                g.ReqQuotation,
                                g.TempCol01,
                                g.TempCol02,
                                g.AdultCount,
                                g.ChildCount,
                                g.BabyCount,
                                g.TripPeriod,
                                g.TripBudget,
                            })
                            .ToList();

            var data3 = data2.Select(s => new
                            {
                                s.Key.post_type,
                                s.Key.post_ID,
                                s.Key.post_title,
                                s.Key.ist_date,
                                s.Key.post_status,
                                s.Key.name,
                                s.Key.CU_YY,
                                s.Key.CU_SEQ,
                                s.Key.post_thumb,
                                s.Key.post_options,
                                s.Key.post_category,
                                s.Key.CORP_CODE,
                                s.Key.PDT_TYPE_CODE,
                                s.Key.PDT_IST_EMP_NO,
                                s.Key.PDT_YY,
                                s.Key.PDT_SEQ,
                                s.Key.pdt_name,
                                s.Key.post_content,
                                review = s.Select(sl => new review
                                {
                                    post_ID = sl.review != null ? sl.review.post_ID : 0,
                                    CU_YY = sl.review != null ? sl.review.CU_YY : "",
                                    CU_SEQ = sl.review != null ? sl.review.CU_SEQ : 0,
                                    name = sl.review != null ? sl.review.name : "",
                                    post_content = sl.review != null ? sl.review.post_content : "",
                                    post_parent = sl.review != null ? sl.review.post_parent : 0,
                                    ist_date = sl.review != null ? sl.review.ist_date : "",
                                    category = sl.review != null ? sl.review.category : ""
                                }).ToList(),
                                s.Key.DptCode,
                                s.Key.RtnCode,
                                s.Key.TripNation,
                                s.Key.TripArea,
                                s.Key.DptDate,
                                s.Key.Keyword,
                                s.Key.ForTrip,
                                s.Key.TransferCode,
                                s.Key.ReqQuotation,
                                s.Key.TempCol01,
                                s.Key.TempCol02,
                                s.Key.AdultCount,
                                s.Key.ChildCount,
                                s.Key.BabyCount,
                                s.Key.TripPeriod,
                                s.Key.TripBudget,
                            });

            int bdCnt = data3.Count(),
                ListNum = 10;

            var result = data3.Skip((idx - 1) * ListNum)
                             .Take(ListNum)
                             .ToList();

            ViewBag.Cnt = bdCnt;
            ViewBag.pageNum = idx;
            ViewBag.boardData = result;

            return View(data3);
        }

        // GET: Community/Write
        public ActionResult Write()
        {
            return View();
        }

        public ActionResult Update(FormCollection f)
        {
            string type = f["type"];
            string title = f["title"];
            string content = f["content"];
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            string key = f["key"];

            string category = f["area[]"];

            string status = f["secret[]"];


            string DptCode = "",
                   RtnCode = "",
                   TripNation = "",
                   TripArea = "",
                   DptDate = "",
                   keyword = "",
                   ForTrip = "",
                   TransferCode = "",
                   ReqQuotation = "",
                   TempCol01 = "",
                   TempCol02 = "";

            int AdultCount = 0,
                ChildCount = 0,
                BabyCount = 0,
                TripPeriod = 0,
                TripBudget = 0;

            if (type == "customer")
            {
                int A = 0,
                    C = 0,
                    B = 0;
                
                try { A = Convert.ToInt32(f["aCnt"]); } catch { }
                try { C = Convert.ToInt32(f["cCnt"]); } catch { }
                try { B = Convert.ToInt32(f["bCnt"]); } catch { }

                DptCode = f["sCity"];
                RtnCode = f["eCity"];
                TransferCode = f["traff"];
                AdultCount = A;
                ChildCount = C;
                BabyCount = B;
                DptDate = f["date"];
            }
            else if (type == "qna")
            {
                category = f["category"];
            }

            string mode = "i";

            var user = Session["user"] as User;

            var memData = from mf in nDB.CU001
                          where mf.ASHOP_SITE_CD + mf.CU_YY + mf.CU_SEQ == user.Login
                          select new
                          {
                              CU_YY = mf.CU_YY,
                              CU_SEQ = mf.CU_SEQ,
                              phone = mf.HANDPHONE1 + mf.HANDPHONE2 + mf.HANDPHONE3,
                              id = mf.CU_ID,
                              mf.CU_NM_KOR
                          };

            int postID = 1;
            var chkBoard = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                        .Where(w => w.post_type.Equals(type))
                                        .Where(w => w.post_ID.ToString().Equals(key));


            var boardData = from bf in DB.NT_Board_2
                            where bf.post_type == type && bf.CORP_CODE == corp_code
                            orderby bf.post_ID descending
                            select new
                            {
                                postid = bf.post_ID
                            };

            try { postID = int.Parse(boardData.Take(1).Single().postid.ToString()) + 1; }
            catch { }

            if (chkBoard.Any() && int.Parse(key) > 0)
            {
                mode = "u";
            }

            if (mode == "i")
            {
                NT_Board_2 bData = new NT_Board_2();

                bData.CORP_CODE = corp_code;
                bData.post_ID = postID;
                bData.CU_YY = memData.Single().CU_YY;
                bData.CU_SEQ = memData.Single().CU_SEQ;
                bData.post_title = title;
                bData.post_content = content;
                bData.post_type = type;
                bData.post_status = status;
                bData.DEL_FG = "N";

                bData.ist_date = date;
                bData.ist_time = time;

                bData.udt_date = date;
                bData.udt_time = time;

                bData.name = memData.Single().CU_NM_KOR;
                bData.password = null;

                bData.PDT_TYPE_CODE = null;
                bData.PDT_IST_EMP_NO = null;
                bData.PDT_YY = null;
                bData.PDT_SEQ = null;

                bData.post_category = category;

                switch (type)
                {
                    case "qna":
                        break;
                }

                bData.DptCode = DptCode;
                bData.RtnCode = RtnCode;
                bData.TripNation = TripNation;
                bData.TripArea = TripArea;
                bData.DptDate = DptDate;
                bData.Keyword = keyword;
                bData.ForTrip = ForTrip;
                bData.TransferCode = TransferCode;
                bData.ReqQuotation = ReqQuotation;
                bData.TempCol01 = TempCol01;
                bData.TempCol02 = TempCol02;

                bData.AdultCount = AdultCount;
                bData.ChildCount = ChildCount;
                bData.BabyCount = BabyCount;
                bData.TripPeriod = TripPeriod;
                bData.TripBudget = TripBudget;

                DB.NT_Board_2.Add(bData);
                DB.SaveChanges();
            }
            else if (mode == "u")
            {

            }

            string id = "",
                   cont = "",
                   tel = "",
                   tStr = "";

            switch (type)
            {
                case "qna":
                    tStr = "질문과답변";
                    break;
                case "review":
                    tStr = "고객리뷰";
                    break;
                case "customer":
                    tStr = "맞춤여행문의";
                    break;
            }

            id = "10209";

            cont = "[엔타비 " + tStr + " 접수] \r\n" + title + " \r\n" + tStr + " 등록이 " + DateTime.Now.ToString("yyyy년 MM월 dd일 HH시 mm분") + " 에 접수 되었습니다. \r\n확인 후 답변 부탁 드립니다.";

            tel = "01073440940";    //신팀장님

            if (!string.IsNullOrEmpty(tStr))
            {
                kakaoMSG(id, cont, tel);

                tel = "01048241152";    //주형
                kakaoMSG(id, cont, tel);
            }

            //if (type == "helper")
            //{
            //    tel = "01049029550";    //리나

            //    kakaoMSG(id, cont, tel);
            //}

            //string userid = memData.Single().id;

            //id = "10073";
            //cont = "[엔타비 " + tStr + " 접수] \r\n" + userid + " 님 등록하신 질문에 대한 접수가 되었습니다. \r\n담당자가 확인하여 \r\n24시간 이내에(업무시간기준) \r\n답변을 드릴 수 있도록 하겠습니다. \r\n감사합니다. \r\n\r\n고객센터 : 051-466-4602 \r\n(운영시간 : 오전 9시~6시) \r\n\r\n여행갈땐 엔타비 바로가기 \r\nwww.ntabi.co.kr";
            //tel = memData.Single().phone;

            //if (!string.IsNullOrEmpty(tel))
            //{
            //    kakaoMSG(id, cont, tel);
            //}

            return Redirect("/Community/Views?type=" + type + "&key=" + postID);
        }

        public JsonResult getNotice()
        {
            DateTime date = DateTime.Now.AddMonths(-1);

            var result = DB.NT_Board_2.Where(w => w.post_type.Equals("notice"))
                                    .Where(w => w.CORP_CODE.Equals("NTB"))
                                    .Where(w => w.DEL_FG.Equals("N"))
                                    .ToList()
                                    .Where(w => DateTime.ParseExact(w.ist_date, "yyyyMMdd", CultureInfo.InvariantCulture) > date)
                                    .Select(s => new
                                    {
                                        key = s.post_ID,
                                        title = s.post_title,
                                    })
                                    .OrderByDescending(o => o.key);

            return Json(result);
        }

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
    }
}