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
    [MyAuthorize]
    public class ExamsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            return View(db.Exam.ToList()); //Просто список экзаменов
        }

        public ActionResult Create()
        { 
            return View(); //просто новый экзамен
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Subject,Date,Auditory,Corp,Teacher")] Exam exam)
        {
            if (ModelState.IsValid) //если все верно
            {
                db.Exam.Add(exam); //добавляем к списку экзаменов
                db.SaveChanges(); //накатываем к списку
                return RedirectToAction("Index");
            }

            return View(exam);
        }

        public ActionResult Find()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Find([Bind(Include = "Date")] Exam exam)
        {
            var s = db.Exam.Where(x => x.Date == exam.Date).ToList(); //находим экзамен с такой датой
            return View("Index", s);
        }

        // GET: Exams/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) //если ид нет
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            Exam exam = db.Exam.Find(id); //находим экзамен
            if (exam == null) //не нашли?
            {
                return HttpNotFound(); //ошибка!
            }
            return View(exam); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Subject,Date,Auditory,Corp,Teacher")] Exam exam)
        {
            if (ModelState.IsValid) //Если все верно
            {
                db.Entry(exam).State = EntityState.Modified; //состояние - изменено
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index"); //обратно к списку
            }
            return View(exam); //иначе все по-новой
        }

        // GET: Exams/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) //нет ид?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            Exam exam = db.Exam.Find(id);//находим экзамен
            if (exam == null) //не нашли?
            {
                return HttpNotFound(); //ошибка!
            }
            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Exam exam = db.Exam.Find(id); //подтвердили удаление, потому
            db.Exam.Remove(exam); //находим и удаляем экзамен
            db.SaveChanges(); //накатываем изменения
            return RedirectToAction("Index"); //к списку
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
