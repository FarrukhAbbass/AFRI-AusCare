using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class AccountController : Controller
    {
        private readonly DatabaseContext _context;

        public AccountController(DatabaseContext context)
        {
            _context = context;
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var adminSetting = _context.AdminSettings.SingleOrDefault(x => x.UserEmail == email && x.Password == password);
            if (adminSetting != null)
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
