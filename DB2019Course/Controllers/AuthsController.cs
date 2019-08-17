using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DB2019Course.Models;
using Microsoft.AspNet.Identity;

namespace DB2019Course.Controllers
{
    public class AuthsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Login()
        {
            return View(); //Просто возвращаем страницу
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "Login,Password")] Auth auth)
        {
            if (ModelState.IsValid) //если все заполнено (что нетрудно(
            {
                
                var user = db.Auth.Find(auth.Login); //ищем юзера с таким логином
                if (user != null && user.Password == auth.Password && !db.Logged.Any(x=>x.Session == HttpContext.Session.SessionID))
                { //если логин подошёл и мы еще не залогинены
                    db.Logged.Add(new Logged() { Session = HttpContext.Session.SessionID, Login = auth.Login });
                    db.SaveChanges(); //залогинились и слили в БД
                }

                return RedirectToAction("Index", "Groups"); //и на главную
            }

            return View(auth);
        }

        public ActionResult NotAuthorized()
        {
            return View(); //возвращаем страницу неавторизации
        }
    }
}
