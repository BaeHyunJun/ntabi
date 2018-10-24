using Ntabi.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace NCompany.Controllers
{
    public class EditorController : Controller
    {
        public static void Resize(string importPath, string exportPath, int targetX, int targetY)
        {
            Image originalImage = Image.FromFile(importPath);

            double ratioX = targetX / (double)originalImage.Width;
            double ratioY = targetY / (double)originalImage.Height;

            double ratio = Math.Min(ratioX, ratioY);

            int newWidth = (int)(originalImage.Width * ratio);
            int newHeight = (int)(originalImage.Height * ratio);

            Bitmap newImage = new Bitmap(targetX, targetY);
            using (Graphics g = Graphics.FromImage(newImage))
            {
                g.FillRectangle(Brushes.Transparent, 0, 0, newImage.Width, newImage.Height);
                g.DrawImage(originalImage, (targetX - newWidth) / 2, (targetY - newHeight) / 2, newWidth, newHeight);
            }

            newImage.Save(exportPath);

            originalImage.Dispose();
            newImage.Dispose();
        }

        [HttpPost]
        public string FileUploader_html5()
        {
            int len = Request.ContentLength;
            byte[] bytes = new byte[len];

            Request.InputStream.Read(bytes, 0, len);

            string fileName = System.Web.HttpUtility.UrlDecode(Request.Headers["file-name"], Encoding.UTF8),
                   fileSize = Request.Headers["file-size"],
                   fileType = Request.Headers["file-type"];

            string[] isExt = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };
            string extension = Path.GetExtension(fileName);

            string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");
            string fileNames = nowDt + "_" + fileName;

            string rtxt = "NOTALLOW_" + fileName;

            string Loc = Server.MapPath("/mobile/thumb/");

            string fileloc = string.Format("{0}\\{1}", Loc, fileNames);

            if (isExt.Contains(extension))
            {
                try
                {
                    FileStream fs = new FileStream(fileloc, FileMode.Create, FileAccess.ReadWrite);

                    fs.Write(bytes, 0, len);

                    fs.Close();

                    rtxt = "&bNewLine=true&sFileName=" + fileName + "&sFileURL=" + Request.Url.GetLeftPart(UriPartial.Authority) + "/mobile/thumb/" + fileNames;
                }
                catch (Exception e)
                {
                    rtxt = e.ToString();
                }
            }

            return rtxt;
        }

        public ActionResult FileUploader(string isFeature, string target)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            string url = "callback_func=" + Request["callback_func"];

            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file];

                //Image img123 = Image.FromStream(hpf.InputStream);

                //var img = new System.Drawing.Bitmap(img123, new Size(400, 300));

                if (hpf.ContentLength == 0)
                    continue;

                string nowDt = DateTime.Now.ToString("yyyyMMddhhmmss");

                string fileName = nowDt + "_" + Path.GetFileName(hpf.FileName);

                string resizeFile = nowDt + "_400x300_" + Path.GetFileName(hpf.FileName);

                string savedFileNmae = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/mobile/thumb/", fileName);
                string resizeSavedFileNmae = Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "/mobile/thumb/", resizeFile);

                url += "&bNewLine=true&sFileName=" + Path.GetFileName(hpf.FileName) + "&sFileURL=" + Request.Url.GetLeftPart(UriPartial.Authority) + "/mobile/thumb/" + resizeFile + "&isFeature=" + isFeature + "&target=" + target;

                hpf.SaveAs(savedFileNmae);

                //ResizeAndSave("/mobile/thumb/", "resize_" + fileName, hpf.InputStream, hpf.ContentLength, true);

                //WebImage img = new WebImage(hpf.InputStream);

                ////img.Resize(400, 300, false, true);
                ////img.Crop(0, 0, 300, 400);


                //var width = img.Width;
                //var height = img.Height;

                //if (width < 400)
                //{
                //    width = 400;
                //}

                //if (height < 300)
                //{
                //    height = 300;
                //}

                //img.Resize(width, height, false);
                //img.Crop(0, 0, height - 300, width - 400);

                //img.Save(savedFileNmae);

                int width = 400,
                    height = 300;

                if (isFeature == "pdt")
                {
                    width = 600;
                    height = 400;
                }

                Resize(savedFileNmae, resizeSavedFileNmae, width, height);
            }

            ViewBag.url = url;

            return View();
        }

        public ActionResult Photo_uploader(string path, string isFeature)
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult smart_editor2_inputarea()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult SmartEditor2Skin()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }

        public ActionResult SmartEditor2Skin_vou()
        {
            var user = Session["user"] as User;

            if (user == null)
            {
                return Content("<script>alert('잘못된 접근경로입니다.');location.href='/'</script>");
            }

            return View();
        }
    }
}