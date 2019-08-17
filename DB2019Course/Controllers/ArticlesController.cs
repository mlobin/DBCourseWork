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
    [MyAuthorize()]
    public class ArticlesController : Controller
    {
        private Entities db = new Entities();
        [AllowAnonymous]
        public ActionResult Index(int? id) //список статей автора с id
        {
            if (id is null || !db.Aspirant.Any(x=>x.Pass==id))  //если нет такого аспиранта
                return HttpNotFound();
            var article = db.Article.Include(a => a.Work);      //Забираем статьи
            article = article.Where(x => x.AuthorPass == id);   //обрезаем только с таким автором
            ViewBag.AuthorId = id;                              //передаем в представление для ActionLink
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", id, output);
            ViewBag.Name = (string)output.Value; //Забираем имя-фамилию-отчество своей процедурой
            return View(article.ToList());       //отдаем список в представление
        }

        public ActionResult Details(int? author, int? id) //О конкретной статье автора
        {
            if (id == null || author == null) //если чего-то нет, кидаем ошибку
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var article = db.Article.Find(id, author); //ищем статью
            if (article == null) //не нашли - ошибку
                return HttpNotFound();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", article.AuthorPass, output);
            ViewBag.Name = (string)output.Value; //передаем имя-фамилию-отчество своей процедурой
            return View(article); //передаем модель в представление
        }

        public ActionResult Create(int? id) //создаем статью автору
        {
            if (id == null || !db.Aspirant.Any(x=>x.Pass == id)) //если нет такого автора
                return HttpNotFound();                          //ошибку
            ViewBag.AuthorId = id;                              //передаем автора для ActionLink
            List<string> enums = new List<string>();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            foreach (var t in db.Leader.ToList()) //Создаем список
            {
                db.JoinNames("Leader", t.PassId, output);
                enums.Add((string)output.Value + " (пропуск №"+t.PassId.ToString()+")");
            } //вписываем туда всех руководителей
            enums.Add("Нет");
            ViewBag.Id = new SelectList(enums, "Нет"); //и вариант без соавтора
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,AuthorPass,Journal,Issue,ISBN,CoauthorPass,Year")] Article article)
        {
            Work work = new Work(); //создаем работу
            work.UDC = Request.Form["Work.UDC"];
            work.Name = Request.Form["Work.Name"];
            work.AuthorPass = int.Parse(Request.Form["AuthorPass"]);
            work.Department = Request.Form["Work.Department"];
            work.Theme = Request.Form["Work.Theme"];
            work = db.Work.Add(work); //заполняем параметры и добавляем
            db.SaveChanges(); 
            article.Id = work.Id; //указываем Id для внешнего ключа 
            article.AuthorPass = work.AuthorPass; //И пропуск автора
            string pass = (string)Request.Form["CoauthorPas"]; //разбираем строку соавтора
            article.CoauthorPass = (pass == "Нет") ? null 
                : (int?)Int32.Parse(pass.Substring(pass.IndexOf('№') + 1).TrimEnd(')')); //Собираем число после знака номера
                                                                                         //и без последней закр. скобки
            db.Article.Add(article); //добавляем статью
            db.SaveChanges();
            return RedirectToAction("Index", new {id = work.AuthorPass }); //и к списку статей
        }

        public ActionResult Edit(int? author, int? id)
        {
            if (id == null || author == null) //если нет автора или статьи
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //кидаем ошибку
            }
            Article article = db.Article.Find(id, author); //ищем статью
            if (article == null)
            {
                return HttpNotFound(); //не нашли - ошибка!
            }
            List<string> enums = new List<string>();
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            foreach (var t in db.Leader.ToList()) 
            {
                db.JoinNames("Leader", t.PassId, output);
                enums.Add((string)output.Value + " (пропуск №" + t.PassId.ToString() + ")");
            }//снова список руководителей
            enums.Add("Нет");
            ViewBag.Id = new SelectList(enums, "Нет");//и вариант без соавтора
            return View(article);
        }

        [AllowAnonymous]
        public ActionResult Leader(int? id)
        {
            if (id is null || !db.Leader.Any(x => x.PassId == id))  //проверяем, не передал ли кто-то невалидный id 
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);         //и передан ли он вообще

            ViewBag.LeaderId = id;  //передадим в представление id
            var articles = db.Article.Include(a => a.Leader).Where(x => x.Leader.PassId == id);
            return View("Index", articles.ToList()); //передаем статьи, где этот человек - соавтор
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,AuthorPass,Journal,Issue,ISBN,CoauthorPass,Year")] Article article)
        {
            var v = Request.Form;
            if (ModelState.IsValid)//если все верно заполнено
            {
                var t = db.Work.Find(article.Id, article.AuthorPass); //находим работу
                t.Name = Request.Form["Work.Name"];                   //её тоже надо отредактировать
                t.Theme = Request.Form["Work.Theme"];
                t.UDC = Request.Form["Work.UDC"];
                t.Department = Request.Form["Work.Department"];
                string pass = (string)Request.Form["CoauthorPas"];
                article.CoauthorPass = (pass == "Нет") ? null
                    : (int?)Int32.Parse(pass.Substring(pass.IndexOf('№') + 1).TrimEnd(')')); //Собираем число после знака номера
                                                                                             //и без последней закр. скобки
                db.Entry(t).State = EntityState.Modified;//вешаем на работу и статью
                db.Entry(article).State = EntityState.Modified; //статус "изменено"
                db.SaveChanges();
                return RedirectToAction("Index", new {id = t.AuthorPass }); //скидываем изменения и к списку
            }
            ViewBag.Id = new SelectList(db.Work, "Id", "Name", article.Id);  //иначе еще раз
            return View(article);
        }

        public ActionResult Delete(int? author, int? id)
        { 
            if (id == null || author == null) //если нет автора или статьи - ошибка
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Article article = db.Article.Find(id, author);
            if (article == null) //если не нашли статью - ошибка
            {
                return HttpNotFound();
            }
            ObjectParameter output = new ObjectParameter("result", typeof(string));
            db.JoinNames("Aspirant", article.AuthorPass, output);
            ViewBag.Name = (string)output.Value;//передаем имя-фамилию-отчество для представления
            return View(article);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? author, int id)
        {
            Article article = db.Article.Find(id, author); //удаление подтверждено
            db.Article.Remove(article); //находим и удаляем статью
            Work work = db.Work.Find(id, author); //потом то же самое
            db.Work.Remove(work); //с работой
            db.SaveChanges(); // накатываем изменения
            return RedirectToAction("Index", new {id = author }); //и к списку работ
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
