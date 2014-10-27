using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DonateForGood.Models;

namespace DonateForGood.Controllers
{
    public class HomeController : Controller
    {
        private DonateForGoodEntities db = new DonateForGoodEntities();


        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login([Bind(Include = "UserName,Password")] User user)
        {
            if(ModelState.IsValid)
            {
                var getUserDetails = (from tableUser in db.Users
                                      where tableUser.UserName == user.UserName && tableUser.Password == user.Password
                                      select tableUser
                                    );
                if(getUserDetails==null)
                {
                  // user is invalid
                }
                else
                {
                    IEnumerable<User> getUserId = (from tableUser in db.Users
                                      where tableUser.UserName == user.UserName && tableUser.Password == user.Password
                                      select tableUser
                                    );
                    Session["LoggedInUserId"] = getUserId.First().UserId;
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult RecentActivityPartial()
        {
            var RecentActivity = (from tableItemPost in db.ItemPosts
                                  join tableUser in db.Users
                                  on tableItemPost.PhoneNumber equals tableUser.PhoneNumber
                                  orderby tableItemPost.ModifiedOn descending
                                  select new RecentActivity()
                                  {
                                      ItemName = tableItemPost.ItemName,
                                      FirstName = tableUser.FirstName,
                                      LastName = tableUser.LastName,
                                      ModifiedOn = tableItemPost.ModifiedOn

                                  }).ToList();
                         

            
           

            return View(RecentActivity);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}