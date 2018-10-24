using NCompany.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class CarController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        // GET: Car
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

        //    return View();
        //}

        //// GET: Car
        //[HttpPost]
        //public ActionResult Index(FormCollection f)
        //{
            var carData = DB.CAR_0.Select(s => new
            {
                s.CAR_SEQ,
                s.CAR_NAME,
                s.CAR_REG_NUM,
                s.CAR_TYPE_CODE,
                s.CAR_AREA_CODE,
                s.CAR_NOTE,
                s.CAR_RENT_CORP,
                s.CAR_FEATURE,
            });

            return View(carData);
            //string sDay = f["sday"],
            //       area = f["area"],
            //       type = f["type"];

            //int cSeq = Convert.ToInt32(f["name"].ToString());

            //sDay = Regex.Replace(sDay, @"\D", "");

            //var carData = DB.CAR_0.Where(w => w.CAR_AREA_CODE.Equals(area))
            //                      .Where(w => w.CAR_TYPE_CODE.Equals(type))
            //                      .Where(w => w.CAR_SEQ == cSeq)
            //                      .Select(s => new
            //                      {
            //                          s.CAR_SEQ,
            //                          s.CAR_NAME,
            //                          s.CAR_REG_NUM,
            //                          s.CAR_TYPE_CODE,
            //                          s.CAR_AREA_CODE,
            //                          s.CAR_NOTE,
            //                          s.CAR_RENT_CORP,
            //                          s.CAR_FEATURE,
            //                          COU_SEQ = 0
            //                      });

            ////carData = carData.Join(DB.COU_0.Where(w => w.COU_CAR_SEQ == cSeq),
            ////                      a => a.CAR_SEQ,
            ////                      b => b.COU_CAR_SEQ,
            ////                      (a, b) => new
            ////                      {
            ////                          a.CAR_SEQ,
            ////                          a.CAR_NAME,
            ////                          a.CAR_REG_NUM,
            ////                          a.CAR_TYPE_CODE,
            ////                          a.CAR_AREA_CODE,
            ////                          a.CAR_NOTE,
            ////                          a.CAR_RENT_CORP,
            ////                          a.CAR_FEATURE,
            ////                      });

            //carData = (from a in carData
            //           join b in DB.COU_0.Where(w => w.COU_CAR_SEQ == cSeq).Where(w => w.COU_DATE.Equals(sDay)) on a.CAR_SEQ.ToString() equals b.COU_CAR_SEQ.ToString() into c
            //           from d in c.DefaultIfEmpty()
            //           select new
            //           {
            //               a.CAR_SEQ,
            //               a.CAR_NAME,
            //               a.CAR_REG_NUM,
            //               a.CAR_TYPE_CODE,
            //               a.CAR_AREA_CODE,
            //               a.CAR_NOTE,
            //               a.CAR_RENT_CORP,
            //               a.CAR_FEATURE,
            //               COU_SEQ = d == null ? 0 : d.COU_SEQ,
            //           }
            //          );

            ////schData = (from f in schData
            ////           join j in DB.CAR_0 on f.COU_CAR_SEQ.ToString() equals j.CAR_SEQ.ToString() into jj
            ////           from fj in jj.DefaultIfEmpty()
            ////           select new
            ////           {
            ////               //f.title,
            ////               //f.date,
            ////               //car = fj == null ? "" : fj.CAR_NAME + "(" + fj.CAR_REG_NUM + ")",
            ////               //f.emp,
            ////               //type = fj == null ? "" : fj.CAR_TYPE_CODE,
            ////               //f.driver,
            ////               //f.cnt,
            ////               //f.note,
            ////               //f.seq,


            ////               f.COU_SEQ,
            ////               f.COU_TITLE,
            ////               f.COU_DATE,
            ////               f.COU_AREA_CODE,
            ////               f.COU_EMP_NO,
            ////               f.COU_CAR_SEQ,
            ////               f.COU_DRIVER,
            ////               f.COU_NOTE,
            ////               type = fj == null ? "" : fj.CAR_TYPE_CODE,
            ////           }
            ////          );

            //var guideData = DB.EMP_0.Where(w => w.PER_CODE.Equals("GD"))
            //                        .Select(s => new
            //                        {
            //                            s.EMP_NO,
            //                            s.EMP_NAME
            //                        });

            //var courseData = DB.COU_0.Where(w => w.COU_DATE.Equals(sDay))
            //                         .Select(s => new
            //                         {
            //                             s.COU_SEQ,
            //                             s.COU_EMP_NO,
            //                             s.COU_DRIVER,
            //                             s.COU_TITLE,
            //                             s.COU_NOTE,
            //                         });

            //string sDay2 = sDay.Substring(0, 4) + "-" + sDay.Substring(4, 2) + "-" + sDay.Substring(6, 2);

            ////var customerData = DB.EVE_0.Where(w => w.EVE_DATE.Equals(sDay2))
            ////                           .Where(w => w.EVE_SET != "Y")
            ////                           .Select(s => new
            ////                           {
            ////                               s.EVE_DATE,
            ////                               s.EVE_SEQ,
            ////                               s.EVE_NAME,
            ////                               s.EVE_CNT,
            ////                           });

            ////int currentCsSeq = carData.FirstOrDefault().COU_SEQ;

            ////var customerData2 = DB.COU_1.Where(w => w.COU_DATE.Equals(sDay2))
            ////                            .Where(w => w.COU_SEQ == currentCsSeq)
            ////                            .Select(s => new
            ////                            {
            ////                                s.COU_DATE,
            ////                                //s.COU_SEQ,
            ////                                s.EVE_SEQ,
            ////                                s.EVE_NAME,
            ////                                s.EVE_CNT
            ////                            });

            //ViewBag.guide = guideData;
            //ViewBag.course = courseData;
            //ViewBag.date = sDay;
            ////ViewBag.customer = customerData;
            ////ViewBag.customer2 = customerData2;

            //return View(carData);
        }

        public ActionResult Schedule()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult Course(string searchCourse = "", string searchArea = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (!string.IsNullOrEmpty(searchCourse) && !string.IsNullOrEmpty(searchArea))
            {
                var couData = DB.COU_0.Where(w => w.COU_AREA_CODE.Equals(searchArea))
                                      .Where(w => w.COU_SEQ.ToString().Equals(searchCourse))
                                      .Select(s => new
                                      {
                                          s.COU_AREA_CODE,
                                          s.COU_ADDRESS,
                                          s.COU_LATITUDE,
                                          s.COU_LONGITUDE,
                                          s.COU_TITLE,
                                          s.COU_CONT,
                                          s.COU_attachment,
                                          s.COU_SEQ,
                                      });

                return View(couData);
            }

            return View();
        }

        public ActionResult setCourse(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string area = "",
                   address = "",
                   lat = "",
                   lng = "",
                   title = "",
                   content = "",
                   attach = "",
                   mode = "",
                   seq = "";
            
            seq = f["seq"];
            area = f["area"];
            address = f["address"];
            lat = f["lat"];
            lng = f["lng"];
            title = f["title"];
            content = HttpUtility.UrlDecode(f["txt"]);
            mode = f["mode"];
            attach = f["attach"];

            HttpFileCollectionBase uploadedFiles = Request.Files;

            int maxFile = 10;

            switch (mode)
            {
                case "i":
                    if (uploadedFiles.Count > maxFile)
                    {
                        return Content("<script>alert('한번에 올릴 수 있는 파일수는 " + maxFile.ToString() + "개입니다.');location.href='/Car/Course';</script>");// Redirect("/Event/");
                    }

                    for (int i = 0; i < uploadedFiles.Count; i++)
                    {
                        if (uploadedFiles[i].ContentLength > 0 && !String.IsNullOrEmpty(uploadedFiles[i].FileName))
                        {
                            var fileName = Path.GetFileName(uploadedFiles[i].FileName);

                            string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

                            string fileNames = nowDt + "_" + fileName;

                            var path = Path.Combine(Server.MapPath("~/attachment"), fileNames);

                            uploadedFiles[i].SaveAs(path);


                            string fileloc = Request.Url.GetLeftPart(UriPartial.Authority) + "/attachment/" + fileNames;

                            attach += fileloc + ";";
                        }
                    }

                    int idx = 1;

                    try
                    {
                        int couCnt = DB.COU_0.Where(w => w.COU_AREA_CODE.Equals(area))
                                             .GroupBy(g => g.CORP_CODE)
                                             .Select(s => new { seq = s.Max(max => max.COU_SEQ) })
                                             .FirstOrDefault()
                                             .seq;

                        idx = couCnt + 1;
                    }
                    catch
                    {

                    }

                    COU_0 course = new COU_0();

                    course.CORP_CODE = "NTB";
                    course.COU_SEQ = idx;
                    course.COU_AREA_CODE = area;
                    course.COU_ADDRESS = address;
                    course.COU_LATITUDE = lat;
                    course.COU_LONGITUDE = lng;
                    course.COU_TITLE = title;
                    course.COU_CONT = content;
                    course.COU_attachment = attach;

                    DB.COU_0.Add(course);
                    break;

                case "u":
                    var attachData = DB.COU_0.Where(w => w.COU_SEQ.ToString().Equals(seq))
                                             .Where(w => w.COU_AREA_CODE.Equals(area))
                                             .FirstOrDefault();

                    string attachText = attachData.COU_attachment.ToString();

                    string[] attachments = attachText.Split(';');
                    
                    int attachCnt = attachments.Length;
                    
                    var temp = new List<string>();

                    attach = "";

                    foreach(string tempAttach in attachments)
                    {
                        if (!string.IsNullOrEmpty(tempAttach))
                        {
                            temp.Add(tempAttach);

                            attach += tempAttach + ";";
                        }
                    }

                    if (uploadedFiles.Count + temp.Count > maxFile)
                    {
                        return Content("<script>alert('한번에 올릴 수 있는 파일수는 " + maxFile.ToString() + "개입니다.');location.href='/Car/Course';</script>");// Redirect("/Event/");
                    }

                    for (int i = 0; i < uploadedFiles.Count; i++)
                    {
                        if (uploadedFiles[i].ContentLength > 0 && !String.IsNullOrEmpty(uploadedFiles[i].FileName))
                        {
                            var fileName = Path.GetFileName(uploadedFiles[i].FileName);

                            string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

                            string fileNames = nowDt + "_" + fileName;

                            var path = Path.Combine(Server.MapPath("~/attachment"), fileNames);

                            uploadedFiles[i].SaveAs(path);


                            string fileloc = Request.Url.GetLeftPart(UriPartial.Authority) + "/attachment/" + fileNames;

                            attach += fileloc + ";";
                        }
                    }

                    var couData = DB.COU_0.Where(w => w.COU_SEQ.ToString().Equals(seq))
                                          .Where(w => w.CORP_CODE.Equals("NTB"))
                                          .Where(w => w.COU_AREA_CODE.Equals(area))
                                          .FirstOrDefault();

                    couData.COU_ADDRESS = address;
                    couData.COU_LATITUDE = lat;
                    couData.COU_LONGITUDE = lng;
                    couData.COU_CONT = content;
                    couData.COU_attachment = attach;
                    break;
            }

            DB.SaveChanges();

            return Content("<script>alert('코스가 저장되었습니다.');location.href='/Car/Course'</script>");
            
            //return Redirect("/Car/Course");
        }

        public ActionResult removeCourse(int seq, string area)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var csData = DB.COU_0.Where(w => w.COU_SEQ == seq)
                                 .Where(w => w.COU_AREA_CODE.Equals(area))
                                 .FirstOrDefault();

            DB.COU_0.Remove(csData);

            DB.SaveChanges();

            return Content("<script>alert('코스가 삭제되었습니다.');location.href='/Car/Course'</script>");
        }

        public ActionResult dayCourse(string tit = "", string sDay = "", string area = "", string course = "", string sub = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            try
            {

                var guideData = DB.EMP_0.Where(w => w.PER_CODE.Equals("GD"))
                                        .Select(s => new
                                        {
                                            s.EMP_NO,
                                            s.EMP_NAME
                                        });

                ViewBag.guide = guideData;

                //if (!string.IsNullOrEmpty(tit) && !string.IsNullOrEmpty(sDay) && !string.IsNullOrEmpty(area) && !string.IsNullOrEmpty(course))
                if (!string.IsNullOrEmpty(sDay) && !string.IsNullOrEmpty(area) && !string.IsNullOrEmpty(course))
                {
                    string txtDate = Regex.Replace(sDay, @"\D", "");

                    txtDate = txtDate.Substring(0, 4) + "-" + txtDate.Substring(4, 2) + "-" + txtDate.Substring(6, 2);

                    var csData = DB.COU_1.Where(w => w.COU_DATE.Equals(txtDate))
                                         .Where(w => w.COU_AREA_CODE.Equals(area))
                                         .Where(w => w.COU_SEQ.ToString().Equals(course))
                        //.Where(w => w.COU_TITLE.Equals(tit))
                                         .Where(w => w.COU_SUB_NUM.ToString().Equals(sub))
                                         .Select(s => new
                                         {
                                             s.COU_SEQ,
                                             s.COU_TITLE,
                                             s.COU_DATE,
                                             s.COU_AREA_CODE,
                                             s.CAR_TYPE_CODE,
                                             s.COU_CAR_SEQ,
                                             s.COU_EMP_NO,
                                             s.COU_DRIVER,
                                             s.COU_NOTE,
                                             s.COU_CNT,
                                             s.COU_SUB_NUM,
                                         });

                    //csData = csData.Join(DB.CAR_0,
                    //                    a => a.COU_CAR_SEQ,
                    //                    b => b.CAR_SEQ,
                    //                    (a, b) => new
                    //                    {
                    //                        a.COU_SEQ,
                    //                        a.COU_TITLE,
                    //                        a.COU_DATE,
                    //                        a.COU_AREA_CODE,
                    //                        b.CAR_TYPE_CODE,
                    //                        a.COU_CAR_SEQ,
                    //                        a.COU_EMP_NO,
                    //                        a.COU_DRIVER,
                    //                        a.COU_NOTE,
                    //                        a.COU_CNT,
                    //                    });

                    //csData = from a in csData
                    //         join b in DB.CAR_0 on a.COU_CAR_SEQ equals b.CAR_SEQ into c
                    //         from d in c.DefaultIfEmpty()
                    //         select new
                    //         {
                    //             a.COU_SEQ,
                    //             a.COU_TITLE,
                    //             a.COU_DATE,
                    //             a.COU_AREA_CODE,
                    //             CAR_TYPE_CODE = d == null ? "" : d.CAR_TYPE_CODE,
                    //             a.COU_CAR_SEQ,
                    //             a.COU_EMP_NO,
                    //             a.COU_DRIVER,
                    //             a.COU_NOTE,
                    //             a.COU_CNT,
                    //             a.COU_SUB_NUM,
                    //         };


                    //string sDay2 = txtDate.Substring(0, 4) + "-" + txtDate.Substring(4, 2) + "-" + txtDate.Substring(6, 2);
                    string title = csData.Single().COU_TITLE;

                    var customerData = DB.EVE_0.Where(w => w.EVE_DATE.Equals(sDay))
                                               .Where(w => w.EVE_AREA_CODE.Equals(area))
                                               .Where(w => w.EVE_SET != "Y")
                                               .Select(s => new
                                               {
                                                   s.EVE_DATE,
                                                   s.EVE_SEQ,
                                                   s.EVE_NAME,
                                                   s.EVE_CNT,
                                                   s.EVE_OFFICE_CODE,
                                                   s.EVE_TEL,
                                                   s.EVE_REG_DATE,
                                               });

                    int currentCsSeq = csData.FirstOrDefault().COU_SEQ;

                    var customerData2 = DB.COU_2.Where(w => w.COU_DATE.Equals(sDay))
                                                .Where(w => w.COU_SEQ == currentCsSeq)
                                                .Where(w => w.COU_TITLE.Equals(title))
                                                .Select(s => new
                                                {
                                                    s.COU_DATE,
                                                    //s.COU_SEQ,
                                                    s.EVE_SEQ,
                                                    s.EVE_NAME,
                                                    s.EVE_CNT,
                                                    s.EVE_OFFICE_CODE,
                                                    s.EVE_TEL,
                                                    s.EVE_REG_DATE,
                                                });

                    ViewBag.customer = customerData;
                    ViewBag.customer2 = customerData2;

                    return View(csData);
                }

                ViewBag.customer = "";
                ViewBag.customer2 = "";

                return View();
            }
            catch
            {
                return Redirect("/Car/Schedule");
            }
        }

        // GET: Car/Uploader
        public ActionResult Uploader()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult setDivision(string COU_SEQ, string COU_DATE, string COU_AREA_CODE, string COU_TITLE, string COU_SUB_NUM)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var guideData = DB.EMP_0.Where(w => w.PER_CODE.Equals("GD"))
                                    .Select(s => new
                                    {
                                        s.EMP_NO,
                                        s.EMP_NAME
                                    });

            ViewBag.seq = COU_SEQ;
            ViewBag.title = COU_TITLE + " (2)";
            ViewBag.date = COU_DATE;
            ViewBag.area = COU_AREA_CODE;
            ViewBag.guide = guideData;
            ViewBag.subnum = Convert.ToInt32(COU_SUB_NUM) + 1;

            return View();
        }

        //public ActionResult setSchedule(int seq = 0, string date = "")
        //{
        //    var schData = DB.COU_0.Select(s => new
        //                                 {
        //                                     s.COU_SEQ,
        //                                     s.COU_TITLE,
        //                                     s.COU_DATE,
        //                                     s.COU_AREA_CODE,
        //                                     s.COU_EMP_NO,
        //                                     s.COU_CAR_SEQ,
        //                                     s.COU_DRIVER,
        //                                     s.COU_NOTE,
        //                                     type = "",
        //                                 });

        //    if (!string.IsNullOrEmpty(date) && seq != 0)
        //    {
        //        string txtDate = Regex.Replace(date, @"\D", "");

        //        schData = schData.Where(w => w.COU_DATE.Equals(txtDate))
        //                         .Where(w => w.COU_SEQ == seq);

        //        schData = (from f in schData
        //                   join j in DB.CAR_0 on f.COU_CAR_SEQ.ToString() equals j.CAR_SEQ.ToString() into jj
        //                   from fj in jj.DefaultIfEmpty()
        //                   select new
        //                   {
        //                       //f.title,
        //                       //f.date,
        //                       //car = fj == null ? "" : fj.CAR_NAME + "(" + fj.CAR_REG_NUM + ")",
        //                       //f.emp,
        //                       //type = fj == null ? "" : fj.CAR_TYPE_CODE,
        //                       //f.driver,
        //                       //f.cnt,
        //                       //f.note,
        //                       //f.seq,


        //                       f.COU_SEQ,
        //                       f.COU_TITLE,
        //                       f.COU_DATE,
        //                       f.COU_AREA_CODE,
        //                       f.COU_EMP_NO,
        //                       f.COU_CAR_SEQ,
        //                       f.COU_DRIVER,
        //                       f.COU_NOTE,
        //                       type = fj == null ? "" : fj.CAR_TYPE_CODE,
        //                   }
        //                  );
        //    }
        //    else
        //    {
        //        schData = null;
        //    }

        //    var guideData = DB.EMP_0.Where(w => w.PER_CODE.Equals("GD"))
        //                            .Select(s => new
        //                            {
        //                                s.EMP_NO,
        //                                s.EMP_NAME
        //                            })
        //                            .OrderBy(o => o.EMP_NAME);

        //    ViewBag.guide = guideData;

        //    return View(schData);
        //}

        // Post: Car/CarUpload
        public ActionResult CarUpload(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string feature = f["feature"],
                   title = f["name"],
                   num = f["num"],
                   type = f["type"],
                   area = f["area"],
                   note = f["note"],
                   rent = f["rent"],
                   seq = f["seq"];

            var chkData = DB.CAR_0.Where(w => w.CAR_SEQ.ToString().Equals(seq));

            if (chkData.Any())
            {
                chkData.Single().CAR_NAME = title;
                chkData.Single().CAR_REG_NUM = num;
                chkData.Single().CAR_TYPE_CODE = type;
                chkData.Single().CAR_AREA_CODE = area;
                chkData.Single().CAR_NOTE = note;
                chkData.Single().CAR_RENT_CORP = rent;
                chkData.Single().CAR_FEATURE = feature;
            }
            else
            {
                int idx = 1;

                int carCnt = 0;
                
                try
                {
                    carCnt = DB.CAR_0.GroupBy(g => g.CORP_CODE)
                                     .Select(s => new { seq = s.Max(max => max.CAR_SEQ) })
                                     .FirstOrDefault()
                                     .seq;
                }
                catch
                {

                }

                idx += carCnt;

                CAR_0 Data = new CAR_0();

                Data.CORP_CODE = "NTB";
                Data.CAR_SEQ = idx;

                Data.CAR_NAME = title;
                Data.CAR_REG_NUM = num;
                Data.CAR_TYPE_CODE = type;
                Data.CAR_AREA_CODE = area;
                Data.CAR_NOTE = note;
                Data.CAR_RENT_CORP = rent;
                Data.CAR_FEATURE = feature;

                DB.CAR_0.Add(Data);
            }

            DB.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult updateCourse(FormCollection f)//string date, int seq, string mode, int guide = 0, string driver = "", string title = "", string area = "", int car = 0, string note = "", int sub_num = 1)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string seq = f["seq"],
                   area = f["area"],
                   sub_num = f["sub_num"],
                   mode = f["mode"],
                   title = f["title"],
                   date = f["date"],
                   type = f["type"],
                   car = string.IsNullOrEmpty(f["car"]) ? "0" : f["car"],
                   guide = f["guide"],
                   driver = f["driver"],
                   note = f["note"];

            string txtDate = Regex.Replace(date, @"\D", "");

            txtDate = txtDate.Substring(0, 4) + "-" + txtDate.Substring(4, 2) + "-" + txtDate.Substring(6, 2);

            var chkData = DB.COU_1.Where(w => w.COU_DATE.Equals(txtDate))
                                  .Where(w => w.COU_AREA_CODE.Equals(area))
                                  .Where(w => w.COU_SEQ.ToString().Equals(seq))
                                  .Where(w => w.COU_SUB_NUM.ToString().Equals(sub_num));

            if (Convert.ToInt32(sub_num) < 1 && Convert.ToInt32(sub_num) > 9)
            {
                return Redirect("/Car/Schedule");
            }

            if (mode == "d")
            {
                DB.COU_1.Remove(chkData.FirstOrDefault());
                DB.SaveChanges();

                return Redirect("/Car/Schedule");
            }

            if (string.IsNullOrEmpty(driver))
            {
                try
                {
                    driver = DB.EMP_0.Where(w => w.EMP_NO.ToString().Equals(guide))
                                     .Select(s => new { s.EMP_NAME }).FirstOrDefault().EMP_NAME;
                }
                catch
                {

                }
            }

            if (chkData.Any())
            {
                chkData.Single().COU_TITLE = title;
                chkData.Single().COU_EMP_NO = Convert.ToInt32(guide);
                chkData.Single().COU_DRIVER = driver;
                chkData.Single().CAR_TYPE_CODE = type;
                chkData.Single().COU_CAR_SEQ = Convert.ToInt32(car);
                chkData.Single().COU_NOTE = note;
            }
            else
            {
                int idx = 0;

                int couCnt = DB.COU_1.Where(w => w.COU_DATE.Equals(txtDate))
                                     .Where(w => w.COU_AREA_CODE.Equals(area))
                                     .Where(w => w.COU_SEQ.ToString().Equals(seq))
                                     .Where(w => w.COU_SUB_NUM.ToString().Equals(sub_num))
                                     .Count();

                idx = Convert.ToInt32(seq) + couCnt + 1;

                COU_1 Data = new COU_1();

                Data.CORP_CODE = "NTB";
                Data.COU_SEQ = idx;
                Data.COU_DATE = txtDate;
                chkData.Single().CAR_TYPE_CODE = type;
                Data.COU_CAR_SEQ = Convert.ToInt32(car);
                Data.COU_TITLE = title;
                Data.COU_EMP_NO = Convert.ToInt32(guide);
                Data.COU_AREA_CODE = area;
                Data.COU_NOTE = note;
                Data.COU_CNT = 0;
                Data.COU_SUB_NUM = Convert.ToInt32(sub_num);

                Data.COU_DRIVER = driver;

                DB.COU_1.Add(Data);
            }

            DB.SaveChanges();

            string redirectUrl = "/Car/dayCourse?sDay=" + date + "&area=" + area + "&course=" + seq + "&sub=" + sub_num;

            return Redirect(redirectUrl);
        }

        //public JsonResult removeCourse(int seq, string date)
        //{
        //    string txtDate = Regex.Replace(date, @"\D", "");

        //    var csData = DB.COU_0.Where(w => w.COU_SEQ.Equals(seq))
        //                         .Where(w => w.COU_DATE.Equals(txtDate))
        //                         .FirstOrDefault();

        //    DB.COU_0.Remove(csData);

        //    DB.SaveChanges();

        //    return Json("");
        //}

        public JsonResult removeCar(string seq)
        {
            var carData = DB.CAR_0.Where(w => w.CAR_SEQ.ToString().Equals(seq))
                                  .FirstOrDefault();

            DB.CAR_0.Remove(carData);

            DB.SaveChanges();

            return Json("");
        }

        ////public JsonResult updateCustomer(string date, int csSeq, int evSeq, string type)
        //public JsonResult updateCustomer(string date, int csSeq, string regDate, int evSeq, string title, string area, string type)
        //{
        //    string txtDate = Regex.Replace(date, @"\D", "");

        //    var csData = DB.COU_0.Where(w => w.COU_DATE.Equals(txtDate))
        //                         .Where(w => w.COU_SEQ == csSeq)
        //                         .FirstOrDefault();

        //    int cnt = Convert.ToInt32(csData.COU_CNT.ToString());

        //    string evDate = txtDate.Substring(0, 4) + "-" + txtDate.Substring(4, 2) + "-" + txtDate.Substring(6, 2);

        //    //var evData = DB.EVE_0.Where(w => w.EVE_DATE.Equals(evDate))
        //    //                     .Where(w => w.EVE_SEQ == evSeq)
        //    //                     //.Select(s => new
        //    //                     //{
        //    //                     //    s.EVE_NAME,
        //    //                     //    s.EVE_CNT
        //    //                     //})
        //    //                     .FirstOrDefault();


        //    var evData = DB.EVE_0.Where(w => w.EVE_REG_DATE.Equals(regDate))
        //                         .Where(w => w.EVE_SEQ == evSeq)
        //                         .FirstOrDefault();

        //    //string title = csData.COU_TITLE;
        //    string name = evData.EVE_NAME;

        //    if (type == "+")
        //    {
        //        cnt += Convert.ToInt32(evData.EVE_CNT.ToString());

        //        COU_1 Data = new COU_1();

        //        Data.CORP_CODE = "NTB";
        //        Data.COU_DATE = evDate;
        //        Data.COU_SEQ = csSeq;
        //        Data.COU_TITLE = title;
        //        Data.EVE_SEQ = evSeq;
        //        Data.EVE_REG_DATE = regDate;
        //        Data.EVE_NAME = name;
        //        Data.EVE_CNT = evData.EVE_CNT;
        //        Data.EVE_OFFICE_CODE = evData.EVE_OFFICE_CODE;
        //        Data.EVE_TEL = evData.EVE_TEL;

        //        DB.COU_1.Add(Data);

        //        evData.EVE_SET = "Y";
        //    }
        //    else
        //    {
        //        cnt -= Convert.ToInt32(evData.EVE_CNT.ToString());

        //        COU_1 Data = DB.COU_1.Where(w => w.COU_DATE.Equals(evDate))
        //                             .Where(w => w.EVE_REG_DATE.Equals(regDate))
        //                             .Where(w => w.COU_SEQ == csSeq)
        //                             .Where(w => w.COU_TITLE.Equals(title))
        //                             .Where(w => w.EVE_SEQ == evSeq)
        //                             .Where(w => w.EVE_NAME.Equals(name))
        //                             .FirstOrDefault();

        //        DB.COU_1.Remove(Data);

        //        evData.EVE_SET = "N";
        //    }

        //    csData.COU_CNT = cnt;

        //    DB.SaveChanges();

        //    return Json(cnt);
        //}

        //public JsonResult JSON_updateCourse(string date, int seq, int guide, string driver, string title = "", string area = "", int car = 0, string note = "")
        //{
        //    string txtDate = Regex.Replace(date, @"\D", "");

        //    var chkData = DB.COU_0.Where(w => w.COU_DATE.Equals(txtDate))
        //                          .Where(w => w.COU_SEQ == seq);

        //    if (string.IsNullOrEmpty(driver))
        //    {
        //        driver = DB.EMP_0.Where(w => w.EMP_NO == guide)
        //                         .Select(s => new { s.EMP_NAME }).FirstOrDefault().EMP_NAME;
        //    }

        //    if (chkData.Any())
        //    {
        //        chkData.Single().COU_EMP_NO = guide;
        //        chkData.Single().COU_DRIVER = driver;
        //        chkData.Single().COU_CAR_SEQ = car;
        //        chkData.Single().COU_NOTE = note;
        //    }
        //    else
        //    {
        //        int couCnt = DB.COU_0.Where(w => w.COU_DATE.Equals(txtDate))
        //                             .Count();

        //        seq = couCnt + 1;

        //        COU_0 Data = new COU_0();

        //        Data.CORP_CODE = "NTB";
        //        Data.COU_SEQ = seq;
        //        Data.COU_DATE = txtDate;
        //        Data.COU_CAR_SEQ = car;
        //        Data.COU_TITLE = title;
        //        Data.COU_EMP_NO = guide;
        //        Data.COU_AREA_CODE = area;
        //        Data.COU_NOTE = note;
        //        Data.COU_CNT = 0;

        //        Data.COU_DRIVER = driver;

        //        DB.COU_0.Add(Data);
        //    }

        //    DB.SaveChanges();

        //    return Json(seq);
        //}


        public JsonResult JSON_getSchedule(string type, string area)
        {

            //var schData = DB.COU_0.Join(carData,
            //                           a => a.COU_CAR_SEQ,
            //                           b => b.CAR_SEQ,
            //                           (a, b) => new
            //                           {
            //                               title = a.COU_TITLE,
            //                               date = a.COU_DATE.Substring(0, 4) + "-" + a.COU_DATE.Substring(4, 2) + "-" + a.COU_DATE.Substring(6, 2),
            //                               car = b.CAR_NAME + "(" + b.CAR_REG_NUM + ")",
            //                               emp = a.COU_EMP_NO.ToString(),
            //                               type = b.CAR_TYPE_CODE,
            //                               driver = a.COU_DRIVER,
            //                               cnt = a.COU_CNT,
            //                           });

            var schData = DB.COU_1.Where(w => w.COU_AREA_CODE.Equals(area))
                              .Select(s => new
                              {
                                  title = s.COU_TITLE,
                                  date = s.COU_DATE,
                                  car = s.COU_CAR_SEQ.ToString(),//b.CAR_NAME + "(" + b.CAR_REG_NUM + ")",
                                  emp = s.COU_EMP_NO.ToString(),
                                  type = "",//b.CAR_TYPE_CODE,
                                  driver = s.COU_DRIVER,
                                  cnt = s.COU_CNT,
                                  note = s.COU_NOTE,
                                  seq = s.COU_SEQ,
                                  area = s.COU_AREA_CODE,
                                  sub = s.COU_SUB_NUM,
                              });

            //schData = schData.Where(w => w.area.Equals(area));

            //schData = schData.Join(DB.CAR_0,
            //                      a => a.car,
            //                      b => b.CAR_SEQ.ToString(),
            //                      (a, b) => new
            //                      {
            //                          a.title,
            //                          a.date,
            //                          car = b.CAR_NAME + "(" + b.CAR_REG_NUM + ")",
            //                          a.emp,
            //                          type = b.CAR_TYPE_CODE,
            //                          a.driver,
            //                          a.cnt,
            //                          a.note,
            //                          a.seq,
            //                      });

            if (type != "ALL")
            {
                var carData = DB.CAR_0.Where(w => w.CAR_AREA_CODE.Equals(area))
                                      .Select(s => new
                                      {
                                          s.CAR_SEQ,
                                          s.CAR_NAME,
                                          s.CAR_REG_NUM,
                                          s.CAR_TYPE_CODE,
                                      });

                carData = carData.Where(w => w.CAR_TYPE_CODE.Equals(type));

                schData = schData.Join(carData,
                                      a => a.car,
                                      b => b.CAR_SEQ.ToString(),
                                      (a, b) => new
                                      {
                                          a.title,
                                          a.date,
                                          car = b.CAR_NAME + "(" + b.CAR_REG_NUM + ")",
                                          a.emp,
                                          type = b.CAR_TYPE_CODE,
                                          a.driver,
                                          a.cnt,
                                          a.note,
                                          a.seq,
                                          a.area,
                                          a.sub,
                                      });
            }
            else
            {
                schData = (from f in schData
                           join j in DB.CAR_0 on f.car equals j.CAR_SEQ.ToString() into jj
                           from fj in jj.DefaultIfEmpty()
                           select new
                           {
                               f.title,
                               f.date,
                               car = fj == null ? "" : fj.CAR_NAME + "(" + fj.CAR_REG_NUM + ")",
                               f.emp,
                               type = fj == null ? "" : fj.CAR_TYPE_CODE,
                               f.driver,
                               f.cnt,
                               f.note,
                               f.seq,
                               f.area,
                               f.sub,
                           }
                          );
            }

            //schData = schData.Join(DB.EMP_0,
            //                      a => a.emp,
            //                      b => b.EMP_NO.ToString(),
            //                      (a, b) => new
            //                      {
            //                          a.title,
            //                          a.date,
            //                          a.car,
            //                          emp = b.EMP_NAME.ToString(),
            //                          type = a.type,
            //                          a.driver,
            //                          a.cnt,
            //                          a.note,
            //                          a.seq,
            //                          a.area,
            //                      });

            schData = (from f in schData
                       join j in DB.EMP_0 on f.emp equals j.EMP_NO.ToString() into jj
                       from fj in jj.DefaultIfEmpty()
                       select new
                       {
                           f.title,
                           f.date,
                           f.car,
                           emp = fj == null ? "" : fj.EMP_NAME.ToString(),
                           f.type,
                           f.driver,
                           f.cnt,
                           f.note,
                           f.seq,
                           f.area,
                           f.sub,
                       }
                      );

            return Json(schData);
        }

        public JsonResult JSON_getCar(string type, string seq = "")
        //public JsonResult JSON_getCar(string area, string type, string seq = "")
        {
            var Data = DB.CAR_0//.Where(w => w.CAR_AREA_CODE.Equals(area))
                               .Where(w => w.CAR_TYPE_CODE.Equals(type))
                               .Select(s => new
                               {
                                   s.CAR_SEQ,
                                   s.CAR_NAME,
                                   s.CAR_REG_NUM,
                                   s.CAR_TYPE_CODE,
                                   s.CAR_AREA_CODE,
                                   s.CAR_NOTE,
                                   s.CAR_RENT_CORP,
                                   s.CAR_FEATURE,
                               });

            if (!string.IsNullOrEmpty(seq))
            {
                Data = Data.Where(w => w.CAR_SEQ.ToString().Equals(seq));
            }

            return Json(Data);
        }

        public JsonResult JSON_removeAttachment(int seq, string area, string attach)
        {
            var attachData = DB.COU_0.Where(w => w.COU_SEQ == seq)
                                     .Where(w => w.COU_AREA_CODE.Equals(area))
                                     .FirstOrDefault();

            string attachText = attachData.COU_attachment.ToString();

            string[] attachments = attachText.Split(';');

            string newAttach = "";

            foreach (string attachment in attachments)
            {
                if (attachment.Equals(attach))
                    continue;

                newAttach += attachment + ";";
            }

            attachData.COU_attachment = newAttach;

            DB.SaveChanges();

            return Json("삭제하였습니다.");
        }

        //public JsonResult JSON_getCourse(string date, int seq = 0, string area = "")
        //{
        //    string txtDate = Regex.Replace(date, @"\D", "");

        //    var Data = DB.COU_0.Where(w => w.COU_DATE.Equals(txtDate))
        //                       .Select(s => new
        //                       {
        //                           s.COU_EMP_NO,
        //                           s.COU_DRIVER,
        //                           s.COU_NOTE,
        //                           s.COU_CNT,
        //                           s.COU_SEQ,
        //                           s.COU_AREA_CODE,
        //                           s.COU_TITLE,
        //                       });

        //    if (seq != 0)
        //    {
        //        Data = Data.Where(w => w.COU_SEQ == seq);
        //    }

        //    if (!string.IsNullOrEmpty(area))
        //    {
        //        Data = Data.Where(w => w.COU_AREA_CODE.Equals(area));
        //    }

        //    return Json(Data);
        //}
    }
}