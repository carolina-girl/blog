﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using System.Xml.Linq;
using System.Web.UI.WebControls;

namespace Blog.Controllers
{
    [RequireHttps]
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private object slug;

        // GET: Comments
        public ActionResult Index()
        {
            return View();
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // GET: Comments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        public ActionResult Create([Bind(Include = "Id,PostId,Body,Created")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                comment.Created = DateTimeOffset.Now;
                comment.AuthorId = User.Identity.GetUserId();
                db.Comments.Add(comment);
                db.SaveChanges();
                var slug = db.BlogPosts.FirstOrDefault(p => p.Id == comment.PostId).Slug;
                return RedirectToAction("Details", "BlogPosts", new { Slug = slug });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,Body")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                db.Comments.Attach(comment);
                //db.Entry.comment.State = EntityState.Modified;
                db.Entry(comment).Property("Body").IsModified = true;
                db.SaveChanges();
                return RedirectToAction("Index", "BlogPosts", new { Slug = slug });
            }

            return View(comment);
        }

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int? id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();

            return RedirectToAction("Details", "BlogPosts");
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