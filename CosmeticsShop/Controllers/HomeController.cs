using CosmeticsShop.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Product = CosmeticsShop.Models.Product;

namespace CosmeticsShop.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        ShoppingEntities db = new ShoppingEntities();
        public ActionResult Index()
        {
            if (Session["User"] != null && (Session["User"] as Models.User).UserTypeID == 1)
            {
                return RedirectToAction("Index", "Admin");
            }
            if (Session["Cart"] == null)
            {
                Session["Cart"] = new List<ItemCart>();
            }
            ViewBag.ListProduct = db.Products.Where(x => x.IsActive == true && x.PurchasedCount > 0).OrderByDescending(x => x.PurchasedCount).ToList();
            return View();
        }
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(Models.User user)
        {
            Models.User check = db.Users.SingleOrDefault(x => x.Email == user.Email);
            if (check != null)
            {
                ViewBag.Message = "Email already exists";
                return View();
            }

            Models.User userAdded = new Models.User();
            try
            {
                user.UserTypeID = 2;
                user.Address = "user.jpg";
                userAdded = db.Users.Add(user);
                db.SaveChanges();
            }
            catch (Exception)
            {
                ViewBag.Message = "Sign up failure";
            }
            ViewBag.Message = "Sign up successful";
            return View();
        }
        public ActionResult SignIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignIn(string Email, string Password)
        {
            Models.User check = db.Users.SingleOrDefault(x => x.Email == Email && x.Password == Password);
            if (check != null)
            {
                Session["User"] = check;

                if (check.UserTypeID == 1)
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            ViewBag.Message = "Email or password is incorrect";
            return View();
        }
        public ActionResult SignOut()
        {
            Session.Remove("User");
            return RedirectToAction("Index");
        }

        public ActionResult Quiz()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("SignIn", "Home");
            }
            return View();
        }
        public ActionResult Suggest()
        {
            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true).ToList();
            ViewBag.ListProduct = Session["Suggest"] as List<Product>;
            return View();
        }
        [HttpPost]
        public ActionResult Suggest(List<string> data)
        {
            var product = new List<Product>();
            ViewBag.ListCategory = db.Categories.Where(x => x.IsActive == true).ToList();
            var s = string.Join(" ", data);
            if (s.Contains("shiny") && s.Contains("red") && s.Contains("itchy"))
            {
                product = db.Products.Where(x => x.Type == "Combination").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "You have combination skin." }, JsonRequestBehavior.AllowGet);
            }
            else if (s.Contains("shiny"))
            {
                product = db.Products.Where(x => x.Type == "Oily").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "You have oily skin." }, JsonRequestBehavior.AllowGet);
            }
            else if (s.Contains("itchy"))
            {
                product = db.Products.Where(x => x.Type == "Dry").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "You have dry skin." }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                product = db.Products.Where(x => x.Type == "Sensitive").ToList();
                Session["Suggest"] = product;
                return Json(new { message = "You have sensitive skin." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}