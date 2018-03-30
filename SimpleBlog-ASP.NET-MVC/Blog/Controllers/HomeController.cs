using System;
using Blog.Models;
using System.Web.Mvc;
using System.Linq;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {        
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

   
        public ActionResult Tutorials()
        {
            return View();
        }
    }
}