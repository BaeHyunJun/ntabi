using NCompany.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class NaverController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB NDB = new NtabiDB();

        public string getAreaTxt(string code)
        {
            string txt = "";

            switch (code)
            {
                case "KYU":
                    txt = "규슈";
                    break;
                case "TOK":
                    txt = "도쿄";
                    break;
                case "OSA":
                    txt = "오사카";
                    break;
                case "HOK":
                    txt = "홋카이도";
                    break;
                case "TSU":
                    txt = "대마도";
                    break;
                case "OKI":
                    txt = "오키나와";
                    break;
                case "NGY":
                case "NGO":
                    txt = "나고야";
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
                default:
                    txt = code;
                    break;
            }

            return txt;
        }

        public string getTypeTxt(string code)
        {
            string txt = "";

            switch (code)
            {
                case "LT":
                    txt = "현지투어";
                    break;
                default:
                    txt = code;
                    break;
            }

            return txt;
        }

        // GET: Naver
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            //var uri = "http://ntabi.kr/naver/naver1.txt";

            //HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
            //HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            //var filePath = resp.GetResponseStream();

            string filePath = Path.Combine(Server.MapPath("~/naverPdt"), "naver.txt");

            DataTable dt = new DataTable();

            using (StreamReader rdr = new StreamReader(filePath))
            {
                dt = ConvertTabTexttoDataTable(filePath);
            }

            //resp.Dispose();

            var data = new List<naver>();

            int idx = 0;

            foreach (DataRow dr in dt.Rows)
            {
                var nData = new naver();

                idx++;

                nData = setData(idx, dr);

                data.Add(nData);
            }

            return View(data);
        }

        public ActionResult detPdt(string code = "", int i = 0)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (!string.IsNullOrEmpty(code))
            {
                string filePath = Path.Combine(Server.MapPath("~/naverPdt"), "naver.txt");

                DataTable dt = new DataTable();

                using (StreamReader rdr = new StreamReader(filePath))
                {
                    dt = ConvertTabTexttoDataTable(filePath);
                }

                var data = new List<naver>();

                int idx = 0;

                foreach (DataRow dr in dt.Rows)
                {
                    idx++;

                    if (!code.Equals(dr.ItemArray[0].ToString()) || i != idx)
                        continue;

                    var nData = new naver();

                    nData = setData(idx, dr);

                    data.Add(nData);
                }

                return View(data);
            }

            return View();
        }

        public ActionResult getProduct(string brand)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            if (brand.Equals("엔데이트립"))
            {
                var pdtData = DB.PDT_0.Where(w => w.PDT_PROC_CODE.Equals("02"))
                                      .Select(s => new
                                      {
                                          Code = s.CORP_CODE + s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ,
                                         Title = s.PDT_TITLE,
                                          key = s.PDT_IST_EMP_NO,
                                      });

                DateTime now = DateTime.Now;

                var chkDayLists = pdtData.Join(DB.PDT_1,
                                                   a => a.Code,
                                                   b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                   (a, b) => new
                                                   {
                                                       a.Code,
                                                       a.Title,
                                                       b.PDT_DATE,
                                                       a.key
                                                   }).ToList().Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                                   .GroupBy(g => new { g.Code, g.Title, g.key })
                                                   .Select(s => new
                                                   {
                                                       s.Key.Code,
                                                       s.Key.Title,
                                                       s.Key.key,
                                                   });

                return View(chkDayLists);
            }

            return View();
        }

        public ActionResult updatePdt(FormCollection f)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string filePath = Path.Combine(Server.MapPath("~/naverPdt"), "naver.txt");

            string id = f["id"],
                   price_pc = f["price_pc"],
                   link = f["link"],
                   image_link = f["image_link"],
                   category_name1 = f["category_name1"],
                   price_mobile = f["price_mobile"],
                   mobile_link = f["mobile_link"],
                   category_name2 = f["category_name2"],
                   category_name3 = f["category_name3"],
                   category_name4 = f["category_name4"],
                   brand = f["brand"],
                   pdtTitle = f["title"],
                   search_tag = f["search_tag"],
                   event_words = f["event_words"],
                   attribute = f["attribute"],
                   mode = f["mode"].ToString().ToUpper();

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string writeText = id + "\t" +
                               pdtTitle + "\t" +
                               price_pc + "\t" +
                               price_mobile + "\t\t" +
                               link + "\t" +
                               mobile_link + "\t" +
                               image_link + "\t\t" +
                               category_name1 + "\t" +
                               category_name2 + "\t" +
                               category_name3 + "\t" +
                               category_name4 + "\t" +
                               "50001645" + "\t\t\t\t\t\t\t\t\t\t\t\t" +
                               brand + "\t\t\t\t" +
                               event_words + "\t\t\t\t\t\t" +
                               search_tag.Replace(',', '|') + "\t\t\t\t\t\t" +
                               "0" + "\t\t\t" +
                               attribute.Replace(',', '^') + "\t\t\t\t\t" +
                               mode + "\t" +
                               now;

            if (mode == "I")
            {
                StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("euc-kr"));
                sw.WriteLine(writeText);
                sw.Close();
            } 
            else if (mode == "U")
            {
                int idx = Convert.ToInt32(f["idx"].ToString());

                StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("euc-kr"));

                List<string> lines = new List<string>();
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                sr.Close();

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("euc-kr"));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (i == idx)
                    {
                        sw.WriteLine(writeText);
                    }
                    else
                    {
                        sw.WriteLine(lines[i]);
                    }
                }

                sw.Close();
            } 
            else if (mode == "D")
            {
                int idx = Convert.ToInt32(f["idx"].ToString());

                StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("euc-kr"));

                List<string> lines = new List<string>();
                string line;

                while ((line = sr.ReadLine()) != null)
                {
                    lines.Add(line);
                }

                sr.Close();

                StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("euc-kr"));

                for (int i = 0; i < lines.Count; i++)
                {
                    if (i == idx)
                    {
                        continue;
                    }
                    else
                    {
                        sw.WriteLine(lines[i]);
                    }
                }

                sw.Close();
            }

            return Redirect("/Naver");
        }

        public JsonResult resetPdt()
        {
            string filePath = Path.Combine(Server.MapPath("~/naverPdt"), "naver.txt");

            DataTable dt = new DataTable();

            using (StreamReader rdr = new StreamReader(filePath))
            {
                dt = ConvertTabTexttoDataTable(filePath);
            }

            var data = new List<naver>();

            int idx = 0;

            foreach (DataRow dr in dt.Rows)
            {
                string pdtCode = dr.ItemArray[0].ToString(),
                       pdtTitle = dr.ItemArray[1].ToString();

                DateTime now = DateTime.Now;

                var chkData = DB.PDT_0.Where(w => pdtCode.Equals(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                                      .Where(w => w.PDT_TITLE.Equals(pdtTitle))
                                      .Where(w => w.PDT_PROC_CODE.Equals("02"))
                                      .Select(s => new
                                      {
                                          Code = pdtCode,
                                          Title = pdtTitle,
                                          key = s.PDT_IST_EMP_NO,
                                      });

                var chkDayLists = chkData.Join(DB.PDT_1,
                                               a => a.Code,
                                               b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                               (a, b) => new
                                               {
                                                   a.Code,
                                                   a.Title,
                                                   b.PDT_DATE,
                                                   a.key
                                               }).ToList().Where(w => DateTime.ParseExact(w.PDT_DATE, "yyyy-MM-dd", CultureInfo.InvariantCulture) > now)
                                               .GroupBy(g => new { g.Code, g.Title, g.key });

                if (!chkDayLists.Any())
                {
                    // 삭제 상품 백업시키기

                    continue;
                }

                var nData = new naver();

                nData = setData(idx, dr);

                data.Add(nData);
            }

            StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("euc-kr"));

            for (int i = 0; i < data.Count; i++)
            {
                sw.WriteLine(ConvertNaverToString(data[i]));
            }

            sw.Close();

            return Json("상품 업데이트 완료");
        }

        public JsonResult getPdtData(string code, string key)
        {
            string url = "ndaytrip.com/Products/Views?";

            var defaultLists = DB.PDT_0.Where(w => code.Equals(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                               .Where(w => w.PDT_IST_EMP_NO.ToString().Equals(key))
                               .ToList()
                               .Select(s => new
                               {
                                   Code = code,
                                   Title = s.PDT_TITLE,
                                   key,
                                   category1 = getNationTxt(s.PDT_NATION_CODE.ToString()),
                                   category2 = getAreaTxt(s.PDT_AREA_CODE.ToString()),
                                   category3 = getTypeTxt(s.PDT_TYPE_CODE),
                               });

            var pdt_ImgLists = defaultLists.Join(DB.PDT_2,
                                                a => a.Code,
                                                b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                                (a, b) => new
                                                {
                                                    a.Code,
                                                    a.Title,
                                                    b.PDT_IMG,
                                                    a.key,
                                                    a.category1,
                                                    a.category2,
                                                    a.category3,
                                                });

            var minPriceLists = DB.PRC_0.GroupBy(g => new { g.CORP_CODE, g.PDT_TYPE_CODE, g.PDT_IST_EMP_NO, g.PDT_YY, g.PDT_SEQ })
                                        .Select(s => new
                                        {
                                            Code = s.Key.CORP_CODE + s.Key.PDT_TYPE_CODE + s.Key.PDT_IST_EMP_NO + s.Key.PDT_YY + s.Key.PDT_SEQ,
                                            Currency = s.Min(min => min.PRC_Currency_CODE),
                                            Price = s.Min(min => min.PRC_Adult + min.PRC_Profit),
                                            key = s.Key.PDT_IST_EMP_NO,
                                        });

            var PdtLists = pdt_ImgLists.Join(minPriceLists,
                                            a => a.Code + a.key,
                                            b => b.Code + b.key,
                                            (a, b) => new
                                            {
                                                a.Code,
                                                a.Title,
                                                a.PDT_IMG,
                                                b.Currency,
                                                b.Price,
                                                a.key,
                                                link = "http://" + url + "code=" + a.Code + "&key=" + a.key,
                                                mobile_link = "http://" + "m." + url + "code=" + a.Code + "&key=" + a.key,
                                                category1 = a.category1,
                                                category2 = a.category2,
                                                category3 = a.category3,
                                                category4 = "",
                                            });

            var trfData = DB.TRF_0.Where(w => code.Equals(w.CORP_CODE + w.PDT_TYPE_CODE + w.PDT_IST_EMP_NO + w.PDT_YY + w.PDT_SEQ))
                                  .Select(s => new
                                  {
                                      code,
                                      key = s.PDT_IST_EMP_NO,
                                      s.TRF_TITLE
                                  });

            var resultLists = PdtLists.Join(trfData,
                                            a => a.Code + a.key,
                                            b => b.code + b.key,
                                            (a, b) => new
                                            {
                                                a.Code,
                                                a.Title,
                                                a.PDT_IMG,
                                                a.Currency,
                                                a.Price,
                                                a.key,
                                                a.link,
                                                a.mobile_link,
                                                a.category1,
                                                a.category2,
                                                a.category3,
                                                category4 = b.TRF_TITLE,
                                            });

            return Json(resultLists);
        }

        public static DataTable ConvertTabTexttoDataTable(string strFilePath)
        {
            StreamReader sr = new StreamReader(strFilePath, Encoding.GetEncoding("euc-kr"));
            string[] headers = sr.ReadLine().Split('\t');
            DataTable dt = new DataTable();

            foreach (string header in headers)
            {
                dt.Columns.Add(header);
            }

            while (!sr.EndOfStream)
            {
                string[] rows = sr.ReadLine().Split('\t');
                DataRow dr = dt.NewRow();
                for (int i = 0; i < headers.Length; i++)
                {
                    dr[i] = rows[i];
                }
                dt.Rows.Add(dr);
            }

            sr.Close();

            return dt;
        } 

        public string ConvertNaverToString(naver n)
        {
            string text = "";

            text = n.id + "\t" +
                   n.title + "\t" +
                   n.price_pc + "\t" +
                   n.price_mobile + "\t" +
                   n.normal_price + "\t" +
                   n.link + "\t" +
                   n.mobile_link + "\t" +
                   n.image_link + "\t" +
                   n.add_image_link + "\t" +
                   n.category_name1 + "\t" +
                   n.category_name2 + "\t" +
                   n.category_name3 + "\t" +
                   n.category_name4 + "\t" +
                   n.naver_category + "\t" +
                   n.naver_product_id + "\t" +
                   n.condition + "\t" +
                   n.import_flag + "\t" +
                   n.paraller_import + "\t" +
                   n.order_made + "\t" +
                   n.product_flag + "\t" +
                   n.adult + "\t" +
                   n.goods_type + "\t" +
                   n.barcode + "\t" +
                   n.manufacture_define_number + "\t" +
                   n.model_number + "\t" +
                   n.brand + "\t" +
                   n.maker + "\t" +
                   n.origin + "\t" +
                   n.card_event + "\t" +
                   n.event_words + "\t" +
                   n.coupon + "\t" +
                   n.partner_coupon_downlad + "\t" +
                   n.interest_free_event + "\t" +
                   n.point + "\t" +
                   n.installation_costs + "\t" +
                   n.search_tag + "\t" +
                   n.group_id + "\t" +
                   n.vendor_id + "\t" +
                   n.coordi_id + "\t" +
                   n.minimum_purchase_quantity + "\t" +
                   n.review_count + "\t" +
                   n.shipping + "\t" +
                   n.delivery_grade + "\t" +
                   n.delivery_detail + "\t" +
                   n.attribute + "\t" +
                   n.option_detail + "\t" +
                   n.seller_id + "\t" +
                   n.age_group + "\t" +
                   n.gender + "\t" +
                   n.classs + "\t" +
                   n.update_time;
            
            return text;
        }

        public naver setData(int idx, DataRow dr)
        {
            naver data = new naver();

            data.index                         = idx;

            data.id                            = !string.IsNullOrEmpty(dr.ItemArray[0].ToString()) ? dr.ItemArray[0].ToString() : "";
            data.title                         = !string.IsNullOrEmpty(dr.ItemArray[1].ToString()) ? dr.ItemArray[1].ToString() : "";
            data.price_pc                      = !string.IsNullOrEmpty(dr.ItemArray[2].ToString()) ? Convert.ToInt32(dr.ItemArray[2].ToString()) : 0;
            data.price_mobile                  = !string.IsNullOrEmpty(dr.ItemArray[3].ToString()) ? Convert.ToInt32(dr.ItemArray[3].ToString()) : 0;
            data.normal_price                  = !string.IsNullOrEmpty(dr.ItemArray[4].ToString()) ? Convert.ToInt32(dr.ItemArray[4].ToString()) : 0;
            data.link                          = !string.IsNullOrEmpty(dr.ItemArray[5].ToString()) ? dr.ItemArray[5].ToString() : "";
            data.mobile_link                   = !string.IsNullOrEmpty(dr.ItemArray[6].ToString()) ? dr.ItemArray[6].ToString() : "";
            data.image_link                    = !string.IsNullOrEmpty(dr.ItemArray[7].ToString()) ? dr.ItemArray[7].ToString() : "";
            data.add_image_link                = !string.IsNullOrEmpty(dr.ItemArray[8].ToString()) ? dr.ItemArray[8].ToString() : "";
            data.category_name1                = !string.IsNullOrEmpty(dr.ItemArray[9].ToString()) ? dr.ItemArray[9].ToString() : "";
            data.category_name2                = !string.IsNullOrEmpty(dr.ItemArray[10].ToString()) ? dr.ItemArray[10].ToString() : "";
            data.category_name3                = !string.IsNullOrEmpty(dr.ItemArray[11].ToString()) ? dr.ItemArray[11].ToString() : "";
            data.category_name4                = !string.IsNullOrEmpty(dr.ItemArray[12].ToString()) ? dr.ItemArray[12].ToString() : "";
            data.naver_category                = !string.IsNullOrEmpty(dr.ItemArray[13].ToString()) ? dr.ItemArray[13].ToString() : "";
            data.naver_product_id              = !string.IsNullOrEmpty(dr.ItemArray[14].ToString()) ? dr.ItemArray[14].ToString() : "";
            data.condition                     = !string.IsNullOrEmpty(dr.ItemArray[15].ToString()) ? dr.ItemArray[15].ToString() : "";
            data.import_flag                   = !string.IsNullOrEmpty(dr.ItemArray[16].ToString()) ? dr.ItemArray[16].ToString() : "";
            data.paraller_import               = !string.IsNullOrEmpty(dr.ItemArray[17].ToString()) ? dr.ItemArray[17].ToString() : "";
            data.order_made                    = !string.IsNullOrEmpty(dr.ItemArray[18].ToString()) ? dr.ItemArray[18].ToString() : "";
            data.product_flag                  = !string.IsNullOrEmpty(dr.ItemArray[19].ToString()) ? dr.ItemArray[19].ToString() : "";
            data.adult                         = !string.IsNullOrEmpty(dr.ItemArray[20].ToString()) ? dr.ItemArray[20].ToString() : "";
            data.goods_type                    = !string.IsNullOrEmpty(dr.ItemArray[21].ToString()) ? dr.ItemArray[21].ToString() : "";
            data.barcode                       = !string.IsNullOrEmpty(dr.ItemArray[22].ToString()) ? dr.ItemArray[22].ToString() : "";
            data.manufacture_define_number     = !string.IsNullOrEmpty(dr.ItemArray[23].ToString()) ? dr.ItemArray[23].ToString() : "";
            data.model_number                  = !string.IsNullOrEmpty(dr.ItemArray[24].ToString()) ? dr.ItemArray[24].ToString() : "";
            data.brand                         = !string.IsNullOrEmpty(dr.ItemArray[25].ToString()) ? dr.ItemArray[25].ToString() : "";
            data.maker                         = !string.IsNullOrEmpty(dr.ItemArray[26].ToString()) ? dr.ItemArray[26].ToString() : "";
            data.origin                        = !string.IsNullOrEmpty(dr.ItemArray[27].ToString()) ? dr.ItemArray[27].ToString() : "";
            data.card_event                    = !string.IsNullOrEmpty(dr.ItemArray[28].ToString()) ? dr.ItemArray[28].ToString() : "";
            data.event_words                   = !string.IsNullOrEmpty(dr.ItemArray[29].ToString()) ? dr.ItemArray[29].ToString() : "";
            data.coupon                        = !string.IsNullOrEmpty(dr.ItemArray[30].ToString()) ? dr.ItemArray[30].ToString() : "";
            data.partner_coupon_downlad        = !string.IsNullOrEmpty(dr.ItemArray[31].ToString()) ? dr.ItemArray[31].ToString() : "";
            data.interest_free_event           = !string.IsNullOrEmpty(dr.ItemArray[32].ToString()) ? dr.ItemArray[32].ToString() : "";
            data.point                         = !string.IsNullOrEmpty(dr.ItemArray[33].ToString()) ? dr.ItemArray[33].ToString() : "";
            data.installation_costs            = !string.IsNullOrEmpty(dr.ItemArray[34].ToString()) ? dr.ItemArray[34].ToString() : "";
            data.search_tag                    = !string.IsNullOrEmpty(dr.ItemArray[35].ToString()) ? dr.ItemArray[35].ToString().Replace('|', ',') : "";
            data.group_id                      = !string.IsNullOrEmpty(dr.ItemArray[36].ToString()) ? dr.ItemArray[36].ToString() : "";
            data.vendor_id                     = !string.IsNullOrEmpty(dr.ItemArray[37].ToString()) ? dr.ItemArray[37].ToString() : "";
            data.coordi_id                     = !string.IsNullOrEmpty(dr.ItemArray[38].ToString()) ? dr.ItemArray[38].ToString() : "";
            data.minimum_purchase_quantity     = !string.IsNullOrEmpty(dr.ItemArray[39].ToString()) ? dr.ItemArray[39].ToString() : "";
            data.review_count                  = !string.IsNullOrEmpty(dr.ItemArray[40].ToString()) ? Convert.ToInt32(dr.ItemArray[40].ToString()) : 0;
            data.shipping                      = !string.IsNullOrEmpty(dr.ItemArray[41].ToString()) ? dr.ItemArray[41].ToString() : "";
            data.delivery_grade                = !string.IsNullOrEmpty(dr.ItemArray[42].ToString()) ? dr.ItemArray[42].ToString() : "";
            data.delivery_detail               = !string.IsNullOrEmpty(dr.ItemArray[43].ToString()) ? dr.ItemArray[43].ToString() : "";
            data.attribute                     = !string.IsNullOrEmpty(dr.ItemArray[44].ToString()) ? dr.ItemArray[44].ToString().Replace('^', ',') : "";
            data.option_detail                 = !string.IsNullOrEmpty(dr.ItemArray[45].ToString()) ? dr.ItemArray[45].ToString() : "";
            data.seller_id                     = !string.IsNullOrEmpty(dr.ItemArray[46].ToString()) ? dr.ItemArray[46].ToString() : "";
            data.age_group                     = !string.IsNullOrEmpty(dr.ItemArray[47].ToString()) ? dr.ItemArray[47].ToString() : "";
            data.gender                        = !string.IsNullOrEmpty(dr.ItemArray[48].ToString()) ? dr.ItemArray[48].ToString() : "";
            data.classs                        = !string.IsNullOrEmpty(dr.ItemArray[49].ToString()) ? dr.ItemArray[49].ToString() : "";
            data.update_time                   = !string.IsNullOrEmpty(dr.ItemArray[50].ToString()) ? dr.ItemArray[50].ToString() : "";

            return data;
        }
    }
}