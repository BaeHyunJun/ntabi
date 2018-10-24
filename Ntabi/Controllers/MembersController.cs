using Newtonsoft.Json.Linq;
using Ntabi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml;

namespace Ntabi.Controllers
{
    public class MembersController : Controller
    {
        NtabiDB NDB = new NtabiDB();

        // GET: Members
        public ActionResult Index()
        {
            return View();
        }

        // GET: Members/chkMail
        public ActionResult chkMail()
        {
            return View();
        }

        // GET: Members/Join
        public ActionResult Join(string key)
        {
            if (!String.IsNullOrEmpty(key))
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(key);

                ViewBag.mail = ticket.Name;

                if (ticket.Expired)
                {
                    ViewBag.expired = false;
                }
                else
                {
                    ViewBag.expired = true;
                }
            }

            return View();
        }

        // GET: Members/getidpwd
        public ActionResult getidpwd()
        {
            return View();
        }

        // GET: Members/getid
        public ActionResult getid(string name, string email)
        {
            var memData = NDB.CU001.Where(w => w.DEL_FG != "Y")
                                   .Where(w => w.CU_NM_KOR.Equals(name))
                                   .Where(w => w.EMAIL.Equals(email))
                                   .Select(s => new
                                   {
                                       s.CU_ID,
                                   });

            if (memData.Any())
            {
                return View(memData);
            }

            return Content("<script>alert('해당정보와 일치하는 아이디가 존재하지 않습니다.'); location.href='/Members/getidpwd';</script>");
        }

        // GET: Members/changePwd
        public ActionResult changePwd()
        {
            return View();
        }

        [HttpPost]
        public JsonResult changePW(string id, string pwd, string key)
        {
            string txt = "";

            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(key);

            if (ticket.Expired)
            {
                txt = "메일 인증 기간이 지났습니다. 다시 인증해주세요.";
            }
            else
            {
                var userData = NDB.CU001.Where(w => w.CU_ID.Equals(id));

                if (userData.Any())
                {

                    MD5 md5Hash = MD5.Create();
                    string hash = GetMd5Hash(md5Hash, pwd);

                    userData.Single().CU_PASS = hash.Substring(0, 20);

                    NDB.SaveChanges();

                    txt = "비밀 번호가 정상적으로 변경 되었습니다.";
                }
                else
                {
                    txt = "아이디가 존재하지 않습니다.";
                }
            }

            return Json(txt);
        }

        public JsonResult JSON_getID(string name, string email)
        {
            var memData = NDB.CU001.Where(w => w.DEL_FG != "Y")
                                   .Where(w => w.CU_NM_KOR.Equals(name))
                                   .Where(w => w.EMAIL.Equals(email))
                                   .Select(s => new
                                   {
                                       s.CU_ID,
                                   });

            if (memData.Any())
            {
                //ViewBag.id = memData.SingleOrDefault().CU_ID;

                //return Redirect("/Members/getid?id=" + memData.Single().CU_ID);
                return Json(memData);
            }

            return Json("회원 정보와 일치하는 아이디가 존재하지 않습니다.");
        }

        [HttpPost]
        public JsonResult getPW(string id, string name, string mail)
        {
            string txt = "";

            var chkID = NDB.CU001.Where(w => w.CU_ID.Equals(id))
                                 .Where(w => w.CU_NM_KOR.Equals(name))
                                 .Where(w => w.EMAIL.Equals(mail))
                                 .Where(w => w.DEL_FG.Equals("N"));

            if (chkID.Any())
            {
                txt = "임시비밀번호 재설정 인증 메일을 발송 했습니다.";

                string authStr = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(id + name, true, 24 * 60));
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);

                string _senderID = "helper@ndaytrip.com";
                string _senderName = "엔데이트립";
                string _title = "임시 비밀번호 재설정 인증 메일입니다.(발신전용)";
                string _body = "";

                //"<table style='width:800px; text-align:center;'><tbody><tr><td><img src='http://ntravels.co.kr/Images/authmailheader.png'/></td></tr>"
                //     + "<tr><td><table style='width: 650px; margin: 0 auto; text-align: left; margin-top: 10px; color: gray;'><tbody><tr><td style='font-weight: bold; font-size: 16px; height: 50px;'>임시비밀번호 재설정 인증안내</td></tr>"
                //     + "<tr><td>본 이메일의 본인인증 확인을 통해 비밀번호를 재설정 하실 수 있습니다.<br />"
                //     + "비밀번호 재설정을 위해 아래 링크를 클릭해주세요.</td></tr>"
                //     + "<tr><td style='height: 100px;'><a href='" + domain + "/Members/changePwd?passkey=" + authStr
                //     + "'>이메일 인증</a></td></tr>"
                //     + "<tr><td>※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔트레블스 <a href='#' style='color:black; font-weight: bold; text-decoration: underline;'>고객센터</a> 로 문의해 주세요.</td></tr>"
                //     + "</tbody></table></td></tr>"
                //     + "<tr style='width: 760px; display: inline-table; margin: 10px auto;'><td style='height:50px; border-top: 1px solid rgb(210, 210, 210); border-bottom: 1px solid rgb(210, 210, 210); color: gray;'>"
                //     + "<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>개인정보처리방침</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>여행약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>해외여행자보험</a>"
                //     + "<tr><td>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</td></tr>"
                //     + "</tbody></table>";

                _body = " <div style='text-align: center;'>" +
                            "<p>" +
                                "<img src='" + domain + "/Content/Images/Members/password.png' alt='메일인증'>" +
                            "</p>" +
                            "<h1 style='font-size: 25px;'>임시비밀번호 재설정 인증안내</h1>" +
                            "<h4 style='font-weight: normal;'>" +
                                "본 이메일의 본인인증 확인을 통해 비밀번호를 재설정 하실 수 있습니다.<br/>" +
                                "비밀번호 재설정을 위해 아래 링크를 클릭해주세요." +
                            "</h4>" +
                            "<h4 style='font-weight: normal;color:red;' >※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔타비 <b><a href='" + domain + "/Community/Lists?type=qna'>고객센터</a></b> 로 문의해 주세요.</h4>" +
                            "<center>" +
                                "<p style='width:800px; padding:10px; background-color: #8acb20; color: #fff;'>" +
                                    "<a href='" + domain + "/Members/Join?key=" + authStr + "'>이메일 인증하기</a>" +
                                "</p>" +
                            "</center>" +
                            "<div>" +
                                "<hr width='800px;'>" +
                                "<h8 style='margin:0px 12px 0px 12px;'>" +
                                    "<a href='" + domain + "/Company/privacy'>개인정보처리방침</a>" +
                                "</h8>" +
                                "<h8>ㅣ</h8>" +
                                "<h8 style='margin:0px 12px 0px 12px;'>" +
                                    "<a href='" + domain + "/Company/service'>여행약관</a>" +
                                "</h8>" +
                                "<h8>ㅣ</h8>" +
                                "<h8 style='margin:0px 12px 0px 12px;'>" +
                                    "<a href='http://www.tourinsu.co.kr/bnn/?uk=4132ee273b4d9'>해외여행자보험</a>" +
                                "</h8>" +
                                "<hr width='800px;'>" +
                            "</div>" +
                            "<h8>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</h8>" +
                        "</div>";

                try
                {
                    MailMessage _message = new MailMessage();
                    _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                    _message.To.Add(mail);
                    _message.Subject = _title;
                    _message.SubjectEncoding = System.Text.Encoding.UTF8;
                    _message.Body = _body;
                    _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                    SmtpClient server = new SmtpClient("smtp.worksmobile.com", 587);
                    server.EnableSsl = true;
                    server.UseDefaultCredentials = false;
                    server.DeliveryMethod = SmtpDeliveryMethod.Network;
                    server.Credentials = new System.Net.NetworkCredential("ntravels@ntravels.co.kr", "ntravels1!");
                    server.Send(_message);
                }
                catch (ArgumentException error)
                {
                    ViewBag.err = error;
                }
            }

            return Json(txt);
        }

        // POST: Members/Confirm
        [HttpPost]
        public ActionResult Confirm(FormCollection f)
        {
            string id           = f["hiddenID"],                 
                   name         = f["inputName"],               
                   pwd          = f["inputPassword"],           
                   email        = f["hiddenEmail"],              
                   receiveMail  = f["receiveEmail"] == "false" ? "N" : "Y",            
                   LastName     = f["inputLastName"],           
                   FIrstName    = f["inputFIrstName"],          
                   Tel          = f["inputTel"],
                   receiveTel   = f["receiveTel"] == "false" ? "N" : "Y",              
                   Sex          = f["inputSex"],
                   Birth        = f["inputBirth"],              
                   Address      = f["inputAddress"],              
                   Address_F    = f["inputAddress_F"];     

            ViewBag.ID = id;       

            var memData = NDB.CU001.Where(w => w.CU_ID.Equals(id));

            if (memData.Any())
            {
                return View();
            }

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, pwd);

            string date = DateTime.Now.ToString("yyyy").ToString();

            int seq = 1;

            try
            {
                seq = NDB.CU001.Where(w => w.CU_YY.Contains(date)).GroupBy(g => g.CU_YY).Select(s => s.Max(m => m.CU_SEQ)).SingleOrDefault() + 1;
            }
            catch
            {
            }

            CU001 data = new CU001();

            data.ASHOP_SITE_CD      = "ASPN15TRGT";
            data.CU_YY              = date;
            data.CU_SEQ             = seq;
            data.CU_ID              = id;
            data.CU_PASS            = hash.Substring(0, 20);
            data.CU_NM_KOR          = name;
            data.CU_NM_ENG_FIRST    = FIrstName;
            data.CU_NM_ENG_LAST     = LastName;
            data.BIRTHDAY           = Birth;
            data.CU_HOME_ADDR_F     = Address;
            data.CU_HOME_ADDR       = Address_F;
            data.HANDPHONE1         = Tel.Length > 1 ? Tel.Substring(0, 3) : "";
            data.HANDPHONE2         = Tel.Length > 1 ? Tel.Substring(3, Tel.Length - 7) : "";
            data.HANDPHONE3         = Tel.Length > 1 ? Tel.Substring(Tel.Length - 4) : "";
            data.SMS_YN             = receiveTel;
            data.SEX                = Sex;
            data.EMAIL              = email;
            data.EMAIL_YN           = receiveMail;
            data.DEL_FG             = "N";
            data.INS_DT             = DateTime.Now.ToString("yyyyMMdd");

            NDB.CU001.Add(data);

            NDB.SaveChanges();

            return View();
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

        public JsonResult getLogin(string id, string pwd)
        {
            var memData = NDB.CU001.Where(w => w.DEL_FG != "Y")
                                   .Where(w => w.CU_ID.Equals(id))
                                   .Select(s => new
                                   {
                                       PK = s.ASHOP_SITE_CD + s.CU_YY + s.CU_SEQ,
                                       Name = s.CU_NM_KOR,
                                       s.CU_ID,
                                       s.CU_PASS,
                                       s.DEL_FG,
                                   });

            if (memData.Any())
            {
                string chkData = memData.Single().DEL_FG.ToString();

                if (chkData == "M")
                {
                    return Json("메일 인증을 하지 않았습니다. 인증을 완료해주세요.");
                }

                MD5 md5Hash = MD5.Create();
                string hash = GetMd5Hash(md5Hash, pwd);

                var data = memData.Where(p => p.CU_PASS.Equals(hash.Substring(0, 20)));

                if (data.Count() == 0)
                {
                    return Json("현재 아이디와 비밀번호가 일치하지 않습니다.");
                }
                else
                {
                    Session["user"] = new User() { Login = data.Single().PK, ID = data.Single().CU_ID, Name = data.Single().Name };

                    return Json("반갑습니다. " + data.Single().Name + "님");
                }
            }

            return Json("해당 아이디가 존재하지 않습니다.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            User user = (User)(Session["user"]);

            if (!string.IsNullOrEmpty(user.ID))
            {
                Session.Abandon();
                Session["user"] = null;

                Response.Redirect(Request.Url.GetLeftPart(UriPartial.Authority));

                return View();
            }
            else
                return Redirect("/");
        }

        public JsonResult chkID(string id)
        {
            var chkIdData = NDB.CU001.Where(w => w.CU_ID.Equals(id));

            string txt = "사용 가능한 아이디입니다";

            if (chkIdData.Any())
            {
                txt = "동일한 아이디가 존재합니다.";
            }

            return Json(txt);
        }

        public JsonResult senderMail(string emailID, string emailDomain)
        {
            string email = emailID + "@" + emailDomain;

            if (!String.IsNullOrEmpty(email))
            {
                string pattern = @"^[-A-Za-z0-9_]+[-A-Za-z0-9_.]*[@]{1}[-A-Za-z0-9_]+[-A-Za-z0-9_.]*[.]{1}[A-Za-z]{2,5}$";

                if (System.Text.RegularExpressions.Regex.IsMatch(email, pattern))
                {
                    //메일 보내기

                    var data = NDB.CU001.Where(o => o.DEL_FG == "N").Select(s => new { s.CU_ID, s.EMAIL });

                    data = data.Where(o => o.EMAIL == email);

                    data = data.Where(o => o.CU_ID != null);

                    int cnt = data.Count();

                    if (data.Any())
                    {
                        return Json("이미 존재하는 메일 주소입니다.");
                    }
                    else
                    {
                        string authStr = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(email, true, 24 * 60));
                        string domain = Request.Url.GetLeftPart(UriPartial.Authority);

                        string _senderID = "ntravels@ntravels.co.kr";
                        string _senderName = "NTravels";
                        string _title = "회원가입 인증메일입니다.(발신전용)";
                        string _body = "";

                        //"<table style='width:800px; text-align:center;'><tbody><tr><td><img src='http://ntravels.co.kr/Images/authmailheader.png'/></td></tr>"
                        //     + "<tr><td><table style='width: 650px; margin: 0 auto; text-align: left; margin-top: 10px; color: gray;'><tbody><tr><td style='font-weight: bold; font-size: 16px; height: 50px;'>엔트레블스 회원가입 신청을 해주셔서 감사합니다.</td></tr>"
                        //     + "<tr><td>이메일 주소 확인을 통한 회원 가입 승인 후 엔트레블스의 다양한 서비스를 이용하실 수 있습니다.<br />"
                        //     + "가입 승인을 위해 아래 링크를 클릭하시면, 이메일 인증이 확인되며, 엔트레블스 회원가입이 완료 됩니다.</td></tr>"
                        //     + "<tr><td style='height: 100px;'><a href='" + domain + "/Members/Join?key=" + authStr
                        //     + "'>이메일 인증</a></td></tr>"
                        //     + "<tr><td>※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔타비 <a href='" + domain + "/Community/Lists?type=qna' style='color:black; font-weight: bold; text-decoration: underline;'>고객센터</a> 로 문의해 주세요.</td></tr>"
                        //     + "</tbody></table></td></tr>"
                        //     + "<tr style='width: 760px; display: inline-table; margin: 10px auto;'><td style='height:50px; border-top: 1px solid rgb(210, 210, 210); border-bottom: 1px solid rgb(210, 210, 210); color: gray;'>"
                        //     + "<a href='" + domain + "/Company/privacy' style='color: gray; text-decoration: none; margin: 0 20px;'>개인정보처리방침</a>|<a href='" + domain + "/Company/service' style='color: gray; text-decoration: none; margin: 0 20px;'>여행약관</a>|<a href='http://www.tourinsu.co.kr/bnn/?uk=4132ee273b4d9' style='color: gray; text-decoration: none; margin: 0 20px;'>해외여행자보험</a>"
                        //     + "<tr><td>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</td></tr>"
                        //     + "</tbody></table>";

                        _body = " <div style='text-align: center;'>" + 
                                    "<p>" + 
                                        "<img src='" + domain + "/Content/Images/Members/mail.png' alt='메일인증'>" + 
                                    "</p>" + 
                                    "<h1 style='font-size: 25px;'>엔타비 회원가입 신청을 해주셔서 감사합니다</h1>" + 
                                    "<h4 style='font-weight: normal;'>" + 
                                        "이메일 주소 확인을 통한 회원 가입 승인 후 엔타비의 다양한 서비스를 이용하실 수 있습니다.<br/>" + 
                                        "가입 승인을 위해 아래 링크를 클릭하시면, 이메일 인증이 확인되며, 엔타비 회원가입이 완료 됩니다." + 
                                    "</h4>" +
                                    "<h4 style='font-weight: normal;color:red;' >※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔타비 <b><a href='" + domain + "/Community/Lists?type=qna'>고객센터</a></b> 로 문의해 주세요.</h4>" + 
                                    "<center>" + 
                                        "<p style='width:800px; padding:10px; background-color: #8acb20; color: #fff;'>" + 
                                            "<a href='" + domain + "/Members/Join?key=" + authStr + "'>이메일 인증하기</a>" + 
                                        "</p>" + 
                                    "</center>" + 
                                    "<div>" + 
                                        "<hr width='800px;'>" + 
                                        "<h8 style='margin:0px 12px 0px 12px;'>" +
                                            "<a href='" + domain + "/Company/privacy'>개인정보처리방침</a>" + 
                                        "</h8>" + 
                                        "<h8>ㅣ</h8>" +
                                        "<h8 style='margin:0px 12px 0px 12px;'>" +
                                            "<a href='" + domain + "/Company/service'>여행약관</a>" +
                                        "</h8>" +
                                        "<h8>ㅣ</h8>" +
                                        "<h8 style='margin:0px 12px 0px 12px;'>" +
                                            "<a href='http://www.tourinsu.co.kr/bnn/?uk=4132ee273b4d9'>해외여행자보험</a>" +
                                        "</h8>" +
                                        "<hr width='800px;'>" +
                                    "</div>" +
                                    "<h8>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</h8>" + 
                                "</div>";

                        try
                        {
                            MailMessage _message = new MailMessage();
                            _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                            _message.To.Add(email);
                            _message.Subject = _title;
                            _message.SubjectEncoding = System.Text.Encoding.UTF8;
                            _message.Body = _body;
                            _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                            SmtpClient server = new SmtpClient("smtp.worksmobile.com", 587);
                            server.EnableSsl = true;
                            server.UseDefaultCredentials = false;
                            server.DeliveryMethod = SmtpDeliveryMethod.Network;
                            server.Credentials = new System.Net.NetworkCredential("ntravels@ntravels.co.kr", "ntravels1!");
                            server.Send(_message);

                            return Json("인증 메일을 보냈습니다. 확인해주세요.");
                        }
                        catch (Exception error)
                        {
                            return Json(error);
                        }
                    }
                }
                else
                {
                    return Json("메일 주소를 다시 확인해주세요.");
                }
            }
            return Json("메일 주소를 다시 확인해주세요.");
        }

        public ActionResult naverLogin()
        {
            string client_ID = "LBYlXf_7HE9rGXbFoFEb";
            string host = Request.Url.Host;
            string redirect_uri = "http://ntabi.kr/Members/CallbackNaver";

            string stateToken = "";

            DateTime now = DateTime.Now;
            Random rand = new Random();

            MD5 md5Hash = MD5.Create();

            stateToken = GetMd5Hash(md5Hash, now.ToString("yyyyMMddhhmmss") + rand.Next(1, 99999));

            Session["stateToken"] = stateToken;

            string loginUrl = "https://nid.naver.com/oauth2.0/authorize?response_type=code&client_id=" + client_ID + "&redirect_uri=" + redirect_uri + "&state=" + stateToken;

            return Redirect(loginUrl);
        }

        [HttpGet]
        public ActionResult CallbackNaver(string state, string code, string error = "")
        {
            string client_ID = "LBYlXf_7HE9rGXbFoFEb";
            string client_secret = "H5R5nhVKcq";
            string url = "/";

            if (!string.IsNullOrEmpty(error))
            {
                return Content("<script>alert('네이버 로그인을 취소했습니다.');window.opener.location.href='http://ntabi.kr';window.close();</script>");
            }

            if (Session["stateToken"].Equals(state))
            {
                url = "https://nid.naver.com/oauth2.0/token?client_id=" + client_ID + "&client_secret=" + client_secret + "&grant_type=authorization_code&state=" + state + "&code=" + code;

                System.Net.WebRequest request = WebRequest.Create(url);
                request.ContentType = "application/json";
                request.Method = "GET";

                WebResponse response = request.GetResponse();
                Stream ostream = response.GetResponseStream();
                StreamReader sr = new StreamReader(ostream, Encoding.UTF8);

                string str = sr.ReadToEnd();

                sr.Close();
                response.Close();

                JObject jObj = JObject.Parse(str);

                string accessToken = jObj["access_token"].ToString();
                string refreshToken = jObj["refresh_token"].ToString();
                string tokenType = jObj["token_type"].ToString();
                string expires = jObj["expires_in"].ToString();

                request = WebRequest.Create("https://apis.naver.com/nidlogin/nid/getUserProfile.xml");
                request.ContentType = "application/xml";
                request.Method = "GET";
                request.Headers["Authorization"] = tokenType + " " + accessToken;

                response = request.GetResponse();
                ostream = response.GetResponseStream();
                sr = new StreamReader(ostream, Encoding.UTF8);

                str = sr.ReadToEnd();

                sr.Close();
                response.Close();

                XmlDocument reader = new XmlDocument();
                reader.LoadXml(str);

                XmlNodeList xnl = reader.GetElementsByTagName("response");

                string id = "",
                       name = "",
                       email = "",
                       gender = "";

                foreach (XmlNode xn in xnl)
                {
                    name = xn["name"].InnerText;
                    email = xn["email"].InnerText;
                    gender = xn["gender"].InnerText;
                    id = xn["id"].InnerText;
                }

                snsLogin("naver", id, name, email, gender);

                return Content("<script>window.opener.location.reload();window.close();</script>");

            }

            return Redirect(url);
        }

        /// <summary>
        /// id : 27102989, name : 배현준, email : hj3ds@naver.com, gender : M
        /// </summary>
        /// <param name="type"> sns 종류 </param>
        /// <param name="id"> key ( facebook : id, naver : accesstoken ) </param>
        /// <param name="name"> 이름 </param>
        /// <param name="email"> 이메일 </param>
        /// <param name="gender"> 성별 </param>
        /// <param name="redirect"> 콜백 주소 </param>
        /// <returns></returns>
        public JsonResult snsLogin(string type, string id, string name, string email = "", string gender = "", string redirect = "/")
        {
            string str = "";

            // 회원 체크

            var chkToken = NDB.CU_Meta.Where(w => w.meta_value.Equals(id))
                             .Join(NDB.CU001,
                                  a => (a.CU_YY + a.CU_SEQ),
                                  b => (b.CU_YY + b.CU_SEQ),
                                  (a, b) => new
                                  {
                                      a.CU_YY,
                                      a.CU_SEQ,
                                      b.DEL_FG,
                                  })
                             .Where(w => w.DEL_FG.Equals("N"));

            if (!chkToken.Any())
            {
                // 회원가입 시키기
                str = setSnsMem(type, id, name, email, gender);
            }

            // 로그인 하기

            var data = NDB.CU_Meta.Where(w => w.meta_value.Equals(id))
                         .Join(NDB.CU001.Where(w => w.DEL_FG.Equals("N")),
                             a => (a.CU_YY + a.CU_SEQ),
                             b => (b.CU_YY + b.CU_SEQ),
                             (a, b) => new
                             {
                                 PK = b.ASHOP_SITE_CD + b.CU_YY + b.CU_SEQ,
                                 Name = b.CU_NM_KOR,
                                 ID = b.CU_ID,
                             });

            Session["user"] = new User() { Login = data.Single().PK, ID = data.Single().ID, Name = data.Single().Name };

            if (!string.IsNullOrEmpty(redirect))
            {
                str = HttpUtility.HtmlDecode(redirect);

                return Json(str);
            }

            return Json(str);
        }

        public string setSnsMem(string type, string userID, string name, string email = "", string gender = "")
        {
            string str = "";

            switch (type)
            {
                case "facebook":
                    str = "fb";
                    break;
                case "naver":
                    str = "nv";
                    break;
            }

            string ID = "";

            try
            {
                ID = str + ":" + email.Split('@')[0];
            }
            catch
            {
                return "이메일 없음 아이디 생성하기";
            }

            var chkID = NDB.CU001.Where(w => w.CU_ID.Contains(ID)).Count();

            ID += chkID;

            string sex = "";

            try
            {
                sex = gender.Substring(0, 1).ToUpper();
            }
            catch
            {
                sex = "M";
            }

            // 회원가입 

            DateTime insDate = DateTime.Now;

            string ASHOP_SITE_CD = "ASPN15TRGT";
            int cu_Seq = 1;
            string YY = insDate.ToString("yyyy");

            var cuSeq = from cuf in NDB.CU001
                        where cuf.CU_YY.Contains(YY)
                        group cuf by new { cuf.CU_YY } into g
                        select new
                        {
                            seq = g.Max(m => m.CU_SEQ)
                        };

            try { cu_Seq = cuSeq.Single().seq + 1; }
            catch { }

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, str + ":1231235");

            CU001 customer = new CU001();

            customer.ASHOP_SITE_CD = ASHOP_SITE_CD;
            customer.CU_YY = YY;
            customer.CU_SEQ = cu_Seq;

            customer.CU_ID = ID;
            customer.CU_PASS = hash.Substring(0, 20);

            customer.CU_NM_KOR = name;
            customer.EMAIL = email;
            customer.SEX = sex;

            customer.SMS_YN = "N";
            customer.EMAIL_YN = "N";
            customer.CU_DM_YN = "Y";
            customer.PP_YN = "N";
            customer.DEL_FG = "N";
            customer.CU_PCARD_YN = "N";
            customer.AUTH_YN = "N";

            customer.REG_WEB_CD = str;

            customer.INS_DT = insDate.ToString("yyyyMMdd");

            NDB.CU001.Add(customer);

            int metaID = 1;

            var metaData = from mf in NDB.CU_Meta.Where(w => w.CU_YY.Equals(YY)).Where(w => w.CU_SEQ == cu_Seq).GroupBy(g => new { g.CU_YY, g.CU_SEQ })
                           select new
                           {
                               metaid = mf.Max(m => m.meta_id)
                           };

            try { metaID = int.Parse(metaData.Single().metaid.ToString()) + 1; }
            catch { }

            CU_Meta meta = new CU_Meta();

            meta.meta_id = metaID;
            meta.CU_YY = YY;
            meta.CU_SEQ = cu_Seq;
            meta.meta_key = "type";
            meta.meta_value = type;

            NDB.CU_Meta.Add(meta);

            metaID++;

            CU_Meta meta2 = new CU_Meta();

            meta2.meta_id = metaID;
            meta2.CU_YY = YY;
            meta2.CU_SEQ = cu_Seq;
            meta2.meta_key = "userID";
            meta2.meta_value = userID;

            NDB.CU_Meta.Add(meta2);

            NDB.SaveChanges();


            return "";// DB.CU_Meta.Where(w => w.meta_value.Equals(userID)).Select(s => new { s.CU_YY, s.CU_SEQ }).ToList();
        }
    }
}