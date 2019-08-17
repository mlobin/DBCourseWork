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
    public class LeadersController : Controller
    {
        private Entities db = new Entities();

        // GET: Leaders
        public ActionResult Index()
        {
            return View(db.Leader.ToList()); //Просто выводим
        }

        public ActionResult Aspi(int? id)
        {
            if (id==null || !db.Aspirant.Any(y => y.Pass == id)) //нет айди или аспиранта?
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //ошибка!
            ViewBag.PassId = id; //передаем в представление айди для ActionLink
            return View("Index",db.Leader.Where(x=>x.Aspirant.Any(y=>y.Pass==id)).ToList()); //и передаем только
                                                                                  //рук-лей этого аспиранта
        }

        public ActionResult Find()
        {
            return View(); //просто поле поиска
        }

        [HttpPost]
        public ActionResult Find(string name)
        {
            var res = db.Leader.Where(x => (x.Name.Contains(name) || x.Lastname.Contains(name) || x.Surname.Contains(name)));
            return View("Index", res.ToList()); //показываем то, где поиск есть в имени, отчестве или фамилии
        }


        public ActionResult Details(int? id)
        {
            if (id == null) //если нет id
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);//ошибка
            Leader leader = db.Leader.Find(id); //ищем руководителя
            if (leader == null) //не нашли?
                return HttpNotFound(); //ошибка!
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Leader", leader.PassId, output);
            ViewBag.Leader = (string)output.Value; //и имя-фамилию-отчество руководителя в представление
            return View(leader); 
        }

        public ActionResult Create(int? id)
        {
            ViewBag.Id = id;
            return View(); //передаем id в представление
        }

        public ActionResult News(int? id)
        {
            ViewBag.aspiId = id;
            List<string> enums = new List<string>();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            foreach (var t in db.Leader.Where(x=>!x.Aspirant.Any(y=>y.Pass==id)).ToList())
            {
                db.JoinNames("Leader", t.PassId, output);
                enums.Add((string)output.Value + " (пропуск №" + t.PassId.ToString() + ")");
            }//Собираем своей процедурой все имена руководителей в список
            enums.Add("Добавить");
            ViewBag.Id = new SelectList(enums, "Добавить"); //плюс "новый"
            return View(); //и выбираем
        }

        [HttpPost]
        public ActionResult News()
        {
            string leader = Request.Form["Name"];
            if (leader == "Добавить")
                return Create(int.Parse(Request.Form["AspiId"]));
            var i =  (int?)Int32.Parse(leader.Substring(leader.IndexOf('№') + 1).TrimEnd(')')); //Собираем число после знака номера
                                                                                                //и без последней закр. скобки
            Leader lead = db.Leader.Find(i);
            Aspirant aspirant = db.Aspirant.Find(int.Parse(Request.Form["AspiId"])); //добавляем руководителя
            lead.Aspirant.Add(aspirant);        //аспиранту и наоборот
            db.Entry(lead).State = EntityState.Modified; //выставляем состояния в БД
            db.Entry(aspirant).State = EntityState.Modified;
            db.SaveChanges(); //сохраняем изменения
            return RedirectToAction("Details","Aspirants",new {id = aspirant.Pass }); //И к конкретному аспиранту
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PassId,Department,Degree,Title,Name,Surname,Lastname,Position")] Leader leader)
        {
            if (ModelState.IsValid)//если все введено верно
            {
                db.Leader.Add(leader); //добавляем руководителя
                db.SaveChanges(); //сохраняем
                return RedirectToAction("Index"); //и к списку
            }

            return View(leader);//иначе все по-новой
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)         //нет id - ошибка
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Leader leader = db.Leader.Find(id); //ищем руководителя
            if (leader == null) //не нашли? ошибка!
                return HttpNotFound();
            return View(leader);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PassId,Department,Degree,Title,Name,Surname,Lastname,Position")] Leader leader)
        {
            if (ModelState.IsValid) //все введено верно
            {
                db.Entry(leader).State = EntityState.Modified; //руководитель изменен
                db.SaveChanges(); //накатываем изменения
                return RedirectToAction("Index"); //и к списку
            }
            return View(leader); //иначе все по-новой
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)         //нет id - ошибка
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Leader leader = db.Leader.Find(id); //ищем руководителя
            if (leader == null) //не нашли? ошибка!
                return HttpNotFound();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Leader", leader.PassId, output);
            ViewBag.Leader = (string)output.Value;//своей процедурой передаем в представление имя-фамилию
            return View(leader);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Leader leader = db.Leader.Find(id);//удаление подтверждено, находим руководителя
            db.Leader.Remove(leader); //удаляем его
            db.SaveChanges(); //сохраняем
            return RedirectToAction("Index"); //К списку
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
