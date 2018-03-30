using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        private BlogDataEntities db = new BlogDataEntities();

        public ActionResult Index()
        {
            var posts = from p in db.Post
                        select p;
            
            return View(posts.ToList());
        }

        public ActionResult CreatePost()
        {
            var user = HttpContext.User.Identity.Name;

            if (user == "")
            {
                TempData["toLoginAllert"] = "To create post please log in.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        public ActionResult Category()
        {
            var posts = from p in db.Post
                        select p;

            return View(posts.ToList());
        }
    }
}