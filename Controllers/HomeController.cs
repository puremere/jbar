﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jbar.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult driver()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult loadowner()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult help()
        {
            return View();
        }
        public ActionResult support()
        {
            return View();
        }

       
    }
}