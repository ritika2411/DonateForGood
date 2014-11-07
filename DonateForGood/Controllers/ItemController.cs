using System;
using System.Collections.Generic;
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
        private byte[] imageData = null;



        //public ActionResult Create1()
        //{
        //    return View();
        //}

        

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase file)
        {
            
            if (file.ContentLength <= 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (file.ContentLength > 0)
            {
                
                using (var binaryReader = new BinaryReader(file.InputStream))
                {
                    imageData = binaryReader.ReadBytes(file.ContentLength);
                }
            }

            Session["imageData"] = imageData;
            Session["imageFileName"] = file.FileName;

            return RedirectToAction("Index","Home");


        }

        public ActionResult UploadImage()
        {
            return View();
        }


        // GET: /Item/
        public ActionResult Index(string searchString, string ItemCategoryId_, string LocationId_, string ItemStatusId)
        {

            int itemCategoryId = 0;
            int LocationId = 0;
            int itemStatus = 0; 
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName");
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName");
            var itemposts = db.ItemPosts.Include(i => i.ItemCategory).Include(i => i.ItemCollectPreference).Include(i => i.Location).Include(i => i.ItemStatu).Include(i => i.User);
            if (!String.IsNullOrEmpty(searchString))
            {
                itemposts = itemposts.Where(s => s.ItemName.ToUpper().Contains(searchString.ToUpper())
                                       || s.Description.ToUpper().Contains(searchString.ToUpper()));
                TempData["searchString"] = searchString;

            }
            if (!String.IsNullOrEmpty(ItemCategoryId_))
            {
                Int32.TryParse(ItemCategoryId_, out itemCategoryId);
                itemposts = itemposts.Where(s => s.ItemCategoryId_.Equals(itemCategoryId));
                TempData["ItemCategoryId"] = ItemCategoryId_;
            }
            if (!String.IsNullOrEmpty(LocationId_))
            {
                Int32.TryParse(LocationId_, out LocationId);
                itemposts = itemposts.Where(s => s.LocationId_.Equals(LocationId));
                TempData["LocationId"] = LocationId_;
            }
            if (!String.IsNullOrEmpty(ItemStatusId))
            {
                Int32.TryParse(ItemStatusId, out itemStatus);
                itemposts = itemposts.Where(s => s.ItemStatusId==itemStatus);
            }           

            return View(itemposts.ToList());
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["Id"] = id; 
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
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName");
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName");
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName");

            int userId = 0;
            if (Session["LoggedInUserId"] != null)
            {
                userId = int.Parse(Session["LoggedInUserId"].ToString());
            }
            if (userId > 0)
            {
                var PhoneNumber = (from tableUser in db.Users
                                   where (tableUser.UserId == userId)
                                   select tableUser.PhoneNumber).First();
                var EmailAddress = (from tableUser in db.Users
                                    where (tableUser.UserId == userId)
                                    select tableUser.EmailAddress).First();

                ViewBag.PhoneNumber = PhoneNumber;
                ViewBag.EmailAddress = EmailAddress;
            }

            return View();
        }

        // POST: /Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ItemPostId,ItemCategoryId_,LocationId_,ItemName,PhoneNumber,EmailAddress_,Quantity,Description,Photo1,Photo2,Photo3,UserTypeId,CreatedOn,ModifiedOn,ItemStatusId,ContactByEmail,ContactByPhone,DisplayMyPhoneNo,DisplayMyEmail_,ItemCollectPreferenceId")] ItemPost itempost)
        {
            if (ModelState.IsValid)
            {
                itempost.UserId = GetUserId(itempost);
                //itempost.Photo1 = imageData;
                itempost.Photo1 = Session["imageData"] as byte[];
                db.ItemPosts.Add(itempost);
                db.SaveChanges();
                Session["imageData"] = null;
                Session["imageFileName"] = null;
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
                    return RedirectToAction("Create","Item");
                }

            }
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName", itempost.ItemCategoryId_);
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);
            
            return View(itempost);
        }

        // GET: /Item/Create
        public ActionResult QuickCreate()
        {
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName");
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName");
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName");
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName");

            int userId = 0;
            if (Session["LoggedInUserId"] != null)
            {
                userId = int.Parse(Session["LoggedInUserId"].ToString());
            }
            if (userId > 0)
            {
                var PhoneNumber = (from tableUser in db.Users
                                   where (tableUser.UserId == userId)
                                   select tableUser.PhoneNumber).First();
                var EmailAddress = (from tableUser in db.Users
                                    where (tableUser.UserId == userId)
                                    select tableUser.EmailAddress).First();

                ViewBag.PhoneNumber = PhoneNumber;
                ViewBag.EmailAddress = EmailAddress;
            }

            return View();
        }

        // POST: /Item/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult QuickCreate([Bind(Include = "ItemPostId,ItemCategoryId_,LocationId_,ItemName,PhoneNumber,EmailAddress_,Quantity,Description,Photo1,Photo2,Photo3,UserTypeId,CreatedOn,ModifiedOn,ItemStatusId,ContactByEmail,ContactByPhone,DisplayMyPhoneNo,DisplayMyEmail_,ItemCollectPreferenceId")] ItemPost itempost)
        {
            if (ModelState.IsValid)
            {
                itempost.UserId = GetUserId(itempost);
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
                    return RedirectToAction("Index", "Home");
                }

            }
            ViewBag.ItemCategoryId_ = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName", itempost.ItemCategoryId_);
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);

            return View(itempost);
        }

        public ActionResult SendEmail(string toList, string subject, string EmailDescription)
        {
            SendMail(toList, subject, EmailDescription);
            return RedirectToAction("Details", new { Id = TempData["Id"] });
        }

        public string SendMail(string toList, string subject, string body)
        {

            MailMessage message = new MailMessage();
            SmtpClient smtpClient = new SmtpClient();
            string msg = string.Empty;
            try
            {
                MailAddress fromAddress = new MailAddress("GHCDonateForGood@gmail.com");
                message.From = fromAddress;
                message.To.Add(toList);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                smtpClient.Host = "smtp.gmail.com";   // We use gmail as our smtp client
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential("GHCDonateForGood@gmail.com", "ABCD!@#$");

                smtpClient.Send(message);
                msg = "Successful";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        public int GetUserId(ItemPost itempost)
        {
            int userId = 0;
            if (Session["LoggedInUserId"] != null)
            { 
                userId = int.Parse(Session["LoggedInUserId"].ToString());
            }
            if (userId == 0)
            {
                DonateForGood.Models.User user = new Models.User
                {
                    UserTypeId = 2,
                    PhoneNumber = itempost.PhoneNumber,
                    EmailAddress = itempost.EmailAddress_
                };
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
            }
            return userId;   
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
            ViewBag.LocationId_ = new SelectList(db.Locations, "LocationId", "LocationName", itempost.LocationId_);
            ViewBag.ItemName = itempost.ItemName;
            ViewBag.PhoneNumber = itempost.PhoneNumber;
            ViewBag.EmailAddress = itempost.EmailAddress_;
            ViewBag.Quantity = itempost.Quantity;
            ViewBag.Description = itempost.Description;
            ViewBag.ItemCollectPreferenceId = new SelectList(db.ItemCollectPreferences, "ItemCollectPreferenceId", "ItemCollectPreferenceName", itempost.ItemCollectPreferenceId);
            ViewBag.ItemStatusId = new SelectList(db.ItemStatus, "ItemStatusId", "ItemStatusName", itempost.ItemStatusId);
            return View(itempost);
        }

        // POST: /Item/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ItemPostId,ItemCategoryId_,LocationId_,ItemName,PhoneNumber,EmailAddress_,Quantity,Description,Photo1,Photo2,Photo3,UserTypeId,CreatedOn,ModifiedOn,ItemStatusId,ContactByEmail,ContactByPhone,DisplayMyPhoneNo,DisplayMyEmail_,ItemCollectPreferenceId")] ItemPost itempost)
        {
            if (ModelState.IsValid)
            {
                itempost.UserId = GetUserId(itempost);
                db.Entry(itempost).State = EntityState.Modified;
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
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index","Home");
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

        public ActionResult LoadItems()
        {
            ViewBag.ItemsCategory = new SelectList(db.ItemCategories, "ItemCategoryId", "ItemCategoryName");
            return View();
        }

    }
}
