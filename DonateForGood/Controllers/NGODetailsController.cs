﻿using System;
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
    public class NGODetailsController : Controller
    {
        private DonateForGoodEntities db = new DonateForGoodEntities();

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] User user)
        {
            if(ModelState.IsValid)
            {
                int userId = (from tableUser in db.Users
                              where tableUser.UserName == user.UserName
                              select tableUser.UserId).First();

            }
            return View();
        }

        // GET: NGODetails
        public ActionResult Index()
        {
            var getNGOList = (from tableUser in db.Users
                                  join tableNgoList in db.NGODetails
                                  on tableUser.UserId equals tableNgoList.UserId
                                  
                              select new NGORegistration()
                              {
                                  FirstName = tableUser.FirstName,
                                  PhoneNumber = tableUser.PhoneNumber,
                                  EmailAddress = tableUser.EmailAddress,
                                  WebSiteURL = tableNgoList.WebSiteURL,
                                  CityName = tableNgoList.City.CityName,
                                  UserId = tableUser.UserId

                              }).ToList();

            return View(getNGOList.ToList());
        }

        // GET: NGODetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGODetail nGODetail = db.NGODetails.Find(id);           

            // get NGO Detail
            var getNGODetail = (from tableUser in db.Users
                              join tableNgoList in db.NGODetails
                              on tableUser.UserId equals tableNgoList.UserId
                                where tableUser.UserId == (int)id
                              select new NGORegistration()
                              {
                                    UserId = tableUser.UserId,
                                    Description = tableNgoList.Description,
                                    WebSiteURL = tableNgoList.WebSiteURL,
                                    ContactNumber1 =tableNgoList.ContactNumber1,                                    
                                    Photo = tableNgoList.Photo,
                                    ContactPersonName =tableNgoList.ContactPersonName,
                                    Address1 =tableNgoList.Address1,
                                    Address2 =tableNgoList.Address2,
                                    CityName =tableNgoList.City.CityName,
                                    CountryName=tableNgoList.Country.CountryName,
                                    StateName=tableNgoList.State.StateName,
                                    LocationName=tableNgoList.Location.LocationName,
                                    Zipcode=tableNgoList.Zipcode,
                                    Landmark=tableNgoList.Landmark,
                                    UserTypeId=tableUser.UserTypeId,
                                    FirstName=tableUser.FirstName,
                                    LastName=tableUser.LastName,
                                    PhoneNumber=tableUser.PhoneNumber,
                                    EmailAddress=tableUser.EmailAddress
      
                                    }).ToList();

            if (getNGODetail == null)
            {
                return HttpNotFound();
            }
            return View(getNGODetail.ToList());
        }

        // GET: NGODetails/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName");
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName");
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName");
            ViewBag.StateId = new SelectList(db.States, "StateId", "StateName");
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName");
            ViewBag.SecurityQuestionID = new SelectList(db.SecurityQuestions, "SecurityQuestionID", "QuestionTitle");
            return View();
        }

        // POST: NGODetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserName,Password,SecurityQuestionID,SecurityAnswer,FirstName,Description,WebSiteURL,PhoneNumber,ContactNumber1,ContactPersonName," +
            "EmailAddress,Address1,Address2,Landmark,LocationId,CityId,Zipcode,StateId,CountryId")] NGORegistration nGORegisration)
        {

            if (ModelState.IsValid)
            {
            DonateForGood.Models.User user = new Models.User
            {
                FirstName=nGORegisration.FirstName,
                LastName = nGORegisration.FirstName,
                UserName=nGORegisration.UserName,
                UserTypeId=2,
                Password=nGORegisration.Password,
                SecurityQuestionID = nGORegisration.SecurityQuestionID,
                SecurityAnswer = nGORegisration.SecurityAnswer,
                PhoneNumber=nGORegisration.PhoneNumber,
                EmailAddress=nGORegisration.EmailAddress

            };
            db.Users.Add(user);
            db.SaveChanges();

            int userId = (from tableUser in db.Users
                       where tableUser.FirstName == user.FirstName
                       select tableUser.UserId).First();

            byte[] imageValue = null;

            if (Session["imageData"] != null)
            {
                imageValue = Session["imageData"] as byte[];
            }
            
            NGODetail ngoDetails = new NGODetail
            {
                UserId = userId,
                Description = nGORegisration.Description,
                WebSiteURL = nGORegisration.WebSiteURL,
                ContactNumber1=nGORegisration.ContactNumber1,
                ContactPersonName=nGORegisration.ContactPersonName,
                Address1=nGORegisration.Address1,
                Address2 = nGORegisration.Address2,
                Landmark=nGORegisration.Landmark,
                LocationId = nGORegisration.LocationId,
                CityId=nGORegisration.CityId,
                Zipcode=nGORegisration.Zipcode,
                StateId=nGORegisration.StateId,
                CountryId=nGORegisration.CountryId,
                Photo=imageValue
            };


            db.NGODetails.Add(ngoDetails);
            db.SaveChanges();
                
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", nGORegisration.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", nGORegisration.CountryId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", nGORegisration.LocationId);
            ViewBag.StateId = new SelectList(db.States, "StateId", "StateName", nGORegisration.StateId);

            return View(nGORegisration);
        }

        // GET: NGODetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGODetail nGODetail = db.NGODetails.Find(id);
            if (nGODetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", nGODetail.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", nGODetail.CountryId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", nGODetail.LocationId);
            ViewBag.StateId = new SelectList(db.States, "StateId", "StateName", nGODetail.StateId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", nGODetail.UserId);
            return View(nGODetail);
        }

        // POST: NGODetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserId,Description,WebSiteURL,ContactNumber1,ContactNumber2,Photo,ContactPersonName,Address1,Address2,CityId,LocationId,StateId,CountryId,Zipcode,Landmark")] NGODetail nGODetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(nGODetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "CityName", nGODetail.CityId);
            ViewBag.CountryId = new SelectList(db.Countries, "CountryId", "CountryName", nGODetail.CountryId);
            ViewBag.LocationId = new SelectList(db.Locations, "LocationId", "LocationName", nGODetail.LocationId);
            ViewBag.StateId = new SelectList(db.States, "StateId", "StateName", nGODetail.StateId);
            ViewBag.UserId = new SelectList(db.Users, "UserId", "FirstName", nGODetail.UserId);
            return View(nGODetail);
        }

        // GET: NGODetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            NGODetail nGODetail = db.NGODetails.Find(id);
            if (nGODetail == null)
            {
                return HttpNotFound();
            }
            return View(nGODetail);
        }

        // POST: NGODetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            NGODetail nGODetail = db.NGODetails.Find(id);
            db.NGODetails.Remove(nGODetail);
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
