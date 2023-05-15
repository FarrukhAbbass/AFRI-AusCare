using Microsoft.AspNetCore.Mvc;

namespace AFRI_AusCare.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            if (email == "user@example.com" && password == "password")
            {
                this.HttpContext.Session.SetString("UserId", email);
                return RedirectToAction("Index", "Events");
            }
            else
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View();
            }
        }

        public ActionResult Logout()
        {
            this.HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
