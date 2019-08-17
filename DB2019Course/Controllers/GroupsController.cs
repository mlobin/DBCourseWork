using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DB2019Course.Models;

namespace DB2019Course.Controllers
{
    public class GroupsController : Controller
    {
        private Entities db = new Entities();

        [AllowAnonymous]                
        public ActionResult Index()
        {
            return View(db.Group.ToList()); //Просто список всех групп
        }

        [MyAuthorize]                       //доступ для админов и преподавателей
        public ActionResult Details(int? id)
        {
            if (id == null)                 //Если id не передан
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);     //бросаем эксепшен
            }
            Group group = db.Group.Find(id); //ищем группу
            if (group == null) //если не нашли
            {
                return HttpNotFound(); //тоже бросаем эксепшен
            }
            return View(group);
        }

        [MyAuthorize(Roles ="Admin")]  //Добавлять может только админ
        public ActionResult Create()
        {
            return View();//просто представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyAuthorize(Roles = "Admin")] //на всякий случай тоже обернем в фильтр авторизации
        public ActionResult Create([Bind(Include = "Id,Faculty,Stage,Name,Specialty")] Group group)
        {
            if (ModelState.IsValid) 
            {
                db.Group.Add(group); //добавляем группу
                db.SaveChanges();    
                return RedirectToAction("Index"); //переходим к списку
            }

            return View(group); //иначе еще раз
        }

        [MyAuthorize(Roles = "Admin")] //редактировать тоже только админ
        public ActionResult Edit(int? id) 
        {
            if (id == null)//Если id не передан
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //бросаем эксепшен
            }
            Group group = db.Group.Find(id); //находим группу
            if (group == null) 
            {
                return HttpNotFound();
            }
            return View(group); //передаем в представление
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [MyAuthorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "Id,Faculty,Stage,Name,Specialty")] Group group)
        {
            if (ModelState.IsValid) //если все данные введены
            {
                db.Entry(group).State = EntityState.Modified; //обозначаем группу как редактированную
                db.SaveChanges();
                return RedirectToAction("Index"); //уходим к списку
            }
            return View(group); //иначе еще раз редактируем
        }

        [MyAuthorize(Roles = "Admin")] //Удаляет тоже админ
        public ActionResult Delete(int? id)
        {
            if (id == null)//Если id не передан
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = db.Group.Find(id); //Ищем группу
            if (group == null)
            {
                return HttpNotFound();
            }
            if (group.Aspirant.Count > 0) //если в группе есть аспиранты
                return View("DeleteImpossible",group); //сообщаем о невозможности
            return View(group); //иначе удаляем
        }

        [MyAuthorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = db.Group.Find(id); //находим
            db.Group.Remove(group); //удаляем
            db.SaveChanges(); //сливаем обновления в БД 
            return RedirectToAction("Index"); //уходим к списку
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
