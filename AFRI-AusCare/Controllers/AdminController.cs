using AFRI_AusCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AFRI_AusCare.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseContext _context;

        public AdminController(DatabaseContext context)
        {
            _context = context;
        }


        // GET: Admin/Edit/5
        public IActionResult Credentials()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var adminSetting = _context.AdminSettings.SingleOrDefault(x => x.Id == 1);
                return View(adminSetting);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult BankAccount()
        {
            if (HttpContext.Session.Get("UserId") != null)
            {
                var adminSetting = _context.AdminSettings.SingleOrDefault(x => x.Id == 1);
                return View(adminSetting);
            }
            else
            {
                return RedirectToAction("Credentials");
            }
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBankAccount(AdminSetting adminSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var admin = _context.AdminSettings.FirstOrDefault(x => x.Id == 1);
                    if (admin != null)
                    {
                        admin.BankABN = adminSetting.BankABN;
                        admin.BankName = adminSetting.BankName;
                        admin.AccountNumber = adminSetting.AccountNumber;
                        admin.AccountBSB = adminSetting.AccountBSB;
                        _context.Update(admin);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("BankAccount");
            }
            return View(adminSetting);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCredentials(AdminSetting adminSetting)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var admin = _context.AdminSettings.FirstOrDefault(x => x.Id == 1);
                    if (admin != null)
                    {
                        admin.UserEmail = adminSetting.UserEmail;
                        admin.Password = adminSetting.Password;
                        _context.Update(admin);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {

                }
                return RedirectToAction("Index", "Events");
            }
            return View(adminSetting);
        }
    }
}
