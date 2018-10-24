using mNtabi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mNtabi.Controllers
{
    public class CommunityController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NTB";

        // GET: Commnunity
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {

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

            return View();
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
                    nickName = nDB.CU001.Where(w => w.CU_YY.ToString().Equals(User_YY))
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
                                             s.name,
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
                                             s.post_content,
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
                                rcont = rf.rcont,
                                //parent = rf.,
                                lf1.post_content,
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

            var data2 = data.GroupBy(g => new
            {
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

            string secret = f["secret"];

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

                try { A = Convert.ToInt32(f["aCnt"]); }
                catch { }
                try { C = Convert.ToInt32(f["cCnt"]); }
                catch { }
                try { B = Convert.ToInt32(f["bCnt"]); }
                catch { }

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
                bData.post_status = secret == "Y" ? "Y" : "";
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
                    case "customer":
                        bData.post_status = "Y";
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

            return Redirect("/Community/Lists?type=" + type);
            //return Redirect("/Community/Views?type=" + type + "&key=" + postID);
        }


        public string getName(string CU_YY, int CU_SEQ)
        {
            string rName = "";

            var cuData = nDB.CU001.Where(w => w.CU_YY.Equals(CU_YY))
                                  .Where(w => w.CU_SEQ == CU_SEQ);

            if (cuData.Any())
            {
                rName = cuData.SingleOrDefault()
                              .CU_NM_KOR;
            }
            
            return rName;
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
    }
}