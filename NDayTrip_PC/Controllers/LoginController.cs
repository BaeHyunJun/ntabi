using NDayTrip_PC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NDayTrip_PC.Controllers
{
    public class LoginController : Controller
    {
        NtabiDB NDB = new NtabiDB();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        // GET: Login/getidpwd
        public ActionResult getidpwd()
        {
            return View();
        }

        // GET: Login/changePwd
        public ActionResult changePwd()
        {
            return View();
        }

        // GET: Login/Membership
        public ActionResult Membership()
        {
            return View();
        }

        // GET: Login/AuthMail
        public ActionResult AuthMail(string key)
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

        public JsonResult getID(string name, string birth, string email)
        {
            string BirthDay = birth.Replace("/", "").Replace("-", "").Replace(".", "");

            var memData = NDB.CU001.Where(w => w.CU_NM_KOR.Equals(name));

            memData = memData.Where(w => w.BIRTHDAY.Equals(BirthDay));

            memData = memData.Where(w => w.EMAIL.Equals(email));

            if (memData.Any())
            {
                return Json(memData.Single().CU_ID);
            }

            return Json("회원 정보와 일치하는 아이디가 존재하지 않습니다.");
        }

        [HttpPost]
        public JsonResult getPW(string id, string name, string birth, string mail)
        {
            string txt = "";

            string BirthDay = birth.Replace("/", "").Replace("-", "").Replace(".", "");

            var chkID = NDB.CU001.Where(w => w.CU_ID.Equals(id)).Where(w => w.CU_NM_KOR.Equals(name)).Where(w => w.BIRTHDAY.Equals(BirthDay)).Where(w => w.EMAIL.Equals(mail)).Where(w => w.DEL_FG.Equals("N"));

            if (chkID.Any())
            {
                txt = "임시비밀번호 재설정 인증 메일을 발송 했습니다.";

                string authStr = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(id + name, true, 24 * 60));
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);

                string _senderID = "helper@ndaytrip.com";
                string _senderName = "엔데이트립";
                string _title = "임시 비밀번호 재설정 인증 메일입니다.(발신전용)";
                string _body = "<table style='width:800px; text-align:center;'><tbody><tr><td><img src='http://ntravels.co.kr/Images/authmailheader.png'/></td></tr>"
                    + "<tr><td><table style='width: 650px; margin: 0 auto; text-align: left; margin-top: 10px; color: gray;'><tbody><tr><td style='font-weight: bold; font-size: 16px; height: 50px;'>임시비밀번호 재설정 인증안내</td></tr>"
                    + "<tr><td>본 이메일의 본인인증 확인을 통해 비밀번호를 재설정 하실 수 있습니다.<br />"
                    + "비밀번호 재설정을 위해 아래 링크를 클릭해주세요.</td></tr>"
                    + "<tr><td style='height: 100px;'><a href='" + domain + "/Login/changePwd?passkey=" + authStr
                    + "'>이메일 인증</a></td></tr>"
                    + "<tr><td>※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔트레블스 <a href='#' style='color:black; font-weight: bold; text-decoration: underline;'>고객센터</a> 로 문의해 주세요.</td></tr>"
                    + "</tbody></table></td></tr>"
                    + "<tr style='width: 760px; display: inline-table; margin: 10px auto;'><td style='height:50px; border-top: 1px solid rgb(210, 210, 210); border-bottom: 1px solid rgb(210, 210, 210); color: gray;'>"
                    + "<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>개인정보처리방침</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>여행약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 40px;'>해외여행자보험</a>"
                    + "<tr><td>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</td></tr>"
                    + "</tbody></table>";

                try
                {
                    MailMessage _message = new MailMessage();
                    _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                    _message.To.Add(mail);
                    _message.Subject = _title;
                    _message.SubjectEncoding = System.Text.Encoding.UTF8;
                    _message.Body = _body;
                    _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

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
            }

            return Json(txt);
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

        public JsonResult senderMail(string email)
        {
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
                        string _body = "<table style='width:800px; text-align:center;'><tbody><tr><td><img src='http://ntravels.co.kr/Images/authmailheader.png'/></td></tr>"
                            + "<tr><td><table style='width: 650px; margin: 0 auto; text-align: left; margin-top: 10px; color: gray;'><tbody><tr><td style='font-weight: bold; font-size: 16px; height: 50px;'>엔트레블스 회원가입 신청을 해주셔서 감사합니다.</td></tr>"
                            + "<tr><td>이메일 주소 확인을 통한 회원 가입 승인 후 엔트레블스의 다양한 서비스를 이용하실 수 있습니다.<br />"
                            + "가입 승인을 위해 아래 링크를 클릭하시면, 이메일 인증이 확인되며, 엔트레블스 회원가입이 완료 됩니다.</td></tr>"
                            + "<tr><td style='height: 100px;'><a href='" + domain + "/Login/AuthMail?key=" + authStr
                            + "'>이메일 인증</a></td></tr>"
                            + "<tr><td>※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔트레블스 <a href='#' style='color:black; font-weight: bold; text-decoration: underline;'>고객센터</a> 로 문의해 주세요.</td></tr>"
                            + "</tbody></table></td></tr>"
                            + "<tr style='width: 760px; display: inline-table; margin: 10px auto;'><td style='height:50px; border-top: 1px solid rgb(210, 210, 210); border-bottom: 1px solid rgb(210, 210, 210); color: gray;'>"
                            + "<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>개인정보처리방침</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>여행약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>이용약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>해외여행자보험</a>"
                            + "<tr><td>COPYRIGHT BY NTRAVELS. ALL RIGHT RESERVED.</td></tr>"
                            + "</tbody></table>";

                        try
                        {
                            MailMessage _message = new MailMessage();
                            _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
                            _message.To.Add(email);
                            _message.Subject = _title;
                            _message.SubjectEncoding = System.Text.Encoding.UTF8;
                            _message.Body = _body;
                            _message.IsBodyHtml = true;  //내용에 html이 포함된 경우

                            SmtpClient server = new SmtpClient("smtp.gmail.com", 587);
                            server.EnableSsl = true;
                            server.UseDefaultCredentials = false;
                            server.DeliveryMethod = SmtpDeliveryMethod.Network;
                            server.Credentials = new System.Net.NetworkCredential("ntabinday", "ntabi4605");
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
                    //return Json("인증 메일 유효기간이 지났습니다. 다시 인증 메일을 받아주세요.");
                }
            }
            return Json("메일 주소를 다시 확인해주세요.");
        }

        public JsonResult SignUp(string id, string pwd, string name, string tel, string mail, string birth)
        {
            var memData = NDB.CU001.Where(w => w.CU_ID.Equals(id));

            if (memData.Any())
            {
                return Json("같은 아이디가 존재합니다. 아이디를 변경해 주세요.");
            }

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, pwd);

            string BirthDay = birth.Replace("/", "").Replace("-", "").Replace(".", "");

            string Tel1 = tel.Split('-')[0];
            string Tel2 = tel.Split('-')[1];
            string Tel3 = tel.Split('-')[2];

            CU001 data = new CU001();

            data.ASHOP_SITE_CD = "ASPN15TRGT";

            data.CU_ID = id;
            data.CU_PASS = hash.Substring(0, 20);
            data.CU_NM_KOR = name;
            data.BIRTHDAY = BirthDay;
            data.HANDPHONE1 = Tel1;
            data.HANDPHONE2 = Tel2;
            data.HANDPHONE3 = Tel3;
            data.EMAIL = mail;
            data.DEL_FG = "N";
            data.INS_DT = DateTime.Now.ToString("yyyyMMdd");
            data.REG_WEB_CD = "NT";

            string date = DateTime.Now.ToString("yyyy").ToString();
            data.CU_YY = date;

            int seq = 1;

            try
            {
                seq = NDB.CU001.Where(w => w.CU_YY.Contains(date)).GroupBy(g => g.CU_YY).Select(s => s.Max(m => m.CU_SEQ)).SingleOrDefault() + 1;
            }
            catch
            {
            }

            data.CU_SEQ = seq;

            NDB.CU001.Add(data);

            NDB.SaveChanges();


            //// 메일 보내기
            //string authStr = FormsAuthentication.Encrypt(new FormsAuthenticationTicket(mail, true, 24 * 60));
            //string domain = Request.Url.GetLeftPart(UriPartial.Authority);

            //string _senderID = "helper@ndaytrip.com";
            //string _senderName = "엔데이트립";
            //string _title = "회원가입 인증메일입니다.(발신전용)";
            //string _body = "<table style='width:800px; text-align:center;'><tbody><tr><td><img src='" + domain + "/Content/Images/mailTop.jpg'/></td></tr>"
            //    + "<tr><td><table style='width: 650px; margin: 0 auto; text-align: left; margin-top: 10px; color: gray;'><tbody><tr><td style='font-weight: bold; font-size: 16px; height: 50px;'>엔데이트립 회원가입 신청을 해주셔서 감사합니다.</td></tr>"
            //    + "<tr><td>이메일 주소 확인을 통한 회원 가입 승인 후 엔데이트립의 다양한 서비스를 이용하실 수 있습니다.<br />"
            //    + "가입 승인을 위해 아래 링크를 클릭하시면, 이메일 인증이 확인되며, 엔데이트립 회원가입이 완료 됩니다.</td></tr>"
            //    + "<tr><td style='height: 100px;'><a href='" + domain + "/Home/AuthMail?userID=" + id + "&key=" + authStr
            //    + "'>이메일 인증</a></td></tr>"
            //    + "<tr><td>※ 본 메일은 발신전용 메일로 회신되지 않습니다. 문의사항이 있으시면 엔데이트립 <a href='#' style='color:black; font-weight: bold; text-decoration: underline;'>고객센터</a> 로 문의해 주세요.</td></tr>"
            //    + "</tbody></table></td></tr>"
            //    + "<tr style='width: 760px; display: inline-table; margin: 10px auto;'><td style='height:50px; border-top: 1px solid rgb(210, 210, 210); border-bottom: 1px solid rgb(210, 210, 210); color: gray;'>"
            //    + "<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>개인정보처리방침</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>여행약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>이용약관</a>|<a href='#' style='color: gray; text-decoration: none; margin: 0 20px;'>해외여행자보험</a>"
            //    + "<tr><td>COPYRIGHT BY NDAYTRIP. ALL RIGHT RESERVED.</td></tr>"
            //    + "</tbody></table>";
            //try
            //{
            //    MailMessage _message = new MailMessage();
            //    _message.From = new MailAddress(_senderID, _senderName, System.Text.Encoding.UTF8);
            //    _message.To.Add(mail);
            //    _message.Subject = _title;
            //    _message.SubjectEncoding = System.Text.Encoding.UTF8;
            //    _message.Body = _body;
            //    _message.IsBodyHtml = true;

            //    SmtpClient server = new SmtpClient("smtp.worksmobile.com", 587);
            //    server.EnableSsl = true;
            //    server.UseDefaultCredentials = false;
            //    server.DeliveryMethod = SmtpDeliveryMethod.Network;
            //    server.Credentials = new System.Net.NetworkCredential("helper@ndaytrip.com", "nday12#$");
            //    server.Send(_message);
            //}
            //catch (Exception error)
            //{
            //    return Json("메일보내기 에러 : " + error + "");
            //}

            return Json("회원가입이 완료 되었습니다.");
        }
    }
}