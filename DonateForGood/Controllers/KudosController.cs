using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DonateForGood.Models;
using System.IO;

namespace DonateForGood.Controllers
{
    public class KudosController : Controller
    {
        private DonateForGoodEntities db = new DonateForGoodEntities();




        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            byte[] imageData = null;

            if (file.ContentLength <= 0)
            {
                return RedirectToAction("Create", "Kudos");
            }

            if (file.ContentLength > 0)
            {

                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    imageData = binaryReader.ReadBytes(file.ContentLength);
                }
            }

            Session["imageDataKudos"] = imageData;
            Session["imageFileNameKudos"] = file.FileName;

            return RedirectToAction("Create", "Kudos");


        }

        public ActionResult UploadImage()
        {
            return View();
        }

        // GET: /Kudos/
        public ActionResult Index()
        {
            var KudosList = (from tableKudos in db.Kudos
                             join tableUser in db.Users
                             on tableKudos.User.UserId equals tableUser.UserId
                             select new KudosList()
                             {
                                 Comment = tableKudos.Comment,
                                 FirstName = tableUser.FirstName,
                                 Photo = tableKudos.Photo

                             }).ToList();

            return View(KudosList);  
            
        }

        // GET: /Kudos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kudo kudo = db.Kudos.Find(id);
            if (kudo == null)
            {
                return HttpNotFound();
            }
            return View(kudo);
        }

        // GET: /Kudos/Create
        public ActionResult Create(string ItemPostId,string NGOUserId)
        {
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName");
            if (!String.IsNullOrEmpty(ItemPostId)) {
                ViewBag.ItemPostId = ItemPostId;
            }
            if (!String.IsNullOrEmpty(NGOUserId))
            {
                ViewBag.NGOUserId = NGOUserId;
            }            
            return View();
        }




        // POST: /Kudos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="KudosId,Comment,Photo,NGOUserId,ItemPostId")] Kudo kudo)
        {
            

            if (ModelState.IsValid)
            {
                
                kudo.Photo = Session["imageDataKudos"] as byte[];
                db.Kudos.Add(kudo);
                db.SaveChanges();
                Session["imageDataKudos"] = null;
                Session["imageFileNameKudos"] = null;
                return RedirectToAction("Index");
            }
            Session["imageDataKudos"] = null;
            Session["imageFileNameKudos"] = null;
            ViewBag.ItemPostId = new SelectList(db.ItemPosts, "ItemPostId", "ItemName", kudo.ItemPostId);
            ViewBag.NGOUserId = new SelectList(db.Users, "UserId", "FirstName", kudo.NGOUserId);
            return View(kudo);
        }

        // GET: /Kudos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kudo kudo = db.Kudos.Find(id);
            if (kudo == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemPostId = new SelectList(db.ItemPosts, "ItemPostId", "ItemName", kudo.ItemPostId);
            ViewBag.NGOUserId = new SelectList(db.Users, "UserId", "FirstName", kudo.NGOUserId);
            return View(kudo);
        }

        // POST: /Kudos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="KudosId,Comment,Photo,NGOUserId,ItemPostId")] Kudo kudo)
        {
            if (ModelState.IsValid)
            {
                db.Entry(kudo).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemPostId = new SelectList(db.ItemPosts, "ItemPostId", "ItemName", kudo.ItemPostId);
            ViewBag.NGOUserId = new SelectList(db.Users, "UserId", "FirstName", kudo.NGOUserId);
            return View(kudo);
        }

        // GET: /Kudos/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Kudo kudo = db.Kudos.Find(id);
            if (kudo == null)
            {
                return HttpNotFound();
            }
            return View(kudo);
        }

        // POST: /Kudos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Kudo kudo = db.Kudos.Find(id);
            db.Kudos.Remove(kudo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        public ActionResult _KudosListPartial()
        {
            var KudosList = (from tableKudos in db.Kudos
                                  join tableUser in db.Users
                                  on tableKudos.User.UserId equals tableUser.UserId
                                  select new KudosList()
                                  {
                                      Comment = tableKudos.Comment,
                                      FirstName = tableUser.FirstName,
                                      Photo=tableKudos.Photo

                                  }).ToList();

            return View(KudosList);            
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
