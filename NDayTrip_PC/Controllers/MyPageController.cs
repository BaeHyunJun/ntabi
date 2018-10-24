using NDayTrip_PC.Models;
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

namespace NDayTrip_PC.Controllers
{
    public class MyPageController : Controller
    {
        DataBase DB = new DataBase();
        NtabiDB nDB = new NtabiDB();

        string corp_code = "NDT";

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

        // GET: MyPage
        public ActionResult Index()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Redirect("/Login");
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

        public ActionResult updateInfo(FormCollection f)
        {
            string NM_KOR = f["name"],
                   NM_ENG_FIRST = f["first"],
                   NM_ENG_LAST = f["last"],
                   EMAIL = f["email"],
                   HANDPHONE = f["tel"],
                   BIRTHDAY = f["birth"],
                   SEX = f["sex"],
                   PASSWORD = f["pwd"],
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
            userData.Single().EMAIL = EMAIL;
            userData.Single().CO_TEL1 = TEL1;
            userData.Single().CO_TEL2 = TEL2;
            userData.Single().CO_TEL3 = TEL3;
            userData.Single().BIRTHDAY = BIRTHDAY;
            userData.Single().SEX = SEX;
            userData.Single().CU_PASS = hash.Substring(0, 20);

            nDB.SaveChanges();

            return Redirect("/MyPage/Info");
        }

        public ActionResult Views(string d, int k)
        {
            string revDay = d;
            int revSeq = k;

            var Data0 = DB.REV_2.Where(w => w.REV_DAY.Equals(revDay))
                                .Where(w => w.REV_SEQ == revSeq);

            int cnt = Data0.Count();
            string CU_NAME = Data0.Where(w => w.REV_CU_SEQ == 1).Select(s => new { s.CU_NAME }).Single().CU_NAME.ToString();

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
                                      pdtCode = a.CORP_CODE + a.PDT_TYPE_CODE + a.PDT_IST_EMP_NO + a.PDT_YY + a.PDT_SEQ,
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
                                      a.pdtCode,
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

            ViewBag.accData = accData;

            return View(Data3);
        }

        // GET: MyPage/Board
        public ActionResult Board()
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
                                        });

            int bd_Cnt = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.CU_YY.Equals(user_YY))
                                      .Where(w => w.CU_SEQ == user_seq)
                                      .Count();

            ViewBag.bd_Cnt = bd_Cnt;

            int rev_Cnt = revData.Count();

            ViewBag.user_Name = user_Name;
            ViewBag.rev_Cnt = rev_Cnt;

            return View(modelData);
        }

        // GET: MyPage/Info
        public ActionResult Info()
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
                                        s.CU_ID,
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
                                        });

            int bd_Cnt = DB.NT_Board_2.Where(w => w.CORP_CODE.Equals(corp_code))
                                      .Where(w => w.DEL_FG.Equals("N"))
                                      .Where(w => w.CU_YY.Equals(user_YY))
                                      .Where(w => w.CU_SEQ == user_seq)
                                      .Count();

            ViewBag.bd_Cnt = bd_Cnt;

            int rev_Cnt = revData.Count();

            ViewBag.user_Name = user_Name;
            ViewBag.rev_Cnt = rev_Cnt;

            return View(userData);
        }

        [HttpPost]
        public ActionResult payApprove(FormCollection f)
        {
            //연계기관정보
            string org_cd = "03320001";                             //체크페이에서 제공하는 기관ID

            //암호화키
            string key = "checkpay@!22ntabi!21$#2!!!#2a1b2";        //체크페이와 협의하여 지정         

            //거래일자 및 시각
            string trx_dt = DateTime.Now.ToString("yyyyMMdd");
            string trx_tm = DateTime.Now.ToString("HHmmss");

            string orderDate = f["orderDate"].ToString(),
                   orderNo = f["orderNo"].ToString(),
                   pdtTitle = f["title"].ToString(),
                   price = f["price"].ToString(),
                   sucUrl = f["receive_url"].ToString();

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

            string url = "https://dev.checkpay.co.kr/CPIF_AFFL_510.act?ID=" + org_cd +
                                                                     "&RQ_DTIME=" + trx_dt + trx_tm +
                                                                     "&TNO=" + trx_dt + trx_tm +
                                                                     "&EV=" + EV +
                                                                     "&VV=" + VV +
                                                                     "&EM=AES" +
                                                                     "&VM=HmacSHA256";

            return Redirect(url);
        }

        public ActionResult checkpay_receive(string ID, string RQ_DTIME, string EV, string VV, string EM, string VM, string RC, string RM)
        {
            //암호화키
            string key = "checkpay@!22ntabi!21$#2!!!#2a1b2";        //체크페이와 협의하여 지정   

            ////데이터 수신
            //string rEV = ""; //(string)rlt["EV"].GetValue();
            //string rHmac = ""; //(string)rlt["VV"].GetValue();

            if (!VerifyMac(key, EV, VV, false))
            {
                Debug.WriteLine("데이터 검증 에러");
            }

            string decEV = DecryptAesBase64(EV, key, false).Substring(14);

            //string을 JSON형태로 변환
            JsonTextParser parser = new JsonTextParser();
            JsonObject obj = parser.Parse(decEV);
            JsonObjectCollection rlt = (JsonObjectCollection)obj;

            //string REV_DAY = rlt[0].order_tr_dt

            string name = "",
                   value = "";

            string REV_DAY = "",
                   REV_SEQ = "",
                   TR_DATE = "",
                   TR_TIME = "",
                   BANK = "",
                   totalPrice = "";

            foreach(JsonObject jo in rlt)
            {
                name = jo.Name;
                value = (string)jo.GetValue();

                switch(name)
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

            int raIdx = 1;

            var raData = DB.REV_3.Where(w => w.REV_DAY.Equals(REV_DAY))
                                 .Where(w => w.REV_SEQ.ToString().Equals(REV_SEQ))
                                 .GroupBy(g => new { g.REV_DAY, g.REV_SEQ })
                                 .Select(s => new
                                 {
                                     idx = s.Max(max => max.REV_ACC_SEQ),
                                 });

            if (raData.Any())
            {
                raIdx += raData.Single().idx;
            }

            var rData = DB.REV_0.Where(w => w.REV_DAY.Equals(REV_DAY))
                                .Where(w => w.REV_SEQ.ToString().Equals(REV_SEQ));

            string emp_no = rData.Single().CHG_EMP_NO.ToString();

            var user = Session["user"] as User;

            string userPK = user.Login;

            string CU_NM = nDB.CU001.Where(w => (w.ASHOP_SITE_CD + w.CU_YY + w.CU_SEQ).Equals(userPK)).Single().CU_NM_KOR;

            string raDate = DateTime.Now.ToString("yyyyMMdd");

            REV_3 Data = new REV_3();

            Data.CORP_CODE = "NTB";
            Data.REV_DAY = REV_DAY;
            Data.REV_SEQ = Convert.ToInt32(REV_SEQ);
            Data.REV_ACC_SEQ = raIdx;
            Data.REV_ACC_DATE = raDate;
            Data.REV_ACC_REG_DATE = raDate;
            Data.REV_ACC_IST_EMP_NO = Convert.ToInt32(emp_no);
            Data.REV_ACC_GB_CODE = "TBPY";
            Data.REV_ACC_DET_SEQ = BANK;
            Data.REV_ACC_PRICE = Convert.ToInt32(totalPrice.ToString());
            Data.REV_ACC_NAME = CU_NM;
            Data.REV_ACC_CONTENT = "";
            Data.ACC_DAY = "";
            Data.ACC_SEQ = 0;
            Data.ACC_SUB_SEQ = 0;

            DB.REV_3.Add(Data);

            DB.SaveChanges();

            chkPrice(REV_DAY, Convert.ToInt32(REV_SEQ));

            string Url = "?d=" + REV_DAY + "&k=" + REV_SEQ;

            return Redirect("/Mypage/Views" + Url);
        }

        [HttpPost]
        public ActionResult approval(FormCollection f)
        {
            //Request Value Define
            //----------------------
            string sCrossKey = "d8b2bbc7de3a13dd8d04215bec15e9ed"; //설정필요
            string sShopId = "ntravels";                              //설정필요
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

                return Redirect("/Mypage/Views" + Url);
            }
            else
            {
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

        // GET: MyPage/Allat_receive
        public ActionResult Allat_receive()
        {
            return View();
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

        //데이터검증
        public bool VerifyMac(String skey, String data, String hmac, bool encode)
        {
            String decryptedData = DecryptAesBase64(data, skey, encode);
            String checkHmac = EncryptHmacSha256(decryptedData.Substring(14), skey, encode);
            if (hmac.Equals(checkHmac))
                return true;

            return false;
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
    }
}