using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DB2019Course.Models;

namespace DB2019Course.Controllers
{
    [MyAuthorize]
    public class DissersController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index(int? id)
        {
            if (id == null) //нет ид?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            Disser disser = db.Disser.Where(x => x.AuthorPass == id).SingleOrDefault(); //находим диссертацию
            if (disser == null) //не нашли?
            {
                ViewBag.AuthorPass = id;
                return View("Create"); //значит, создаем
            }
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Name = (string)output.Value; //иначе забираем своей процедурой имя-фамилию
            return View("Details", disser); //И смотрим детали
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AuthorPass,Nomenclature,Readiness")] Disser disser)
        {
            if (ModelState.IsValid) //если все верно заполнено
            {
                Work work = new Work();
                work.UDC = Request.Form["Work.UDC"]; //создаем и заполняем поля работы
                work.Name = Request.Form["Work.Name"];
                work.AuthorPass = int.Parse(Request.Form["AuthorPass"]);
                work.Department = Request.Form["Work.Department"];
                work.Theme = Request.Form["Work.Theme"];
                work = db.Work.Add(work); //работу сливаем в БД
                db.SaveChanges();
                disser.Id = work.Id; //ИД
                disser.AuthorPass = work.AuthorPass; //И пропуск автора как ключ в диссертацию
                db.Disser.Add(disser); //добавляем диссертацию
                db.SaveChanges();//сливаем изменения
                return RedirectToAction("Index", new {id = disser.AuthorPass }); //и к списку
            }
            return View(disser);
        }

        public ActionResult Edit(int? id) 
        {
            if (id == null) //нет ид?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);//ошибка!
            }
            Disser disser = db.Disser.Where(x=>x.AuthorPass == id).SingleOrDefault(); //находим диссертацию
            if (disser == null)
            {
                return HttpNotFound();
            }
            return View(disser); //редактируем её
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AuthorPass,Nomenclature,Readiness")] Disser disser)
        {
            if (ModelState.IsValid) //Если все верно внесли
            {
                disser = db.Disser.Where(x => x.AuthorPass == disser.AuthorPass).SingleOrDefault(); 
                var t = disser.Work;//находим релевантную работу
                t.Name = Request.Form["Work.Name"];
                t.Theme = Request.Form["Work.Theme"]; //заполняем работу
                t.UDC = Request.Form["Work.UDC"];
                t.Department = Request.Form["Work.Department"];
                disser.Nomenclature = Request.Form["Nomenclature"];
                disser.Readiness = Int32.Parse(Request.Form["Readiness"]);
                disser.Id = t.Id;
                disser.Work = disser.Work;
                db.Entry(t).State = EntityState.Modified;
                db.Entry(disser).State = EntityState.Modified; //выставляем состояние изменено
                db.SaveChanges();//накатываем изменения
                return RedirectToAction("Index", new {id = disser.AuthorPass }); //и к списку
            }
            return View(disser);//иначе все по-новой
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
