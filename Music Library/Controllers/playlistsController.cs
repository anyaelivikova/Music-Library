using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Music_Library.Models;

namespace Music_Library.Controllers
{
    [Authorize]
    public class playlistsController : Controller
    {
        private Entities db = new Entities();
        private int GetAuthorizedUserId()
        {
            var user = db.users.SingleOrDefault(u => u.user_name == User.Identity.Name);
            if(user != null)
            {
                return user.id;
            }
            return -1;
        }
        public ActionResult SongList(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var songs = db.playlists;
            if (songs == null)
            {
                return HttpNotFound();
            }
            return View(songs);
        }
        // GET: playlists
        public ActionResult Index()
        {
            
             var playlists = db.playlists.ToList().Where(p => p.user_id == GetAuthorizedUserId());

            return View(playlists.ToList());
        }

        // GET: playlists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var playlists = db.playlists.ToList().Where(p => p.user_id == GetAuthorizedUserId() && p.id == id).FirstOrDefault();
            if (playlists == null)
            {
                return HttpNotFound();
            }
            return View(playlists);
        }
        public ActionResult GetImage(int id)
        {
            // fetch image data from database
            byte[] imageData = db.playlists.Find(id).playlist_cover;
            if(imageData == null)
            {
                return null;
            }


            return File(imageData, "image/jpg");
        }
        // GET: playlists/Create
        public ActionResult Create()
        {
            ViewBag.user_id = new SelectList(db.users, "id", "user_name");
            return View();
        }

        // POST: playlists/Create
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,playlist_name,playlist_cover,user_id")] playlists playlists)
        {
            if (ModelState.IsValid)
            {
                db.playlists.Add(playlists);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.user_id = new SelectList(db.users, "id", "user_name", playlists.user_id);
            return View(playlists);
        }

        // GET: playlists/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            playlists playlists = db.playlists.Find(id);
            if (playlists == null)
            {
                return HttpNotFound();
            }
            ViewBag.user_id = new SelectList(db.users, "id", "user_name", playlists.user_id);
            return View(playlists);
        }

        // POST: playlists/Edit/5
        // Чтобы защититься от атак чрезмерной передачи данных, включите определенные свойства, для которых следует установить привязку. Дополнительные 
        // сведения см. в разделе https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,playlist_name,user_id")] playlists playlists)
        {

            HttpPostedFileBase file = Request.Files["playlist_cover"];
            
            byte[] imageBytes;
            BinaryReader reader = new BinaryReader(file.InputStream);
            imageBytes = reader.ReadBytes(file.ContentLength);
            playlists.playlist_cover = imageBytes;


            if (ModelState.IsValid)
            {
                db.Entry(playlists).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.user_id = new SelectList(db.users, "id", "user_name", playlists.user_id);
            return View(playlists);
        }

        // GET: playlists/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            playlists playlists = db.playlists.Find(id);
            if (playlists == null)
            {
                return HttpNotFound();
            }
            return View(playlists);
        }

        // POST: playlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            playlists playlists = db.playlists.Find(id);
            db.playlists.Remove(playlists);
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
