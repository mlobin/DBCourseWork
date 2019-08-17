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
    public class VotingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Votings
        public ActionResult Index(int? author, int? id)
        {
            if (author==null || id == null) //проверяем на существование или ошибка
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Disser d = db.Disser.Where(x => x.AuthorPass == author).SingleOrDefault(); //находим диссертацию
            Defence def = db.Defence
                .Where(x => (x.DisserId == d.Id && (x.Number == id) && x.AuthorPass == author))
                .SingleOrDefault(); //и защиту
            Voting voting = db.Voting
                .Where(x => x.DisserId == def.DisserId && x.AuthorPass == def.AuthorPass && x.DefenceId == def.Number)
                .SingleOrDefault(); //и голосование
            if (voting != null)
                return View("Details", voting); //если уже есть голосование, то смотрим его
            ViewBag.Defence = def; 
            return View("Create");//иначе создаем новое
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "VoteNumber,Members,Arrived,Pro,Contra,Not_Voted,DisserId,AuthorPass,DefenceId")] Voting voting)
        {
            if (ModelState.IsValid) //если все введено верно
            {
                db.Voting.Add(voting); //добавили в БД 
                db.SaveChanges(); //сохранили изменения
                return RedirectToAction("Index", new {id = voting.DefenceId, author = voting.AuthorPass }); //и к деталям
            }

            return View(voting); //иначе все по-новой
        }

        public ActionResult Print(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x => x.Pass == author)) //проверка на существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            Voting voting = db.Voting.Where(x => x.AuthorPass == author && x.DefenceId == id).SingleOrDefault();
            if (voting == null) //находим голосование
                return HttpNotFound();
            Aspirant authorObj = voting.Defence.Disser.Work.Aspirant;
            Work workObj = voting.Defence.Disser.Work;
            Leader leaderObj = authorObj.Leader.ToList()[0];
            ViewBag.Ability = "Состав диссертационного совета утверждён в количестве "+voting.Members + " человек.";
            ViewBag.Result = (voting.Pro > voting.Contra) ? "" : "не";
            ViewBag.Arr = "Присутствовали на заседании "+voting.Arrived+" человек.";
            ViewBag.Author =  authorObj.Name+" "+authorObj.SurName+" "+authorObj.LastName;
            ViewBag.Theme = "по теме \""+workObj.Name+"\" по специальности "+voting.Defence.Disser.Nomenclature;
            ViewBag.Leader = "Научный руководитель " + leaderObj.Degree.ToLower() + ", " + leaderObj.Position.ToLower() 
                + " " + leaderObj.Name + " " + leaderObj.Surname + " " + leaderObj.Lastname;
            ViewBag.Opponents = voting.Defence.Opponent 
                + ", " + voting.Defence.Opponent2 + " дали положительные отзывы на диссертацию.";
            string cnt = voting.Defence.Disser.Review.Count < 5 ? " отзыва " : " отзывов";
            ViewBag.Reviews = "Всего поступило " + voting.Defence.Disser.Review.Count + cnt;
            ViewBag.ResultCnt = "\"За\" - "+voting.Pro + ", \"Против\" - "+voting.Contra+", недействительных бюллетеней - "+voting.Not_Voted;
            ViewBag.Nomenclature = voting.Defence.Disser.Nomenclature; //генерируем необходимый текст, подробнее см. схему
            ViewBag.Num = voting.VoteNumber;
            ViewBag.Date = voting.Defence.Date.ToShortDateString();
            return View(voting);
        }

        public ActionResult Edit(int? author, int? id)
        {
            if (id == null || author == null || !db.Aspirant.Any(x=>x.Pass==author)) //проверяем на существование
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest); //нет - ошибка
            Voting voting = db.Voting.Where(x=>x.AuthorPass==author&&x.DefenceId==id).SingleOrDefault(); //находим голосование
            if (voting == null) //не нашли?
                return HttpNotFound(); //ошибка
            return View(voting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "VoteNumber,Members,Arrived,Pro,Contra,Not_Voted,DisserId,AuthorPass,DefenceId")] Voting voting)
        {
            if (ModelState.IsValid) //Если все верно введено
            { 
                db.Entry(voting).State = EntityState.Modified; //состояние БД изменено
                db.SaveChanges(); //сохраняем изменения
                return RedirectToAction("Index", new { id = voting.Defence.Number, author = voting.AuthorPass }); //и к жеталям
            }
            return View(voting); //Иначе все по-новой
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
