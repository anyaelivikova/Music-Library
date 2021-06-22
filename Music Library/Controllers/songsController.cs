using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Music_Library.Models;

namespace Music_Library.Controllers
{
    [Authorize]
    public class songsController : Controller
    {
        private Entities db = new Entities();
        private int GetAuthorizedUserId()
        {
            var user = db.users.SingleOrDefault(u => u.user_name == User.Identity.Name);
            if (user != null)
            {
                return user.id;
            }
            return -1;
        }
        // GET: songs
        public ActionResult Index()
        {

            var songs = db.songs.ToList().Where(s => s.user_id == GetAuthorizedUserId());
            return View(songs.ToList());
        }

        // GET: songs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var songs = db.songs.ToList().Where(s => s.user_id == GetAuthorizedUserId() && s.id == id).FirstOrDefault();
            if (songs == null)
            {
                return HttpNotFound();
            }
            return View(songs);
        }

        // GET: songs/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "id", "user_name");
            return View();
        }

        // POST: songs/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,song_name,album_name,duration,song_cover,user_id")] songs songs)
        {
            if (ModelState.IsValid)
            {
                db.songs.Add(songs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.users, "id", "user_name", songs.user_id);
            return View(songs);
        }

        // GET: songs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            songs songs = db.songs.Find(id);
            if (songs == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.users, "id", "user_name", songs.user_id);
            return View(songs);
        }

        // POST: songs/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,song_name,album_name,duration,song_cover,user_id")] songs songs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(songs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.users, "id", "user_name", songs.user_id);
            return View(songs);
        }

        // GET: songs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            songs songs = db.songs.Find(id);
            if (songs == null)
            {
                return HttpNotFound();
            }
            return View(songs);
        }

        // POST: songs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            songs songs = db.songs.Find(id);
            db.songs.Remove(songs);
            db.SaveChanges();
            return RedirectToAction("Index");
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
