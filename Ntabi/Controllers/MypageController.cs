using Ntabi.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Json;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Ntabi.Controllers
{
    public class MypageController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NTB";

        // GET: Mypage
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Redirect("/");
            }

            var userData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login))
                                    .Select(s => new
                                    {
                                        s.CU_NM_KOR,
                                        s.CU_NM_ENG_FIRST,
                                        s.CU_NM_ENG_LAST,
                                        s.EMAIL,
                                        s.HANDPHONE1,
                                        s.HANDPHONE2,
                                        s.HANDPHONE3,
                                        s.BIRTHDAY,
                                        s.CU_HOME_ADDR,
                                        s.SEX,
                                        s.CU_YY,
                                        s.CU_SEQ,
                                    });

            string user_YY = userData.Single().CU_YY.ToString();
            string user_Name = userData.Single().CU_NM_KOR.ToString();

            int user_seq = Convert.ToInt32(userData.Single().CU_SEQ.ToString());

            var revData = DB.REV_0.Where(w => w.CU_YY.Equals(user_YY))
                                  .Where(w => w.CU_SEQ == user_seq)
                                  .Select(s => new
                                  {
                                      s.REV_DAY,
                                      s.REV_SEQ,
                                      s.PDT_TITLE,
                                      s.REV_STARTDAY,
                                      TOTAL_CNT = s.ADULT_CNT + s.BABY_CNT + s.CHILD_CNT,
                                      s.REV_STATE,
                                      s.REV_PRICE,
                                      pdtCode = s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ
                                  });

            var modelData = revData.Join(DB.PDT_2,
                                        a => a.pdtCode,
                                        b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                        (a, b) => new
                                        {
                                            a.REV_DAY,
                                            a.REV_SEQ,
                                            a.PDT_TITLE,
                                            a.REV_STARTDAY,
                                            a.TOTAL_CNT,
                                            a.REV_STATE,
                                            a.REV_PRICE,
                                            b.PDT_IMG,
                                        })
                                        .OrderByDescending(o => o.REV_STARTDAY);

            int bd_Cnt = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.CU_YY.Equals(user_YY))
                                      .Where(w => w.CU_SEQ == user_seq)
                                      .Count();

            var resultData = modelData.Where(w => w.REV_STATE != "10");

            var cancelData = modelData.Where(w => w.REV_STATE == "10");

            ViewBag.bd_Cnt = bd_Cnt;

            int rev_Cnt = revData.Count();

            ViewBag.user_Name = user_Name;
            ViewBag.rev_Cnt = rev_Cnt;
            ViewBag.cancelData = cancelData;

            return View(resultData);
        }

        // GET: Mypage/LikePdt
        public ActionResult LikePdt()
        {
            return View();
        }

        // GET: Mypage/MyBoard
        public ActionResult MyBoard(int idx = 0)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Redirect("/");
            }

            var userData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login))
                                    .Select(s => new
                                    {
                                        s.CU_NM_KOR,
                                        s.CU_NM_ENG_FIRST,
                                        s.CU_NM_ENG_LAST,
                                        s.EMAIL,
                                        s.HANDPHONE1,
                                        s.HANDPHONE2,
                                        s.HANDPHONE3,
                                        s.BIRTHDAY,
                                        s.CU_HOME_ADDR,
                                        s.SEX,
                                        s.CU_YY,
                                        s.CU_SEQ,
                                    });

            string user_YY = userData.Single().CU_YY.ToString();
            string user_Name = userData.Single().CU_NM_KOR.ToString();

            int user_seq = Convert.ToInt32(userData.Single().CU_SEQ.ToString());

            var revData = DB.REV_0.Where(w => w.CU_YY.Equals(user_YY))
                                  .Where(w => w.CU_SEQ == user_seq)
                                  .Select(s => new
                                  {
                                      s.REV_DAY,
                                      s.REV_SEQ,
                                      s.PDT_TITLE,
                                      s.REV_STARTDAY,
                                      TOTAL_CNT = s.ADULT_CNT + s.BABY_CNT + s.CHILD_CNT,
                                      s.REV_STATE,
                                      s.REV_PRICE,
                                      pdtCode = s.PDT_TYPE_CODE + s.PDT_IST_EMP_NO + s.PDT_YY + s.PDT_SEQ
                                  });

            //var modelData = revData.Join(DB.PDT_2,
            //                            a => a.pdtCode,
            //                            b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
            //                            (a, b) => new
            //                            {
            //                                a.REV_DAY,
            //                                a.REV_SEQ,
            //                                a.PDT_TITLE,
            //                                a.REV_STARTDAY,
            //                                a.TOTAL_CNT,
            //                                a.REV_STATE,
            //                                a.REV_PRICE,
            //                                b.PDT_IMG,
            //                            });

            int bd_Cnt = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.CU_YY.Equals(user_YY))
                                      .Where(w => w.CU_SEQ == user_seq)
                                      .Count();

            ViewBag.bd_Cnt = bd_Cnt;

            int rev_Cnt = revData.Count();

            ViewBag.user_Name = user_Name;
            ViewBag.rev_Cnt = rev_Cnt;



            var board = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                     //.Where(w => w.post_type.Equals(type))
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

            board = board.Where(w => w.CU_YY.ToString().Equals(user_YY))
                         .Where(w => w.CU_SEQ == user_seq)
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


            return View(result);
        }

        // GET: Mypage/MyInfo
        public ActionResult MyInfo()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            var memData = from mf in nDB.CU001
                          where mf.ASHOP_SITE_CD + mf.CU_YY + mf.CU_SEQ == user.Login
                          select new
                          {
                              CU_ID = mf.CU_ID,
                              CU_NM_KOR = mf.CU_NM_KOR,
                              SEX = mf.SEX,
                              CU_NM_ENG_FIRST = mf.CU_NM_ENG_FIRST,
                              CU_NM_ENG_LAST = mf.CU_NM_ENG_LAST,
                              BIRTHDAY = mf.BIRTHDAY,
                              HANDPHONE1 = mf.HANDPHONE1,
                              HANDPHONE2 = mf.HANDPHONE2,
                              HANDPHONE3 = mf.HANDPHONE3,
                              EMAIL = mf.EMAIL,
                              EMAIL_YN = mf.EMAIL_YN,
                              CU_HOME_ADDR = mf.CU_HOME_ADDR,
                              CU_HOME_ADDR_F = mf.CU_HOME_ADDR_F
                          };

            return View(memData);
        }

        public ActionResult updateInfo(FormCollection f)
        {
            string NM_KOR = f["inputName"],
                   NM_ENG_FIRST = f["inputFIrstName"],
                   NM_ENG_LAST = f["inputLastName"],
                   HANDPHONE = f["inputTel"],
                   BIRTHDAY = f["inputBirth"],
                   SEX = f["inputSex"],
                   PASSWORD = f["inputPassword"],
                   TEL1, TEL2, TEL3;

            TEL1 = HANDPHONE.Substring(0, 3);
            TEL3 = HANDPHONE.Substring(HANDPHONE.Length - 4);

            int lastIndex = HANDPHONE.IndexOf(TEL3);

            TEL2 = HANDPHONE.Substring(3, lastIndex - 3);

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, PASSWORD);

            var user = Session["user"] as User;

            var userData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(user.Login));

            userData.Single().CU_NM_KOR = NM_KOR;
            userData.Single().CU_NM_ENG_FIRST = NM_ENG_FIRST;
            userData.Single().CU_NM_ENG_LAST = NM_ENG_LAST;
            userData.Single().CO_TEL1 = TEL1;
            userData.Single().CO_TEL2 = TEL2;
            userData.Single().CO_TEL3 = TEL3;
            userData.Single().BIRTHDAY = BIRTHDAY;
            userData.Single().SEX = SEX;
            userData.Single().CU_PASS = hash.Substring(0, 20);

            nDB.SaveChanges();

            return Content("<script>alert('정보수정이 완료되었습니다.'); location.href='/MyPage/MyInfo'</script>");

            return Redirect("/MyPage/MyInfo");
        }

        // GET: Mypage/revPdt
        public ActionResult revPdt(string d, int k)
        {
            string revDay = d;
            int revSeq = k;

            var Data0 = DB.REV_2.Where(w => w.REV_DAY.Equals(revDay))
                                .Where(w => w.REV_SEQ == revSeq);

            var user = Session["user"] as User;

            int cnt = Data0.Count();
            string CU_NAME  = Data0.Where(w => w.REV_CU_SEQ == 1).Select(s => new { s.CU_NAME }).Single().CU_NAME.ToString();

            var Data1 = DB.REV_0.Where(w => w.REV_DAY.Equals(revDay))
                                .Where(w => w.REV_SEQ == revSeq)
                                .Select(s => new
                                {
                                    s.CORP_CODE,
                                    s.REV_DAY,
                                    s.REV_SEQ,
                                    s.REV_STARTDAY,
                                    s.PDT_TYPE_CODE,
                                    s.PDT_IST_EMP_NO,
                                    s.PDT_YY,
                                    s.PDT_SEQ,
                                    s.PDT_TITLE,
                                    s.CHG_EMP_NO,
                                    s.REV_STATE,
                                    CU_NAME = CU_NAME,
                                    CU_CNT = cnt,
                                    s.REV_CONTENT,
                                    s.REV_PRICE,
                                    s.PDT_DAYS_CODE,
                                });

            var Data2 = Data1.Join(DB.PDT_0,
                                  a => a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                  b => b.CORP_CODE + b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ,
                                  (a, b) => new
                                  {
                                      a.REV_DAY,
                                      a.REV_SEQ,
                                      a.REV_STARTDAY,
                                      a.PDT_TITLE,
                                      b.PDT_CONTENT,
                                      a.CHG_EMP_NO,
                                      a.REV_STATE,
                                      a.CU_NAME,
                                      a.CU_CNT,
                                      a.REV_CONTENT,
                                      a.REV_PRICE,
                                      a.CORP_CODE,
                                      a.PDT_TYPE_CODE,
                                      a.PDT_IST_EMP_NO,
                                      a.PDT_YY,
                                      a.PDT_SEQ,
                                      a.PDT_DAYS_CODE,
                                  });

            var Data3 = Data2.Join(DB.EMP_0,
                                  a => a.CHG_EMP_NO,
                                  b => b.EMP_NO,
                                  (a, b) => new
                                  {
                                      a.REV_DAY,
                                      a.REV_SEQ,
                                      a.REV_STARTDAY,
                                      a.PDT_TITLE,
                                      a.PDT_CONTENT,
                                      b.EMP_NAME,
                                      TEL = b.EMP_TEL1 + "-" + b.EMP_TEL2 + "-" + b.EMP_TEL3,
                                      b.EMP_EMAIL,
                                      a.REV_STATE,
                                      a.CU_NAME,
                                      a.CU_CNT,
                                      a.REV_CONTENT,
                                      a.REV_PRICE,
                                      pdtCode = a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
                                      a.PDT_DAYS_CODE,
                                      a.CORP_CODE,
                                      a.PDT_TYPE_CODE,
                                      a.PDT_IST_EMP_NO,
                                      a.PDT_YY,
                                      a.PDT_SEQ,
                                  }).ToList();

            var trf1 = DB.TRF_0.Where(w => w.TRF_SUB_SEQ == 1).ToList();
            var trf2 = DB.TRF_0.Where(w => w.TRF_SUB_SEQ == 2).ToList();

            var Data4 = Data3.Join(trf1,
                                   a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
                                   b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
                                  (a, b) => new
                                  {
                                      a.REV_DAY,
                                      a.REV_SEQ,
                                      a.REV_STARTDAY,
                                      a.PDT_TITLE,
                                      a.PDT_CONTENT,
                                      a.EMP_NAME,
                                      a.TEL,
                                      a.EMP_EMAIL,
                                      a.REV_STATE,
                                      a.CU_NAME,
                                      a.CU_CNT,
                                      a.REV_CONTENT,
                                      a.REV_PRICE,
                                      a.pdtCode,
                                      a.PDT_DAYS_CODE,
                                      sTit = b.TRF_TITLE,
                                      a.CORP_CODE,
                                      a.PDT_TYPE_CODE,
                                      a.PDT_IST_EMP_NO,
                                      a.PDT_YY,
                                      a.PDT_SEQ,
                                  });

            //var Data5 = Data4.Join(trf2,
            //                       a => a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ + "_" + a.PDT_IST_EMP_NO,
            //                       b => b.PDT_TYPE_CODE + b.PDT_IST_EMP_NO + b.PDT_YY + b.PDT_SEQ + "_" + b.PDT_IST_EMP_NO,
            //                      (a, b) => new
            //                      {
            //                          a.REV_DAY,
            //                          a.REV_SEQ,
            //                          a.REV_STARTDAY,
            //                          a.PDT_TITLE,
            //                          a.PDT_CONTENT,
            //                          a.EMP_NAME,
            //                          a.TEL,
            //                          a.EMP_EMAIL,
            //                          a.REV_STATE,
            //                          a.CU_NAME,
            //                          a.CU_CNT,
            //                          a.REV_CONTENT,
            //                          a.REV_PRICE,
            //                          a.pdtCode,
            //                          a.PDT_DAYS_CODE,
            //                          a.sTit,
            //                          eTit = b.TRF_TITLE
            //                      });

            var Data5 = from f in Data4
                        join j in trf2 on f.CORP_CODE + f.PDT_TYPE_CODE + f.PDT_IST_EMP_NO + f.PDT_YY + f.PDT_SEQ + "_" + f.PDT_IST_EMP_NO equals j.CORP_CODE + j.PDT_TYPE_CODE + j.PDT_IST_EMP_NO + j.PDT_YY + j.PDT_SEQ + "_" + j.PDT_IST_EMP_NO into jj
                        from jf in jj.DefaultIfEmpty()
                        select new
                        {
                            f.REV_DAY,
                            f.REV_SEQ,
                            f.REV_STARTDAY,
                            f.PDT_TITLE,
                            f.PDT_CONTENT,
                            f.EMP_NAME,
                            f.TEL,
                            f.EMP_EMAIL,
                            f.REV_STATE,
                            f.CU_NAME,
                            f.CU_CNT,
                            f.REV_CONTENT,
                            f.REV_PRICE,
                            f.pdtCode,
                            f.PDT_DAYS_CODE,
                            f.sTit,
                            eTit = jf == null ? "" : jf.TRF_TITLE
                        };

            var tourData = DB.REV_2.Where(w => w.REV_DAY.Equals(revDay))
                                   .Where(w => w.REV_SEQ == revSeq)
                                   .ToList()
                                   .Select(s => new
                                   {
                                       s.CU_NAME,
                                       s.CU_NAME_LAST,
                                       s.CU_NAME_FIRST,
                                       s.CU_GB,
                                       s.CU_SEX,
                                       s.CU_Birthdate,
                                       s.CU_TEL,
                                   });

            var accData = DB.REV_3.Where(w => w.REV_DAY.Equals(revDay))
                                  .Where(w => w.REV_SEQ == revSeq)
                                  .ToList()
                                  .Select(s => new
                                  {
                                      s.REV_ACC_SEQ,
                                      REV_ACC_DATE = convertDate8To10(s.REV_ACC_DATE),
                                      REV_ACC_REG_DATE = convertDate8To10(s.REV_ACC_REG_DATE),
                                      REV_ACC_IST_EMP_NO = getName(Convert.ToInt32(s.REV_ACC_IST_EMP_NO.ToString())),
                                      REV_ACC_GB_CODE = getAccGB(s.REV_ACC_GB_CODE),
                                      REV_ACC_DET_SEQ = getDetail(s.REV_ACC_GB_CODE, s.REV_ACC_DET_SEQ.ToString()),
                                      s.REV_ACC_PRICE,
                                      s.REV_ACC_NAME,
                                      s.REV_ACC_CONTENT,
                                  });

            var hotels = DB.TB_HtlResInfo.Where(w => w.REV_DAY.Equals(revDay))
                                         .Where(w => w.REV_SEQ == revSeq)
                                         .Select(s => new
                                         {
                                             s.HTL_ChkIn,
                                             s.HTL_SEQ,
                                             s.HTL_RoomType,
                                             s.Htl_ResComment,
                                         });

            var htlData = DB.HTL_0.Join(hotels,                
                a => a.HTL_SEQ + "_",
                b => b.HTL_SEQ + "_", 
                (a, b) => new 
                { 
                    b.HTL_ChkIn, 
                    a.HTL_NAME, 
                    b.HTL_RoomType,
                    b.Htl_ResComment,
                });

            var memData = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).ToString().Equals(user.Login))
                                   .Select(s => new
                                   {
                                       s.CU_NM_KOR,
                                       s.CU_ID,
                                       s.CU_CO_ADDR_F,
                                   });

            ViewBag.htlData = htlData;
            ViewBag.accData = accData;
            ViewBag.tourData = tourData;
            ViewBag.memData = memData;

            return View(Data5);
        }

        public string getName(int emp)
        {
            string name = "";

            name = DB.EMP_0.Where(w => w.EMP_NO == emp).Single().EMP_NAME;

            return name;
        }

        public string getDetail(string type, string detail)
        {
            string name = "";

            try
            {
                name = DB.DETAIL.Where(w => w.DET_TYPE.Equals(type))
                                .Where(w => w.DET_SEQ.ToString().Equals(detail))
                                .SingleOrDefault()
                                .DET_NAME.ToString();
            }
            catch (Exception e)
            {
                name = "";
            }

            return name;
        }

        public string getAccGB(string type)
        {
            string txt = "";

            switch (type)
            {
                case "CASH":
                    txt = "현금 결제";
                    break;
                case "BANK":
                    txt = "은행 입금";
                    break;
                case "CARD":
                    txt = "카드 결제";
                    break;
                case "EFTS":
                    txt = "전자 결제";
                    break;
            }

            return txt;
        }

        public string convertDate8To10(string date)
        {
            if (date.Length != 8)
                return date;

            string result = date.Substring(0, 4) + "-" + date.Substring(4, 2) + "-" + date.Substring(6, 2);

            return result;
        }

        static string GetMd5Hash(MD5 md5Hash, string input)
        {

            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder sBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }









































        public ActionResult Allat_receive()
        {
            return View();
        }

        /*
         * ----------고객용------------
         * 10072 : 회원 가입시
         * 10073 : 문의 글 작성시
         * 10212 : 답변 글 작성시
         * 10075 : 예약시
         * 10076 : 회원 탈퇴시
         * 10077 : 카드 결제시
         * 
         * ----------담당자용------------
         * 10209 : 문의 글 작성시
         * 10210 : 예약시
         * 10211 : 카드 결제시
         */
        public void kakaoMSG(string id, string content, string tel)
        {
            TSMS_AGENT_MESSAGE MSG = new TSMS_AGENT_MESSAGE();

            MSG.SERVICE_SEQNO = 1610001783;
            MSG.RESERVE7 = id;
            MSG.SEND_MESSAGE = content;
            MSG.SUBJECT = "엔트래블스입니다.";
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



        [HttpPost]
        public ActionResult payApprove(FormCollection f)
        {
            //연계기관정보
            string org_cd = "03320001";                             //체크페이에서 제공하는 기관ID

            //암호화키
            string key = "checkpay@!%^#&ntabi!@1$#3!@!#*&@";        //체크페이와 협의하여 지정         

            //key = "checkpay@!22ntabi!21$#2!!!#2a1b2";

            //거래일자 및 시각
            string trx_dt = DateTime.Now.ToString("yyyyMMdd");
            string trx_tm = DateTime.Now.ToString("HHmmss");

            string orderDate = f["revDate"].ToString(),
                   orderNo = f["revSeq"].ToString(),
                   pdtTitle = f["allat_product_nm"].ToString(),
                   price = f["allat_amt"].ToString(),
                   sucUrl = f["shop_receive_url"].ToString();

            //sucUrl = "http://ntabi.kr/MyPage/checkpay_acc";

            //상품정보
            //URLConnection 방식의 데이터 조회시 사용
            JsonObjectCollection col = new JsonObjectCollection();  //System.Net.Json
            col.Add(new JsonStringValue("order_tr_dt", orderDate));//"20180205"));
            col.Add(new JsonStringValue("order_tr_no", orderNo + "_" + trx_tm));//"2"));
            col.Add(new JsonStringValue("res_suc_url", sucUrl));//"http://localhost:9842/Home/redirect"));
            col.Add(new JsonStringValue("res_err_url", sucUrl));//"http://localhost:9842/"));
            col.Add(new JsonStringValue("prdt_nm", pdtTitle));//"[부관훼리] 인생술집 1탄 도란도란 겨울 오뎅투어"));
            col.Add(new JsonStringValue("prdt_amt", price));//"1000"));
            col.Add(new JsonStringValue("mob_no", ""));
            col.Add(new JsonStringValue("ci", ""));

            string strJson = col.ToString();

            //데이터암호화            
            string EV = EncryptAesBase64(trx_dt + trx_tm + strJson, key);
            string VV = EncryptHmacSha256(strJson, key, true);

            //string url = "https://dev.checkpay.co.kr/CPIF_AFFL_510.act?ID=" + org_cd +
            //                                                         "&RQ_DTIME=" + trx_dt + trx_tm +
            //                                                         "&TNO=" + trx_dt + trx_tm +
            //                                                         "&EV=" + EV +
            //                                                         "&VV=" + VV +
            //                                                         "&EM=AES" +
            //                                                         "&VM=HmacSHA256";

            string url = "https://www.checkpay.co.kr/CPIF_AFFL_510.act?ID=" + org_cd +
                                                                     "&RQ_DTIME=" + trx_dt + trx_tm +
                                                                     "&TNO=" + trx_dt + trx_tm +
                                                                     "&EV=" + EV +
                                                                     "&VV=" + VV +
                                                                     "&EM=AES" +
                                                                     "&VM=HmacSHA256";

            return Redirect(url);
        }

        public ActionResult checkpay_acc(string ID, string RQ_DTIME, string TNO, string EV, string VV, string EM, string VM, string RC, string RM)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('로그인을 확인해 주세요.');location.href='/'</script>");
            }

            string userPK = user.Login;

            string name = "",
                   value = "";

            string REV_DAY = "",
                   REV_SEQ = "",
                   TR_DATE = "",
                   TR_TIME = "",
                   BANK = "",
                   totalPrice = "";

            //암호화키
            string key = "checkpay@!%^#&ntabi!@1$#3!@!#*&@";        //체크페이와 협의하여 지정   

            //key = "checkpay@!22ntabi!21$#2!!!#2a1b2";

            if (!VerifyMac(key, EV, VV, false))
            {
                Debug.WriteLine("데이터 검증 에러");
            }

            string decEV = DecryptAesBase64(EV, key, false).Substring(14);

            //string을 JSON형태로 변환
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(decEV);
            JsonObjectCollection rlt = (JsonObjectCollection)obj;

            foreach (JsonObject jo in rlt)
            {
                name = jo.Name;
                value = (string)jo.GetValue();

                switch (name)
                {
                    case "order_tr_dt":
                        REV_DAY = value;
                        break;
                    case "order_tr_no":
                        REV_SEQ = value.Split('_')[0];
                        break;
                    case "tr_dt":
                        TR_DATE = value;
                        break;
                    case "tr_tm":
                        TR_TIME = value;
                        break;
                    case "bnk_cd":
                        BANK = value;
                        break;
                    case "prdt_amt":
                        totalPrice = value;
                        break;
                }
            }

            string Url = "";

            //RE009 deposit = new RE009();

            //deposit.ASHOP_SITE_CD = "ASPN15TRGT";

            //string RES_DAY = REV_DAY;
            ////string[] seqArr = sOrderNo.Substring(8).Split('_');

            //int RES_SEQ = int.Parse(REV_SEQ);

            //string date = DateTime.Now.ToString("yyyyMMdd");

            //var rData = DB.RE001.Where(w => w.RES_DAY.Equals(RES_DAY)).Where(w => w.RES_SEQ.Equals(RES_SEQ));

            //string emp_no = rData.Single().CHA_EMP_NO.ToString();

            //string CU_NM = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_NM_KOR;

            //int io_seq = 1;

            //var ioSeq = from cuf in DB.RE009
            //            where (cuf.RES_DAY + cuf.RES_SEQ).Equals(RES_DAY + RES_SEQ.ToString())
            //            group cuf by new { cuf.RES_DAY, cuf.RES_SEQ } into g
            //            select new
            //            {
            //                seq = g.Max(m => m.IO_SEQ)
            //            };

            //try { io_seq = ioSeq.Single().seq + 1; }
            //catch { }

            //deposit.RES_DAY = RES_DAY;
            //deposit.RES_SEQ = RES_SEQ;

            //deposit.INS_DT = date;
            //deposit.IO_DAY = date;
            //deposit.IO_SEQ = Convert.ToInt16(io_seq);

            //deposit.EMP_NO = emp_no;
            //deposit.NM = CU_NM;

            //deposit.IO_AMT = Convert.ToDecimal(totalPrice);
            //deposit.RFND_AMT = Convert.ToDecimal(int.Parse(totalPrice) * 0.033);

            ////deposit.QUOTA_TERM = Int16.Parse(sSellMm);
            ////deposit.QUOTA_YN = (sSellMm == "00") ? "N" : "Y";

            //deposit.IO_FG = "1";
            //deposit.IO_FG_CD = "H";
            //deposit.IO_DETAIL_CD = "H3";
            //deposit.RFND_RATE = Convert.ToDecimal(3.3);
            //deposit.MISU_YN = "N";
            //deposit.MONEY_CD = "1";

            //DB.RE009.Add(deposit);
            //DB.SaveChanges();

            //Url = "?date=" + RES_DAY + "&key=" + RES_SEQ;

            //string REV_DAY = rlt[0].order_tr_dt

            //string id = "10077",
            //       cID = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_ID,
            //       cont = "[엔타비 결제] \r\n안녕 하세요 " + cID + "님 \r\n결제 하신 금액은 " + String.Format("{0:#,##0}", totalPrice) + "원 입니다. \r\n담당자 확인 후 연락 드리겠습니다. \r\n\r\n고객센터 : 051-466-4602 \r\n(운영시간 : 오전 9시~6시) \r\n\r\n여행갈땐 엔타비 바로가기 \r\nwww.ntabi.co.kr",
            //       tel = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE1 + DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE2 + DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE3;

            //kakaoMSG(id, cont, tel);


            //var emData = DB.EM001.Where(w => w.EMP_NO.Equals(emp_no)).Single();

            //id = "10077";
            //cID = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_ID;
            //cont = "[엔타비 결제] \r\n" + cID + "님이 결제를 하였습니다. \r\n결제 금액 : " + String.Format("{0:#,##0}", totalPrice) + " 원 \r\n결제자 : " + CU_NM + " \r\n확인 부탁 드립니다.";
            //tel = emData.HANDPHONE1 + emData.HANDPHONE2 + emData.HANDPHONE3;

            //kakaoMSG(id, cont, tel);

            return Redirect("/Mypage/chkProduct" + Url);
        }

        public ActionResult checkpay_receive(string ID, string RQ_DTIME, string TNO, string EV, string VV, string EM, string VM, string RC, string RM)
        {

            //암호화키
            string key = "checkpay@!%^#&ntabi!@1$#3!@!#*&@";        //체크페이와 협의하여 지정   

            //key = "checkpay@!22ntabi!21$#2!!!#2a1b2";

            ////데이터 수신
            //string rEV = ""; //(string)rlt["EV"].GetValue();
            //string rHmac = ""; //(string)rlt["VV"].GetValue();

            if (!VerifyMac(key, EV, VV, false))
            {
                Debug.WriteLine("데이터 검증 에러");
            }

            string decEV = DecryptAesBase64(EV, key, false).Substring(14);

            string returnTxt = "{" +
                                   "\"ID\":\"" + ID.ToString() + "\"," +
                                   "\"RS_DTIME\":\"" + RQ_DTIME.ToString() + "\"," +
                                   "\"TNO\":\"" + TNO.ToString() + "\"," +
                                   "\"EV\":\"" + EV.ToString() + "\"," +
                                   "\"VV\":\"" + VV.ToString() + "\"," +
                                   "\"EM\":\"" + EM.ToString() + "\"," +
                                   "\"VM\":\"" + VM.ToString() + "\"," +
                                   "\"RC\":\"" + RC.ToString() + "\"," +
                                   "\"RM\":\"" + RM.ToString() + "\"" +
                               "}";

            //string을 JSON형태로 변환
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(returnTxt);
            JsonObjectCollection rlt = (JsonObjectCollection)obj;

            return View(rlt);
        }

        /***********************************
         * 전송할 데이터 생성
         * Base64 변환 및 URL 인코딩(UTF-8)
         ***********************************/
        public String EncryptAesBase64(String input, string key)
        {
            string aesInput = AESEncrypt256(input, key);            //AES256암호화

            Debug.WriteLine("input--->>" + input);

            return System.Web.HttpContext.Current.Server.UrlEncode(aesInput);
        }

        //Hmacsha256암호화
        public String EncryptHmacSha256(String input, string key, bool isEncoding)
        {
            string rlt = "";
            string shaInput = sha256_hash(input, key);                   //sha256암호화
            byte[] StrByte = Encoding.UTF8.GetBytes(shaInput);

            rlt = Convert.ToBase64String(StrByte); //Convert.FromBase64String(src); 

            if (isEncoding)
                return HttpUtility.UrlEncode(shaInput, Encoding.UTF8);
            else
                return shaInput;
        }

        // Hmac SHA256  256bit 암호화
        public String sha256_hash(String message, string key)
        {
            key = key ?? "";
            byte[] keyByte = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);


            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);

                return Convert.ToBase64String(hashmessage);
            }
        }

        /***********************************
         * 수신데이터 복호화
         * Base64 변환 및 URL 인코딩(UTF-8)
         ***********************************/
        public String DecryptAesBase64(String output, string key, bool isEncoding)
        {
            string base64encStr = "";
            if (isEncoding)
                base64encStr = HttpUtility.UrlDecode(output, Encoding.UTF8);  //URL디코딩
            else
                base64encStr = output;

            string aesOutput = AESDecrypt256(base64encStr, key);                 //AES256복호화

            return aesOutput;  //URL디코딩
        }

        //AES_256 복호화
        public String AESDecrypt256(String Input, String key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Convert.FromBase64String(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Encoding.UTF8.GetString(xBuff);
            return Output;
        }

        //AES_256 암호화
        public String AESEncrypt256(string Input, string key)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = Encoding.UTF8.GetBytes(key);
            aes.IV = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(Input);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            String Output = Convert.ToBase64String(xBuff);

            return Output;
        }

        //데이터검증
        public bool VerifyMac(String skey, String data, String hmac, bool encode)
        {
            String decryptedData = DecryptAesBase64(data, skey, encode);
            String checkHmac = EncryptHmacSha256(decryptedData.Substring(14), skey, encode);
            if (hmac.Equals(checkHmac))
                return true;

            return false;
        }

        [HttpPost]
        public ActionResult approval(FormCollection f)
        {
            //Request Value Define
            //----------------------
            string sCrossKey = "d0710c593beacf516164220fb7133a6f"; //설정필요
            string sShopId = "ntabi1";                              //설정필요
            int iAmt = int.Parse(f["allat_amt"]);                 // [중요]승인금액(allat_amt) 다시 Setting
            // allat_amt input값을 그대로 Setting 하는 것 보다
            // 해킹 방지를 위하여 장바구니의 Session 값을 이용하는 것을 권장

            string sEncData = f["allat_enc_data"];
            string strReq = "";

            // 요청 데이터 설정
            //----------------------
            strReq = "allat_shop_id=" + sShopId;
            strReq += "&allat_amt=" + iAmt;
            strReq += "&allat_enc_data=" + sEncData;
            strReq += "&allat_cross_key=" + sCrossKey;


            // 올앳과 통신 후 결과값 받기 : AllatUtil.ApprovalReq->통신함수, Hashtable->결과값
            //-----------------------------------------------------------------------------
            AllatUtil atUtil = new AllatUtil();
            Hashtable aHt = atUtil.ApprovalReq(strReq, "SSL");

            // 결제 결과 값 확인
            //------------------
            string sReplyCd = "";
            string sReplyMsg = "";
            if (aHt != null)
            {
                sReplyCd = (string)aHt["reply_cd"];
                sReplyMsg = (string)aHt["reply_msg"];
            }
            else
            {
                //에러 처리
            }

            /* 결과값 처리
            --------------------------------------------------------------------------
               결과 값이 '0000'이면 정상임. 단, allat_test_yn=Y 일경우 '0001'이 정상임.
               실제 결제   : allat_test_yn=N 일 경우 reply_cd=0000 이면 정상
               테스트 결제 : allat_test_yn=Y 일 경우 reply_cd=0001 이면 정상
            --------------------------------------------------------------------------*/
            if (sReplyCd.Equals("0000"))
            {
                // reply_cd "0000" 일때만 성공
                string sOrderNo = (string)aHt["order_no"];
                string sAmt = (string)aHt["amt"];
                string sPayType = (string)aHt["pay_type"];
                string sApprovalYmdHms = (string)aHt["approval_ymdhms"];
                string sSeqNo = (string)aHt["seq_no"];
                string sApprovalNo = (string)aHt["approval_no"];
                string sCardId = (string)aHt["card_id"];
                string sCardNm = (string)aHt["card_nm"];
                string sSellMm = (string)aHt["sell_mm"];
                string sZerofeeYn = (string)aHt["zerofee_yn"];
                string sCertYn = (string)aHt["cert_yn"];
                string sContractYn = (string)aHt["contract_yn"];
                string sSaveAmt = (string)aHt["save_amt"];
                string sBankId = (string)aHt["bank_id"];
                string sBankNm = (string)aHt["bank_nm"];
                string sCashBillNo = (string)aHt["cash_bill_no"];
                string sCashApprovalNo = (string)aHt["cash_approval_no"];
                string sEscrowYn = (string)aHt["escrow_yn"];
                string sAccountNo = (string)aHt["account_no"];
                string sAccountNm = (string)aHt["account_nm"];
                string sIncomeAccNm = (string)aHt["income_account_nm"];
                string sIncomeLimitYmd = (string)aHt["income_limit_ymd"];
                string sIncomeExpectYmd = (string)aHt["income_expect_ymd"];
                string sCashYn = (string)aHt["cash_yn"];
                string sHpId = (string)aHt["hp_id"];
                string sTicketId = (string)aHt["ticket_id"];
                string sTicketPay = (string)aHt["ticket_pay_type"];
                string sTicketName = (string)aHt["ticket_nm"];

                string[] keys = sOrderNo.Split('_');

                string REV_DAY = keys[0].ToString();

                int REV_SEQ = Convert.ToInt32(keys[1].ToString());

                string Url = "?d=" + REV_DAY + "&k=" + REV_SEQ;

                int raIdx = 1;

                var raData = DB.REV_3.Where(w => w.REV_DAY.Equals(REV_DAY))
                                     .Where(w => w.REV_SEQ == REV_SEQ)
                                     .GroupBy(g => new { g.REV_DAY, g.REV_SEQ })
                                     .Select(s => new
                                     {
                                         idx = s.Max(max => max.REV_ACC_SEQ),
                                     });

                if (raData.Any())
                {
                    raIdx += raData.Single().idx;
                }

                var rData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY)).Where(w => w.REV_SEQ.Equals(REV_SEQ));

                string emp_no = rData.Single().CHG_EMP_NO.ToString();

                var user = Session["user"] as User;

                string userPK = user.Login;

                string CU_NM = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_NM_KOR;

                string raDate = DateTime.Now.ToString("yyyyMMdd");

                REV_3 Data = new REV_3();

                Data.CORP_CODE = "NTB";
                Data.REV_DAY = REV_DAY;
                Data.REV_SEQ = REV_SEQ;
                Data.REV_ACC_SEQ = raIdx;
                Data.REV_ACC_DATE = raDate;
                Data.REV_ACC_REG_DATE = raDate;
                Data.REV_ACC_IST_EMP_NO = Convert.ToInt32(emp_no);
                Data.REV_ACC_GB_CODE = "EFTS";
                Data.REV_ACC_DET_SEQ = sCardId.ToString();
                Data.REV_ACC_PRICE = Convert.ToInt32(sAmt.ToString());
                Data.REV_ACC_NAME = CU_NM;
                Data.REV_ACC_CONTENT = "";
                Data.ACC_DAY = "";
                Data.ACC_SEQ = 0;
                Data.ACC_SUB_SEQ = 0;

                DB.REV_3.Add(Data);

                DB.SaveChanges();

                chkPrice(REV_DAY, REV_SEQ);

                return Redirect("/Mypage/revPdt" + Url);

                //string Url = "";

                //RE009 deposit = new RE009();

                //deposit.ASHOP_SITE_CD = "ASPN15TRGT";

                //string RES_DAY = sOrderNo.Substring(0, 8);
                //string[] seqArr = sOrderNo.Substring(8).Split('_');

                //int RES_SEQ = int.Parse(seqArr[0]);

                //string date = DateTime.Now.ToString("yyyyMMdd");

                //var rData = DB.RE001.Where(w => w.RES_DAY.Equals(RES_DAY)).Where(w => w.RES_SEQ.Equals(RES_SEQ));

                //string emp_no = rData.Single().CHA_EMP_NO.ToString();

                //var user = Session["user"] as User;

                //string userPK = user.Login;

                //string CU_NM = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_NM_KOR;

                //int io_seq = 1;

                //var ioSeq = from cuf in DB.RE009
                //            where (cuf.RES_DAY + cuf.RES_SEQ).Equals(RES_DAY + RES_SEQ.ToString())
                //            group cuf by new { cuf.RES_DAY, cuf.RES_SEQ } into g
                //            select new
                //            {
                //                seq = g.Max(m => m.IO_SEQ)
                //            };

                //try { io_seq = ioSeq.Single().seq + 1; }
                //catch { }

                //deposit.RES_DAY = RES_DAY;
                //deposit.RES_SEQ = RES_SEQ;

                //deposit.INS_DT = date;
                //deposit.IO_DAY = date;
                //deposit.IO_SEQ = Convert.ToInt16(io_seq);

                //deposit.EMP_NO = emp_no;
                //deposit.NM = CU_NM;

                //deposit.IO_AMT = Convert.ToDecimal(sAmt);
                //deposit.RFND_AMT = Convert.ToDecimal(int.Parse(sAmt) * 0.033);

                //deposit.QUOTA_TERM = Int16.Parse(sSellMm);
                //deposit.QUOTA_YN = (sSellMm == "00") ? "N" : "Y";

                //deposit.IO_FG = "1";
                //deposit.IO_FG_CD = "H";
                //deposit.IO_DETAIL_CD = "H1";
                //deposit.RFND_RATE = Convert.ToDecimal(3.3);
                //deposit.MISU_YN = "N";
                //deposit.MONEY_CD = "1";

                //DB.RE009.Add(deposit);
                //DB.SaveChanges();

                //Url = "?date=" + RES_DAY + "&key=" + RES_SEQ;

                //string id = "10077",
                //       cID = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_ID,
                //       cont = "[엔타비 결제] \r\n안녕 하세요 " + cID + "님 \r\n결제 하신 금액은 " + String.Format("{0:#,##0}", sAmt) + "원 입니다. \r\n담당자 확인 후 연락 드리겠습니다. \r\n\r\n고객센터 : 051-466-4602 \r\n(운영시간 : 오전 9시~6시) \r\n\r\n여행갈땐 엔타비 바로가기 \r\nwww.ntabi.co.kr",
                //       tel = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE1 + DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE2 + DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().HANDPHONE3;

                //kakaoMSG(id, cont, tel);


                //var emData = DB.EM001.Where(w => w.EMP_NO.Equals(emp_no)).Single();

                //id = "10077";
                //cID = DB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_ID;
                //cont = "[엔타비 결제] \r\n" + cID + "님이 결제를 하였습니다. \r\n결제 금액 : " + String.Format("{0:#,##0}", sAmt) + " 원 \r\n결제자 : " + CU_NM + " \r\n확인 부탁 드립니다.";
                //tel = emData.HANDPHONE1 + emData.HANDPHONE2 + emData.HANDPHONE3;

                //kakaoMSG(id, cont, tel);


                //return Redirect("/Mypage/chkProduct" + Url);
                //return Redirect("/Mypage");

                //return View();
            }
            else
            {
                // reply_cd 가 "0000" 아닐때는 에러 (자세한 내용은 매뉴얼참조)
                // reply_msg 가 실패에 대한 메세지

                return Content("결과코드             : " + sReplyCd + "<br>결과메세지           : " + sReplyMsg + "<br>");
            }
        }

        public void chkPrice(string date, int seq)
        {
            var raData = DB.REV_3.Where(w => w.REV_DAY.Equals(date))
                                 .Where(w => w.REV_SEQ == seq)
                                 .GroupBy(g => new
                                 {
                                     g.REV_DAY,
                                     g.REV_SEQ
                                 })
                                 .Select(s => new
                                 {
                                     price = s.Sum(sum => sum.REV_ACC_PRICE)
                                 });

            var reData = DB.REV_0.Where(w => w.REV_DAY.Equals(date))
                                 .Where(w => w.REV_SEQ == seq);

            int accPrice = Convert.ToInt32(raData.Single().price.ToString()),
                maxPrice = Convert.ToInt32(reData.Single().REV_PRICE.ToString());

            if (accPrice > maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "F";
            }
            else if (accPrice < maxPrice)
            {
                reData.Single().REV_CHK_PRICE = "N";
            }
            else
            {
                reData.Single().REV_CHK_PRICE = "Y";
            }

            DB.SaveChanges();

        }
    }
}