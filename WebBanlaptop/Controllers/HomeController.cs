﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebBanlaptop.Models;

namespace WebBanlaptop.Controllers
{
    public class HomeController : Controller
    {
        QLBANLAPTOPEntities db = new QLBANLAPTOPEntities();
        public ActionResult Index()
        {
            var lstSPNB = db.SANPHAM.Where(n => n.SPNOIBAT == true);
            ViewBag.lstSPNB = lstSPNB;
            var lstSPM = db.SANPHAM.Where(n => n.SANPHAMMOI == true);
            ViewBag.lstSPM = lstSPM;
            return View();
        }
        public ActionResult navbarPartial()
        {
            var lstSP = db.SANPHAM;
            return PartialView(lstSP);
        }

    }
}