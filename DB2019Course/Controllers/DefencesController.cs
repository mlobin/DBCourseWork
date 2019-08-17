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
    public class DefencesController : Controller
    {
        private Entities db = new Entities();

        // GET: Defences
        public ActionResult Index(int? id) //защиты для аспиранта с таким ID
        {
            if (id == null || !db.Aspirant.Any(x=>x.Pass==id)) //если нет такого аспиранта
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            var defence = db.Defence.Include(d => d.Disser).Where(x=>x.AuthorPass==id); //только с этим автором
            ViewBag.AuthorPass = id;      //передаем в представление для ActionLink
            return View(defence.ToList()); //и возвращаем представление
        }

        public ActionResult Details(int? author, int? id) //защита с конкретным номером для аспиранта
        {
            if (id == null || author == null || !db.Aspirant.Any(x=>x.Pass==author)) //если хоть чего-то нет
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            int uid = db.Disser.Where(x => x.AuthorPass == author).Single().Work.Id; //забираем Id диссертации
            Defence defence = db.Defence.Find(id, author, uid); //находим защиту
            if (defence == null) //если не нашли
            {
                return HttpNotFound(); //ошибка!
            }
            return View(defence); //показываем защиту
        }

        public ActionResult Create(int? id) //добавляем защиту для автора
        {
            if (id == null || !db.Aspirant.Any(x => x.Pass == id)) //нет автора?
            { 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            ViewBag.DisserId = db.Disser.Where(x => x.AuthorPass == id).Single().Id; //находим Id
            ViewBag.AuthorPass = id;// передаем в представление
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Number,AuthorPass,DisserId,Date,Opponent,Opponent2,Result,City,Organisation,Building,Auditory")] Defence defence)
        {
            if (ModelState.IsValid) //если все вбито правильно
            {
                db.Defence.Add(defence); //добавляем защиту
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index", new {id = defence.AuthorPass }); //обратно к списку
            }

            ViewBag.DisserId = db.Disser.Where(x => x.AuthorPass == defence.AuthorPass).Single().Id; //иначе
            return View(defence); //еще раз
        }

        public ActionResult Edit(int? author, int? id) //изменяем конкретную защиту
        {
            if (author == null || !db.Aspirant.Any(x => x.Pass == author)) //нет автора?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            int uid = db.Disser.Where(x => x.AuthorPass == author).Single().Work.Id; //находим Id
            Defence defence = db.Defence.Find(id, author, uid); //находим защиту
            if (defence == null)//не нашли защиту?
            {
                return HttpNotFound();//ошибка!
            }
            ViewBag.DisserId = db.Disser.Where(x => x.AuthorPass == defence.AuthorPass).Single().Id;
            return View(defence);//передаем айди в представление и переходим туда
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Number,AuthorPass,DisserId,Date,Opponent,Opponent2,Result,City,Organisation,Building,Auditory")] Defence defence)
        {
            if (ModelState.IsValid)//Если все введено верно
            {
                db.Entry(defence).State = EntityState.Modified; //ставим статус "изменено"
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index", new { id = defence.AuthorPass}); //обратно к списку
            }
            ViewBag.DisserId = db.Disser.Where(x => x.AuthorPass == defence.AuthorPass).Single().Id;
            return View(defence); //иначе все по-новой
        }

        public ActionResult Delete(int? author, int? id) 
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //чего-то нет?
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            }
            int uid = db.Disser.Where(x => x.AuthorPass == author).Single().Work.Id; 
            Defence defence = db.Defence.Find(id, author, uid);//находим защиту
            if (defence == null) //не нашли?
            {
                return HttpNotFound(); //ошибка!
            }
            return View(defence); //показываем представление
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int author, int id)
        {
            int uid = db.Disser.Where(x => x.AuthorPass == author).Single().Work.Id;
            Defence defence = db.Defence.Find(id, author, uid); //находим защиту
            db.Defence.Remove(defence); //удаляем защиту
            db.SaveChanges(); //накатываем изменения
            return RedirectToAction("Index", new {id = author }); //к списку
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
