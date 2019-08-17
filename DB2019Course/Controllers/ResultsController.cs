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
    public class ResultsController : Controller
    {
        private Entities db = new Entities();

        // GET: Results
        public ActionResult Index(int? id)
        {
            var result = db.Result.Include(r => r.Aspirant).Include(r => r.Exam).Where(x=>x.Aspirant.Pass==id);
            ViewBag.AspirantId = id; //Находим оценки, передаем id в представление
            return View(result.ToList());
        }

        public ActionResult Exam(int? id)
        {
            var result = db.Result.Include(r => r.Aspirant).Include(r => r.Exam).Where(x => x.Exam.Id == id);
            ViewBag.ExamId = id; //находим оценки для конкретного экзамена
            return View("Index",result.ToList());
        }

        public ActionResult Create(int? id)
        {
            var t = db.Aspirant.ToList();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            for (int i = 0; i < t.Count; i++)
            {
                db.JoinNames("Aspirant", t[i].Pass, output);
                t[i].Name = (string)output.Value +" гр. "+ t[i].Group.Name;
            }
            ViewBag.PassId = new SelectList(t, "Pass", "Name"); //своей процедурой собираем список из
            ViewBag.ExamId = id; //тех, кто может получить оценку за экзамен
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PassId,ExamId,Result1")] Result result)
        {
            if (ModelState.IsValid) //Если все введено верно
            {
                db.Result.Add(result); //добавляем оценку
                db.SaveChanges(); //сохраняем БД
                return RedirectToAction("Exam", new {id = result.ExamId }); //И к экзамену
            }

            var t = db.Aspirant.ToList();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            for (int i = 0; i < t.Count; i++)
            {
                db.JoinNames("Aspirant", t[i].Pass, output);
                t[i].Name = (string)output.Value + " гр. " + t[i].Group.Name;
            }
            ViewBag.PassId = new SelectList(t, "Pass", "Name");
            ViewBag.ExamId = result.ExamId; //иначе все по-новой
            return View(result);
        }

        public ActionResult Edit(int? author, int? id)
        {
            if (id == null || author==null || !db.Aspirant.Any(x=>x.Pass==author)) //если нет автора или экзамена
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка
            Result result = db.Result.Find(author, id); //находим оценку
            if (result == null) //нет оценки? ошиька
                return HttpNotFound();
            ViewBag.PassId = author; //передаем аспиранта и экзамен
            ViewBag.ExamId = id;
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PassId,ExamId,Result1")] Result result)
        {
            if (ModelState.IsValid) //Если все введено верно
            {
                db.Entry(result).State = EntityState.Modified; //выставляем состояние бд
                db.SaveChanges(); //сохраняем БД
                return RedirectToAction("Index", new {id = result.PassId});//К списку
            }
            ViewBag.PassId = result.PassId;
            ViewBag.ExamId = result.ExamId;//Иначе все по-новой
            return View(result);
        }

        public ActionResult Delete(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //если нет автора или экзамена
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка
            Result result = db.Result.Find(author, id); //находим оценку
            if (result == null) //нет оценки? ошибка
                return HttpNotFound();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", author, output);
            ViewBag.Aspirant = (string)output.Value; //передаем имя-фамилию своей процедурой
            return View(result);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int author, int id)
        {
            Result result = db.Result.Find(id, author); //Удаление подтвердиили
            db.Result.Remove(result); //находим и удаляем оценку
            db.SaveChanges(); //сохраняем БД
            return RedirectToAction("Index", new { id }); //И к списку
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
