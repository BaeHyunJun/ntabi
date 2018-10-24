using NDayTrip_PC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NDayTrip_PC.Controllers
{
    public class CommunityController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NDT";

        // GET: Community
        public ActionResult Index()
        {
            return View();
        }

        // GET: Community/Lists
        public ActionResult Lists(string t = "notice", int p = 1)
        {
            var board = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                     .Where(w => w.post_type.Equals(t))
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
                                         s.udt_date
                                     })
                                     .OrderByDescending(o => o.udt_date);

            int bdCnt = board.Count(),
                ListNum = 10;

            var result = board.Skip((p - 1) * ListNum)
                              .Take(ListNum)
                              .ToList();

            ViewBag.Cnt = bdCnt;
            ViewBag.pageNum = p;

            return View(result);
        }

        // GET: Community/Views
        public ActionResult Views(string t = "notice", int k = 0)
        {
            var comBoard = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                        .Where(w => w.post_type.Equals(t))                                     
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent == k)
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
                                            s.PDT_SEQ
                                          })
                                        .ToList();

            var board = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                     .Where(w => w.post_type.Equals(t))
                                     .Where(w => w.post_ID == k)
                                     .Select(s => new
                                     {
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         s.post_status,
                                         s.name,
                                         s.post_content,
                                         s.PDT_TYPE_CODE,
                                         s.PDT_IST_EMP_NO,
                                         s.PDT_YY,
                                         s.PDT_SEQ,
                                         s.CU_YY,
                                         s.CU_SEQ,
                                     })
                                     .ToList();

            if (!string.IsNullOrEmpty(board.FirstOrDefault().PDT_TYPE_CODE))
            {
                string pdtCode = "NTB" + board.FirstOrDefault().PDT_TYPE_CODE.Trim() + board.FirstOrDefault().PDT_IST_EMP_NO.ToString().Trim() + board.FirstOrDefault().PDT_YY.ToString().Trim() + board.FirstOrDefault().PDT_SEQ.ToString().Trim();

                return Redirect("/Products/Views?code=" + pdtCode + "&key=" + board.FirstOrDefault().PDT_IST_EMP_NO.ToString());
            }

            var data = (from a in board
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
                            a.post_content,
                            a.PDT_TYPE_CODE,
                            a.PDT_IST_EMP_NO,
                            a.PDT_YY,
                            a.PDT_SEQ,
                            a.CU_YY,
                            a.CU_SEQ,
                            review = p
                        })
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.name, g.post_content, g.PDT_IST_EMP_NO, g.PDT_TYPE_CODE, g.PDT_SEQ, g.PDT_YY, g.CU_SEQ, g.CU_YY })
                        .ToList()
                        .Select(s => new
                        {
                            type = s.Key.post_type,
                            key = s.Key.post_ID,
                            title = s.Key.post_title,
                            date = s.Key.ist_date,
                            status = s.Key.post_status,
                            name = s.Key.name,
                            content = s.Key.post_content,
                            review = s.Select(sl => new review
                            {
                                post_ID = sl.review != null ? sl.review.post_ID : 0,
                                CU_YY = sl.review != null ? sl.review.CU_YY : "",
                                CU_SEQ = sl.review != null ? sl.review.CU_SEQ : 0,
                                name = sl.review != null ? sl.review.name : "",
                                post_content = sl.review != null ? sl.review.post_content : "",
                                post_parent = sl.review != null ? sl.review.post_parent : 0,
                                ist_date = sl.review != null ? sl.review.ist_date : "",
                            }).ToList(),
                            s.Key.PDT_IST_EMP_NO,
                            s.Key.PDT_TYPE_CODE,
                            s.Key.PDT_SEQ,
                            s.Key.PDT_YY,
                            s.Key.CU_SEQ,
                            s.Key.CU_YY
                        });

            return View(data);
        }

        public JsonResult JSON_getBoard(string type, int idx = 0)
        {
            var board = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
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
                                         s.post_options
                                     })
                                     .OrderByDescending(o => o.post_ID);

            var user = Session["user"] as User;

            var userData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login))
                                    .Select(s => new
                                    {
                                        s.CU_YY,
                                        s.CU_SEQ,
                                    });

            string cuYY = userData.FirstOrDefault().CU_YY.ToString();
            string cuSEQ = userData.FirstOrDefault().CU_SEQ.ToString();

            board = board.Where(w => w.CU_YY.ToString().Equals(cuYY))
                         .Where(w => w.CU_SEQ.ToString().Equals(cuSEQ))
                         .Select(s => new
                         {
                             s.post_type,
                             s.post_ID,
                             s.post_title,
                             s.ist_date,
                             s.post_status,
                             s.name,
                             s.CU_YY,
                             s.CU_SEQ,
                             s.post_thumb,
                             s.post_options
                         })
                         .OrderByDescending(o => o.post_ID);

            var result = board.Skip(idx)
                              .Take(5)
                              .ToList();

            return Json(result);
        }

        public JsonResult JSON_Update(string type, int key, string content)
        {
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

            var boardData = from bf in DB.NT_Board_2
                            where bf.post_type == type && corp_code == bf.CORP_CODE
                            orderby bf.post_ID descending
                            select new
                            {
                                postid = bf.post_ID
                            };

            try { postID = int.Parse(boardData.Take(1).Single().postid.ToString()) + 1; }
            catch { }

            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            NT_Board_2 board = new NT_Board_2();

            board.post_ID = postID;
            board.post_type = type;
            board.CU_YY = memData.Single().CU_YY;
            board.CU_SEQ = memData.Single().CU_SEQ;
            board.name = memData.Single().CU_NM_KOR;
            board.post_content = content;
            board.post_parent = key;

            board.post_status = "N";
            board.DEL_FG = "N";
            board.ist_date = date;
            board.ist_time = time;


            if ("NTB_qna" == type)
            {
                var parentTable = DB.NT_Board_2.Where(w => w.post_type.Equals(type))
                                             .Where(w => w.post_ID == key);

                parentTable.Single().post_status = "Y";
            }

            DB.NT_Board_2.Add(board);
            DB.SaveChanges();

            return Json(board);
        }
    }
}