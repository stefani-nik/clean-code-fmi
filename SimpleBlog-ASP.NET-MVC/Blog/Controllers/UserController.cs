using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity.Validation;
using Blog.Models;
using PagedList;
using System.Data.Entity;
using Microsoft.AspNet.Identity;

namespace Blog.Controllers
{
    [ValidateInput(false)]
    public class UserController : Controller
    {
        private BlogDataEntities db = new BlogDataEntities();

        // GET: Users
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSOrt = sortOrder;
            ViewBag.NameSortParam = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParam = sortOrder == "Date" ? "date_asc" : "Date";
            ViewBag.FirstNameParam = String.IsNullOrEmpty(sortOrder) ? "first_asc" : "";
            ViewBag.LastNameParam = String.IsNullOrEmpty(sortOrder) ? "last_asc" : "";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var users = from u in db.User
                        select u;
            
            switch (sortOrder)
            {
                case "name_desc": users = users.OrderBy(u => u.Username.ToLower()); break;
                case "Date": users = users.OrderByDescending(u => u.DateCreated); break;
                case "date_asc": users = users.OrderBy(u => u.DateCreated); break;
                case "first_asc": users = users.OrderBy(u => u.FirstName.ToLower()); break;
                case "last_asc": users = users.OrderBy(u => u.LastName.ToLower()); break;
                default: users = users.OrderBy(u => u.UserId); break;
            }

            int pageSize = 6;
            int pageNumber = (page ?? 1);

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Username.Contains(searchString) ||
                                    u.FirstName.Contains(searchString) ||
                                    u.LastName.Contains(searchString));

                if (users.Count() == 0)
                {
                    ViewBag.NoMatch = "No match with the users. Please enter again.";

                    return View(users.ToPagedList(pageNumber, pageSize));
                }
            }

            return View(users.ToPagedList(pageNumber, pageSize));
        }
        
        [HttpGet]
        public ActionResult Login()
        { return View(); }

        [HttpPost]
        public ActionResult Login(Blog.Models.LoginUser user, string returnUrl)
        {
            if (isValid(user.Username, user.Password, false))
            {
                LoginUser newUser = new LoginUser();
                newUser.Username = user.Username;

                if (ModelState.IsValid)
                {
                    FormsAuthentication.SetAuthCookie(newUser.Username, false);
                    TempData["SuccessLogin"] = "Successful login";
                    
                    return RedirectToAction("Index", "Home");
                }
            }
            
            ModelState.AddModelError("", "The username or password isn\'t correct!");
            return View(user);
        }

        [HttpGet]
        public ActionResult Register()
        { return View(); }

        [HttpPost]
        public ActionResult Register(Models.RegisterUser user)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    using (var db = new Blog.Models.BlogDataEntities())
                    {
                        var crypto = new SimpleCrypto.PBKDF2();
                        var encryptPass = crypto.Compute(user.Password);

                        var newUser = db.User.Create();

                        newUser.Username = user.Username;
                        newUser.Password = encryptPass;
                        newUser.PasswordSalt = crypto.Salt;
                        newUser.FirstName = user.FirstName;
                        newUser.LastName = user.LastName;
                        newUser.DateCreated = DateTime.Now;

                        if(isValid(newUser.Username, newUser.Password, true))
                        {
                            ModelState.AddModelError("", "The username exist in the database. Please choice another username");
                            return View();
                        }

                        db.User.Add(newUser);
                        db.SaveChanges();

                        TempData["successRegister"] = "Successful register";
                        return RedirectToAction("Login", "User");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Data is not correct!");
                }
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);

                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }

            return View();
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                TempData["wrongId"] = "No match with the users. Please enter again.";
                return RedirectToAction("Index", "User");
            }
            User user = db.User.Find(id);
            if (user == null)
            {
                TempData["wrongId"] = "No match with the users. Please enter again.";
                return RedirectToAction("Index", "User");
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details([Bind(Include = "UserId, Username, Password")] User user,
                string newPassword, string newConfirmPassword)
        {
            User newChange = db.User.Find(user.UserId);

            if (!isValid(user.Username, user.Password, false))
            {
                ModelState.AddModelError("", "The old password doesn'\t match. Try again");
                return View(newChange);
            }

            if (newPassword != newConfirmPassword || newPassword.Length < 6)
            {
                ModelState.AddModelError("", "The new password doesn'\t match or is too short. Try again");
                return View(newChange);
            }

            var crypto = new SimpleCrypto.PBKDF2();
            var encryptPass = crypto.Compute(newPassword);

            newChange.Password = encryptPass;
            newChange.PasswordSalt = crypto.Salt;

            db.Entry(newChange).State = EntityState.Modified;
            db.SaveChanges();

            FormsAuthentication.SignOut();
            TempData["changePass"] = "Successful change. Login with your new password";
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        /* This method is used for authentication in login and check if there
          exist user in the base */ 
        private bool isValid(string username, string password, bool forExist)
        {
            var crypto = new SimpleCrypto.PBKDF2();
            bool validation = false;

            using (var db = new Blog.Models.BlogDataEntities())
            {
                var user = db.User.FirstOrDefault(u => u.Username == username);

                if (user != null)
                {
                    if (forExist == true)
                    {
                        return true;
                    }

                    if (user.Password == crypto.Compute(password, user.PasswordSalt)) 
                    {
                        validation = true;
                    }
                }
            }

            return validation;
        }
        
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                TempData["wrongId"] = "No match with the users. Please enter again.";
                return RedirectToAction("Index", "User");
            }

            var name = HttpContext.User.Identity.Name;
            User user = db.User.Find(id);

            if (user == null)
            {
                TempData["wrongId"] = "No match with the users. Please enter again.";
                return RedirectToAction("Index", "User");
            }

            if (user.Username == null || name != user.Username)
            {
                return RedirectToAction("Index", "Home");
            }
            
            return View(user);
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string password)
        {
            User user = db.User.Find(id);
            
            if (!isValid(user.Username, password, false))
            {
                ModelState.AddModelError("", "Wrong password. Please try again");
                return View(user);
            }

            db.User.Remove(user);
            db.SaveChanges();

            FormsAuthentication.SignOut();
            TempData["deleteProfil"] = "Successful delete.";
            return RedirectToAction("Index", "Home");
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
