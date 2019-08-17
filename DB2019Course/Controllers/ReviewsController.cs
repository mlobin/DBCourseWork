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
    public class ReviewsController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index(int? id)
        {
            if (id == null || !db.Aspirant.Any(x=>x.Pass==id)) //проверяем на существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var review = db.Review.Include(r => r.Disser).Where(x=>x.AuthorPass==id); //отзывы только этому автору
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Name = (string)output.Value;//Своей процедурой передаем имя-фамилию
            ViewBag.Id = id;
            return View(review.ToList());
        }

        public ActionResult Details(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //проверяем существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disser dis = db.Disser.Where(x => x.AuthorPass == author).SingleOrDefault(); //находим диссертацию
            Review review = db.Review.Find(id, dis.Id, dis.AuthorPass);//находим отзыв
            if (review == null) //не нашли? Ошибка!
                return HttpNotFound();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", author, output);
            ViewBag.Name = (string)output.Value;//Своей процедурой передаем имя-фамилию
            return View(review);
        }

        public ActionResult Create(int? id)
        {
            if (id == null || !db.Aspirant.Any(x => x.Pass == id) || !db.Disser.Any(x => x.AuthorPass == id))
                return HttpNotFound(); //если нет аспиранта или диссертации с таким ID - ошибка
            ViewBag.DisserId = db.Disser.Where(x => x.AuthorPass == id).SingleOrDefault().Id;
            ViewBag.AuthorPass = id; //передаем диссертацию и автора в представление
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,DisserId,AuthorPass,ReviewAuthor,Organisation")] Review review)
        {
            if (ModelState.IsValid) //если все введено верно
            {
                db.Review.Add(review); //добавляем в БД
                db.SaveChanges(); //сохраняем изменения
                return RedirectToAction("Index", new {id = review.AuthorPass }); //и к списку
            }
            return View(review); //иначе все сноваы
        }

        public ActionResult Edit(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //проверяем существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disser dis = db.Disser.Where(x => x.AuthorPass == author).SingleOrDefault(); //находим диссертацию
            Review review = db.Review.Find(id, dis.Id, dis.AuthorPass);//находим отзыв
            if (review == null) //не нашли? Ошибка!
                return HttpNotFound();
            return View(review);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,DisserId,AuthorPass,ReviewAuthor,Organisation")] Review review)
        {
            if (ModelState.IsValid)//если все введено верно
            {
                db.Entry(review).State = EntityState.Modified; //ставим состояние "изменено"
                db.SaveChanges(); //сохраняем изменения
                return RedirectToAction("Index", new { id = review.AuthorPass }); //и к списку
            }
            return View(review); //иначе все по-новой
        }

        // GET: Reviews/Delete/5
        public ActionResult Delete(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //проверяем существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disser dis = db.Disser.Where(x => x.AuthorPass == author).SingleOrDefault(); //находим диссертацию
            Review review = db.Review.Find(id, dis.Id, dis.AuthorPass);//находим отзыв
            if (review == null) //не нашли? Ошибка!
                return HttpNotFound();
            return View(review);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int author, int id)
        {
            Disser dis = db.Disser.Where(x => x.AuthorPass == author).SingleOrDefault(); //находим диссертацию
            Review review = db.Review.Find(id, dis.Id, dis.AuthorPass); //и отзыв
            if (review == null)  //не нашли? Ошибка
                return HttpNotFound();
            db.Review.Remove(review); //удаляем отзыв
            db.SaveChanges(); //сохраняем изменения
            return RedirectToAction("Index", new {id = review.AuthorPass }); //к списку
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
