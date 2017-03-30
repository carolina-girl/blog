using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using static Blog.Models.Helpers;
using PagedList;
using PagedList.Mvc;
using System.IO;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    //[RequireHttps]
    public class BlogPostsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: BlogPosts/Index
        public ActionResult Index(int? page)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.FName = db.Users.Find(User.Identity.GetUserId()).FirstName;
            }
            int pageSize = 3;       // the number of posts you want to display per page
            int pageNumber = (page ?? 1);

            var listPosts = db.BlogPosts.AsQueryable();
            return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Index(string searchStr, int? page)
        {
            var listPosts = db.BlogPosts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchStr))
            {
                listPosts = listPosts.Where(p => p.Title.Contains(searchStr) ||
                                                 p.Body.Contains(searchStr) ||
                                                 p.Comments.Any(c => c.Body.Contains(searchStr) ||
                                                                     c.Author.FirstName.Contains(searchStr) ||
                                                                     c.Author.LastName.Contains(searchStr) ||
                                                                     c.Author.DisplayName.Contains(searchStr) ||
                                                                     c.Author.Email.Contains(searchStr)));
            }
            int pageSize = 3; //shows 3 blog posts at a time on this page
            int pageNumber = (page ?? 1);
            return View(listPosts.OrderByDescending(p => p.Created).ToPagedList(pageNumber, pageSize));
        }

  
        // GET: BlogPosts/Details/5
        public ActionResult Details(string Slug)
        {
            if (string.IsNullOrWhiteSpace(Slug))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.FirstOrDefault(p => p.Slug == Slug);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }



        [Authorize(Roles = "Admin, Moderator")]
        // GET: BlogPosts/Create
        public ActionResult Create()
        {
            return View();
        }



        // POST: BlogPosts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {

            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }

            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", " ");
                    return View(blogPost);
                }
                if (db.BlogPosts.Any(p => p.Slug == Slug))
                {
                    ModelState.AddModelError("Title", "The title must be unique.");
                    return View(blogPost);
                }
                if (image != null)
                {
                    //relative server path
                    var filePath = "/fileUpload/";
                    //path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    blogPost.MediaURL = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                blogPost.Slug = Slug;
                blogPost.Created = DateTimeOffset.Now;
                db.BlogPosts.Add(blogPost);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blogPost);
        }

        [Authorize(Roles = "Admin, Moderator")]
        // GET: BlogPosts/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(Id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Body,MediaURL,Published")] BlogPost blogPost, HttpPostedFileBase image)
        {
            if (image != null && image.ContentLength > 0)
            {
                //check the file name to make sure its an image
                var ext = Path.GetExtension(image.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif" && ext != ".bmp")
                    ModelState.AddModelError("image", "Invalid Format.");
            }
            if (ModelState.IsValid)
            {
                var Slug = StringUtilities.URLFriendly(blogPost.Title);
                if (String.IsNullOrWhiteSpace(Slug))
                {
                    ModelState.AddModelError("Title", "Invalid title.");
                    return View(blogPost);
                }
                //if (db.BlogPosts.Any(p => p.Slug == Slug))
                //{
                //    ModelState.AddModelError("Title", "The title must be unique.");
                //    return View(blogPost);
                //}
                if (image != null)
                {
                    //relative server path
                    var filePath = "/fileUpload/";
                    //path on physical drive on server
                    var absPath = Server.MapPath("~" + filePath);
                    // media url for relative path
                    blogPost.MediaURL = filePath + image.FileName;
                    //save image
                    image.SaveAs(Path.Combine(absPath, image.FileName));
                }
                blogPost.Slug = Slug;
                db.Entry(blogPost).State = EntityState.Modified;
                blogPost.Created = DateTimeOffset.Now;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(blogPost);
        }


        [Authorize(Roles = "Admin, Moderator")]
        // GET: BlogPosts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BlogPost blogPost = db.BlogPosts.Find(id);
            if (blogPost == null)
            {
                return HttpNotFound();
            }
            return View(blogPost);
        }

        [Authorize(Roles = "Admin, Moderator")]
        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            BlogPost blogPost = db.BlogPosts.Find(id);
            db.BlogPosts.Remove(blogPost);
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

