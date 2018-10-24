using NCompany.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class EventController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        public string getName(int emp)
        {
            string name = "";

            name = DB.EMP_0.Where(w => w.EMP_NO == emp).Single().EMP_NAME;

            return name;
        }

        // GET: Event
        public ActionResult Index(string sDate = "", string eDate = "", string area = "", string title = "", string name = "", string tel = "", string target = "", string emp = "all", string chkVou = "", string orderby = "")
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }
            //var eveData = DB.EVE_0.Select(s => new
            //                      {
            //                          s.EVE_DATE,
            //                          s.EVE_SEQ,
            //                          s.EVE_AREA_CODE,
            //                          s.EVE_TITLE,
            //                          s.EVE_NAME,
            //                          s.EVE_CNT,
            //                          s.EVE_TEL,
            //                          s.EVE_EMAIL,
            //                          s.EVE_OFFICE_CODE,
            //                          s.EVE_ETC,
            //                          s.EVE_REG_DATE,
            //                          s.EVE_PRICE,
            //                      }).ToList();

            var eveData2 = DB.EVE_0.ToList()
                                   .Select(s => new
                                   {
                                       s.EVE_DATE,
                                       s.EVE_SEQ,
                                       s.EVE_AREA_CODE,
                                       s.EVE_TITLE,
                                       s.EVE_NAME,
                                       s.EVE_CNT,
                                       s.EVE_TEL,
                                       s.EVE_EMAIL,
                                       s.EVE_OFFICE_CODE,
                                       s.EVE_ETC,
                                       s.EVE_REG_DATE,
                                       s.EVE_PRICE,
                                       s.EVE_CHKMAIL,
                                       s.EVE_IST_EMP_NO,
                                   });

            if (string.IsNullOrEmpty(emp))
            {
                emp = user.Login.ToString();

                eveData2 = eveData2.Where(w => w.EVE_IST_EMP_NO == Convert.ToInt32(emp)).ToList();
            }
            else if (emp == "all")
            {

            }
            else
            {
                eveData2 = eveData2.Where(w => w.EVE_IST_EMP_NO == Convert.ToInt32(emp)).ToList();
            }

            eveData2 = eveData2.OrderBy(o => o.EVE_DATE);

            if (string.IsNullOrEmpty(sDate))
            {
                //eveData2 = eveData2.OrderBy(o => o.EVE_DATE);
                DateTime now  = DateTime.Now;

                eveData2 = eveData2.Where(w => DateTime.ParseExact(w.EVE_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= now);
            }
            else
            {
                eveData2 = eveData2.Where(w => DateTime.ParseExact(w.EVE_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= DateTime.ParseExact(sDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(eDate))
            {
                eveData2 = eveData2.Where(w => DateTime.ParseExact(w.EVE_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) <= DateTime.ParseExact(eDate, "yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            if (!string.IsNullOrEmpty(area))
            {
                eveData2 = eveData2.Where(w => w.EVE_AREA_CODE.Equals(area));
            }

            if (!string.IsNullOrEmpty(title))
            {
                eveData2 = eveData2.Where(w => w.EVE_TITLE.Contains(title));
            }

            if (!string.IsNullOrEmpty(name))
            {
                eveData2 = eveData2.Where(w => w.EVE_NAME.Contains(name));
            }

            if (!string.IsNullOrEmpty(tel))
            {
                eveData2 = eveData2.Where(w => w.EVE_TEL.Contains(tel));
            }

            if (!string.IsNullOrEmpty(target))
            {
                eveData2 = eveData2.Where(w => w.EVE_OFFICE_CODE.Contains(target));
            }

            if (chkVou == "O")
            {
                eveData2 = eveData2.Where(w => w.EVE_CHKMAIL != "Y");
            }

            return View(eveData2);
        }

        public ActionResult Uploader()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult senderMail(string data)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            ViewBag.data = data;

            return View();
        }

        public ActionResult sender(string data, string area, string course)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string[] keys = data.Split(';');

            foreach (string i in keys)
            {
                if (i == "")
                {
                    continue;
                }

                sendVoucher(i, area, course);//, couData.FirstOrDefault().COU_LATITUDE, couData.FirstOrDefault().COU_LONGITUDE, couData.FirstOrDefault().COU_ADDRESS, couData.FirstOrDefault().COU_CONT, attachPath);
            }

            return Redirect("/Event/");
        }
        
        //public ActionResult sender(string data, string lat, string lng, string address, string txt)
        //{
        //    string[] keys = data.Split(';');

        //    HttpFileCollectionBase uploadedFiles = Request.Files;

        //    int maxFile = 10;

        //    if (uploadedFiles.Count > maxFile)
        //    {
        //        return Content("<script>alert(한번에 올릴 수 있는 파일수는 " + maxFile.ToString() + "개입니다.);location.href='/Event';</script>");// Redirect("/Event/");
        //    }

        //    List<string> attachPath = new List<string>();

        //    for (int i = 0; i < uploadedFiles.Count; i++)
        //    {
        //        if (uploadedFiles[i].ContentLength > 0 && !String.IsNullOrEmpty(uploadedFiles[i].FileName))
        //        {
        //            var fileName = Path.GetFileName(uploadedFiles[i].FileName);

        //            string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

        //            string fileNames = nowDt + "_" + fileName;

        //            var path = Path.Combine(Server.MapPath("~/attachment"), fileNames);

        //            uploadedFiles[i].SaveAs(path);

        //            attachPath.Add(path);
        //        }
        //    }

        //    foreach (string i in keys)
        //    {
        //        if (i == "")
        //        {
        //            continue;
        //        }

        //        sendVoucher(i, lat, lng, address, txt, attachPath);
        //    }

        //    return Redirect("/Event/");
        //}

        [HttpPost]
        public ActionResult File_Upload(FormCollection f, HttpPostedFileBase file)
        {
            string officeCode = f["officeCode"],
                   areaCode = f["areaCode"],
                   type = f["type"];
                   //newCourse = f["newCourse"],
                   //Course = f["Course"];

            if (type == "mode2")
            {
                istEvent(f);
                return RedirectToAction("Index");
            }

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);

                string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

                string fileNames = nowDt + "_" + fileName;

                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/eventFolder"), fileNames);

                file.SaveAs(path);

                switch(officeCode)
                {
                    case "TMT":
                    case "WMP":
                        insertEvent1(path, officeCode, areaCode);//, Course, newCourse);
                        break;
                    case "MRT":
                        insertEvent2(path, officeCode, areaCode);//, Course, newCourse);
                        break;
                }
            }

            return RedirectToAction("Index");
        }

        public void istEvent(FormCollection f)
        {
            string sDate = f["sDate"],
                   name = f["name"],
                   cnt = f["cnt"],
                   title = f["title"],
                   tel = f["tel"],
                   email = f["mail"],
                   etc = f["etc"],
                   officeCode = f["officeCode"],
                   areaCode = f["areaCode"],
                   price = f["price"],
                   date = sDate.Substring(0, 4) + "-" + sDate.Substring(4, 2) + "-" + sDate.Substring(6, 2);
                   //newCourse = f["newCourse"],
                   //Course = f["Course"];

            cnt = Regex.Replace(cnt, @"\D", "");
            price = Regex.Replace(price, @"\D", "");

            DateTime now = DateTime.Now;

            string regDate = now.ToString("yyyy-MM-dd");

            int SEQ = 1;

            var seqData = DB.EVE_0.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.EVE_REG_DATE.Equals(regDate))
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
            Data.EVE_DATE = date;
            Data.EVE_SEQ = SEQ;
            Data.EVE_NAME = name;
            Data.EVE_TEL = tel;
            Data.EVE_EMAIL = string.IsNullOrEmpty(email) ? "-" : email;
            Data.EVE_OFFICE_CODE = officeCode;
            Data.EVE_CNT = Convert.ToInt32(cnt);
            Data.EVE_ETC = string.IsNullOrEmpty(etc) ? "-" : etc;
            Data.EVE_AREA_CODE = areaCode;
            Data.EVE_TITLE = title;
            Data.EVE_REG_DATE = regDate;
            //Data.OFFICE_SEQ = "";
            Data.EVE_PRICE = Convert.ToInt32(price) * Convert.ToInt32(cnt);

            var user = Session["user"] as User;

            Data.EVE_IST_DATE = regDate;
            Data.EVE_IST_EMP_NO = Convert.ToInt32(user.Login.ToString());

            DB.EVE_0.Add(Data);

            DB.SaveChanges();

            //if (newCourse == "Y")
            //{
            //    int csSeq = insertCourse(date, Course, areaCode);
            //    updateCustomer(date, csSeq, regDate, SEQ, Course, areaCode);
            //}
            //else if (newCourse == "N")
            //{
            //    int csSeq = 1;

            //    var csData = DB.COU_0.Where(w => w.COU_DATE.Equals(sDate))
            //                         .Where(w => w.COU_AREA_CODE.Equals(areaCode))
            //                         .Where(w => w.COU_TITLE.Equals(Course));

            //    if (!csData.Any())
            //    {
            //        csSeq = insertCourse(date, Course, areaCode);
            //    }
            //    else
            //    {
            //        csSeq = csData.FirstOrDefault().COU_SEQ;
            //    }

            //    updateCustomer(date, csSeq, regDate, SEQ, Course, areaCode);
            //}
            //else if (newCourse == "X")
            //{
                
            //}

        }

        public static DataTable ConvertCSVtoDataTable(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath);
            string[] headers = sr.ReadLine().Split(',');
            DataTable dt = new DataTable();
            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }
            while (!sr.EndOfStream)
            {
                string[] rows = Regex.Split(sr.ReadLine(), ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public void insertEvent2(string FileName, string officeCode, string areaCode)//, string Course, string newCourse)
        {
            DataTable dt = ConvertCSVtoDataTable(FileName);

            foreach (DataRow dr in dt.Rows)
            {
                udtEvent(dr, officeCode, areaCode);//, Course, newCourse);
            }
        }

        public void insertEvent1(string FileName, string officeCode, string areaCode)//, string Course, string newCourse)
        {
            string strProvider = string.Empty;

            var verExcel = ExcelFileType(FileName);

            switch (verExcel)
            {
                case (-2): throw new Exception(FileName + "의 형식검사중 오류가 발생하였습니다.");
                case (-1): throw new Exception(FileName + "은 엑셀 파일형식이 아닙니다.");
                case (0):
                    // 확장명 XLS (Excel 97~2003 용)
                    strProvider = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" + FileName + "'; Extended Properties='Excel 8.0';");
                    break;
                case (1):
                    // 확장명 XLSX (Excel 2007 이상용)
                    strProvider = string.Format("Provider=Microsoft.ACE.OLEDB.12.0; Data Source='" + FileName + "'; Extended Properties='Excel 12.0';");
                    break;
            }

            OleDbConnection oleDBCon = null;
            OleDbCommand oleDBCom = null;
            OleDbDataReader oleDBReader = null;

            DataTable dt = new DataTable();
            List<string> list = new List<string>();

            string strQuery = "SELECT * FROM ";

            switch(officeCode)
            {
                case "TMT":
                    strQuery += "[구매고객리스트$]";
                    break;
                case "WMP":
                    strQuery += "[주문내역$]";
                    break;
            }

            //oleDBCon = new OleDbConnection(strProvider);
            //oleDBCom = new OleDbCommand(strQuery, oleDBCon);

            //try
            //{
            //    oleDBCon.Open();
            //    oleDBReader = oleDBCom.ExecuteReader(CommandBehavior.CloseConnection);
            //    dt.Load(oleDBReader); 
            //}
            //catch (Exception err) 
            //{ 
            //    Debug.WriteLine(err.Message); 
            //}

            //Console.WriteLine(strProvider);

            oleDBCon = new OleDbConnection(strProvider);
            oleDBCom = new OleDbCommand(strQuery, oleDBCon);

            try
            {
                oleDBCon.Open();
                oleDBReader = oleDBCom.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(oleDBReader);

                oleDBReader.Close();
                oleDBReader.Dispose();
                oleDBCon.Close();
            }
            catch (Exception e)
            {
                //Content("<script>alert(" + e.Message + ")</script>");
                //System.out.println("0보다 작습니다.");
            }
            finally
            {
            }

            foreach (DataRow dr in dt.Rows)
            {
                udtEvent(dr, officeCode, areaCode);//, Course, newCourse);
                //updateCustomer(dr, officeCode, areaCode);
            }
        }

        //public void updateCustomer(DataRow dr, string officeCode, string areaCode)//string date, int csSeq, int evSeq, string type)
        //{
        //    string txtDate = Regex.Replace(date, @"\D", "");

        //    var csData = DB.COU_0.Where(w => w.COU_DATE.Equals(txtDate))
        //                         .Where(w => w.COU_SEQ == csSeq)
        //                         .FirstOrDefault();

        //    int cnt = Convert.ToInt32(csData.COU_CNT.ToString());

        //    string evDate = txtDate.Substring(0, 4) + "-" + txtDate.Substring(4, 2) + "-" + txtDate.Substring(6, 2);

        //    var evData = DB.EVE_0.Where(w => w.EVE_DATE.Equals(evDate))
        //                         .Where(w => w.EVE_SEQ == evSeq)
        //        //.Select(s => new
        //        //{
        //        //    s.EVE_NAME,
        //        //    s.EVE_CNT
        //        //})
        //                         .FirstOrDefault();

        //    if (type == "+")
        //    {
        //        cnt += Convert.ToInt32(evData.EVE_CNT.ToString());

        //        COU_1 Data = new COU_1();

        //        Data.CORP_CODE = "NTB";
        //        Data.COU_DATE = evDate;
        //        Data.COU_SEQ = csSeq;
        //        Data.EVE_SEQ = evSeq;
        //        Data.EVE_NAME = evData.EVE_NAME;
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
        //                            .Where(w => w.COU_SEQ == csSeq)
        //                            .Where(w => w.EVE_SEQ == evSeq)
        //                            .FirstOrDefault();

        //        DB.COU_1.Remove(Data);

        //        evData.EVE_SET = "N";
        //    }

        //    csData.COU_CNT = cnt;

        //    DB.SaveChanges();
        //}

        public static int ExcelFileType(string XlsFile)
        {
            byte[,] ExcelHeader = {
                { 0xD0, 0xCF, 0x11, 0xE0, 0xA1 }, // XLS  File Header
                { 0x50, 0x4B, 0x03, 0x04, 0x14 }  // XLSX File Header
            };
            // result -2=error, -1=not excel , 0=xls , 1=xlsx
            int result = -1;
            FileInfo FI = new FileInfo(XlsFile);
            FileStream FS = FI.Open(FileMode.Open);
            try
            {
                byte[] FH = new byte[5];
                FS.Read(FH, 0, 5);
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (FH[j] != ExcelHeader[i, j]) break;
                        else if (j == 4) result = i;
                    }
                    if (result >= 0) break;
                }
            }
            catch (Exception e)
            {
                result = (-2);
                //throw e;
            }
            finally
            {
                FS.Close();
            }
            return result;
        }

        public void udtEvent(DataRow dr, string officeCode, string areaCode)//, string Course, string newCourse)
        {
            //string accDay = dr.ItemArray[0].ToString(),
            //       nowDate = DateTime.Now.ToString("yyyyMMdd"),
            //       ACC_NUMBER = dr.ItemArray[1].ToString();

            string corp_code = "NTB",
                   date = "",
                   name = "",
                   tel = "",
                   email = "",
                   office = officeCode,
                   etc = "",
                   area = areaCode,
                   title = "",
                   officeSeq = "",
                   regDate = "",
                   price = "";

            int SEQ = 1,
                cnt = 1;

            DateTime Now = DateTime.Now;

            try { 

                switch(officeCode)
                {
                    case "TMT":
                        officeSeq = dr.ItemArray[1].ToString();

                        date = Regex.Replace(dr.ItemArray[15].ToString(), @"\D", "");

                        switch (date.Length)
                        {
                            case 8:
                                date = string.IsNullOrEmpty(date) ? date : date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                                break;
                            case 6:
                                date = string.IsNullOrEmpty(date) ? date : "20" + date.Substring(0, 2) + "-" + date.Substring(2, 2) + "-" + date.Substring(4, 2);
                                break;
                            case 4:
                                date = string.IsNullOrEmpty(date) ? date : Now.Year.ToString() + "-" + date.Substring(0, 2) + "-" + date.Substring(2, 2);
                                break;
                            default:
                                return;
                        }

                        regDate = dr.ItemArray[12].ToString().Split(' ')[0].Replace('.', '-');
                        name = string.IsNullOrEmpty(dr.ItemArray[5].ToString()) ? dr.ItemArray[2].ToString() : dr.ItemArray[5].ToString();
                        tel = string.IsNullOrEmpty(dr.ItemArray[6].ToString()) ? dr.ItemArray[3].ToString() : dr.ItemArray[6].ToString();
                        email = string.IsNullOrEmpty(dr.ItemArray[7].ToString()) ? dr.ItemArray[4].ToString() : dr.ItemArray[7].ToString();
                        title = dr.ItemArray[14].ToString().Length <= 50 ? dr.ItemArray[14].ToString() : dr.ItemArray[14].ToString().Substring(0, 50);//.Substring(0, 100);
                        title = title.Split('.')[1].Split('|')[0].ToString();
                        etc = dr.ItemArray[13].ToString();
                        price = dr.ItemArray[9].ToString();
                        break;

                    case "WMP":
                        officeSeq = dr.ItemArray[0].ToString();

                        if (string.IsNullOrEmpty(officeSeq))
                        {
                            return;
                        }

                        // 구매자 MID 가 생김 한칸씩 뒤로 밀기

                        regDate = dr.ItemArray[7].ToString().Split(' ')[0].Replace('.', '-');
                        name = string.IsNullOrEmpty(dr.ItemArray[11].ToString()) || dr.ItemArray[11].ToString().Equals("-") ? dr.ItemArray[2].ToString() : dr.ItemArray[11].ToString();
                        tel = string.IsNullOrEmpty(dr.ItemArray[12].ToString()) || dr.ItemArray[12].ToString().Equals("-") ? dr.ItemArray[3].ToString() : dr.ItemArray[12].ToString();
                        email = dr.ItemArray[4].ToString();

                        title = dr.ItemArray[14].ToString().Length <= 50 ? dr.ItemArray[14].ToString() : dr.ItemArray[14].ToString().Substring(0, 50);//.Split('^')[1];
                        title = title.Split(':')[1].Split('^')[0].ToString();

                        //date = Regex.Replace(dr.ItemArray[13].ToString().Split('^')[2].Split(':')[0], @"\D", "");

                        //string pattern = @"(\d{2}[/.-]\d{2})|(\d{4}[/.-]\d{2}[/.-]\d{2})";
                        
                        //Regex pattern = new Regex(@"(\d{2}[/.-]\d{2})|(\d{4}[/.-]\d{2}[/.-]\d{2})");
                        Regex pattern = new Regex(@"(\d{1,2}[/.-]\d{1,2})|(\d{4}[/.-]\d{1,2}[/.-]\d{1,2})");

                        //date = Regex.Replace(Regex.Replace(dr.ItemArray[13].ToString(), pattern, "")[1].ToString(), @"\D", "");

                        string aa1 = dr.ItemArray[14].ToString(),
                               aa2 = "", //pattern.Matches(aa1)[1].ToString(),
                               aa3 = ""; //Regex.Replace(aa2, @"\D", "");

                        try
                        {
                            aa2 = pattern.Matches(aa1)[1].ToString();
                        }
                        catch
                        {
                            aa2 = pattern.Matches(aa1)[0].ToString();
                        }

                        aa3 = Regex.Replace(aa2, @"\D", "");

                        date = aa3;

                        switch (date.Length)
                        {
                            case 8:
                                date = string.IsNullOrEmpty(date) ? date : date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);
                                break;
                            case 6:
                                date = string.IsNullOrEmpty(date) ? date : "20" + date.Substring(0, 2) + "-" + date.Substring(2, 2) + "-" + date.Substring(4, 2);
                                break;
                            case 4:
                                date = string.IsNullOrEmpty(date) ? date : Now.Year.ToString() + "-" + date.Substring(0, 2) + "-" + date.Substring(2, 2);
                                break;
                            default:
                                return;
                        }

                        price = dr.ItemArray[15].ToString();

                        try
                        {
                            etc = dr.ItemArray[17].ToString();
                        }
                        catch
                        {
                            etc = "";
                        }

                        break;

                    case "MRT":
                        officeSeq = dr.ItemArray[0].ToString();

                        if (string.IsNullOrEmpty(officeSeq))
                        {
                            return;
                        }

                        date = dr.ItemArray[4].ToString();
                        regDate = dr.ItemArray[5].ToString().Split(' ')[0].Replace('.', '-');
                        tel = dr.ItemArray[11].ToString();
                        title = dr.ItemArray[2].ToString().Length <= 50 ? dr.ItemArray[2].ToString() : dr.ItemArray[2].ToString().Substring(0, 50);//.Substring(0, 100);
                        etc = dr.ItemArray[10].ToString();

                        cnt = Convert.ToInt32(dr.ItemArray[6].ToString());

                        name = dr.ItemArray[9].ToString().Split(' ')[0].ToString();

                        break;
                }
            }
            catch (Exception e)
            {
                return;
            }

            if (string.IsNullOrEmpty(officeSeq))
            {
                return;
            }

            var seqData = DB.EVE_0.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.EVE_REG_DATE.Equals(regDate))
                                  .GroupBy(g => new { g.EVE_REG_DATE })
                                  .Select(s => new { EVE_SEQ = s.Max(max => max.EVE_SEQ) });

            try
            {
                SEQ = Convert.ToInt32(seqData.Single().EVE_SEQ.ToString()) + 1;
            }
            catch (Exception)
            {

            }

            var chkData = DB.EVE_0.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.EVE_DATE.Equals(date))
                                  .Where(w => w.EVE_REG_DATE.Equals(regDate))
                                  .Where(w => w.EVE_NAME.Equals(name))
                                  .Where(w => w.EVE_TEL.Equals(tel))
                                  .Where(w => w.EVE_EMAIL.Equals(email))
                                  .Where(w => w.EVE_TITLE.Equals(title));

            if (chkData.Any())
            {
                chkData.Single().EVE_CNT += 1;
                chkData.Single().EVE_PRICE += Convert.ToInt32(price);

                SEQ -= 1;
            }
            else
            {
                EVE_0 Data = new EVE_0();

                Data.CORP_CODE = "NTB";
                Data.EVE_REG_DATE = regDate;
                Data.EVE_SEQ = SEQ;
                Data.EVE_NAME = name;
                Data.EVE_TEL = tel;
                Data.EVE_EMAIL = string.IsNullOrEmpty(email) ? "-" : email;
                Data.OFFICE_SEQ = officeSeq;
                Data.EVE_CNT = cnt;
                Data.EVE_ETC = string.IsNullOrEmpty(etc) ? "-" : etc;
                Data.EVE_AREA_CODE = areaCode;
                Data.EVE_TITLE = title;
                Data.EVE_DATE = date;
                Data.EVE_OFFICE_CODE = officeCode;
                Data.EVE_PRICE = Convert.ToInt32(price);

                var user = Session["user"] as User;

                DateTime now = DateTime.Now;

                string istDate = now.ToString("yyyy-MM-dd");

                Data.EVE_IST_DATE = regDate;
                Data.EVE_IST_EMP_NO = Convert.ToInt32(user.Login.ToString());

                DB.EVE_0.Add(Data);
            }

            DB.SaveChanges();

            //string AllNumdate = Regex.Replace(date.ToString(), @"\D", "");

            //var csData = DB.COU_0.Where(w => w.COU_DATE.Equals(AllNumdate))
            //                     .Where(w => w.COU_AREA_CODE.Equals(areaCode))
            //                     .Where(w => w.COU_TITLE.Equals(Course));

            ////if (AllNumdate == "20171207" && SEQ == 16)
            ////{
            ////    int a = 111;
            ////}

            //if (chkData.Any() && csData.Any())
            //{
            //    int csSeq = csData.FirstOrDefault().COU_SEQ;

            //    updateCustomer(AllNumdate, csSeq, regDate, SEQ, Course, areaCode, true);

            //    return;
            //}

            ////if (newCourse == "Y")
            ////{
            ////    int csSeq = insertCourse(AllNumdate, Course, areaCode);
            ////    updateCustomer(AllNumdate, csSeq, regDate, SEQ, Course, areaCode);
            ////}
            ////else if (newCourse == "N")
            ////{
            ////    int csSeq = 1;

            ////    if (!csData.Any())
            ////    {
            ////        csSeq = insertCourse(AllNumdate, Course, areaCode);
            ////    }
            ////    else
            ////    {
            ////        csSeq = csData.FirstOrDefault().COU_SEQ;
            ////    }

            ////    updateCustomer(AllNumdate, csSeq, regDate, SEQ, Course, areaCode);
            ////}
        }

        public void deleteEvent(string date, int seq)
        {
            var Data = DB.EVE_0.Where(w => w.EVE_REG_DATE.Equals(date))
                               .Where(w => w.EVE_SEQ == seq)
                               .FirstOrDefault();

            DB.EVE_0.Remove(Data);
            DB.SaveChanges();
        }

        public void deleteCourse(string date, int seq)
        {
            var Data = DB.COU_2.Where(w => w.EVE_REG_DATE.Equals(date))
                               .Where(w => w.EVE_SEQ == seq)
                               .FirstOrDefault();
            try
            {
                DB.COU_2.Remove(Data);

                int cnt = Convert.ToInt32(Data.EVE_CNT.ToString()),
                    couSeq = Convert.ToInt32(Data.COU_SEQ.ToString());

                string couDate = Data.COU_DATE.ToString();

                var course = DB.COU_1.Where(w => w.COU_DATE.Equals(couDate));

                course = course.Where(w => w.COU_SEQ == couSeq);

                var couData = course.FirstOrDefault();

                int maxCnt = Convert.ToInt32(couData.COU_CNT.ToString());

                couData.COU_CNT = maxCnt - cnt;

                if (couData.COU_CNT == 0)
                {
                    DB.COU_1.Remove(couData);
                }

                DB.SaveChanges();
            }
            catch
            {

            }
        }

        public void updateEvent(string data)
        {
            string regDate = "",
                   seq = "",
                   sDate = "",
                   area = "",
                   title = "",
                   cnt = "",
                   tel = "",
                   mail = "",
                   etc = "";

            string[] datas = data.Split(',');

            regDate = datas[0];
            seq     = datas[1];
            sDate   = datas[2];
            area    = datas[3];
            title   = datas[4];
            cnt     = datas[5];
            tel     = datas[6];
            mail    = datas[7];
            etc     = datas[8];

            DateTime now = DateTime.Now;

            string udtDate = now.ToString("yyyy-MM-dd");

            var user = Session["user"] as User;

            var eveData = DB.EVE_0.Where(w => w.EVE_REG_DATE.Equals(regDate))
                                  .Where(w => w.EVE_SEQ.ToString().Equals(seq))
                                  .FirstOrDefault();

            eveData.EVE_DATE = sDate;
            eveData.EVE_AREA_CODE = area;
            eveData.EVE_TITLE = title;
            eveData.EVE_CNT = Convert.ToInt32(cnt);
            eveData.EVE_TEL = tel;
            eveData.EVE_EMAIL = mail;
            eveData.EVE_ETC = etc;
            eveData.EVE_UDT_DATE = udtDate;
            eveData.EVE_UDT_EMP_NO = Convert.ToInt32(user.Login.ToString());

            deleteCourse(regDate, Convert.ToInt32(seq));

            //var csData = DB.COU_1.Where(w => w.COU_DATE.Equals(sDate))
            //                     .Where(w => w.COU_TITLE.Equals(couData.FirstOrDefault().COU_TITLE))
            //                     .Where(w => w.COU_AREA_CODE.Equals(couData.FirstOrDefault().COU_AREA_CODE))
            //                     .Where(w => w.COU_SEQ == couData.FirstOrDefault().COU_SEQ);

            //int csSeq = 1;

            //if (csData.Any())
            //{
            //    csSeq = csData.FirstOrDefault().COU_SUB_NUM;
            //}
            //else
            //{
            //    csSeq = insertCourse(couData.FirstOrDefault().COU_SEQ.ToString(), sDate, couData.FirstOrDefault().COU_TITLE, couData.FirstOrDefault().COU_AREA_CODE);
            //}

            //updateCustomer(couData.FirstOrDefault().COU_SEQ.ToString(), sDate, csSeq, regDate, eveData.EVE_SEQ, couData.FirstOrDefault().COU_TITLE, couData.FirstOrDefault().COU_AREA_CODE);

            DB.SaveChanges();
        }

        public int insertCourse(string seq, string date, string title, string area)
        {
            var couData = DB.COU_1.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.COU_SEQ.ToString().Equals(seq))
                                  .Where(w => w.COU_DATE.Equals(date))
                                  .Where(w => w.COU_TITLE.Equals(title))
                                  .Where(w => w.COU_AREA_CODE.Equals(area));

            int couCnt = 0;

            if (couData.Any())
            {
                couCnt = couData.GroupBy(g => new { g.COU_SEQ, g.COU_DATE, g.COU_TITLE, g.COU_AREA_CODE })
                                .Select(s => new
                                {
                                    seq = s.Max(max => max.COU_SEQ)
                                })
                                .FirstOrDefault()
                                .seq;
            }

            int idx = 1;

            try { idx = couCnt + 1; }
            catch { }

            var user = Session["user"] as User;

            COU_1 Data = new COU_1();

            Data.CORP_CODE = "NTB";
            Data.COU_SEQ = Convert.ToInt32(seq);
            Data.COU_SUB_NUM = idx;
            Data.COU_DATE = date;
            Data.COU_TITLE = title;
            Data.COU_AREA_CODE = area;
            Data.COU_CNT = 0;
            //Data.COU_EMP_NO = Convert.ToInt32(user.Login.ToString());
            Data.COU_NOTE = "";

            DB.COU_1.Add(Data);

            DB.SaveChanges();

            return idx;
        }

        public void updateCustomer(string seq, string date, int csSeq, string regDate, int evSeq, string title, string area, bool overlap = false)
        {
            var couData = DB.COU_1.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Where(w => w.COU_SEQ.ToString().Equals(seq))
                                  .Where(w => w.COU_DATE.Equals(date))
                                  .Where(w => w.COU_TITLE.Equals(title))
                                  .Where(w => w.COU_AREA_CODE.Equals(area))
                                  .Where(w => w.COU_SUB_NUM.ToString().Equals(csSeq.ToString()))
                                  .FirstOrDefault();

            int cnt = Convert.ToInt32(couData.COU_CNT.ToString());

            var evData = DB.EVE_0.Where(w => w.EVE_REG_DATE.Equals(regDate))
                                 .Where(w => w.EVE_SEQ == evSeq)
                                 .FirstOrDefault();

            if (evData == null)
                return;

            if (overlap)
            {
                cnt += 1;
            }
            else
            {
                cnt += Convert.ToInt32(evData.EVE_CNT.ToString());
            }

            string name = evData.EVE_NAME;

            var csData2 = DB.COU_2.Where(w => w.COU_DATE.Equals(date))
                                  .Where(w => w.COU_SEQ == csSeq)
                                  .Where(w => w.COU_TITLE.Equals(title))
                                  .Where(w => w.EVE_NAME.Equals(name))
                                  .Where(w => w.EVE_SEQ == evSeq);

            if (csData2.Any())
            {
                csData2.Single().EVE_CNT = evData.EVE_CNT;
            }
            else
            {
                COU_2 Data = new COU_2();

                Data.CORP_CODE = "NTB";
                Data.COU_DATE = date;
                Data.COU_SEQ = Convert.ToInt32(seq);
                Data.COU_TITLE = title;
                Data.COU_SUB_NUM = csSeq;
                Data.EVE_SEQ = evSeq;
                Data.EVE_REG_DATE = regDate;
                Data.EVE_NAME = evData.EVE_NAME;
                Data.EVE_CNT = evData.EVE_CNT;
                Data.EVE_OFFICE_CODE = evData.EVE_OFFICE_CODE;
                Data.EVE_TEL = evData.EVE_TEL;

                DB.COU_2.Add(Data);
            }

            evData.EVE_SET = "Y";

            couData.COU_CNT = cnt;

            DB.SaveChanges();
        }

        //public JsonResult JSON_senderMail(string data)
        //{
        //    string[] keys = data.Split(';');

        //    foreach (string i in keys)
        //    {
        //        if (i == "")
        //        {
        //            continue;
        //        }

        //        sendVoucher(i);
        //    }

        //    return Json("바우처 전송을 완료 했습니다.");
        //}

        public JsonResult JSON_getCourse(string area)
        {
            var Data = DB.COU_0.Where(w => w.COU_AREA_CODE.Equals(area))
                               .GroupBy(g => new { g.COU_SEQ, g.COU_TITLE })
                               .Select(s => new
                               {
                                   s.Key.COU_TITLE,
                                   s.Key.COU_SEQ,
                               });

            return Json(Data);
        }

        public JsonResult JSON_getCourseData(string seq, string area)
        {
            var Data = DB.COU_0.Where(w => w.COU_SEQ.ToString().Equals(seq))
                               .Where(w => w.COU_AREA_CODE.Equals(area))
                               .Select(s => new
                               {
                                   s.COU_TITLE,
                                   s.COU_ADDRESS,
                                   s.COU_LATITUDE,
                                   s.COU_LONGITUDE,
                                   s.COU_CONT,
                                   s.COU_attachment
                               });

            return Json(Data);
        }

        public JsonResult JSON_updateEvent(string data)
        {
            string[] keys = data.Split(';');

            foreach(string i in keys)
            {
                if (i == "")
                {
                    continue;
                }

                updateEvent(i);
            }

            return Json("고객 정보 수정을 완료 했습니다.");
        }

        public JsonResult JSON_deleteEvent(string data)
        {
            string[] keys = data.Split(',');

            foreach(string i in keys)
            {
                if (i == "")
                {
                    continue;
                }

                string date = i.Split('_')[0].ToString();
                int seq = Convert.ToInt32(i.Split('_')[1].ToString());

                deleteCourse(date, seq);
                deleteEvent(date, seq);
            }

            return Json("고객 정보 삭제를 완료 했습니다.");
        }


        public string sendVoucher(string data, string area, string course)//, string lat = "", string lng = "", string address = "", string txt = "", List<string> attach = null)
        {
            string regDate = "",
                   seq = "",
                   sDate = "",
                   title = "",
                   cnt = "",
                   tel = "",
                   mail = "";

            string[] datas = data.Split(',');

            regDate = datas[0];
            seq = datas[1];

            var eveData = DB.EVE_0.Where(w => w.EVE_REG_DATE.Equals(regDate))
                                  .Where(w => w.EVE_SEQ.ToString().Equals(seq))
                                  .FirstOrDefault();

            sDate = eveData.EVE_DATE.ToString();
            title = eveData.EVE_TITLE.ToString();
            cnt = eveData.EVE_CNT.ToString();
            tel = eveData.EVE_TEL.ToString();
            mail = eveData.EVE_EMAIL.ToString();

            string _senderID = "helper@ndaytrip.com";
            string _senderName = "엔데이트립";

            string name = eveData.EVE_NAME;

            int cusInt = (Convert.ToInt32(cnt) - 1);

            string cusTxt = "대표자 외 " + cusInt + "명";

            if (cusInt < 1)
            {
                cusTxt = "대표자 1명";
            }

            var _title = "[엔데이트립] " + title + "바우처입니다.";

            //var _body = "<div style='position: relative; width: 500px; height: 728px; background: url(http://admin.ntabi.kr/images/ticket.jpg) no-repeat; background-position: 50% 20px;'>" + 
            //                "<h3 style='font-size: 20px; font-weight: bold; height: 22px; line-height: 22px; overflow: hidden; margin: 0; padding: 105px 45px 0;'>" +
            //                    title + 
            //                "</h3>" + 
            //                "<p style='font-size: 16px; font-weight: bold;height: 20px; line-height: 20px; overflow: hidden; margin: 0; padding: 40px 70px 0; display: inline-block;'>" +
            //                    name + 
            //                "</p>" + 
            //                "<p style='font-size: 16px; font-weight: bold; height: 20px; line-height: 20px; overflow: hidden; margin: 0; padding: 40px 0 0 95px; display: inline-block;'>" +
            //                    tel + 
            //                "</p>" + 
            //                "<p style='font-size: 20px; font-weight: bold; height: 22px; line-height: 22px; overflow: hidden; margin: 0; padding: 65px 0 0 290px; display: inline-block;'>" +
            //                    sDate.Substring(0, 4) + "년 " + sDate.Substring(5, 2) + "월 " + sDate.Substring(8, 2) + "일" +
            //                "</p>" + 
            //                "<p style='font-size: 18px; font-weight: bold; height: 22px; line-height: 22px; margin: 0; padding: 75px 0 0 70px; display: inline-block; float: left;'>" +
            //                    cusTxt + "<br>" + 
            //                    //"<label style='font-size:  13px; margin-top: 20px; display: block; font-weight: normal;'>" + 
            //                    //    "성인 3명, 소아 2명, 유아 1명" + 
            //                    //"</label>" + 
            //                "</p>" +
            //                "<p style='font-size: 18px; font-weight: bold; height: 22px; line-height: 22px; margin: 0; padding: 75px 40px; display: inline-block; float: right;'>" +
            //                    //"<a href='https://www.google.co.kr/maps/@" + lat + "," + lng + ",18z?hl=ko' style='color: black; text-decoration: none;'>" +
            //                    //"<a href='https://www.google.co.kr/maps/dir//" + lat + "," + lng + "/@" + lat + "," + lng + ",19z' style='color: black; text-decoration: none;'>" +
            //                    "<a href='http://ndaytrip.com/map?lat=" + lat + "&lng=" + lng + "&z=18' style='color: black; text-decoration: none;'>" +
            //                        address +
            //                    "</a>" +
            //                "</p>" +
            //                "<p style='font-size: 16px; line-height: 20px; margin-top: 180px; padding: 75px 60px 0; display: block;'>" +
            //                    txt.Replace(System.Environment.NewLine, "<br />") + //.ToString().replace("\n", "<br>") +
            //                "</p>" +
            //            "</div>";

            var couData = DB.COU_0.Where(w => w.COU_AREA_CODE.Equals(area))
                                    .Where(w => w.COU_SEQ.ToString().Equals(course))
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

            string[] attachText;

            if (couData.Any())
            {
                attachText = couData.FirstOrDefault().COU_attachment.Split(';');
            }
            else
            {
                return "<script>alert('코스 정보가 정확하지 않습니다.'); location.href='/Event'</script>";
            }

            List<string> attachPath = new List<string>();

            for (int i = 0; i < attachText.Length; i++)
            {
                if (string.IsNullOrEmpty(attachText[i]))
                    continue;

                attachPath.Add(attachText[i]);
            }

            var _body = "<div style='background: url(http://admin.ntabi.kr/images/ticket.jpg) no-repeat; height: 340px; width: 720px; padding-top: 60px; padding-left: 30px; position: relative;'>" + 
                            "<div style='width: 282px; display: inline-block;'>" +
                                "<h5 style='margin: 0; font-size: 20px; color: #2a2a2a; width: 267px; line-height: 30px; height: 60px; overflow: hidden;'>" + title + "</h5>" +
                                "<p style='font-size: 16px; color: #2a2a2a; margin: 0; margin-left: 80px; height: 38px; line-height: 42px; padding-left: 20px;'>" + name + "</p>" +
                                "<p style='font-size: 16px; color: #2a2a2a; margin: 0; margin-left: 80px; height: 38px; line-height: 42px; padding-left: 20px;'>" + tel + "</p>" +
                                "<p style='font-size: 16px; color: #2a2a2a; margin: 0; margin-left: 80px; height: 38px; line-height: 42px; padding-left: 20px;'>" + sDate.Substring(0, 4) + "년 " + sDate.Substring(5, 2) + "월 " + sDate.Substring(8, 2) + "일" + "</p>" +
                                "<p style='font-size: 16px; color: #2a2a2a; margin: 0; margin-left: 80px; height: 38px; line-height: 42px; padding-left: 20px;'>" + cusTxt + "</p>" +
                                "<p style='font-size: 16px; color: #2a2a2a; margin: 0; margin-left: 80px; height: 38px; line-height: 42px; padding-left: 20px;'>" +
                                    "<a href='http://ndaytrip.com/map?lat=" + couData.FirstOrDefault().COU_LATITUDE + "&lng=" + couData.FirstOrDefault().COU_LONGITUDE + "&z=18' target='_blank'>" + couData.FirstOrDefault().COU_ADDRESS + "</a>" + 
                                "</p>" + 
                            "</div>" +
                            "<div style='overflow: auto; padding: 20px; width: 315px; height: 250px; float: right; margin-right: 70px; margin-top: -32px;'>" + couData.FirstOrDefault().COU_CONT.Replace(System.Environment.NewLine, "<br />") + "</div>" + 
                        "</div>";

            string toMail = mail;// "hj@ntabi.co.kr";

            try
            {
                MailMessage _message = new MailMessage();
                _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                _message.To.Add(toMail);
                _message.Subject = _title;
                _message.SubjectEncoding = System.Text.Encoding.UTF8;
                _message.Body = _body;
                _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                if (attachPath != null)
                {
                    foreach (string attStr in attachPath)
                    {
                        string fileNames = Path.GetFileName(attStr);

                        var path = Path.Combine(Server.MapPath("~/attachment"), fileNames);

                        _message.Attachments.Add(new Attachment(path));
                    }
                }

                SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                server.EnableSsl = true;
                server.UseDefaultCredentials = false;
                server.DeliveryMethod = SmtpDeliveryMethod.Network;
                server.Credentials = new System.Net.NetworkCredential("ntabinday", "ntabi4605");
                server.Send(_message);
            }
            catch (ArgumentException error)
            {
                ViewBag.err = error;
            }

            if (eveData.EVE_CHKMAIL != "Y")
            {
                eveData.EVE_CHKMAIL = "Y";

                DB.SaveChanges();

                var csData = DB.COU_1.Where(w => w.COU_DATE.Equals(sDate))
                                     .Where(w => w.COU_TITLE.Equals(couData.FirstOrDefault().COU_TITLE))
                                     .Where(w => w.COU_AREA_CODE.Equals(couData.FirstOrDefault().COU_AREA_CODE))
                                     .Where(w => w.COU_SEQ == couData.FirstOrDefault().COU_SEQ);

                int csSeq = 1;

                if (csData.Any())
                {
                    csSeq = csData.FirstOrDefault().COU_SUB_NUM;
                }
                else
                {
                    csSeq = insertCourse(couData.FirstOrDefault().COU_SEQ.ToString(), sDate, couData.FirstOrDefault().COU_TITLE, couData.FirstOrDefault().COU_AREA_CODE);
                }

                updateCustomer(couData.FirstOrDefault().COU_SEQ.ToString(), sDate, csSeq, regDate, eveData.EVE_SEQ, couData.FirstOrDefault().COU_TITLE, couData.FirstOrDefault().COU_AREA_CODE);
            }

            return "메일 발송 완료";
        }
    }
}