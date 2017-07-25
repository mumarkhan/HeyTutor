using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EadWebProject.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;

namespace EadWebProject.Controllers
{
    public class ProfilesController : Controller
    {
        private EadWebProjectContext db = new EadWebProjectContext();

        // GET: Profiles
        public ActionResult Index(String SearchOptions, String SearchToken, int? page)
        {

            if (SearchOptions != null && SearchToken != null)
            {


                SearchToken = SearchToken.ToUpper();
                if (SearchOptions == "Name")
                {
                    return View(db.Profiles.Include(p => p.User).Where(x => x.FirstName.ToUpper().Contains(SearchToken)).ToList().ToPagedList(page ?? 1, 12));
                }
                if (SearchOptions == "City")
                {
                    return View(db.Profiles.Include(p => p.User).Where(x => x.Address.ToUpper().Contains(SearchToken)).ToList().ToPagedList(page ?? 1, 12));
                }
                if (SearchOptions == "Education")
                {
                    return View(db.Profiles.Include(p => p.User).Where(x => x.Education.ToUpper().Contains(SearchToken)).ToList().ToPagedList(page ?? 1, 12));
                }
                if (SearchOptions == "Experience")
                {
                    return View(db.Profiles.Include(p => p.User).Where(x => x.Experiance.ToUpper().Contains(SearchToken)).ToList().ToPagedList(page ?? 1, 12));
                }
            }

            var profiles = db.Profiles.Include(p => p.User);
            return View(profiles.ToList().ToPagedList(page ?? 1, 12));
        }

        // GET: Profiles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Profile profile = db.Profiles.Find(id);
            int noOfLikes = db.Likes.Where(l => l.Status == true).Count();
            int noOfDisLikes = db.Likes.Where(l => l.Status == false).Count();

            ProfileWithLikes pfwl = new ProfileWithLikes();
            pfwl.MyProfile = profile;
            pfwl.NoOfLikes = noOfLikes;
            pfwl.NoOfDislikes = noOfDisLikes;
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(pfwl);
        }

        // GET: Profiles/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.Users, "UserID", "PasswordHash");
            return View();
        }

        // POST: Profiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]        
        public ActionResult Create([Bind(Include = "UserId,FirstName,LastName,Education,Experiance,Address,ContactNumber,ProfilePic")] Profile profile)
        {
            if (Session["login"] == null)
                return Redirect("/Account/Login");

            String hash = profile.ProfilePic.GetHashCode() + "";
            List<User> users = db.Users.Where(x => x.UserID.ToUpper().Equals(profile.UserID)  && x.PasswordHash.Equals(hash)).ToList();
            if (users.Count == 0)
                return Redirect("/Account/Login");


            var uniqueName = "";
            if (Request.Files["Image"] != null)
            {
                var file = Request.Files["Image"];
                if (file.FileName != "")
                {
                    var ext = System.IO.Path.GetExtension(file.FileName);

                    profile.ProfilePic = uniqueName = Guid.NewGuid().ToString() + "__4__" + file.FileName;

                    var rootPath = Server.MapPath("~/UploadedFiles");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    file.SaveAs(fileSavePath);

                }
            }



            if (ModelState.IsValid)
            {
                db.Profiles.Add(profile);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.Users, "UserID", "PasswordHash", profile.UserID);
            return View(profile);
        }

        //// GET: Profiles/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Profile profile = db.Profiles.Find(id);
        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "UserID", "PasswordHash", profile.UserID);
        //    return View(profile);
        //}

        // POST: Profiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ProfileID,UserID,FirstName,LastName,Education,Experiance,Address,ContactNumber,ProfilePic")] Profile profile)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(profile).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserID = new SelectList(db.Users, "UserID", "PasswordHash", profile.UserID);
        //    return View(profile);
        //}

        // GET: Profiles/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Profile profile = db.Profiles.Find(id);
        //    if (profile == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(profile);
        //}

        //// POST: Profiles/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Profile profile = db.Profiles.Find(id);
        //    db.Profiles.Remove(profile);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Upload()
        {
            return View();
        }




        [HttpPost]
        public ActionResult SaveFile2()
        {





            var uniqueName = "";
            if (Request.Files["myfile"] != null)
            {
                var file = Request.Files["myfile"];
                if (file.FileName != "")
                {


                    //////////////////////the following code first give a unique name to the uploaded file then sanve it
                    //////////////////////while the other code given under this code just javethe file with its same name


                    var ext = System.IO.Path.GetExtension(file.FileName);

                    uniqueName = Guid.NewGuid().ToString() + ext;

                    var rootPath = Server.MapPath("~/UploadedImages");

                    var fileSavePath = System.IO.Path.Combine(rootPath, uniqueName);

                    file.SaveAs(fileSavePath);


                    //////////////////////
                    //////////////////////





                    //////////////////////
                    //////////////////////
                    //////////////////////the following code just save the file

                    //var fileName = Path.GetFileName(file.FileName);

                    //var path = Path.Combine(Server.MapPath("~/UploadedFiles"), fileName);
                    //file.SaveAs(path);

                }
            }

            var data = new
            {
                success = true,
                name = uniqueName

            };
            String userid = (String)Session["login"];
            Profile p = db.Profiles.Where(x => x.UserID.Equals(userid)).First();
            p.ProfilePic = uniqueName;
            db.SaveChanges();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }




}


