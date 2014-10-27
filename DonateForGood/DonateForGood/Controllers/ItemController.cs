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
using System.Net.Mail;

namespace DonateForGood.Controllers
{
    public class ItemController : Controller
    {
        private DonateForGoodEntities db = new DonateForGoodEntities();
        
        // GET: /Item/
        public ActionResult Index()
        {
            var itemposts = db.ItemPosts.Include(i => i.ItemCategory).Include(i => i.ItemCollectPreference).Include(i => i.Location).Include(i => i.ItemStatu).Include(i => i.User);
            return View(itemposts.ToList());
        }

        // GET: /Item/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPost itempost = db.ItemPosts.Find(id);
            if (itempost == null)
            {
                return HttpNotFound();
            }
            return View(itempost);
        }

        private IEnumerable<SelectListItem> AddDefaultOption(IEnumerable<SelectListItem> list, string dataTextField, string selectedValue)
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem() { Text = dataTextField, Value = selectedValue });
            items.AddRange(list);
            return items;
        }


        // GET: /Item/Create
        public ActionResult Create()
        {
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName");
            //ViewBag.ItemCategoryId_ = db.ItemCategories.Select(x => x).ToList().Select(x => new SelectListItem
            //{
            //    Value = x.ItemCategoryId.ToString(),
            //    Text = x.ItemCategoryName.ToString()
            //});
            //ViewBag.ItemCategoryId_ = AddDefaultOption(ViewBag.ItemCategoryId_, "Select Category", "0");
            

            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName");
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName");
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName");
            return View();
        }

        // POST: /Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="ItemPostId,ItemCategoryId_,LocationId_,ItemName,PhoneNumber,EmailAddress_,Quantity,Description,Photo1,Photo2,Photo3,UserTypeId,CreatedOn,ModifiedOn,ItemStatusId,ContactByEmail,ContactByPhone,DisplayMyPhoneNo,DisplayMyEmail_,ItemCollectPreferenceId")] ItemPost itempost)
        {
            if (ModelState.IsValid)
            {                  
                DonateForGood.Models.User user = new Models.User
                {
                    UserTypeId = 2,
                    PhoneNumber = itempost.PhoneNumber,
                    EmailAddress = itempost.EmailAddress_
                };
                int userId = 0;
                var query = (from tableUser in db.Users
                                 where (tableUser.PhoneNumber == user.PhoneNumber || tableUser.EmailAddress == user.EmailAddress)
                                  select tableUser.UserId);
                if (!(query.Any()))
                {
                    db.Users.Add(user);
                    db.SaveChanges();
                    userId = (from tableUser in db.Users
                              where (tableUser.PhoneNumber == user.PhoneNumber || tableUser.EmailAddress == user.EmailAddress)
                              select tableUser.UserId).First();
                }
                else
                {
                    userId = (from tableUser in db.Users
                              where (tableUser.PhoneNumber == user.PhoneNumber || tableUser.EmailAddress == user.EmailAddress)
                              select tableUser.UserId).First();
                }
                  
                itempost.UserId = userId;
                db.ItemPosts.Add(itempost);
                db.SaveChanges();

                string toAddress = itempost.EmailAddress_.ToString();
                string subject = "DonateForGood: " + itempost.ItemName.ToString();
                string body = string.Empty;

                string pageURL = System.Web.HttpContext.Current.Request.Url.ToString();
                pageURL = pageURL.Replace("Create", string.Empty);
                string EditURL = pageURL + "Edit/" + itempost.ItemPostId.ToString();
                string DeleteURL = pageURL + "Delete/" + itempost.ItemPostId.ToString();
                body = "Please click on these links if you want to <a href='" + EditURL + "'>EDIT</a>  OR <a href='" + DeleteURL + "'>DELETE</a> your item post from DonateForGood website";
                              
                if (SendMail(toAddress, subject, body) == "Successful")
                {
                    return RedirectToAction("Index");
                }

            }
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName", itempost.ItemCategoryId_);
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);
            return View(itempost);
        }
        public string SendMail(string toList, string subject, string body)
        {

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress("ritikagarapati@gmail.com");
                message.From = fromAddress;
                message.To.Add(toList);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                smtpClient.Host = "smtp.gmail.com";   // We use gmail as our smtp client
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("ritikagarapati@gmail.com", "ritika!@#$%");

                smtpClient.Send(message);
                msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }
        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase uploadImage)
        {
            if (uploadImage.ContentLength <= 0)
            {
                return RedirectToAction("Create");
            }

            if (uploadImage.ContentLength > 0)
            {
                byte[] imageData = null;
                using (var binaryReader = new BinaryReader(uploadImage.InputStream))
                {
                    imageData = binaryReader.ReadBytes(uploadImage.ContentLength);
                }
            }
            return RedirectToAction("Create");
        }


        // GET: /Item/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPost itempost = db.ItemPosts.Find(id);
            if (itempost == null)
            {
                return HttpNotFound();
            }
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName", itempost.ItemCategoryId_);
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);
            return View(itempost);
        }

        // POST: /Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ItemPostId,ItemCategoryId_,LocationId_,ItemName,PhoneNumber,EmailAddress_,Quantity,Description,Photo1,Photo2,Photo3,UserTypeId,CreatedOn,ModifiedOn,ItemStatusId,ContactByEmail,ContactByPhone,DisplayMyPhoneNo,DisplayMyEmail_,ItemCollectPreferenceId")] ItemPost itempost)
        {
            if (ModelState.IsValid)
            {
                db.Entry(itempost).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName", itempost.ItemCategoryId_);
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);
            return View(itempost);
        }

        // GET: /Item/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ItemPost itempost = db.ItemPosts.Find(id);
            if (itempost == null)
            {
                return HttpNotFound();
            }
            return View(itempost);
        }

        // POST: /Item/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemPost itempost = db.ItemPosts.Find(id);
            db.ItemPosts.Remove(itempost);
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
