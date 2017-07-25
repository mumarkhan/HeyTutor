using EadWebProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EadWebProject.Controllers
{
    public class AccountController : Controller
    {
        private EadWebProjectContext db = new EadWebProjectContext();




        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        // GET: Account
        [ActionName("Signup")]
        [HttpPost]
        public ActionResult Signup2()
        {
            try
            {
                User user = new Models.User();
                user.UserID = Request.Form["Email"];
                user.PasswordHash = "" + Request.Form["Password"].GetHashCode();
                user.RoleID = Request.Form["Role"];
                if(!user.RoleID.Equals("Tutor") && !user.RoleID.Equals("Parent"))
                {
                    throw new Exception("");
                }
                db.Users.Add(user);
                db.SaveChanges();
                return View("Login");
            }
            catch (Exception)
            {
                return View("Signup");
            }
        }


        public ActionResult Logout()
        {
            if(Session["login"] != null)
            {
                Session["login"] = null;
                Session.Abandon();
            }
            return View("Login");
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [ActionName("Login")]
        [HttpPost]
        public ActionResult Login2()
        {
            User user = new Models.User();
            user.UserID = Request.Form["Email"];
            user.PasswordHash = "" + Request.Form["Password"].GetHashCode();
            User tmpUser = db.Users.Find(user.UserID);
            if (tmpUser.PasswordHash.Equals(user.PasswordHash))
            {
                Session["login"] = user.UserID;
                Session["role"] = tmpUser.RoleID;
                return Redirect("/Profiles/Index");
            }
            return View("Login");
        }
    }
}