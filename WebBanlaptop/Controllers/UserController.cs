using System;
using System.Linq;
using System.Web.Mvc;
using WebBanlaptop.Models;
using System.Security.Cryptography;
using System.Text;

namespace WebBanlaptop.Controllers
{
    public class UserController : Controller 
    {
        public static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
        QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
        // GET: User
        [HttpGet]
        public ActionResult DangKy()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DangKy(FormCollection f, KHACHHANG kh)
        {
            var userexist = db.KHACHHANG.Any(x => x.USERNAME == kh.USERNAME);
            var emailexist = db.KHACHHANG.Any(x => x.EMAIL == kh.EMAIL);
            var sdtexist = db.KHACHHANG.Any(x => x.SDT == kh.SDT);
            var cccdexist = db.KHACHHANG.Any(x => x.CCCD == kh.CCCD);
            if (ModelState.IsValid)
            {
                if (userexist)
                {
                    ModelState.AddModelError("USERNAME", "Tên tài khoản đã tồn tại");
                    return View(kh);
                }    
                if(emailexist)
                {
                    ModelState.AddModelError("EMAIL", "Email đã được đăng ký mời nhập emmil khác");
                    return View(kh);
                }
                if (sdtexist)
                {
                    ModelState.AddModelError("SDT", "SĐT đã được đăng ký mời nhập SĐT khác");
                    return View(kh);
                }
                if (cccdexist)
                {
                    ModelState.AddModelError("CCCD", "CCCD đã được đăng ký mời nhập CCCD khác");
                    return View(kh);
                }
                if(kh.SDT.Length != 10)
                {
                    ModelState.AddModelError("SDT", "Phải nhập đủ 10 số");
                }    
                if(kh.CCCD.Length != 9 && kh.CCCD.Length != 12)
                {
                    ModelState.AddModelError("CCCD", "CCCD hoặc là 9 số hoặc là 12 số");
                }    
                else
                {
                    string mk = f["PASSWORD"].ToString();
                    string xnmk = f["NHAPLAIPASSWORD"].ToString();
                    if (mk == xnmk)
                    {
                        string pass = MD5Hash(mk);
                        kh.PASSWORD = pass;
                        db.KHACHHANG.Add(kh);
                        db.SaveChanges();
                       return RedirectToAction("Index","Home");
                    }
                    else
                    {
                        ViewBag.thongbao = "mật khẩu không trung khớp !!!!!!";
                        return View();
                    }  
                }
            }
            return View();
        }
        private static string urlAfterLogin; // lưu lại link đang ở trước khi nhấn đăng nhập
        #region đăng nhập
        [HttpGet]
        public ActionResult Login(string strURL)
        {
            // kiểm tra đường dẫn có null không, tránh tình trạng paste trực tiếp đường link vào url sẽ không lưu được trả về trang chủ
            if (!String.IsNullOrEmpty(strURL))
            {
                urlAfterLogin = strURL;
            }
            else
            {
                RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            var username = collection["username"];
            var password = collection["password"];
            var user = db.KHACHHANG.SingleOrDefault(p => p.USERNAME == username);
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Vui lòng điền đầy đủ nội dung";
                return this.Login(urlAfterLogin);
            }
            else if (user == null)
            {
                ViewData["Error"] = "invalid username";
                return this.Login(urlAfterLogin);
            }
            else if (!String.Equals(password, user.PASSWORD))
            {
                ViewData["Error"] = "invalid password";
                return this.Login(urlAfterLogin);
            }
            else
            {
                Session["User"] = user;
                return Redirect(urlAfterLogin);
            }
        }
        #endregion
        #region đăng xuất
        public ActionResult LogOut() // đăng xuất
        {
            Session["User"] = null;
            urlAfterLogin = null;
            return RedirectToAction("Index", "Home");
        }
        #endregion
        
    }   
}
