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
    public class AspirantsController : Controller
    {
        private Entities db = new Entities();

        [AllowAnonymous]
        public ActionResult Index(int? id)  //Передаем id группы, из которой ищем аспирантов
        {
            if (id is null || !db.Group.Any(x=>x.Id == id))  //проверяем, не передал ли кто-то невалидный id 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);         //и передан ли он вообще

            ViewBag.Group = id;  //передадим в представление id
            var aspirant = db.Aspirant.Include(a => a.Group).Where(x=>x.GroupId==id); 
            return View(aspirant.ToList());
        }

        [AllowAnonymous]
        public ActionResult Leader(int? id)
        {
            if (id is null || !db.Leader.Any(x => x.PassId == id))  //проверяем, не передал ли кто-то невалидный id 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);         //и передан ли он вообще

            ViewBag.Leader = id;  //передадим в представление id
            var aspirant = db.Aspirant.Include(a => a.Group).ToList();
               aspirant = aspirant.Where(x => x.Leader.Any(y => y.PassId == id)).ToList();

            return View("Index",aspirant);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id) //показываем аспиранта по id
        {
            if (id == null) //проверяем на валидность id
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Aspirant aspirant = db.Aspirant.Find(id);//проверяем существование такого аспиранта
            if (aspirant == null)
                return HttpNotFound();
            return View(aspirant); 
        }

        public ActionResult Create(int? id) //Создаем аспиранта в конкретную группу с id!
        {
            if (id is null || !db.Group.Any(x=>x.Id == id)) //проверяем id группы на валидность
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            ViewBag.GroupId = id; //id - ключ, так что больше 1 не будет
            ViewBag.Group = db.Group.Where(x => x.Id == id).Single().Name;                                          //на отсутствие уже проверили
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Pass,Name,LastName,SurName,JoinDate,BirthDate,Status,OutDate,GroupId")] Aspirant aspirant)
        {
            if (ModelState.IsValid)  //если все заполнено верно
            {
                db.Aspirant.Add(aspirant);//добавляем аспиранта

                db.SaveChanges();
                return RedirectToAction("Index",new { id = aspirant.GroupId});//уходим на страницу его группы
            }

            ViewBag.GroupId = aspirant.GroupId; //если не все заполнено,
            ViewBag.Group = aspirant.Group.Name;//то восстанавливаем значения
            return View(aspirant);//и открываем страницу создания еще раз
        }

        public ActionResult Edit(int? id) //редактируем аспиранта с id
        {
            if (id == null) //проверяем id на валидность
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Aspirant aspirant = db.Aspirant.Find(id); //проверяем id на существование
            if (aspirant == null)
                return HttpNotFound();
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Name", aspirant.GroupId); //передаем варианты 
                                                                                        //выбора группы
            return View(aspirant); //открываем представление редактирования
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PassId,Name,LastName,SurName,JoinDate,BirthDate,Status,OutDate,GroupId")] Aspirant aspirant)
        {
            if (ModelState.IsValid) //если все заполнено верно
            {
                db.Entry(aspirant).State = EntityState.Modified; //обновим базу данных
                db.SaveChanges();
                return RedirectToAction("Index",new { id = aspirant.GroupId}); //перейдем на страницу группы аспиранта
            }
            ViewBag.GroupId = new SelectList(db.Group, "Id", "Name", aspirant.GroupId);  //иначе восстановим данные
            return View(aspirant);  //и откроем редактирование еще раз
        }

        public ActionResult Delete(int? id) //удаляем аспиранта с id
        {
            if (id == null)  //проверяем id на валидность
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Aspirant aspirant = db.Aspirant.Find(id); //и на существование
            if (aspirant == null)
                return HttpNotFound();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Name = (string)output.Value;
            return View(aspirant); //если все хорошо, открываем страницу подтверждения
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id) 
        {
            Aspirant aspirant = db.Aspirant.Find(id); //Удаление подтвердили, так что 
            db.Aspirant.Remove(aspirant); //находим и удаляем 
            db.SaveChanges(); //накатываем изменения
            return RedirectToAction("Index", new {id = aspirant.GroupId }); //и к группе
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
