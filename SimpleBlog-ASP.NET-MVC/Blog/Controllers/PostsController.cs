using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Blog.Models;
using Microsoft.AspNet.Identity;
using PagedList;

namespace Blog.Controllers
{
   
    public class PostsController : Controller
    {
        private BlogDataEntities db = new BlogDataEntities();

      
        // GET: Post
        public ActionResult Index(string sortOrder, int? CategoryId, int? page)
        {
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "Content");

            string searchString = null;
            if (CategoryId != null)
            {
                var currentCategory = db.Category.Find(CategoryId);
                searchString = currentCategory.Content; 
            }
           


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var post = from p in db.Post
                        select p;

            switch (sortOrder)
            {
              
                default: post = post.OrderBy(p => p.DatePosted); break;
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                post = post.Where(p => p.Category.Content == searchString);

                if (post.Count() == 0)
                {
                    ViewBag.NoMatch = "No match with the posts. Please enter again.";

                    return View(post.ToPagedList(pageNumber, pageSize));
                }
            }

            return View(post.ToPagedList(pageNumber, pageSize));

        }

        public ActionResult RecentPosts()
        {
            var post = db.Post.OrderByDescending(p => p.DatePosted).Take(5);
            return View(post.ToList());
        }

        // GET: Post/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["wrongIdPost"] = "No match with the posts. Please enter again.";
                return RedirectToAction("Index", "Post");
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                TempData["wrongIdPost"] = "No match with the posts. Please enter again.";
                return RedirectToAction("Index", "Post");
            }
            return View(post);
        }

        // GET: Post/Create
        public ActionResult Create()
        {
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username");
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "Content");
            ViewBag.SelectedTags = new MultiSelectList(db.Tag, "TagId", "TagName");
            return View();
        }

        // POST: Post/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PostId,UserId,Title,About,Content,DatePosted,CategoryId,SelectedTags")] Post post)
        {
            var userName = User.Identity.Name;
            User user = db.User.FirstOrDefault(u => u.Username == userName);
            var newTags = new List<Tag>();
            foreach (var id in post.SelectedTags)
            {
                var postTag = db.Tag.Find(id);
                newTags.Add(postTag);
         
            }
            post.UserId = user.UserId;
            post.User = user;
            post.Tag = newTags;
            
            if (ModelState.IsValid)
            {
                db.Post.Add(post);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", post.UserId);
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "Content", post.CategoryId);
            ViewBag.SelectedTags = new MultiSelectList(db.Tag, "TagId", "TagName", post.Tag);
            return View(post);
        }

        // GET: Post/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (HttpContext.User.Identity.Name != post.User.Username)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", post.UserId);
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "Content", post.CategoryId);
            return View(post);
        }

        // POST: Post/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PostId,UserId,Title,About,Content,DatePosted,CategoryId,Comment,Tag")] Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", post.UserId);
            ViewBag.CategoryId = new SelectList(db.Category, "CategoryId", "Content", post.CategoryId);

            TempData["changePost"] = "You modified your post";
            return RedirectToAction("Index", "Posts");
            
        }

        // GET: Post/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            Post post = db.Post.Find(id);
            if (post == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(post);
        }

        // POST: Post/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Post.Find(id);
            List<Comment> comments = post.Comment.ToList();
            comments.Clear();
            List<Tag> tags = post.Tag.ToList();
            tags.Clear();
            db.Post.Remove(post);
            db.SaveChanges();
            TempData["deletePost"] = "You deleted your post";
            return RedirectToAction("Index", "Posts");
        }

      
        public ActionResult Comment()
        {
            if (User.Identity.Name == "")
            {
                TempData["toLoginAllert"] = "To comment a post please log in.";
                return RedirectToAction("Index", "Home");
            }

 
            return View("Comment");
        }

        [HttpPost]
        public ActionResult Comment(int postId, int? userId, Comment comment)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                var user = db.User.FirstOrDefault(u => u.Username == userName);


                var newComment = db.Comment.Create();
                newComment.Content = comment.Content;
                newComment.UserId = user.UserId;
                newComment.PostId = postId;
                newComment.DatePublished = DateTime.Now;
                db.Comment.Add(newComment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserId = new SelectList(db.User, "UserId", "Username", comment.UserId);
            return View(comment);
        }

        public ActionResult Gallery()
        {
            return View();
        }

        public ActionResult AllCategories()
        {
            return View();
        }

        public ActionResult Category(int categoryId)
        {
            var post = db.Post.Where(p => p.CategoryId == categoryId).ToList();
            return View(post);

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
