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
    public class IndPlansController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index(int? id)  
        {
            if (id == null || !db.Aspirant.Any(x=>x.Pass==id)) //Если нет такого аспиранта
                return HttpNotFound(); //ошибка

            var indPlan = db.IndPlan.Include(i => i.Aspirant).Where(x=>x.AspirantId==id);//только планы этого
                                                                                        //аспиранта
            ViewBag.AspirantId = id; //передадим в представление
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Aspirant = (string)output.Value; //и своей процедурой имя-фамилию-отчество
            return View(indPlan.ToList());
        }

        public ActionResult Create(int? id)
        {
            if (id == null || !db.Aspirant.Any(x => x.Pass == id)) //Если нет такого аспиранта
                return HttpNotFound(); //ошибка
            ViewBag.AspirantId = id; //передадим в представление
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Aspirant = (string)output.Value; //и своей процедурой имя-фамилию-отчество
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AspirantId,WorkName,Size,DueDate,DoneMarker,Readiness,Year")] IndPlan indPlan)
        { 
            if (ModelState.IsValid) //Если все введено верно
            {
                db.IndPlan.Add(indPlan); //добавляем элемент плана
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index", new {id = indPlan.AspirantId }); //и к списку
            }

            ViewBag.AspirantId = indPlan.AspirantId; //иначе
            return View(indPlan); //все по-новой
        }

        public ActionResult Edit(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author))  //если нет такого 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);     //автора или плана - ошибка!
            IndPlan indPlan = db.IndPlan.Find(id, author); //ищем элемент плана
            if (indPlan == null) //не нашли?
                return HttpNotFound(); //ошибка!
            ViewBag.AspirantId = author;  //передаем в представление
            return View(indPlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AspirantId,WorkName,Size,DueDate,DoneMarker,Readiness,Year")] IndPlan indPlan)
        {
            if (ModelState.IsValid) //если все введено верно
            {
                db.Entry(indPlan).State = EntityState.Modified; //состояние - изменено
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index", new {id = indPlan.AspirantId }); //и к списку
            }
            ViewBag.AspirantId = indPlan.AspirantId; //иначе
            return View(indPlan); //все снова
        }

        public ActionResult Delete(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author))  //если нет такого 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);     //автора или плана - ошибка!
            IndPlan indPlan = db.IndPlan.Find(id, author); //ищем элемент плана
            if (indPlan == null) //не нашли?
                return HttpNotFound(); //ошибка!
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Aspirant = (string)output.Value;//имя-фамилию своей процедурой передаем в представление
            return View(indPlan); 
        }

        // POST: IndPlans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int author, int id)
        {
            IndPlan indPlan = db.IndPlan.Find(id, author); //находим элемент плана
            db.IndPlan.Remove(indPlan); //удаляем
            db.SaveChanges(); //сохраняем изменения
            return RedirectToAction("Index", new {id = author } ); //и к списку
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
