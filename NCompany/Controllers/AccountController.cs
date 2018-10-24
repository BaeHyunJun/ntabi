using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using NCompany.Models;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.IO;

namespace NCompany.Controllers
{
    public class AccountController : Controller
    {
        DataBase DB = new DataBase();

        public string getBank(string code)
        {
            string name = "";

            name = DB.BANK.Where(w => w.BANK_CODE.Equals(code)).Single().BANK_NAME;

            return name;
        }

        public string getName(int emp)
        {
            string name = "";

            name = DB.EMP_0.Where(w => w.EMP_NO == emp).Single().EMP_NAME;

            return name;
        }

        public ActionResult Index()
        {
            var accData = DB.ACC_0.Select(s => new
                                  {
                                      ACC_DAY = s.ACC_DAY.Substring(0, 4) + "-" + s.ACC_DAY.Substring(4, 2) + "-" + s.ACC_DAY.Substring(6, 2),
                                      s.ACC_SEQ,
                                      s.ACC_SUB_SEQ,
                                      s.ACC_BANK_CODE,
                                      s.ACC_NUMBER,
                                      s.ACC_FIRST_PRICE,
                                      s.ACC_PRICE,
                                      s.ACC_NAME,
                                      ACC_IST_DAY = s.ACC_IST_DAY.Substring(0, 4) + "-" + s.ACC_IST_DAY.Substring(4, 2) + "-" + s.ACC_IST_DAY.Substring(6, 2),
                                      s.ACC_IST_EMP_NO,
                                      s.ACC_EMP_NO,
                                      s.ACC_CONTENT,
                                  });

            return View(accData);
        }

        public ActionResult Uploader()
        {
            var bnkData = DB.BNK_0.Where(w => w.CORP_CODE.Equals("NTB"))
                                  .Select(s => new
                                  {
                                      s.BNK_CODE,
                                      s.BNK_NUMBER
                                  });

            return View(bnkData);
        }

        [HttpPost]
        public ActionResult File_Upload(FormCollection f, HttpPostedFileBase file)
        {
            string bnkCode = f["BANK_CODE"],
                   bnkNumber = f["BANK_NUMBER"];

            // Verify that the user selected a file
            if (file != null && file.ContentLength > 0)
            {
                // extract only the filename
                var fileName = Path.GetFileName(file.FileName);

                string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

                string fileNames = nowDt + "_" + fileName;

                // store the file inside ~/App_Data/uploads folder
                var path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileNames);

                file.SaveAs(path);

                insertAccount(path, bnkCode, bnkNumber);
            }
            // redirect back to the index action to show the form once again
            return RedirectToAction("Index"); 
        }

        public void insertAccount(string FileName, string BankCode, string BankNumber)
        {
            string strProvider = string.Empty;
            //string FileName = Server.MapPath("~/Content/file/") + "TermsData.xlsx";

            var verExcel = ExcelFileType(FileName);

            switch (verExcel)
            {
                case (-2): throw new Exception(FileName + "의 형식검사중 오류가 발생하였습니다.");
                case (-1): throw new Exception(FileName + "은 엑셀 파일형식이 아닙니다.");
                case (0):
                    // 확장명 XLS (Excel 97~2003 용)
                    strProvider = string.Format("Provider=Microsoft.Jet.OLEDB.4.0; Data Source='" + FileName + "'; Extended Properties='Excel 8.0';");
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

            string strQuery = "SELECT * FROM [Sheet1$]";

            try
            {
                oleDBCon = new OleDbConnection(strProvider);
                oleDBCom = new OleDbCommand(strQuery, oleDBCon);

                oleDBCon.Open();
                oleDBReader = oleDBCom.ExecuteReader(CommandBehavior.CloseConnection);
                dt.Load(oleDBReader);
            }
            catch (Exception e)
            {

            }
            finally
            {
                oleDBReader.Close();
                oleDBReader.Dispose();
                oleDBCon.Close();
            }

            var user = Session["user"] as User;

            var empData = DB.EMP_0.Where(w => w.EMP_ID.Equals(user.ID))
                                  .Select(s => new
                                  {
                                      s.EMP_NO
                                  });

            int emp_no = Convert.ToInt32(empData.Single().EMP_NO.ToString());

            foreach (DataRow dr in dt.Rows)
            {
                udtAccount(dr, BankCode, BankNumber, emp_no);
            }
        }

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

        public void udtAccount(DataRow dr, string BANK_CODE, string BANK_NUMBER, int emp)
        {
            string accDay = dr.ItemArray[0].ToString(),//.Substring(0, 4) + "-" + dr.ItemArray[0].ToString().Substring(4, 2) + "-" + dr.ItemArray[0].ToString().Substring(6, 2),
                   nowDate = DateTime.Now.ToString("yyyyMMdd"),
                   ACC_NUMBER = dr.ItemArray[1].ToString();

            int SEQ = 1;

            var chkData = DB.ACC_0.Where(w => w.ACC_DAY.Equals(accDay))
                                  .GroupBy(g => new { g.ACC_DAY })
                                  .Select(s => new { SEQ = s.Max(max => max.ACC_SEQ) });

            try
            {
                SEQ = Convert.ToInt32(chkData.Single().SEQ.ToString()) + 1;
            }
            catch(Exception)
            {

            }

            if (ACC_NUMBER != BANK_NUMBER)
            {
                return;
            }

            ACC_0 Data = new ACC_0();

            Data.CORP_CODE = "NTB";
            Data.ACC_DAY = accDay;
            Data.ACC_SEQ = SEQ;
            Data.ACC_SUB_SEQ = 1;
            Data.ACC_BANK_CODE = BANK_CODE;
            Data.ACC_NUMBER = ACC_NUMBER;
            Data.ACC_PRICE = Convert.ToInt32(dr.ItemArray[2].ToString().Replace(",", ""));
            Data.ACC_NAME = dr.ItemArray[3].ToString(); ;
            //Data.ACC_EMP_NO = 
            Data.ACC_IST_DAY = nowDate;
            Data.ACC_IST_EMP_NO = emp;
            Data.ACC_FIRST_PRICE = Convert.ToInt32(dr.ItemArray[2].ToString().Replace(",", ""));

            DB.ACC_0.Add(Data);

            DB.SaveChanges();
        }

        public JsonResult JSON_getBnkNumber(string code)
        {
            var Data = DB.BNK_0.Where(w => w.BNK_CODE.Equals(code))
                               .Select(s => new
                               {
                                   s.BNK_NUMBER
                               });

            return Json(Data);
        }
    }
}