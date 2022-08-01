﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using IoTAsema.Models;

namespace IoTAsema.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.LoginError = 0;
            ViewBag.EmailError = 0;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Authorize(Login LoginModel)
        {
            IoTDBEntities db = new IoTDBEntities();

            var LoggedUser = db.Login.SingleOrDefault(x => x.UserName == LoginModel.UserName && x.Password == LoginModel.Password); // Käyttäjätunnusten tarkistus
            if (LoggedUser != null)
            {
                ViewBag.LoginMessage = "Successfull login";
                ViewBag.LoggedStatus = "In";
                ViewBag.LoginError = 0;
                Session["UserName"] = LoggedUser.UserName;
                Session["LoginID"] = LoggedUser.LoginID;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.LoginMessage = "Login unsuccessfull";
                ViewBag.LoggedStatus = "Out";
                ViewBag.LoginError = 1;
                LoginModel.LoginErrorMessage = "Tuntematon käyttäjätunnus tai salasana.";
                return View("Index", LoginModel);
            }
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            ViewBag.LoggedStatus = "Logged Out";
            return RedirectToAction("Index", "Home");
        }
    }
}