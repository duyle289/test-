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

    }   
}