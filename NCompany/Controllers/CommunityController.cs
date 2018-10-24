using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class CommunityController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        public string convertDate8To10(string date)
        {
            if (date.Length != 8)
                return date;

            string result = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);

            return result;
        }

        // GET: Community
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        // GET: Community/Notice
        public ActionResult Notice()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("notice"))
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
                                         s.post_content,
                                         s.CORP_CODE,
                                         s.udt_date,
                                         s.post_category,
                                     })
                                     .OrderByDescending(o => o.udt_date)
                                     .ToList();

            var qna = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                   .Where(w => w.post_type.Equals("qna"))
                                   .Where(w => w.post_parent == null)
                                   .OrderByDescending(o => o.udt_date)
                                   .Select(s => new
                                   {
                                       s.post_title,
                                       s.post_status
                                   });

            var review = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("review"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.udt_date)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.post_options,
                                      });

            ViewBag.qna = qna;
            ViewBag.review = review;

            return View(board);
        }

        // GET: Community/Event
        public ActionResult promotion()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("promotion"))
                                     .Where(w => w.DEL_FG.Equals("N"))
                                     .Where(w => w.post_parent == null)
                                     .Select(s => new
                                     {
                                         s.CORP_CODE,
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         s.post_status,
                                         s.name,
                                         s.post_content,
                                         s.post_thumb,
                                         s.udt_date,
                                     })
                                     .OrderByDescending(o => o.udt_date)
                                     .ToList();

            var comBoard = DB.NT_Board_2.Where(w => w.post_type.Equals("promotion"))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent != null)
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                        })
                                        .ToList();

            var data = (from a in board
                        join b in comBoard on a.CORP_CODE + a.post_ID equals b.CORP_CODE + b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.CORP_CODE,
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.name,
                            a.post_content,
                            a.post_thumb,
                            review = p
                        })
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.name, g.post_content, g.CORP_CODE })
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
                            s.Key.CORP_CODE,
                        });

            return View(data);
        }

        // GET: Community/Event
        public ActionResult Event()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("event"))
                                     .Where(w => w.DEL_FG.Equals("N"))
                                     .Where(w => w.post_parent == null)
                                     .Select(s => new
                                     {
                                         s.CORP_CODE,
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         s.post_status,
                                         s.name,
                                         s.post_content,
                                         s.post_thumb,
                                         s.udt_date,
                                     })
                                     .OrderByDescending(o => o.udt_date)
                                     .ToList();

            var comBoard = DB.NT_Board_2.Where(w => w.post_type.Equals("event"))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent != null)
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                        })
                                        .ToList();

            var data = (from a in board
                        join b in comBoard on a.CORP_CODE + a.post_ID equals b.CORP_CODE + b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.CORP_CODE,
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.name,
                            a.post_content,
                            a.post_thumb,
                            review = p
                        })
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.name, g.post_content, g.CORP_CODE })
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
                            s.Key.CORP_CODE,
                        });

            return View(data);
        }

        public ActionResult Write(string corp, string type, int key = 0)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp))
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
                                         s.post_content,
                                         s.post_thumb,
                                         s.CORP_CODE,
                                         s.post_category,
                                         s.post_options,
                                         s.post_mContent,
                                     })
                                     .ToList();

            return View(board);
        }

        // GET: Community/Recommend
        public ActionResult Recommend()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("recommend"))
                                     .Where(w => w.DEL_FG.Equals("N"))
                                     .Where(w => w.post_parent == null)
                                     .Select(s => new
                                     {
                                         s.CORP_CODE,
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         s.post_status,
                                         s.name,
                                         s.post_content,
                                         s.post_thumb,
                                         s.udt_date,
                                     })
                                     .OrderByDescending(o => o.udt_date)
                                     .ToList();

            var comBoard = DB.NT_Board_2.Where(w => w.post_type.Equals("recommend"))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent != null)
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                        })
                                        .ToList();

            var data = (from a in board
                        join b in comBoard on a.CORP_CODE + a.post_ID equals b.CORP_CODE + b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.CORP_CODE,
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.name,
                            a.post_content,
                            a.post_thumb,
                            review = p
                        })
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.name, g.post_content, g.CORP_CODE })
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
                            s.Key.CORP_CODE,
                        });

            return View(data);
        }

        // GET: Community/Qna
        public ActionResult Qna()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("qna"))
                                     .Where(w => w.DEL_FG.Equals("N"))
                                     .Where(w => w.post_parent == null)
                                     .Select(s => new
                                     {
                                         s.CORP_CODE,
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         s.CU_YY,
                                         s.CU_SEQ,
                                         s.post_status,
                                         s.post_content,
                                         s.udt_date
                                     })
                                     .ToList()
                                     .OrderByDescending(o => o.ist_date)
                                     .Take(10);

            var qnaBoard = board.Join(nDB.CU001.ToList(),
                                     a => a.CU_YY + a.CU_SEQ,
                                     b => b.CU_YY + b.CU_SEQ,
                                     (a, b) => new
                                     {
                                         a.CORP_CODE,
                                         a.post_type,
                                         a.post_ID,
                                         a.post_title,
                                         a.ist_date,
                                         a.post_status,
                                         b.CU_NM_KOR,
                                         a.post_content,
                                         a.udt_date
                                     });

            qnaBoard = qnaBoard.OrderByDescending(o => o.ist_date);

            var comBoard = DB.NT_Board_2.Where(w => w.post_type.Equals("qna"))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent != null)
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                        })
                                        .ToList();

            //var temp1 = qnaBoard.Join(comBoard,
            //                         a => a.CORP_CODE + a.post_ID,
            //                         b => b.CORP_CODE + b.post_parent,
            //                         (a, b) => new
            //                         {
            //                             a.post_type,
            //                             a.post_ID,
            //                             a.post_title,
            //                             a.ist_date,
            //                             a.post_status,
            //                             a.CU_NM_KOR,
            //                             a.post_content,
            //                             review = b
            //                         });

            var temp2 = (
                        from a in qnaBoard
                        join b in comBoard on a.CORP_CODE + a.post_ID equals b.CORP_CODE + b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.CU_NM_KOR,
                            a.post_content,
                            review = p,
                            a.CORP_CODE,
                        }
                        )
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.CU_NM_KOR, g.post_content, g.CORP_CODE })
                        .ToList()
                        .Select(s => new
                        {
                            type = s.Key.post_type,
                            key = s.Key.post_ID,
                            title = s.Key.post_title,
                            date = s.Key.ist_date,
                            status = s.Key.post_status,
                            name = s.Key.CU_NM_KOR,
                            content = s.Key.post_content,
                            //review = string.Join(",", s.Select(sl => sl.review)),//.ToList(),
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
                            s.Key.CORP_CODE,
                        });

            var notice = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("notice"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.ist_date
                                      });

            var review = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("review"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.post_options
                                      });

            ViewBag.notice = notice;
            ViewBag.review = review;

            
            return View(temp2);
        }

        // GET: Community/Review
        public ActionResult Review()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var board = DB.NT_Board_2.Where(w => w.post_type.Equals("review"))
                                     .Where(w => w.DEL_FG.Equals("N"))
                                     .Where(w => w.post_parent == null)
                                     .Select(s => new
                                     {
                                         s.CORP_CODE,
                                         s.post_type,
                                         s.post_ID,
                                         s.post_title,
                                         ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                         //s.CU_YY,
                                         //s.CU_SEQ,
                                         s.name,
                                         s.post_status,
                                         s.post_content,
                                         s.post_options,
                                         s.PDT_TYPE_CODE,
                                         s.PDT_IST_EMP_NO,
                                         s.PDT_YY,
                                         s.PDT_SEQ,
                                         s.udt_date
                                     })
                                        .ToList();

            var pdtData = DB.PDT_2.ToList()
                                  .Join(board,
                                       a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + "_" + a.PDT_YY + a.PDT_SEQ,
                                       b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + "_" + b.PDT_YY + b.PDT_SEQ,
                                       (a, b) => new
                                       {
                                           b.CORP_CODE,
                                           b.post_type,
                                           b.post_ID,
                                           b.post_title,
                                           b.ist_date,
                                           //b.CU_YY,
                                           //b.CU_SEQ,
                                           b.name,
                                           b.post_status,
                                           b.post_content,
                                           b.post_options,
                                           a.PDT_IMG,
                                           b.udt_date,
                                       }).ToList();

            //var qnaBoard = pdtData.Join(nDB.CU001.ToList(),
            //                         a => a.CU_YY + a.CU_SEQ,
            //                         b => b.CU_YY + b.CU_SEQ,
            //                         (a, b) => new
            //                         {
            //                             a.post_type,
            //                             a.post_ID,
            //                             a.post_title,
            //                             a.ist_date,
            //                             a.post_status,
            //                             b.CU_NM_KOR,
            //                             a.post_content,
            //                             a.post_options,
            //                             a.PDT_IMG,
            //                         });

            var qnaBoard = pdtData.OrderByDescending(o => o.udt_date);

            var comBoard = DB.NT_Board_2.Where(w => w.post_type.Equals("review"))
                                        .Where(w => w.DEL_FG.Equals("N"))
                                        .Where(w => w.post_parent != null)
                                        .Select(s => new
                                        {
                                            s.CORP_CODE,
                                            s.post_ID,
                                            s.CU_YY,
                                            s.CU_SEQ,
                                            s.name,
                                            s.post_content,
                                            s.post_parent,
                                            ist_date = s.ist_date.Substring(0, 4) + "-" + s.ist_date.Substring(4, 2) + "-" + s.ist_date.Substring(6, 2),
                                        })
                                        .ToList();

            var temp2 = (
                        from a in qnaBoard
                        join b in comBoard on a.CORP_CODE + a.post_ID equals b.CORP_CODE + b.post_parent into jt
                        from p in jt.DefaultIfEmpty()
                        select new
                        {
                            a.CORP_CODE,
                            a.post_type,
                            a.post_ID,
                            a.post_title,
                            a.ist_date,
                            a.post_status,
                            a.name,
                            a.post_content,
                            review = p,
                            a.post_options,
                            a.PDT_IMG,
                        }
                        )
                        .ToList()
                        .GroupBy(g => new { g.post_type, g.post_ID, g.post_title, g.ist_date, g.post_status, g.name, g.post_content, g.post_options, g.PDT_IMG, g.CORP_CODE })
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
                            //review = string.Join(",", s.Select(sl => sl.review)),//.ToList(),
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
                            s.Key.post_options,
                            s.Key.PDT_IMG,
                            s.Key.CORP_CODE,
                        });

            var qna = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                   .Where(w => w.post_type.Equals("qna"))
                                   .Where(w => w.post_parent == null)
                                   .OrderByDescending(o => o.post_ID)
                                   .Select(s => new
                                   {
                                       s.post_title,
                                       s.post_status
                                   });

            var notice = DB.NT_Board_2.Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.post_type.Equals("notice"))
                                      .Where(w => w.post_parent == null)
                                      .OrderByDescending(o => o.post_ID)
                                      .Select(s => new
                                      {
                                          s.post_title,
                                          s.ist_date
                                      });

            ViewBag.qna = qna;
            ViewBag.notice = notice;

            return View(temp2);
        }

        public ActionResult Update(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string corp = f["corp[]"];
            string type = f["type"];
            string title = f["title"];
            string content = f["content"];
            string mContent = f["mContent"];
            string feature = f["feature"];
            string name = f["name"];
            string option = f["option"];
            string date = DateTime.Now.ToString("yyyyMMdd");
            string time = DateTime.Now.ToString("HH:mm:ss");

            string key = f["key"];

            string status = "N";

            string mode = "i";

            var memData = DB.EMP_0.ToList()
                                  .Where(w => w.EMP_NO == Convert.ToInt32(user.Login))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int postID = 1;
            var chkBoard = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp))
                                        .Where(w => w.post_type.Equals(type))
                                        .Where(w => w.post_ID.ToString().Equals(key));


            var boardData = from bf in DB.NT_Board_2
                            where bf.post_type == type && bf.CORP_CODE == corp
                            orderby bf.post_ID descending
                            select new
                            {
                                postid = bf.post_ID
                            };

            try { postID = int.Parse(boardData.Take(1).Single().postid.ToString()) + 1; }
            catch { }

            if (chkBoard.Any())
            {
                mode = "u";
            }

            if (string.IsNullOrEmpty(name))
            {
                switch(corp)
                {
                    case "NTB":
                        name = "엔타비";
                        break;
                    case "NDT":
                        name = "엔데이트립";
                        break;
                }
            }

            if (mode == "i")
            {
                NT_Board_2 bData = new NT_Board_2();

                bData.CORP_CODE = corp;
                bData.post_ID = postID;
                //bData.CU_YY = memData.Single().CU_YY;
                bData.CU_SEQ = memData.Single().EMP_NO;
                bData.post_title = title;
                bData.post_content = HttpUtility.UrlDecode(content);
                bData.post_mContent = HttpUtility.UrlDecode(mContent);
                bData.post_type = type;
                bData.post_status = status;
                bData.DEL_FG = "N";

                bData.ist_date = date;
                bData.ist_time = time;

                bData.udt_date = date;
                bData.udt_time = time;

                bData.name = name;//memData.Single().EMP_NAME;
                bData.password = null;

                bData.PDT_TYPE_CODE = null;
                bData.PDT_IST_EMP_NO = null;
                bData.PDT_YY = null;
                bData.PDT_SEQ = null;

                bData.post_category = null;

                if (name == "엔타비")
                {
                    bData.post_category = f["category[]"];
                }

                if (!string.IsNullOrEmpty(feature))
                {
                    bData.post_thumb = feature;
                }

                switch (type)
                {
                    case "recommend":
                    case "promotion":
                        bData.post_options = option;
                        break;
                }

                DB.NT_Board_2.Add(bData);
            }
            else if (mode == "u")
            {
                chkBoard.Single().post_title = title;
                chkBoard.Single().post_content = HttpUtility.UrlDecode(content);
                chkBoard.Single().post_mContent = HttpUtility.UrlDecode(mContent);
                chkBoard.Single().name = name;

                if (name == "엔타비")
                {
                    chkBoard.Single().post_category = f["category[]"];
                }

                if (!string.IsNullOrEmpty(feature))
                {
                    chkBoard.Single().post_thumb = feature;
                }

                switch (type)
                {
                    case "recommend":
                    case "promotion":
                        chkBoard.Single().post_options = option;
                        break;
                }
            }

            DB.SaveChanges();

            return Redirect("/Community/" + type);
        }

        public ActionResult SaveText()
        {
            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int emp = empData.SingleOrDefault().EMP_NO;

            var chkNo = DB.CHAT_SaveText.Where(w => w.EMP_NO.ToString().Equals(emp.ToString()))
                                        .Where(w => w.CORP_CODE == null)
                                        .Where(w => w.CHAT_DEL_FG.Equals("N"))
                                        .Select(s => new
                                        {
                                            s.CHAT_NO,
                                            s.CHAT_TEXT
                                        });

            return View(chkNo);
        }

        public ActionResult SaveText2()
        {
            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int emp = empData.SingleOrDefault().EMP_NO;

            var chkNo = DB.CHAT_SaveText.Where(w => w.EMP_NO.ToString().Equals(emp.ToString()))
                                        .Where(w => w.CORP_CODE != null)
                                        .Where(w => w.CHAT_DEL_FG.Equals("N"))
                                        .Select(s => new
                                        {
                                            s.CHAT_NO,
                                            s.CHAT_TEXT,
                                            code = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                            key = s.PDT_IST_EMP_NO
                                        });

            return View(chkNo);
        }

        public void JSON_delete(string corp, string type, string key)
        {
            var chkBoard = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp))
                                        .Where(w => w.post_type.Equals(type))
                                        .Where(w => w.post_ID.ToString().Equals(key));

            chkBoard.Single().DEL_FG = "Y";

            DB.SaveChanges();
        }

        public void JSON_rmTxt(int idx)
        {
            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int emp = empData.SingleOrDefault().EMP_NO;

            var data = DB.CHAT_SaveText.Where(w => w.EMP_NO.ToString().Equals(emp.ToString()))
                                       .Where(w => w.CHAT_NO.ToString().Equals(idx.ToString()));

            if (data.Any())
            {
                //DB.CHAT_SaveText.Remove(data.FirstOrDefault());

                data.FirstOrDefault().CHAT_DEL_FG = "Y";

                DB.SaveChanges();
            }
        }

        public JsonResult JSON_updateTxt(string txt, string code = "", string key = "")
        {
            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int emp = empData.SingleOrDefault().EMP_NO;
            int idx = 1;

            var chkNo = DB.CHAT_SaveText.Where(w => w.EMP_NO.ToString().Equals(emp.ToString()))
                                        .GroupBy(g => new { g.EMP_NO })
                                        .Select(s => new
                                        {
                                            idx = s.Max(max => max.CHAT_NO)
                                        });

            try { idx = int.Parse(chkNo.FirstOrDefault().idx.ToString()) + 1; }
            catch { }

            var date = DateTime.Now;

            CHAT_SaveText data = new CHAT_SaveText();

            data.EMP_NO         = emp;
            data.CHAT_NO        = idx;
            data.CHAT_TEXT      = txt;
            data.CHAT_IST_DATE  = date;
            data.CHAT_DEL_FG    = "N";

            if (!string.IsNullOrEmpty(code))
            {
                var priKey = getCode(code, key);

                string corp = priKey[0];
                string type = priKey[1];
                string pdtyy = priKey[3];

                int empno = Convert.ToInt32(priKey[2]);

                short pdtseq = Convert.ToInt16(priKey[4]);

                data.CORP_CODE = corp;
                data.PDT_TYPE_CODE = type;
                data.PDT_IST_EMP_NO = empno;
                data.PDT_YY = pdtyy;
                data.PDT_SEQ = pdtseq;
            }

            DB.CHAT_SaveText.Add(data);
            DB.SaveChanges();

            return Json(idx);
        }

        public JsonResult JSON_chkNTalk()
        {
            var chkData = DB.CHAT_Room.Where(w => w.CHAT_isNew.Equals("Y"));

            string isNew = "N";

            if (chkData.Any())
            {
                isNew = "Y";
            }

            return Json(isNew);
        }

        public JsonResult JSON_Update(string corp, string type, int key, string content)
        {
            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO,
                                      s.EMP_NAME
                                  });

            int postID = 1;

            var boardData = from bf in DB.NT_Board_2
                            where bf.post_type == type && bf.CORP_CODE == corp
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

            board.CORP_CODE = corp;
            board.post_ID = postID;
            board.post_type = type;
            //board.CU_YY = memData.Single().CU_YY;
            board.CU_SEQ = empData.Single().EMP_NO;
            board.post_content = content;
            board.post_parent = key;

            board.post_status = "N";
            board.DEL_FG = "N";
            board.ist_date = date;
            board.ist_time = time;

            string name = "";

            switch(corp)
            {
                case "NTB":
                    name = "엔타비♡";
                    break;
                case "NDT":
                    name = "엔데뜨♡";
                    break;

            }

            board.name = name;

            if ("qna" == type)
            {
                var parentTable = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp)).Where(w => w.post_type.Equals(type)).Where(w => w.post_ID == key);

                parentTable.Single().post_status = "Y";
            }

            DB.NT_Board_2.Add(board);
            DB.SaveChanges();

            return Json(board);
        }

        // GET: Community/inQuery
        public ActionResult inQuery()
        {
            return View();
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
    }
}